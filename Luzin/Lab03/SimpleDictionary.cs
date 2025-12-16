using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private struct Entry
        {
            public int hashCode;
            public int next;
            public TKey key;
            public TValue value;
        }

        private int[] _buckets;
        private Entry[] _entries;
        private int _count;
        private int _version;
        private int _freeList;
        private int _freeCount;
        private readonly IEqualityComparer<TKey> _comparer;

        public SimpleDictionary() : this(0, null) { }
        public SimpleDictionary(int capacity) : this(capacity, null) { }
        public SimpleDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) { }

        public SimpleDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            if (capacity > 0) Initialize(capacity);
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public TValue this[TKey key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0) return _entries[i].value;
                throw new KeyNotFoundException();
            }
            set => Insert(key, value, false);
        }

        public ICollection<TKey> Keys => new KeyCollection(this);
        public ICollection<TValue> Values => new ValueCollection(this);
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public int Count => _count - _freeCount;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (_count > 0)
            {
                Array.Clear(_buckets, 0, _buckets.Length);
                Array.Clear(_entries, 0, _count);
                _freeList = -1;
                _count = 0;
                _freeCount = 0;
                _version++;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            int i = FindEntry(item.Key);
            if (i >= 0 && EqualityComparer<TValue>.Default.Equals(_entries[i].value, item.Value)) return true;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Destination array is too small");

            int count = _count;
            Entry[] entries = _entries;
            for (int i = 0; i < count; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    array[arrayIndex++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            if (_buckets == null) return false;

            int hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
            int bucket = hashCode % _buckets.Length;
            int last = -1;

            for (int i = _buckets[bucket]; i >= 0; last = i, i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && _comparer.Equals(_entries[i].key, key))
                {
                    if (last < 0) _buckets[bucket] = _entries[i].next;
                    else _entries[last].next = _entries[i].next;

                    _entries[i].hashCode = -1;
                    _entries[i].next = _freeList;
                    _entries[i].key = default;
                    _entries[i].value = default;
                    _freeList = i;
                    _freeCount++;
                    _version++;
                    return true;
                }
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            int i = FindEntry(item.Key);
            if (i >= 0 && EqualityComparer<TValue>.Default.Equals(_entries[i].value, item.Value))
            {
                Remove(item.Key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = _entries[i].value;
                return true;
            }
            value = default;
            return false;
        }

        private int FindEntry(TKey key)
        {
            if (_buckets == null) return -1;

            int hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
            for (int i = _buckets[hashCode % _buckets.Length]; i >= 0; i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && _comparer.Equals(_entries[i].key, key)) return i;
            }
            return -1;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (_buckets == null) Initialize(0);

            int hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
            int targetBucket = hashCode % _buckets.Length;

            for (int i = _buckets[targetBucket]; i >= 0; i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && _comparer.Equals(_entries[i].key, key))
                {
                    if (add) throw new ArgumentException("An element with the same key already exists");
                    _entries[i].value = value;
                    _version++;
                    return;
                }
            }

            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                _freeList = _entries[index].next;
                _freeCount--;
            }
            else
            {
                if (_count == _entries.Length)
                {
                    Resize();
                    targetBucket = hashCode % _buckets.Length;
                }
                index = _count;
                _count++;
            }

            _entries[index].hashCode = hashCode;
            _entries[index].next = _buckets[targetBucket];
            _entries[index].key = key;
            _entries[index].value = value;
            _buckets[targetBucket] = index;
            _version++;
        }

        private void Initialize(int capacity)
        {
            int size = GetPrime(capacity);
            _buckets = new int[size];
            for (int i = 0; i < _buckets.Length; i++) _buckets[i] = -1;
            _entries = new Entry[size];
            _freeList = -1;
        }

        private void Resize()
        {
            int newSize = GetPrime(_count * 2);
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newBuckets.Length; i++) newBuckets[i] = -1;

            Entry[] newEntries = new Entry[newSize];
            Array.Copy(_entries, 0, newEntries, 0, _count);

            for (int i = 0; i < _count; i++)
            {
                if (newEntries[i].hashCode >= 0)
                {
                    int bucket = newEntries[i].hashCode % newSize;
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }

            _buckets = newBuckets;
            _entries = newEntries;
        }

        private static int GetPrime(int min)
        {
            int[] primes = { 3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369 };

            foreach (int prime in primes)
            {
                if (prime >= min) return prime;
            }

            for (int i = (min | 1); i < int.MaxValue; i += 2)
            {
                if (IsPrime(i)) return i;
            }
            return min;
        }

        private static bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0) return candidate == 2;

            int limit = (int)Math.Sqrt(candidate);
            for (int divisor = 3; divisor <= limit; divisor += 2)
            {
                if (candidate % divisor == 0) return false;
            }
            return true;
        }

        private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private SimpleDictionary<TKey, TValue> _dictionary;
            private int _version;
            private int _index;
            private KeyValuePair<TKey, TValue> _current;

            public Enumerator(SimpleDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _current = default;
            }

            public KeyValuePair<TKey, TValue> Current => _current;
            object IEnumerator.Current => _current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_version != _dictionary._version) throw new InvalidOperationException("Collection was modified");

                while (_index < _dictionary._count)
                {
                    if (_dictionary._entries[_index].hashCode >= 0)
                    {
                        _current = new KeyValuePair<TKey, TValue>(
                            _dictionary._entries[_index].key,
                            _dictionary._entries[_index].value);
                        _index++;
                        return true;
                    }
                    _index++;
                }
                _index = _dictionary._count + 1;
                _current = default;
                return false;
            }

            public void Reset()
            {
                if (_version != _dictionary._version) throw new InvalidOperationException("Collection was modified");
                _index = 0;
                _current = default;
            }
        }

        private class KeyCollection : ICollection<TKey>
        {
            private SimpleDictionary<TKey, TValue> _dictionary;

            public KeyCollection(SimpleDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public int Count => _dictionary.Count;
            public bool IsReadOnly => true;

            public void Add(TKey item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(TKey item) => _dictionary.ContainsKey(item);

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array == null) throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length - arrayIndex < _dictionary.Count) throw new ArgumentException("Destination array is too small");

                int count = _dictionary._count;
                Entry[] entries = _dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0) array[arrayIndex++] = entries[i].key;
                }
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return new KeyEnumerator(_dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Remove(TKey item) => throw new NotSupportedException();

            private class KeyEnumerator : IEnumerator<TKey>
            {
                private SimpleDictionary<TKey, TValue> _dictionary;
                private int _index;
                private int _version;
                private TKey _currentKey;

                public KeyEnumerator(SimpleDictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = 0;
                    _currentKey = default;
                }

                public TKey Current => _currentKey;
                object IEnumerator.Current => _currentKey;

                public void Dispose() { }

                public bool MoveNext()
                {
                    if (_version != _dictionary._version) throw new InvalidOperationException("Collection was modified");

                    while (_index < _dictionary._count)
                    {
                        if (_dictionary._entries[_index].hashCode >= 0)
                        {
                            _currentKey = _dictionary._entries[_index].key;
                            _index++;
                            return true;
                        }
                        _index++;
                    }
                    _index = _dictionary._count + 1;
                    _currentKey = default;
                    return false;
                }

                public void Reset()
                {
                    if (_version != _dictionary._version) throw new InvalidOperationException("Collection was modified");
                    _index = 0;
                    _currentKey = default;
                }
            }
        }

        private class ValueCollection : ICollection<TValue>
        {
            private SimpleDictionary<TKey, TValue> _dictionary;

            public ValueCollection(SimpleDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public int Count => _dictionary.Count;
            public bool IsReadOnly => true;

            public void Add(TValue item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();

            public bool Contains(TValue item)
            {
                foreach (var value in this)
                {
                    if (EqualityComparer<TValue>.Default.Equals(value, item)) return true;
                }
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array == null) throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length - arrayIndex < _dictionary.Count) throw new ArgumentException("Destination array is too small");

                int count = _dictionary._count;
                Entry[] entries = _dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0) array[arrayIndex++] = entries[i].value;
                }
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new ValueEnumerator(_dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Remove(TValue item) => throw new NotSupportedException();

            private class ValueEnumerator : IEnumerator<TValue>
            {
                private SimpleDictionary<TKey, TValue> _dictionary;
                private int _index;
                private int _version;
                private TValue _currentValue;

                public ValueEnumerator(SimpleDictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = 0;
                    _currentValue = default;
                }

                public TValue Current => _currentValue;
                object IEnumerator.Current => _currentValue;

                public void Dispose() { }

                public bool MoveNext()
                {
                    if (_version != _dictionary._version) throw new InvalidOperationException("Collection was modified");

                    while (_index < _dictionary._count)
                    {
                        if (_dictionary._entries[_index].hashCode >= 0)
                        {
                            _currentValue = _dictionary._entries[_index].value;
                            _index++;
                            return true;
                        }
                        _index++;
                    }
                    _index = _dictionary._count + 1;
                    _currentValue = default;
                    return false;
                }

                public void Reset()
                {
                    if (_version != _dictionary._version) throw new InvalidOperationException("Collection was modified");
                    _index = 0;
                    _currentValue = default;
                }
            }
        }
    }
}
