using System.Collections;

namespace Lab3
{
    public class SimpleList : IList, ICollection, IEnumerable
    {
        private object[] _items;
        private int _size;
        private int _version;

        public SimpleList()
        {
            _items = Array.Empty<object>();
        }

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));
                _items[index] = value;
                _version++;
            }
        }

        public int Count => _size;
        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public int Add(object value)
        {
            EnsureCapacity(_size + 1);
            _items[_size] = value;
            _version++;
            return _size++;
        }

        public void Clear()
        {
            Array.Clear(_items, 0, _size);
            _size = 0;
            _version++;
        }

        public bool Contains(object value)
        {
            return IndexOf(value) != -1;
        }

        public void CopyTo(Array array, int index)
        {
            Array.Copy(_items, 0, array, index, _size);
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int IndexOf(object value)
        {
            for (int i = 0; i < _size; i++)
            {
                if (Equals(_items[i], value)) return i;
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            if (index < 0 || index > _size) throw new ArgumentOutOfRangeException(nameof(index));

            EnsureCapacity(_size + 1);
            if (index < _size) Array.Copy(_items, index, _items, index + 1, _size - index);

            _items[index] = value;
            _size++;
            _version++;
        }

        public void Remove(object value)
        {
            int index = IndexOf(value);
            if (index >= 0) RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));

            _size--;
            if (index < _size) Array.Copy(_items, index + 1, _items, index, _size - index);

            _items[_size] = null;
            _version++;
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length >= min) return;

            int newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;
            if (newCapacity < min) newCapacity = min;

            Array.Resize(ref _items, newCapacity);
        }

        private class Enumerator : IEnumerator
        {
            private SimpleList _list;
            private int _index;
            private int _version;
            private object _current;

            public Enumerator(SimpleList list)
            {
                _list = list;
                _index = 0;
                _version = list._version;
                _current = null;
            }

            public object Current => _current;

            public bool MoveNext()
            {
                if (_version != _list._version) throw new InvalidOperationException("Collection was modified");

                if (_index < _list._size)
                {
                    _current = _list._items[_index];
                    _index++;
                    return true;
                }
                _current = null;
                return false;
            }

            public void Reset()
            {
                if (_version != _list._version) throw new InvalidOperationException("Collection was modified");
                _index = 0;
                _current = null;
            }
        }
    }
}
