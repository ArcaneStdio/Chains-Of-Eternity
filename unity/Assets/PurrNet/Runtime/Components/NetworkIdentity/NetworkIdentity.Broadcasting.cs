using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
#if UNITASK_PURRNET_SUPPORT
using Cysharp.Threading.Tasks;
#endif
using PurrNet.Logging;
using PurrNet.Modules;
using PurrNet.Packing;
using PurrNet.Pooling;
using PurrNet.Profiler;
using PurrNet.Transports;
using PurrNet.Utils;
using UnityEngine.Scripting;
using Channel = PurrNet.Transports.Channel;

#if !UNITASK_PURRNET_SUPPORT
using RawTask = System.Threading.Tasks.Task;
#else
using RawTask = Cysharp.Threading.Tasks.UniTask;
#endif


namespace PurrNet
{
    public partial class NetworkIdentity
    {
        internal readonly struct InstanceGenericKey : IEquatable<InstanceGenericKey>
        {
            readonly string _methodName;
            readonly int _typesHash;
            readonly int _callerHash;

            public InstanceGenericKey(string methodName, Type caller, Type[] types)
            {
                _methodName = methodName;
                _typesHash = 0;

                _callerHash = caller.GetHashCode();

                for (int i = 0; i < types.Length; i++)
                    _typesHash ^= types[i].GetHashCode();
            }

            public override int GetHashCode() => _methodName.GetHashCode() ^ _typesHash ^ _callerHash;

            public bool Equals(InstanceGenericKey other)
            {
                return _methodName == other._methodName && _typesHash == other._typesHash &&
                       _callerHash == other._callerHash;
            }

            public override bool Equals(object obj)
            {
                return obj is InstanceGenericKey other && Equals(other);
            }
        }

        internal static readonly Dictionary<InstanceGenericKey, MethodInfo> genericMethods =
            new Dictionary<InstanceGenericKey, MethodInfo>();

        [UsedByIL]
        public static void ReadGenericHeader(BitPacker stream, RPCInfo info, int genericCount, int paramCount,
            out GenericRPCHeader rpcHeader)
        {
            uint hash = 0;

            rpcHeader = new GenericRPCHeader
            {
                stream = stream,
                types = new Type[genericCount],
                values = new object[paramCount],
                info = info
            };

            for (int i = 0; i < genericCount; i++)
            {
                Packer<uint>.Read(stream, ref hash);
                var type = Hasher.ResolveType(hash);

                rpcHeader.types[i] = type;
            }
        }

        [UsedByIL]
        protected object CallGeneric(string methodName, GenericRPCHeader rpcHeader)
        {
            var key = new InstanceGenericKey(methodName, GetType(), rpcHeader.types);

            if (!genericMethods.TryGetValue(key, out var gmethod))
            {
                var method = GetType().GetMethod(methodName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                gmethod = method?.MakeGenericMethod(rpcHeader.types);

                genericMethods.Add(key, gmethod);
            }

            if (gmethod == null)
            {
                PurrLogger.LogError($"Calling generic RPC failed. Method '{methodName}' not found.");
                return null;
            }

            try
            {
                return gmethod.Invoke(this, rpcHeader.values);
            }
            catch (TargetInvocationException e)
            {
                var actualException = e.InnerException;

                if (actualException != null)
                {
                    PurrLogger.LogException(actualException);
                    throw BypassLoggingException.instance;
                }

                throw;
            }
        }

        /// <summary>
        /// Used internally to get next RPC id.
        /// Do not use this method directly.
        /// </summary>
        [UsedByIL]
        public Task<T> GetNextId<T>(RPCType rpcType, PlayerID? target, float timeout, out RpcRequest request)
        {
            request = default;

            if (!networkManager)
            {
                return Task.FromException<T>(new InvalidOperationException(
                    "NetworkIdentity is not spawned."));
            }

            bool asServer = rpcType switch
            {
                RPCType.ServerRPC => !networkManager.isClient,
                RPCType.TargetRPC => networkManager.isServer,
                RPCType.ObserversRPC => networkManager.isServer,
                _ => throw new ArgumentOutOfRangeException(nameof(rpcType), rpcType, null)
            };

            if (!networkManager.TryGetModule<RpcRequestResponseModule>(asServer, out var module))
            {
                return Task.FromException<T>(new InvalidOperationException(
                    "RpcRequestResponseModule module is missing."));
            }

            return module.GetNextId<T>(target, timeout, out request);
        }

