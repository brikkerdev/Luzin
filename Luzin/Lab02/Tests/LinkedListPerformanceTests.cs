using System.Diagnostics;
using Xunit;

namespace Lab02
{
    public class LinkedListPerformanceTests : CollectionPerformanceTestBase
    {
        [Fact]
        public void LinkedList_Performance()
        {
            Console.WriteLine("\n--- LinkedList<int> ---");

            var linkedList = CreateAndFillLinkedList(out var addToEndMs);
            Console.WriteLine($"AddToEnd: {addToEndMs:F2} ms");

            var addBeginningMs = MeasureAddToBeginning(linkedList);
            Console.WriteLine($"AddToBeginning: {addBeginningMs:F4} ms");

            var addMiddleMs = MeasureAddToMiddle(linkedList);
            Console.WriteLine($"AddToMiddle: {addMiddleMs:F4} ms");

            var removeBeginningMs = MeasureRemoveFromBeginning(linkedList);
            Console.WriteLine($"RemoveFromBeginning: {removeBeginningMs:F4} ms");

            var removeEndMs = MeasureRemoveFromEnd(linkedList);
            Console.WriteLine($"RemoveFromEnd: {removeEndMs:F4} ms");

            var removeMiddleMs = MeasureRemoveFromMiddle(linkedList);
            Console.WriteLine($"RemoveFromMiddle: {removeMiddleMs:F4} ms");

            var searchMs = MeasureSearchByValue(linkedList);
            Console.WriteLine($"SearchByValue: {searchMs:F4} ms");
        }

        private LinkedList<int> CreateAndFillLinkedList(out double elapsedMs)
        {
            var linkedList = new LinkedList<int>();

            var sw = Stopwatch.StartNew();
            foreach (var item in _testData) linkedList.AddLast(item);
            sw.Stop();

            elapsedMs = sw.Elapsed.TotalMilliseconds;
            Assert.Equal(CollectionSize, linkedList.Count);
            return linkedList;
        }

        private static double MeasureAddToBeginning(LinkedList<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list.AddFirst(-1);
            sw.Stop();

            Assert.Equal(before + 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureAddToMiddle(LinkedList<int> list)
        {
            int before = list.Count;
            var middleNode = GetNodeAtPosition(list, list.Count / 2);
            Assert.NotNull(middleNode);

            var sw = Stopwatch.StartNew();
            list.AddAfter(middleNode!, -2);
            sw.Stop();

            Assert.Equal(before + 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveFromBeginning(LinkedList<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list.RemoveFirst();
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveFromEnd(LinkedList<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list.RemoveLast();
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveFromMiddle(LinkedList<int> list)
        {
            int before = list.Count;
            var middleNode = GetNodeAtPosition(list, list.Count / 2);
            Assert.NotNull(middleNode);

            var sw = Stopwatch.StartNew();
            list.Remove(middleNode!);
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private double MeasureSearchByValue(LinkedList<int> list)
        {
            int valueToFind = _testData[50000];

            var sw = Stopwatch.StartNew();
            bool found = list.Contains(valueToFind);
            sw.Stop();

            Assert.True(found);
            return sw.Elapsed.TotalMilliseconds;
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