using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lab3
{
    public class SimpleDictionaryTests
    {
        [Fact]
        public void Constructor_WithCapacity_CreatesDictionary()
        {
            var dict = new SimpleDictionary<string, int>(10);
            Assert.Equal(0, dict.Count);
        }

        [Fact]
        public void Constructor_WithComparer_UsesComparer()
        {
            var dict = new SimpleDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            dict.Add("KEY", 1);

            Assert.True(dict.ContainsKey("key"));
            Assert.Equal(1, dict["key"]);
        }

        [Fact]
        public void Add_KeyValuePair_IncreasesCount()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("one", 1);
            dict.Add("two", 2);

            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void Add_DuplicateKey_ThrowsException()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("key", 1);

            var exception = Assert.Throws<ArgumentException>(() => dict.Add("key", 2));
            Assert.Contains("same key", exception.Message);
        }

        [Fact]
        public void Indexer_GetSet_WorksCorrectly()
        {
            var dict = new SimpleDictionary<string, string>();
            dict["name"] = "John";
            dict["age"] = "30";

            Assert.Equal("John", dict["name"]);
            Assert.Equal("30", dict["age"]);

            dict["name"] = "Jane";
            Assert.Equal("Jane", dict["name"]);
        }

        [Fact]
        public void Indexer_Get_NonExistentKey_ThrowsException()
        {
            var dict = new SimpleDictionary<int, string>();

            Assert.Throws<KeyNotFoundException>(() => dict[99]);
        }

        [Fact]
        public void ContainsKey_ReturnsCorrectValue()
        {
            var dict = new SimpleDictionary<int, bool>();
            dict.Add(1, true);
            dict.Add(2, false);

            Assert.True(dict.ContainsKey(1));
            Assert.True(dict.ContainsKey(2));
            Assert.False(dict.ContainsKey(3));
        }

        [Fact]
        public void Contains_KeyValuePair_ReturnsCorrectValue()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            Assert.True(dict.Contains(new KeyValuePair<string, int>("a", 1)));
            Assert.False(dict.Contains(new KeyValuePair<string, int>("a", 2)));
            Assert.False(dict.Contains(new KeyValuePair<string, int>("c", 3)));
        }

        [Fact]
        public void Remove_Key_DeletesEntry()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("one", 1);
            dict.Add("two", 2);
            dict.Add("three", 3);

            bool removed = dict.Remove("two");

            Assert.True(removed);
            Assert.Equal(2, dict.Count);
            Assert.False(dict.ContainsKey("two"));
            Assert.True(dict.ContainsKey("one"));
            Assert.True(dict.ContainsKey("three"));
        }

        [Fact]
        public void Remove_NonExistentKey_ReturnsFalse()
        {
            var dict = new SimpleDictionary<int, string>();
            dict.Add(1, "a");

            Assert.False(dict.Remove(99));
            Assert.Equal(1, dict.Count);
        }

        [Fact]
        public void Remove_KeyValuePair_DeletesWhenMatch()
        {
            var dict = new SimpleDictionary<string, string>();
            dict.Add("a", "apple");
            dict.Add("b", "banana");

            Assert.True(dict.Remove(new KeyValuePair<string, string>("a", "apple")));
            Assert.False(dict.Remove(new KeyValuePair<string, string>("b", "wrong")));
            Assert.Equal(1, dict.Count);
        }

        [Fact]
        public void TryGetValue_ReturnsCorrectValue()
        {
            var dict = new SimpleDictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");

            Assert.True(dict.TryGetValue(1, out string value1));
            Assert.Equal("one", value1);

            Assert.False(dict.TryGetValue(3, out string value3));
            Assert.Null(value3);
        }

        [Fact]
        public void Clear_RemovesAllItems()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);

            dict.Clear();

            Assert.Equal(0, dict.Count);
            Assert.False(dict.ContainsKey("a"));
        }

        [Fact]
        public void Keys_Collection_ContainsAllKeys()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("apple", 1);
            dict.Add("banana", 2);
            dict.Add("orange", 3);

            var keys = dict.Keys;

            Assert.Equal(3, keys.Count);
            Assert.Contains("apple", keys);
            Assert.Contains("banana", keys);
            Assert.Contains("orange", keys);
            Assert.DoesNotContain("grape", keys);
        }

        [Fact]
        public void Values_Collection_ContainsAllValues()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 10);
            dict.Add("b", 20);
            dict.Add("c", 30);

            var values = dict.Values;

            Assert.Equal(3, values.Count);
            Assert.Contains(10, values);
            Assert.Contains(20, values);
            Assert.Contains(30, values);
            Assert.DoesNotContain(40, values);
        }

        [Fact]
        public void CopyTo_CopiesKeyValuePairs()
        {
            var dict = new SimpleDictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");
            dict.Add(3, "three");

            var array = new KeyValuePair<int, string>[5];
            dict.CopyTo(array, 1);

            Assert.Equal(default(KeyValuePair<int, string>), array[0]);

            var copiedItems = array.Skip(1).Take(3).ToList();
            Assert.Contains(new KeyValuePair<int, string>(1, "one"), copiedItems);
            Assert.Contains(new KeyValuePair<int, string>(2, "two"), copiedItems);
            Assert.Contains(new KeyValuePair<int, string>(3, "three"), copiedItems);

            Assert.Equal(default(KeyValuePair<int, string>), array[4]);
        }

        [Fact]
        public void CopyTo_NullArray_ThrowsException()
        {
            var dict = new SimpleDictionary<int, string>();
            dict.Add(1, "a");

            Assert.Throws<ArgumentNullException>(() => dict.CopyTo(null, 0));
        }

        [Fact]
        public void GetEnumerator_EnumeratesAllKeyValuePairs()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);

            int sum = 0;
            foreach (var kvp in dict)
            {
                sum += kvp.Value;
            }

            Assert.Equal(6, sum);
        }

        [Fact]
        public void GetEnumerator_Reset_WorksCorrectly()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            var enumerator = dict.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            enumerator.Reset();
            Assert.True(enumerator.MoveNext());
        }

        [Fact]
        public void IsReadOnly_ReturnsFalse()
        {
            var dict = new SimpleDictionary<int, string>();
            Assert.False(dict.IsReadOnly);
        }

        [Fact]
        public void IReadOnlyDictionary_Keys_Works()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            IReadOnlyDictionary<string, int> readOnlyDict = dict;
            var keys = readOnlyDict.Keys;

            Assert.Equal(2, keys.Count());
            Assert.Contains("a", keys);
            Assert.Contains("b", keys);
        }

        [Fact]
        public void IReadOnlyDictionary_Values_Works()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            IReadOnlyDictionary<string, int> readOnlyDict = dict;
            var values = readOnlyDict.Values;

            Assert.Equal(2, values.Count());
            Assert.Contains(1, values);
            Assert.Contains(2, values);
        }
    }
}
