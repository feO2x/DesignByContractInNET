using Common;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
// ReSharper disable InvocationIsSkipped

namespace DbcCodeContracts
{
    public class SimpleList<T> : IEnumerable<T>
    {
        private int _count;
        private T[] _internalArray;

        public SimpleList() : this(4)
        {
        }

        public SimpleList(int initialCapacity)
        {
            Contract.Requires(initialCapacity >= 2);
            Contract.Ensures(_internalArray.Length == initialCapacity);

            _internalArray = new T[initialCapacity];
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(Count >= 0);
            Contract.Invariant(Count <= Capacity);
        }

        public int Count => _count;

        public void Add(T item)
        {
            Contract.Ensures(Count == Contract.OldValue(Count) + 1);
            Contract.Ensures(this[Count - 1].Equals(item));

            if (_count == Capacity)
                EnlargeArray();

            _internalArray[_count] = item;
            _count++;
        }

        private void EnlargeArray()
        {
            var newArray = new T[2*Capacity];
            for (var i = 0; i < Capacity; i++)
            {
                newArray[i] = _internalArray[i];
            }
            _internalArray = newArray;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ArrayEnumerator(_internalArray, _count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            Contract.Ensures(Contract.Result<int>() >= -1);
            Contract.Ensures(Contract.Result<int>() < Count);

            for (var i = 0; i < _count; i++)
            {
                if (item.CompareWithHashCodeAndEquals(_internalArray[i]))
                    return i;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            Contract.Requires(index > 0);
            Contract.Requires(index <= Count);

            Contract.Ensures(Count == Contract.OldValue(Count) + 1);
            Contract.Ensures(this[index].Equals(item));
            Contract.Ensures(Contract.ForAll(
                index + 1, Count, 
                i => this[i].Equals(Contract.OldValue(this[i - 1]))));

            if (_count == Capacity)
                EnlargeArray();

            for (var i = _count - 1; i >= index; i--)
            {
                _internalArray[i + 1] = _internalArray[i];
            }
            _internalArray[index] = item;
            _count++;
        }

        public void RemoveAt(int index)
        {
            Contract.Requires(index > 0 && index < Count);

            Contract.Ensures(Count == Contract.OldValue(Count) - 1);
            Contract.Ensures(Contract.ForAll(index, Count, i => this[i].Equals(Contract.OldValue(this[i + 1]))));

            _internalArray[index] = default(T);

            for (var i = index; i < _count - 1; i++)
            {
                _internalArray[i] = _internalArray[i + 1];
                if (i == _count - 2)
                    _internalArray[i + 1] = default(T);
            }
            _count--;
        }

        public void Clear()
        {
            Contract.Ensures(Count == 0);
            Contract.Ensures(Contract.ForAll(
                _internalArray, i => i == null));

            for (var i = 0; i < _count; i++)
            {
                _internalArray[i] = default(T);
            }
            _count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Contract.Requires(array != null);
            Contract.Requires(arrayIndex >= 0);
            Contract.Requires(array.Length - arrayIndex >= Count);

            Contract.Ensures(Contract.ForAll(
                0, 
                Count, 
                i => array[arrayIndex + i].Equals(_internalArray[i])));

            for (var i = 0; i < _count; i++)
            {
                array[arrayIndex + i] = _internalArray[i];
            }
        }

        public bool Remove(T item)
        {
            var indexOfItemBeingRemoved = IndexOf(item);
            if (indexOfItemBeingRemoved == -1)
                return false;

            RemoveAt(indexOfItemBeingRemoved);
            return true;
        }

        public T this[int index]
        {
            get { return _internalArray[index]; }
            set
            {
                Contract.Requires(index <= Count);
                Contract.Requires(index >= 0);

                Contract.Ensures(_internalArray[index].Equals(value));

                if (index == _count)
                {
                    Add(value);
                    return;
                }

                _internalArray[index] = value;
            }
        }

        public int Capacity => _internalArray.Length;

        public bool IsReadOnly => false;

        private class ArrayEnumerator : IEnumerator<T>
        {
            private readonly int _count;
            private readonly T[] _array;
            private T _currentItem;
            private int _currentIndex = -1;

            public ArrayEnumerator(T[] array, int count)
            {
                _array = array;
                _count = count;
            }

            public T Current => _currentItem;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_currentIndex + 1 == _count)
                    return false;

                _currentIndex++;
                _currentItem = _array[_currentIndex];
                return true;
            }

            public void Reset()
            {
                _currentIndex = -1;
                _currentItem = default(T);
            }
        }
    }
}