        [UsedByIL]
        public RawTask GetNextIdUniTask(RPCType rpcType, PlayerID? target, float timeout, out RpcRequest request)
        {
            request = default;

            if (!networkManager)
            {
                return RawTask.FromException(new InvalidOperationException(
                    "NetworkIdentity is not spawned."));
            }

            bool asServer = rpcType switch
            {
                RPCType.ServerRPC => !networkManager.isClient,
                RPCType.TargetRPC => networkManager.isServer,
                RPCType.ObserversRPC => networkManager.isServer,
                _ => throw new ArgumentOutOfRangeException(nameof(rpcType), rpcType, null)
            };

            if (!networkManager.TryGetModule<RpcRequestResponseModule>(asServer, out var module))
            {
                return RawTask.FromException(new InvalidOperationException(
                    "RpcRequestResponseModule module is missing."));
            }

            return module.GetNextIdUniTask(target, timeout, out request);
        }

        [UsedByIL]
#if !UNITASK_PURRNET_SUPPORT
        public Task<T>
#else
        public UniTask<T>
#endif
            GetNextIdUniTask<T>(RPCType rpcType, PlayerID? target, float timeout, out RpcRequest request)
        {
            request = default;

            if (!networkManager)
            {
                return RawTask.FromException<T>(new InvalidOperationException(
                    "NetworkIdentity is not spawned."));
            }

            bool asServer = rpcType switch
            {
                RPCType.ServerRPC => !networkManager.isClient,
                RPCType.TargetRPC => networkManager.isServer,
                RPCType.ObserversRPC => networkManager.isServer,
                _ => throw new ArgumentOutOfRangeException(nameof(rpcType), rpcType, null)
            };

            if (!networkManager.TryGetModule<RpcRequestResponseModule>(asServer, out var module))
            {
                return RawTask.FromException<T>(new InvalidOperationException(
                    "RpcRequestResponseModule module is missing."));
            }

            return module.GetNextIdUniTask<T>(target, timeout, out request);
        }

        /// <summary>
        /// Used internally to get next RPC id.
        /// Do not use this method directly.
        /// </summary>
        [UsedByIL]
        public Task GetNextId(RPCType rpcType, PlayerID? target, float timeout, out RpcRequest request)
        {
            request = default;

            if (!networkManager)
            {
                return Task.FromException(new InvalidOperationException(
                    "NetworkIdentity is not spawned."));
            }

            bool asServer = rpcType switch
            {
                RPCType.ServerRPC => !networkManager.isClient,
                RPCType.TargetRPC => networkManager.isServer,
                RPCType.ObserversRPC => networkManager.isServer,
                _ => throw new ArgumentOutOfRangeException(nameof(rpcType), rpcType, null)
            };

            if (!networkManager.TryGetModule<RpcRequestResponseModule>(asServer, out var module))
            {
                return Task.FromException(new InvalidOperationException(
                    "RpcRequestResponseModule module is missing."));
            }

            return module.GetNextId(target, timeout, out request);
        }

#if UNITY_EDITOR
        private Type _myType;
#endif

