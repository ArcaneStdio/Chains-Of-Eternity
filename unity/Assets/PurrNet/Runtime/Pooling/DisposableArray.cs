using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace PurrNet.Pooling
{
    public struct DisposableArray<T> : IDisposable, IReadOnlyList<T>, IList<T>
    {
        private bool _shouldDispose;

        public T[] array { get; private set; }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return Array.IndexOf(array, item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(this.array, array, Count);
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public bool isDisposed => !_shouldDispose;

        public int IndexOf(T item)
        {
            return Array.IndexOf(array, item);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get
            {
                if (isDisposed) throw new ObjectDisposedException(nameof(DisposableArray<T>));
                if (index >= Count || index < 0)
                    throw new IndexOutOfRangeException($"Index {index} is out of range for list of size {Count}.");
                return array[index];
            }
            set
            {
                if (isDisposed) throw new ObjectDisposedException(nameof(DisposableArray<T>));
                if (index >= Count || index < 0)
                    throw new IndexOutOfRangeException($"Index {index} is out of range for list of size {Count}.");
                array[index] = value;
            }
        }

        public static DisposableArray<T> Create(int size)
        {
            var rented = ArrayPool<T>.Shared.Rent(size);
            Array.Clear(rented, 0, size);

            return new DisposableArray<T>
            {
                array = rented,
                Count = size,
                _shouldDispose = true
            };
        }

        public static DisposableArray<T> Create(DisposableArray<T> copyFrom)
        {
            var array = ArrayPool<T>.Shared.Rent(copyFrom.Count);
            Array.Copy(copyFrom.array, array, copyFrom.Count);
            return new DisposableArray<T>
            {
                array = array,
                Count = copyFrom.Count,
                _shouldDispose = true
            };
        }

        public static DisposableArray<T> Create(IList<T> copyFrom)
        {
            var array = ArrayPool<T>.Shared.Rent(copyFrom.Count);
            copyFrom.CopyTo(array, 0);
            return new DisposableArray<T>
            {
                array = array,
                Count = copyFrom.Count,
                _shouldDispose = true
            };
        }

        public static DisposableArray<T> Create(T[] copyFrom)
        {
            var array = ArrayPool<T>.Shared.Rent(copyFrom.Length);
            Array.Copy(copyFrom, array, copyFrom.Length);
            return new DisposableArray<T>
            {
                array = array,
                Count = copyFrom.Length,
                _shouldDispose = true
            };
        }

        public void Dispose()
        {
            if (!_shouldDispose) return;
            ArrayPool<T>.Shared.Return(array);
            _shouldDispose = false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (isDisposed) throw new ObjectDisposedException(nameof(DisposableArray<T>));
            for (int i = 0; i < Count; i++)
                yield return array[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
