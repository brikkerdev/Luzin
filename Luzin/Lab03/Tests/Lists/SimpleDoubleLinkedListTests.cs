using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab03
{
    public class SimpleDoubleLinkedListTests
    {
        [Fact]
        public void Add_IncreasesCount()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);
            list.Add(2);

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void AddFirst_AddsToBeginning()
        {
            var list = new SimpleDoubleLinkedList<string>();
            list.AddFirst("b");
            list.AddFirst("a");

            Assert.Equal(2, list.Count);
            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);
        }

        [Fact]
        public void AddLast_AddsToEnd()
        {
            var list = new SimpleDoubleLinkedList<string>();
            list.AddLast("a");
            list.AddLast("b");
            list.AddLast("c");

            Assert.Equal(3, list.Count);
            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);
            Assert.Equal("c", list[2]);
        }

        [Fact]
        public void Indexer_GetSet_WorksCorrectly()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            list[1] = 20;

            Assert.Equal(1, list[0]);
            Assert.Equal(20, list[1]);
            Assert.Equal(3, list[2]);
        }

        [Fact]
        public void Indexer_Get_InvalidIndex_ThrowsException()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => list[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
        }

        [Fact]
        public void Indexer_Set_InvalidIndex_ThrowsException()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => list[1] = 2);
        }

        [Fact]
        public void Contains_ReturnsCorrectValue()
        {
            var list = new SimpleDoubleLinkedList<string>();
            list.Add("apple");
            list.Add("banana");

            Assert.True(list.Contains("apple"));
            Assert.False(list.Contains("orange"));
        }

        [Fact]
        public void IndexOf_ReturnsCorrectIndex()
        {
            var list = new SimpleDoubleLinkedList<string>();
            list.Add("a");
            list.Add("b");
            list.Add("c");
            list.Add("b");

            Assert.Equal(1, list.IndexOf("b"));
            Assert.Equal(-1, list.IndexOf("d"));
        }

        [Fact]
        public void Insert_AddsItemAtPosition()
        {
            var list = new SimpleDoubleLinkedList<string>();
            list.Add("a");
            list.Add("c");
            list.Insert(1, "b");

            Assert.Equal(3, list.Count);
            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);
            Assert.Equal("c", list[2]);
        }

        [Fact]
        public void Insert_AtBeginning_Works()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(2);
            list.Insert(0, 1);

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
        }

        [Fact]
        public void Insert_AtEnd_Works()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);
            list.Insert(1, 2);

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
        }

        [Fact]
        public void Insert_InvalidIndex_ThrowsException()
        {
            var list = new SimpleDoubleLinkedList<int>();

            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(1, 1));
        }

        [Fact]
        public void Remove_DeletesItem()
        {
            var list = new SimpleDoubleLinkedList<string>();
            list.Add("a");
            list.Add("b");
            list.Add("c");

            bool removed = list.Remove("b");

            Assert.True(removed);
            Assert.Equal(2, list.Count);
            Assert.Equal("a", list[0]);
            Assert.Equal("c", list[1]);
        }

        [Fact]
        public void Remove_NonExistent_ReturnsFalse()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);

            Assert.False(list.Remove(2));
            Assert.Equal(1, list.Count);
        }

        [Fact]
        public void RemoveAt_DeletesItemAtIndex()
        {
            var list = new SimpleDoubleLinkedList<string>();
            list.Add("red");
            list.Add("green");
            list.Add("blue");

            list.RemoveAt(1);

            Assert.Equal(2, list.Count);
            Assert.Equal("red", list[0]);
            Assert.Equal("blue", list[1]);
        }

        [Fact]
        public void RemoveAt_InvalidIndex_ThrowsException()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(1));
        }

        [Fact]
        public void RemoveFirst_RemovesAndReturnsFirstItem()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            int first = list.RemoveFirst();

            Assert.Equal(1, first);
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list[0]);
            Assert.Equal(3, list[1]);
        }

        [Fact]
        public void RemoveFirst_EmptyList_ThrowsException()
        {
            var list = new SimpleDoubleLinkedList<string>();

            Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
        }

        [Fact]
        public void RemoveLast_RemovesAndReturnsLastItem()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            int last = list.RemoveLast();

            Assert.Equal(3, last);
            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
        }

        [Fact]
        public void RemoveLast_EmptyList_ThrowsException()
        {
            var list = new SimpleDoubleLinkedList<string>();

            Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
        }

        [Fact]
        public void Clear_RemovesAllItems()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            list.Clear();

            Assert.Equal(0, list.Count);
            Assert.False(list.Contains(1));
        }

        [Fact]
        public void CopyTo_CopiesElements()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(10);
            list.Add(20);
            list.Add(30);

            int[] array = new int[5];
            list.CopyTo(array, 1);

            Assert.Equal(0, array[0]);
            Assert.Equal(10, array[1]);
            Assert.Equal(20, array[2]);
            Assert.Equal(30, array[3]);
            Assert.Equal(0, array[4]);
        }

        [Fact]
        public void CopyTo_NullArray_ThrowsException()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);

            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
        }

        [Fact]
        public void CopyTo_InvalidIndex_ThrowsException()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);
            int[] array = new int[5];

            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(array, -1));
            Assert.Throws<ArgumentException>(() => list.CopyTo(array, 5));
        }

        [Fact]
        public void GetEnumerator_EnumeratesAllItems()
        {
            var list = new SimpleDoubleLinkedList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            int sum = 0;
            foreach (int item in list)
            {
                sum += item;
            }

            Assert.Equal(6, sum);
        }

        [Fact]
        public void GetEnumerator_Reset_WorksCorrectly()
        {
            var list = new SimpleDoubleLinkedList<string>();
            list.Add("a");
            list.Add("b");

            var enumerator = list.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            enumerator.Reset();
            Assert.True(enumerator.MoveNext());
        }

        [Fact]
        public void IsReadOnly_ReturnsFalse()
        {
            var list = new SimpleDoubleLinkedList<int>();
            Assert.False(list.IsReadOnly);
        }
    }
}