        [UsedByIL]
        protected void SendRPC(RPCPacket packet, RPCSignature signature)
        {
#if UNITY_EDITOR
            _myType ??= GetType();
#endif
            if (!isSpawned)
            {
                if (signature is { runLocally: false, channel: Channel.ReliableOrdered or Channel.ReliableUnordered })
                    PurrLogger.LogError($"Trying to send RPC `{signature.rpcName}` from '{GetType().Name}' which is not spawned.", this);
                return;
            }

            if (!networkManager.TryGetModule<RPCModule>(networkManager.isServer, out var module))
            {
                if (signature is { runLocally: false, channel: Channel.ReliableOrdered or Channel.ReliableUnordered })
                    PurrLogger.LogError($"Trying to send RPC `{signature.rpcName}` from `{GetType().Name}` but RPCModule is missing for `{(networkManager.isServer ? "server" : "client")}`.", this);
                return;
            }

            var rules = networkManager.networkRules;
            bool shouldIgnoreOwnership = rules && rules.ShouldIgnoreRequireOwner();

            if (!shouldIgnoreOwnership && signature.requireOwnership && !isOwner)
            {
                if (signature is { runLocally: false, channel: Channel.ReliableOrdered or Channel.ReliableUnordered })
                    PurrLogger.LogError(
                        $"Trying to send RPC '{signature.rpcName}' from '{GetType().Name}' without ownership.", this);
                return;
            }

            bool shouldIgnore = rules && rules.ShouldIgnoreRequireServer();

            if (!shouldIgnore && signature.requireServer && !networkManager.isServer)
            {
                if (signature is { runLocally: false, channel: Channel.ReliableOrdered or Channel.ReliableUnordered })
                    PurrLogger.LogError(
                        $"Trying to send RPC '{signature.rpcName}' from '{GetType().Name}' without server.", this);
                return;
            }

            module.AppendToBufferedRPCs(packet, signature);

            switch (signature.type)
            {
                case RPCType.ServerRPC:
                    if (networkManager.isServerOnly)
                        break;

                    if (signature.runLocally && isServer)
                        break;

#if UNITY_EDITOR
                    Statistics.SentRPC(_myType, signature.type, signature.rpcName, packet.data.segment, this);
#endif
                    SendToServer(packet, signature.channel);
                    break;
                case RPCType.ObserversRPC:
                {
                    if (isServer)
                    {
                        using var players = DisposableList<PlayerID>.Create();
                        for (var i = 0; i < observers.Count; i++)
                        {
                            var player = observers[i];
                            bool isLocalPlayer = player == networkManager.localPlayer;

                            if (signature.runLocally && isLocalPlayer)
                                continue;

                            if (signature.excludeSender && isLocalPlayer)
                                continue;

                            if (signature.excludeOwner && !IsNotOwnerPredicate(player))
                                continue;
#if UNITY_EDITOR
                            Statistics.SentRPC(_myType, signature.type, signature.rpcName, packet.data.segment,
                                this);
#endif
                            players.Add(player);
                        }
                        Send(players, packet, signature.channel);
                    }
                    else
                    {
#if UNITY_EDITOR
                        Statistics.SentRPC(_myType, signature.type, signature.rpcName, packet.data.segment, this);
#endif
                        SendToServer(packet, signature.channel);
                    }
                    break;
                }
                case RPCType.TargetRPC:
#if UNITY_EDITOR
                    Statistics.SentRPC(_myType, signature.type, signature.rpcName, packet.data.segment, this);
#endif
                    if (isServer)
                    {
                        using var targets = signature.GetTargets();
                        Send(targets, packet, signature.channel);
                    }
                    else
                    {
                        using var targets = signature.GetTargets();
                        // TODO: we should batch this into one packet to the server instead of N
                        for (int i = 0; i < targets.Count; i++)
                        {
                            packet.targetPlayerId = targets[i];
                            SendToServer(packet, signature.channel);
                        }
                    }
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        [UsedByIL]
        public bool ValidateReceivingRPC(RPCInfo info, RPCSignature signature, IRpc data, bool asServer)
        {
#if UNITY_EDITOR
            _myType ??= GetType();
            Statistics.ReceivedRPC(_myType, signature.type, signature.rpcName, data.rpcData.segment, this);
#endif
            return ValidateIncomingRPC(info, signature, data, asServer);
        }

        internal bool ValidateIncomingRPC(RPCInfo info, RPCSignature signature, IRpc data, bool asServer)
        {
            var rules = networkManager.networkRules;
            bool shouldIgnoreOwnership = rules && rules.ShouldIgnoreRequireOwner();

            if (!networkManager.TryGetModule<RPCModule>(networkManager.isServer, out var module))
                return false;

            if (!shouldIgnoreOwnership && signature.requireOwnership && info.sender != owner)
                return false;

            if (signature.excludeOwner && isOwner)
                return false;

            if (signature.type == RPCType.ServerRPC)
            {
                if (!asServer)
                {
                    PurrLogger.LogError(
                        $"Trying to receive server RPC '{signature.rpcName}' from '{name}' on client. Aborting RPC call.",
                        this);
                    return false;
                }

                var idObservers = observers;

                if (idObservers == null)
                {
                    PurrLogger.LogError(
                        $"Trying to receive server RPC '{signature.rpcName}' from '{name}' but failed to get observers.",
                        this);
                    return false;
                }

                if (!IsObserver(info.sender) && signature.channel == Channel.ReliableOrdered)
                {
                    PurrLogger.LogError(
                        $"Trying to receive server RPC '{signature.rpcName}' from '{name}' by player '{info.sender}' which is not an observer. Aborting RPC call.",
                        this);
                    return false;
                }

                return true;
            }

            if (!asServer)
            {
                return true;
            }

            bool shouldIgnore = rules && rules.ShouldIgnoreRequireServer();

            if (!shouldIgnore && signature.requireServer)
            {
                PurrLogger.LogError(
                    $"Trying to receive client RPC '{signature.rpcName}' from '{name}' on server. " +
                    "If you want automatic forwarding use 'requireServer: false'.", this);
                return false;
            }

            switch (signature.type)
            {
                case RPCType.ServerRPC: throw new InvalidOperationException("ServerRPC should be handled by server.");

                case RPCType.ObserversRPC:
                {
                    var rawData = BroadcastModule.GetImmediateData(data);
                    using var players = DisposableList<PlayerID>.Create(observers.Count);
                    var cachedOwner = owner;

                    for (var i = 0; i < observers.Count; ++i)
                    {
                        var observer = observers[i];

                        bool ignoreSender = observer == info.sender && (signature.excludeSender || signature.runLocally);
                        bool ignoreOwner = signature.excludeOwner && observer == cachedOwner;

                        if (ignoreSender || ignoreOwner)
                            continue;

                        players.Add(observer);
                    }

                    Send(players, rawData, signature.channel);
                    AppendToBufferedRPCs(signature, data, module);
                    return !isClient;
                }
                case RPCType.TargetRPC:
                {
                    var rawData = BroadcastModule.GetImmediateData(data);
                    SendToTarget(data.targetPlayerId, rawData, signature.channel);
                    AppendToBufferedRPCs(signature, data, module);
                    return false;
                }
                default: throw new ArgumentOutOfRangeException(nameof(signature.type));
            }
        }

        private static void AppendToBufferedRPCs(RPCSignature signature, IRpc data, RPCModule module)
        {
            switch (data)
            {
                case RPCPacket rpcPacket:
                    module.AppendToBufferedRPCs(rpcPacket, signature);
                    break;
                case ChildRPCPacket childRpcPacket:
                    module.AppendToBufferedRPCs(childRpcPacket, signature);
                    break;
            }
        }

        public void Send<T>(PlayerID player, T packet, Channel method = Channel.ReliableOrdered)
        {
            if (networkManager.isServer)
                networkManager.GetModule<PlayersManager>(true).Send(player, packet, method);
        }

        public void Send(PlayerID player, ByteData data, Channel method = Channel.ReliableOrdered)
        {
            if (networkManager.isServer)
                networkManager.GetModule<PlayersManager>(true).SendRaw(player, data, method);
        }

        [Preserve]
        public void SendToTarget(PlayerID player, ByteData data, Channel method = Channel.ReliableOrdered)
        {
            if (!IsObserver(player))
            {
                PurrLogger.LogError($"Trying to send TargetRPC to player '{player}' which is not observing '{name}'.",
                    this);
                return;
            }

            Send(player, data, method);
        }

        [Preserve]
        public void SendToTarget<T>(PlayerID player, T packet, Channel method = Channel.ReliableOrdered)
        {
            if (!IsObserver(player))
            {
                PurrLogger.LogError($"Trying to send TargetRPC to player '{player}' which is not observing '{name}'.",
                    this);
                return;
            }

            Send(player, packet, method);
        }

        public void Send<T>(IReadOnlyList<PlayerID> players, T data, Channel method = Channel.ReliableOrdered)
        {
            if (networkManager.isServer)
                networkManager.GetModule<PlayersManager>(true).Send(players, data, method);
        }

        public void Send(IReadOnlyList<PlayerID> players, ByteData data, Channel method = Channel.ReliableOrdered)
        {
            if (networkManager.isServer)
                networkManager.GetModule<PlayersManager>(true).SendRaw(players, data, method);
        }

        public void SendToServer<T>(T packet, Channel method = Channel.ReliableOrdered)
        {
            if (networkManager.isClient)
                networkManager.GetModule<PlayersManager>(false).SendToServer(packet, method);
        }
    }
}
