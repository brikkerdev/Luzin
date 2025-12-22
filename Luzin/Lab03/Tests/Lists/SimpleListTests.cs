using System;
using System.Collections;
using Xunit;

namespace Lab03
{
    public class SimpleListTests
    {
        [Fact]
        public void Add_IncreasesCount()
        {
            var list = new SimpleList();
            list.Add(10);
            list.Add("test");

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void Indexer_GetSet_WorksCorrectly()
        {
            var list = new SimpleList();
            list.Add(1);
            list.Add(2);

            list[0] = 100;

            Assert.Equal(100, list[0]);
            Assert.Equal(2, list[1]);
        }

        [Fact]
        public void Indexer_Get_InvalidIndex_ThrowsException()
        {
            var list = new SimpleList();
            list.Add(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => list[5]);
        }

        [Fact]
        public void Indexer_Set_InvalidIndex_ThrowsException()
        {
            var list = new SimpleList();
            list.Add(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => list[5] = 10);
        }

        [Fact]
        public void Contains_ReturnsTrue_WhenItemExists()
        {
            var list = new SimpleList();
            list.Add("apple");
            list.Add("banana");

            Assert.True(list.Contains("apple"));
            Assert.False(list.Contains("orange"));
        }

        [Fact]
        public void IndexOf_ReturnsCorrectIndex()
        {
            var list = new SimpleList();
            list.Add("first");
            list.Add("second");
            list.Add("third");

            Assert.Equal(1, list.IndexOf("second"));
            Assert.Equal(-1, list.IndexOf("fourth"));
        }

        [Fact]
        public void Insert_AddsItemAtSpecifiedPosition()
        {
            var list = new SimpleList();
            list.Add("a");
            list.Add("c");
            list.Insert(1, "b");

            Assert.Equal(3, list.Count);
            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);
            Assert.Equal("c", list[2]);
        }

        [Fact]
        public void Insert_InvalidIndex_ThrowsException()
        {
            var list = new SimpleList();

            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(-1, "item"));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(1, "item"));
        }

        [Fact]
        public void Remove_DeletesFirstOccurrence()
        {
            var list = new SimpleList();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(2);

            list.Remove(2);

            Assert.Equal(3, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(3, list[1]);
            Assert.Equal(2, list[2]);
        }

        [Fact]
        public void RemoveAt_DeletesItemAtIndex()
        {
            var list = new SimpleList();
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
            var list = new SimpleList();
            list.Add(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(1));
        }

        [Fact]
        public void Clear_RemovesAllItems()
        {
            var list = new SimpleList();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            list.Clear();

            Assert.Equal(0, list.Count);
        }

        [Fact]
        public void CopyTo_CopiesElementsToArray()
        {
            var list = new SimpleList();
            list.Add(10);
            list.Add(20);
            list.Add(30);

            object[] array = new object[5];
            list.CopyTo(array, 1);

            Assert.Null(array[0]);
            Assert.Equal(10, array[1]);
            Assert.Equal(20, array[2]);
            Assert.Equal(30, array[3]);
            Assert.Null(array[4]);
        }

        [Fact]
        public void CopyTo_NullArray_ThrowsException()
        {
            var list = new SimpleList();
            list.Add(1);

            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
        }

        [Fact]
        public void CopyTo_InvalidIndex_ThrowsException()
        {
            var list = new SimpleList();
            list.Add(1);
            object[] array = new object[5];

            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(array, -1));
            Assert.Throws<ArgumentException>(() => list.CopyTo(array, 5));
        }

        [Fact]
        public void GetEnumerator_EnumeratesAllItems()
        {
            var list = new SimpleList();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            int sum = 0;
            foreach (object item in list)
            {
                sum += (int)item;
            }

            Assert.Equal(6, sum);
        }

        [Fact]
        public void GetEnumerator_Reset_WorksCorrectly()
        {
            var list = new SimpleList();
            list.Add("a");
            list.Add("b");

            var enumerator = list.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal("a", enumerator.Current);

            enumerator.Reset();
            Assert.True(enumerator.MoveNext());
            Assert.Equal("a", enumerator.Current);
        }

        [Fact]
        public void IsFixedSize_ReturnsFalse()
        {
            var list = new SimpleList();
            Assert.False(list.IsFixedSize);
        }

        [Fact]
        public void IsReadOnly_ReturnsFalse()
        {
            var list = new SimpleList();
            Assert.False(list.IsReadOnly);
        }

        [Fact]
        public void IsSynchronized_ReturnsFalse()
        {
            var list = new SimpleList();
            Assert.False(list.IsSynchronized);
        }

        [Fact]
        public void SyncRoot_ReturnsThis()
        {
            var list = new SimpleList();
            Assert.Same(list, list.SyncRoot);
        }
    }
}
