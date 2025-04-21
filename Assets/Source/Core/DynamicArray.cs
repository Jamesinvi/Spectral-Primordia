using System;
using System.Collections.Generic;

namespace Spectral.Core
{
    public class DynamicArray<T>
    {
        private bool _lastOperationWasAdd;
        private int _nextFreeIndex;
        public T[] data;
        public int length;

        public DynamicArray(int capacity)
        {
            data = new T[capacity];
            length = 0;
            _nextFreeIndex = 0;
        }

        public void Add(T item)
        {
            if (_nextFreeIndex >= data.Length)
            {
                var newData = new T[length * 2];
                Array.Copy(data, 0, newData, 0, length);
                data = newData;
            }

            data[_nextFreeIndex] = item;
            // If we added at or beyond current length, extend length
            if (_nextFreeIndex >= length)
                length = _nextFreeIndex + 1;

            _nextFreeIndex = _lastOperationWasAdd ? _nextFreeIndex + 1 : length;
            _lastOperationWasAdd = true;
        }

        public void RemoveAt(int index)
        {
            data[index] = default!;
            if (index == length - 1)
                length--;

            _nextFreeIndex = index;
            _lastOperationWasAdd = false;
        }

        public Span<T> AsSpan()
        {
            return data.AsSpan(0, length);
        }

        public void Clear()
        {
            length = 0;
            _lastOperationWasAdd = false;
            _nextFreeIndex = 0;
        }

        public bool Contains(T toCheck)
        {
            var comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < length; i++)
                if (comparer.Equals(data[i], toCheck))
                    return true;
            return false;
        }
    }
}