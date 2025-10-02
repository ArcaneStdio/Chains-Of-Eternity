using UnityEngine;
using PurrNet.Logging;
using PurrNet.Modules;
using System;
using JetBrains.Annotations;
using PurrNet.Packing;
using PurrNet.Transports;
using PurrNet.Utils;

namespace PurrNet
{
    [Serializable]
    public class SyncVar<T> : NetworkModule, ITick
    {
        private TickManager _tickManager;

        [SerializeField, PurrLock] private T _value;

        private bool _isDirty;

        [SerializeField, Space(-5), Header("Sync Settings"), PurrLock]
        private bool _ownerAuth;

        [SerializeField, Min(0)] private float _sendIntervalInSeconds;

        public bool ownerAuth => _ownerAuth;

        public float sendIntervalInSeconds
        {
            get => _sendIntervalInSeconds;
            set => _sendIntervalInSeconds = value;
        }

        public event Action<T> onChanged;

        public delegate void ActionWithOld(T oldValue, T newValue);
        public event ActionWithOld onChangedWithOld;

        public bool isControllingSyncVar => parent.IsController(_ownerAuth);

        public T value
        {
            get => _value;
            set
            {
                bool bothNull = value == null && _value == null;
                bool bothEqual = value != null && value.Equals(_value);

                if (bothNull || bothEqual)
                    return;

                if (isSpawned && !parent.IsController(_ownerAuth))
                {
                    PurrLogger.LogError(
                        $"Invalid permissions when setting `<b>SyncVar<{typeof(T).Name}> {name}</b>` on `{parent.name}`." +
                        $"\n{GetPermissionErrorDetails(_ownerAuth, this)}", parent);
                    return;
                }

                var oldValue = _value;
                _value = value;
                _isDirty = true;

                onChanged?.Invoke(value);
                onChangedWithOld?.Invoke(oldValue, value);
            }
        }

        public override void OnOwnerChanged(PlayerID? oldOwner, PlayerID? newOwner, bool isSpawnEvent, bool asServer)
        {
            if (isSpawnEvent)
                return;

            /*if (_ownerAuth && asServer)
            {
                _id = 0;
                SendLatestStateToAll(_id, _value);
            }*/

            if (_ownerAuth)
            {
                _id = 0;

                if (isOwner)
                    _isDirty = true;
            }
        }

        BitPacker GetValue()
        {
            var packer = BitPackerPool.Get();
            Packer<T>.Write(packer, _value);
            return packer;
        }

        public override void OnObserverAdded(PlayerID player, bool isSpawner)
        {
            if (isSpawner && ownerAuth && owner == player)
                return;

            using var v = GetValue();
            SendLatestState(player, _id, v);
        }

        public override void OnDespawned()
        {
            if (isControllingSyncVar)
            {
                _id += 1;
                FlushImmediately();
            }
        }

        public void SetDirty()
        {
            _isDirty = true;
        }

        private float _lastSendTime;

        private void ForceSendUnreliable()
        {
            using var v = GetValue();

            if (isServer)
                SendToAll(_id++, v);
            else SendToServer(_id++, v);
        }

        private void ForceSendReliable()
        {
            using var v = GetValue();

            if (isServer)
                SendToAllReliably(_id++, v);
            else SendToServerReliably(_id++, v);
        }

        public void FlushImmediately()
        {
            ForceSendReliable();
            _lastSendTime = Time.time;
            _wasLastDirty = false;
            _isDirty = false;
        }

        public void OnTick(float delta)
        {
            bool isControlling = parent.IsController(_ownerAuth);

            if (!isControlling)
                return;

            float timeSinceLastSend = Time.time - _lastSendTime;

            if (timeSinceLastSend < _sendIntervalInSeconds)
                return;

            if (_isDirty)
            {
                ForceSendUnreliable();

                _lastSendTime = Time.time;
                _wasLastDirty = true;
                _isDirty = false;
            }
            else
            {
                if (_wasLastDirty)
                {
                    ForceSendReliable();
                    _wasLastDirty = false;
                }
            }
        }

        private ulong _id;
        private bool _wasLastDirty;

        public SyncVar(T initialValue = default, float sendIntervalInSeconds = 0f, bool ownerAuth = false)
        {
            _value = initialValue;
            _sendIntervalInSeconds = sendIntervalInSeconds;
            _ownerAuth = ownerAuth;
        }

        private static T _cache;

        [TargetRpc, UsedImplicitly]
        private void SendLatestState(PlayerID player, PackedULong packetId, BitPacker newValue)
        {
            using (newValue)
            {
                if (isServer) return;

                _id = packetId;

                Packer<T>.Read(newValue, ref _cache);
                newValue.SetBitPosition(0);

                bool bothNull = _value == null && _cache == null;
                bool bothEqual = _value != null && _value.Equals(_cache);

                if (bothNull || bothEqual)
                    return;

                var oldValue = value;
                Packer<T>.Read(newValue, ref _value);
                onChanged?.Invoke(value);
                onChangedWithOld?.Invoke(oldValue, value);
            }
        }

        [ServerRpc(Channel.Unreliable, requireOwnership: true)]
        private void SendToServer(PackedULong packetId, BitPacker newValue)
        {
            using (newValue)
            {
                if (!_ownerAuth) return;

                OnReceivedValue(packetId, newValue);
                SendToOthers(packetId, newValue);
            }
        }

        [ServerRpc(Channel.ReliableOrdered, requireOwnership: true)]
        private void SendToServerReliably(PackedULong packetId, BitPacker newValue)
        {
            using (newValue)
            {
                if (!_ownerAuth) return;

                OnReceivedValue(packetId, newValue);
                SendToOthersReliably(packetId, newValue);
            }
        }

        [ObserversRpc(Channel.Unreliable, excludeOwner: true)]
        private void SendToOthers(PackedULong packetId, BitPacker newValue)
        {
            using (newValue)
            {
                if (!isServer) OnReceivedValue(packetId, newValue);
            }
        }

        [ObserversRpc(Channel.ReliableOrdered, excludeOwner: true)]
        private void SendToOthersReliably(PackedULong packetId, BitPacker newValue)
        {
            using (newValue)
            {
                if (!isHost) OnReceivedValue(packetId, newValue);
            }
        }

        [ObserversRpc(Channel.Unreliable)]
        private void SendToAll(PackedULong packetId, BitPacker newValue)
        {
            using (newValue)
            {
                if (!isHost) OnReceivedValue(packetId, newValue);
            }
        }

        [ObserversRpc(Channel.ReliableOrdered)]
        private void SendToAllReliably(PackedULong packetId, BitPacker newValue)
        {
            using (newValue)
            {
                if (!isHost) OnReceivedValue(packetId, newValue);
            }
        }

        private void OnReceivedValue(PackedULong packetId, BitPacker newValue)
        {
            bool isControlling = parent.IsController(_ownerAuth);

            if (isControlling)
            {
                return;
            }

            if (packetId <= _id)
            {
                return;
            }

            _id = packetId;

            Packer<T>.Read(newValue, ref _cache);
            int readPos = newValue.positionInBits;
            newValue.SetBitPosition(0);

            bool bothNull = _value == null && _cache == null;
            bool bothEqual = _value != null && _value.Equals(_cache);

            if (bothNull || bothEqual)
            {
                newValue.SetBitPosition(readPos);
                return;
            }

            var oldValue = value;
            Packer<T>.Read(newValue, ref _value);
            onChanged?.Invoke(value);
            onChangedWithOld?.Invoke(oldValue, value);
        }

        public static implicit operator T(SyncVar<T> syncVar)
        {
            return syncVar._value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
