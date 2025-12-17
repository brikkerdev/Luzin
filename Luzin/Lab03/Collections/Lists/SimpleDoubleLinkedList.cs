using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab03
{
    public class SimpleDoubleLinkedList<T> : IList<T>
    {
        private class Node
        {
            public T Value;
            public Node Next;
            public Node Previous;

            public Node(T value)
            {
                Value = value;
            }
        }

        private Node _head;
        private Node _tail;
        private int _count;
        private int _version;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
                return GetNodeAt(index).Value;
            }
            set
            {
                if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
                GetNodeAt(index).Value = value;
                _version++;
            }
        }

        public int Count => _count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            AddLast(item);
        }

        public void AddFirst(T item)
        {
            Node newNode = new Node(item);

            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                newNode.Next = _head;
                _head.Previous = newNode;
                _head = newNode;
            }

            _count++;
            _version++;
        }

        public void AddLast(T item)
        {
            Node newNode = new Node(item);

            if (_tail == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail.Next = newNode;
                newNode.Previous = _tail;
                _tail = newNode;
            }

            _count++;
            _version++;
        }

        public void Clear()
        {
            Node current = _head;
            while (current != null)
            {
                Node next = current.Next;
                current.Previous = null;
                current.Next = null;
                current = next;
            }

            _head = null;
            _tail = null;
            _count = 0;
            _version++;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _count) throw new ArgumentException("Destination array is too small");

            Node current = _head;
            while (current != null)
            {
                array[arrayIndex++] = current.Value;
                current = current.Next;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            Node current = _head;
            int index = 0;

            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item)) return index;
                current = current.Next;
                index++;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _count) throw new ArgumentOutOfRangeException(nameof(index));

            if (index == 0)
            {
                AddFirst(item);
            }
            else if (index == _count)
            {
                AddLast(item);
            }
            else
            {
                Node currentNode = GetNodeAt(index);
                Node newNode = new Node(item);

                newNode.Previous = currentNode.Previous;
                newNode.Next = currentNode;
                currentNode.Previous.Next = newNode;
                currentNode.Previous = newNode;

                _count++;
                _version++;
            }
        }

        public bool Remove(T item)
        {
            Node current = _head;

            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item))
                {
                    RemoveNode(current);
                    return true;
                }
                current = current.Next;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
            Node nodeToRemove = GetNodeAt(index);
            RemoveNode(nodeToRemove);
        }

        public T RemoveFirst()
        {
            if (_head == null) throw new InvalidOperationException("List is empty");

            T value = _head.Value;
            RemoveNode(_head);
            return value;
        }

        public T RemoveLast()
        {
            if (_tail == null) throw new InvalidOperationException("List is empty");

            T value = _tail.Value;
            RemoveNode(_tail);
            return value;
        }

        private Node GetNodeAt(int index)
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));

            if (index < _count / 2)
            {
                Node current = _head;
                for (int i = 0; i < index; i++)
                {
                    current = current.Next;
                }
                return current;
            }
            else
            {
                Node current = _tail;
                for (int i = _count - 1; i > index; i--)
                {
                    current = current.Previous;
                }
                return current;
            }
        }

        private void RemoveNode(Node node)
        {
            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            else
            {
                _head = node.Next;
            }

            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
            else
            {
                _tail = node.Previous;
            }

            node.Previous = null;
            node.Next = null;
            _count--;
            _version++;
        }

        private class Enumerator : IEnumerator<T>
        {
            private SimpleDoubleLinkedList<T> _list;
            private Node _current;
            private int _version;
            private bool _started;

            public Enumerator(SimpleDoubleLinkedList<T> list)
            {
                _list = list;
                _version = list._version;
                _current = null;
                _started = false;
            }

            public T Current => _current.Value;
            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_version != _list._version) throw new InvalidOperationException("Collection was modified");

                if (!_started)
                {
                    _current = _list._head;
                    _started = true;
                }
                else
                {
                    _current = _current?.Next;
                }

                return _current != null;
            }

            public void Reset()
            {
                if (_version != _list._version) throw new InvalidOperationException("Collection was modified");
                _current = null;
                _started = false;
            }
        }
    }
}
