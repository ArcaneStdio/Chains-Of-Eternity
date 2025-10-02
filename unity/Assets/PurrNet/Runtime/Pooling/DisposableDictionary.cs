using System;
using System.Collections;
using System.Collections.Generic;

namespace PurrNet.Pooling
{
    public struct DisposableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
        where TKey : notnull
    {
        private bool _isAllocated;

        public bool isDisposed => !_isAllocated;

        public Dictionary<TKey, TValue> dictionary { get; private set; }

        public static DisposableDictionary<TKey, TValue> Create()
        {
            var val = new DisposableDictionary<TKey, TValue>();
            val.dictionary = DictionaryPool<TKey, TValue>.Instantiate();
            val._isAllocated = true;
            return val;
        }
        public static DisposableDictionary<TKey, TValue> Create(IDictionary<TKey, TValue> copyFrom)
        {
            var val = new DisposableDictionary<TKey, TValue>();
            val.dictionary = DictionaryPool<TKey, TValue>.Instantiate();
            foreach (var kvp in copyFrom)
                val.dictionary.Add(kvp.Key, kvp.Value);
            val._isAllocated = true;
            return val;
        }

        public void Dispose()
        {
            if (!_isAllocated) return;

            if (dictionary != null)
                DictionaryPool<TKey, TValue>.Destroy(dictionary);
            _isAllocated = false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            return dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(dictionary[item.Key], item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex + dictionary.Count > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            foreach (var kvp in dictionary)
                array[arrayIndex++] = kvp;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            if (dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(dictionary[item.Key], item.Value))
                return dictionary.Remove(item.Key);
            return false;
        }

        public int Count
        {
            get
            {
                if (!_isAllocated)
                    throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
                return dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (!_isAllocated)
                    throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
                return false;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            return dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!_isAllocated)
            {
                value = default;
                return false;
            }
            return dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!_isAllocated)
                    throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
                return dictionary[key];
            }
            set
            {
                if (!_isAllocated)
                    throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
                dictionary[key] = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (!_isAllocated)
                    throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
                return dictionary.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (!_isAllocated)
                    throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
                return dictionary.Values;
            }
        }

        public TValue GetValueOrDefault(TKey key)
        {
            if (!_isAllocated)
                throw new ObjectDisposedException(nameof(DisposableDictionary<TKey, TValue>));
            return dictionary.GetValueOrDefault(key);
        }
    }
}
