using System.Collections.Generic;
using UnityEngine;

namespace PurrNet
{
    public abstract class PlayerIdentity<T> : NetworkIdentity where T : NetworkIdentity
    {
        private static FirstElementMap<PlayerID, T> _allPlayers = new();
        public static IReadOnlyDictionary<PlayerID, T> allPlayers => _allPlayers.first;

        protected override void OnOwnerChanged(PlayerID? oldOwner, PlayerID? newOwner, bool asServer)
        {
            base.OnOwnerChanged(oldOwner, newOwner, asServer);

            if (asServer && isHost)
                return;

            if (oldOwner.HasValue)
                _allPlayers.Remove(this as T);

            if (newOwner.HasValue)
                _allPlayers.AddItem(newOwner.Value, this as T);
        }

        protected override void OnDespawned(bool asServer)
        {
            base.OnDespawned(asServer);

            _allPlayers.Remove(this as T);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _allPlayers.Remove(this as T);
        }

        public static bool TryGetLocal(out T player)
        {
            if (!NetworkManager.main)
            {
                player = null;
                return false;
            }

            return _allPlayers.TryGetFirst(NetworkManager.main.localPlayer, out player);
        }

        public static bool TryGetPlayer(PlayerID playerId, out T player)
        {
            return _allPlayers.TryGetFirst(playerId, out player);
        }

        private sealed class FirstElementMap<TKey, T>
        {
            readonly Dictionary<TKey, List<T>> _lists = new();
            readonly Dictionary<TKey, T> _first = new();
            public IReadOnlyDictionary<TKey, T> first => _first;
            public List<T> this[TKey key]
            {
                get => _lists[key];
                set
                {
                    _lists[key] = value;
                    _first[key] = value != null && value.Count > 0 ? value[0] : default;
                }
            }
            public void AddItem(TKey key, T item)
            {
                if (!_lists.TryGetValue(key, out var list))
                {
                    list = new List<T>();
                    _lists[key] = list;
                }
                if (list.Count == 0) _first[key] = item;
                list.Add(item);
            }
            public bool RemoveAt(TKey key, int index)
            {
                if (!_lists.TryGetValue(key, out var list)) return false;
                if ((uint)index >= (uint)list.Count) return false;
                bool wasFirst = index == 0;
                list.RemoveAt(index);
                if (list.Count == 0) { _lists.Remove(key); _first.Remove(key); }
                else if (wasFirst) _first[key] = list[0];
                return true;
            }

            public bool Remove(T item)
            {
                TKey foundKey = default;
                int foundIndex = -1;
                foreach (var kv in _lists)
                {
                    int idx = kv.Value.IndexOf(item);
                    if (idx >= 0) { foundKey = kv.Key; foundIndex = idx; break; }
                }
                if (foundIndex < 0) return false;
                return RemoveAt(foundKey, foundIndex);
            }

            public bool RemoveKey(TKey key)
            {
                _first.Remove(key);
                return _lists.Remove(key);
            }
            public bool TryGetFirst(TKey key, out T value) => _first.TryGetValue(key, out value);
        }
    }
}
