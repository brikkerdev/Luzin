using System.Diagnostics;
using System.Collections.Immutable;
using Xunit;

namespace Lab2
{
    public class CollectionPerformanceTests
    {
        private const int CollectionSize = 100000;

        private readonly List<int> _testData;

        public CollectionPerformanceTests()
        {
            var random = new Random(42);
            _testData = Enumerable.Range(0, CollectionSize)
                .Select(_ => random.Next(1, 1000000))
                .ToList();
        }

        [Fact]
        public void List_Performance()
        {
            Console.WriteLine("\n--- List<int> ---");
            var results = new Dictionary<string, double>();

            var list = new List<int>(capacity: CollectionSize);

            // Add to end
            var sw = Stopwatch.StartNew();
            foreach (var item in _testData) list.Add(item);
            sw.Stop();
            results["AddToEnd"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"AddToEnd: {results["AddToEnd"]:F2} ms");
            Assert.Equal(CollectionSize, list.Count);

            // Add to beginning
            sw.Restart();
            list.Insert(0, -1);
            sw.Stop();
            results["AddToBeginning"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"AddToBeginning: {results["AddToBeginning"]:F4} ms");
            Assert.Equal(CollectionSize + 1, list.Count);

            // Add to middle
            sw.Restart();
            list.Insert(list.Count / 2, -2);
            sw.Stop();
            results["AddToMiddle"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"AddToMiddle: {results["AddToMiddle"]:F4} ms");
            Assert.Equal(CollectionSize + 2, list.Count);

            // Remove from beginning
            sw.Restart();
            list.RemoveAt(0);
            sw.Stop();
            results["RemoveFromBeginning"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveFromBeginning: {results["RemoveFromBeginning"]:F4} ms");
            Assert.Equal(CollectionSize + 1, list.Count);

            // Remove from end
            sw.Restart();
            list.RemoveAt(list.Count - 1);
            sw.Stop();
            results["RemoveFromEnd"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveFromEnd: {results["RemoveFromEnd"]:F4} ms");
            Assert.Equal(CollectionSize, list.Count);

            // Remove from middle
            sw.Restart();
            list.RemoveAt(list.Count / 2);
            sw.Stop();
            results["RemoveFromMiddle"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveFromMiddle: {results["RemoveFromMiddle"]:F4} ms");
            Assert.Equal(CollectionSize - 1, list.Count);

            // Search by value
            int valueToFind = _testData[50000];
            sw.Restart();
            bool found = list.Contains(valueToFind);
            sw.Stop();
            results["SearchByValue"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"SearchByValue: {results["SearchByValue"]:F4} ms");
            Assert.True(found);

            // Get by index
            int index = Math.Min(50000, list.Count - 1);
            sw.Restart();
            int element = list[index];
            sw.Stop();
            results["GetByIndex"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"GetByIndex: {results["GetByIndex"]:F6} ms");
            Assert.Equal(list[index], element);
        }

        [Fact]
        public void LinkedList_Performance()
        {
            Console.WriteLine("\n--- LinkedList<int> ---");
            var results = new Dictionary<string, double>();

            var linkedList = new LinkedList<int>();

            // Add to end
            var sw = Stopwatch.StartNew();
            foreach (var item in _testData) linkedList.AddLast(item);
            sw.Stop();
            results["AddToEnd"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"AddToEnd: {results["AddToEnd"]:F2} ms");
            Assert.Equal(CollectionSize, linkedList.Count);

            // Add to beginning
            sw.Restart();
            linkedList.AddFirst(-1);
            sw.Stop();
            results["AddToBeginning"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"AddToBeginning: {results["AddToBeginning"]:F4} ms");
            Assert.Equal(CollectionSize + 1, linkedList.Count);

            // Add to middle
            var middleNode = GetNodeAtPosition(linkedList, linkedList.Count / 2);
            Assert.NotNull(middleNode);

            sw.Restart();
            linkedList.AddAfter(middleNode!, -2);
            sw.Stop();
            results["AddToMiddle"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"AddToMiddle: {results["AddToMiddle"]:F4} ms");
            Assert.Equal(CollectionSize + 2, linkedList.Count);

            // Remove from beginning
            sw.Restart();
            linkedList.RemoveFirst();
            sw.Stop();
            results["RemoveFromBeginning"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveFromBeginning: {results["RemoveFromBeginning"]:F4} ms");
            Assert.Equal(CollectionSize + 1, linkedList.Count);

            // Remove from end
            sw.Restart();
            linkedList.RemoveLast();
            sw.Stop();
            results["RemoveFromEnd"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveFromEnd: {results["RemoveFromEnd"]:F4} ms");
            Assert.Equal(CollectionSize, linkedList.Count);

            // Remove from middle
            middleNode = GetNodeAtPosition(linkedList, linkedList.Count / 2);
            Assert.NotNull(middleNode);

            sw.Restart();
            linkedList.Remove(middleNode!);
            sw.Stop();
            results["RemoveFromMiddle"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveFromMiddle: {results["RemoveFromMiddle"]:F4} ms");
            Assert.Equal(CollectionSize - 1, linkedList.Count);

            // Search by value
            int valueToFind = _testData[50000];
            sw.Restart();
            bool found = linkedList.Contains(valueToFind);
            sw.Stop();
            results["SearchByValue"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"SearchByValue: {results["SearchByValue"]:F4} ms");
            Assert.True(found);
        }

        [Fact]
        public void Queue_Performance()
        {
            Console.WriteLine("\n--- Queue<int> ---");
            var results = new Dictionary<string, double>();

            var queue = new Queue<int>();

            // Enqueue
            var sw = Stopwatch.StartNew();
            foreach (var item in _testData) queue.Enqueue(item);
            sw.Stop();
            results["Enqueue"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"Enqueue: {results["Enqueue"]:F2} ms");
            Assert.Equal(CollectionSize, queue.Count);

            // Dequeue
            sw.Restart();
            int removed = queue.Dequeue();
            sw.Stop();
            results["Dequeue"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"Dequeue: {results["Dequeue"]:F6} ms");
            Assert.Equal(CollectionSize - 1, queue.Count);

            // Search by value
            int valueToFind = _testData[50000];
            sw.Restart();
            bool found = queue.Contains(valueToFind);
            sw.Stop();
            results["SearchByValue"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"SearchByValue: {results["SearchByValue"]:F4} ms");
            Assert.True(found || removed == valueToFind);
        }

        [Fact]
        public void Stack_Performance()
        {
            Console.WriteLine("\n--- Stack<int> ---");
            var results = new Dictionary<string, double>();

            var stack = new Stack<int>();

            // Push
            var sw = Stopwatch.StartNew();
            foreach (var item in _testData) stack.Push(item);
            sw.Stop();
            results["Push"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"Push: {results["Push"]:F2} ms");
            Assert.Equal(CollectionSize, stack.Count);

            // Pop
            sw.Restart();
            int removed = stack.Pop();
            sw.Stop();
            results["Pop"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"Pop: {results["Pop"]:F6} ms");
            Assert.Equal(CollectionSize - 1, stack.Count);

            // Search by value
            int valueToFind = _testData[50000];
            sw.Restart();
            bool found = stack.Contains(valueToFind);
            sw.Stop();
            results["SearchByValue"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"SearchByValue: {results["SearchByValue"]:F4} ms");
            Assert.True(found || removed == valueToFind);
        }

        [Fact]
        public void ImmutableList_Performance()
        {
            Console.WriteLine("\n--- ImmutableList<int> ---");
            var results = new Dictionary<string, double>();

            var immutableList = ImmutableList.Create<int>();

            // AddRange (add to end)
            var sw = Stopwatch.StartNew();
            immutableList = immutableList.AddRange(_testData);
            sw.Stop();
            results["AddRange"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"AddRange: {results["AddRange"]:F2} ms");
            Assert.Equal(CollectionSize, immutableList.Count);

            // Insert at beginning
            sw.Restart();
            immutableList = immutableList.Insert(0, -1);
            sw.Stop();
            results["InsertBeginning"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"InsertBeginning: {results["InsertBeginning"]:F4} ms");
            Assert.Equal(CollectionSize + 1, immutableList.Count);

            // Insert in middle
            sw.Restart();
            immutableList = immutableList.Insert(immutableList.Count / 2, -2);
            sw.Stop();
            results["InsertMiddle"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"InsertMiddle: {results["InsertMiddle"]:F4} ms");
            Assert.Equal(CollectionSize + 2, immutableList.Count);

            // Remove from beginning
            sw.Restart();
            immutableList = immutableList.RemoveAt(0);
            sw.Stop();
            results["RemoveBeginning"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveBeginning: {results["RemoveBeginning"]:F4} ms");
            Assert.Equal(CollectionSize + 1, immutableList.Count);

            // Remove from end
            sw.Restart();
            immutableList = immutableList.RemoveAt(immutableList.Count - 1);
            sw.Stop();
            results["RemoveEnd"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveEnd: {results["RemoveEnd"]:F4} ms");
            Assert.Equal(CollectionSize, immutableList.Count);

            // Remove from middle
            sw.Restart();
            immutableList = immutableList.RemoveAt(immutableList.Count / 2);
            sw.Stop();
            results["RemoveMiddle"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"RemoveMiddle: {results["RemoveMiddle"]:F4} ms");
            Assert.Equal(CollectionSize - 1, immutableList.Count);

            // Search by value
            int valueToFind = _testData[50000];
            sw.Restart();
            bool found = immutableList.Contains(valueToFind);
            sw.Stop();
            results["SearchByValue"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"SearchByValue: {results["SearchByValue"]:F4} ms");
            Assert.True(found);

            // Get by index
            int index = Math.Min(50000, immutableList.Count - 1);
            sw.Restart();
            int element = immutableList[index];
            sw.Stop();
            results["GetByIndex"] = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"GetByIndex: {results["GetByIndex"]:F6} ms");
            Assert.Equal(immutableList[index], element);
        }

        private static LinkedListNode<T>? GetNodeAtPosition<T>(LinkedList<T> list, int position)
        {
            if (position < 0 || position >= list.Count) return null;

            var current = list.First;
            for (int i = 0; i < position && current != null; i++)
                current = current.Next;

            return current;
        }
    }
}