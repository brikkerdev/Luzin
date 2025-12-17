using System.Diagnostics;
using Xunit;

namespace Lab02
{
    public class ListPerformanceTests : CollectionPerformanceTestBase
    {
        [Fact]
        public void List_Performance()
        {
            Console.WriteLine("\n--- List<int> ---");

            var list = CreateAndFillList(out var addToEndMs);
            Console.WriteLine($"AddToEnd: {addToEndMs:F2} ms");

            var addBeginningMs = MeasureAddToBeginning(list);
            Console.WriteLine($"AddToBeginning: {addBeginningMs:F4} ms");

            var addMiddleMs = MeasureAddToMiddle(list);
            Console.WriteLine($"AddToMiddle: {addMiddleMs:F4} ms");

            var removeBeginningMs = MeasureRemoveFromBeginning(list);
            Console.WriteLine($"RemoveFromBeginning: {removeBeginningMs:F4} ms");

            var removeEndMs = MeasureRemoveFromEnd(list);
            Console.WriteLine($"RemoveFromEnd: {removeEndMs:F4} ms");

            var removeMiddleMs = MeasureRemoveFromMiddle(list);
            Console.WriteLine($"RemoveFromMiddle: {removeMiddleMs:F4} ms");

            var searchMs = MeasureSearchByValue(list);
            Console.WriteLine($"SearchByValue: {searchMs:F4} ms");

            var getByIndexMs = MeasureGetByIndex(list);
            Console.WriteLine($"GetByIndex: {getByIndexMs:F6} ms");
        }

        private List<int> CreateAndFillList(out double elapsedMs)
        {
            var list = new List<int>(capacity: CollectionSize);

            var sw = Stopwatch.StartNew();
            foreach (var item in _testData) list.Add(item);
            sw.Stop();

            elapsedMs = sw.Elapsed.TotalMilliseconds;
            Assert.Equal(CollectionSize, list.Count);
            return list;
        }

        private static double MeasureAddToBeginning(List<int> list)
        {
            var sw = Stopwatch.StartNew();
            list.Insert(0, -1);
            sw.Stop();

            Assert.True(list.Count > 0);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureAddToMiddle(List<int> list)
        {
            var sw = Stopwatch.StartNew();
            list.Insert(list.Count / 2, -2);
            sw.Stop();

            Assert.True(list.Count > 1);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveFromBeginning(List<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list.RemoveAt(0);
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveFromEnd(List<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list.RemoveAt(list.Count - 1);
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveFromMiddle(List<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list.RemoveAt(list.Count / 2);
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private double MeasureSearchByValue(List<int> list)
        {
            int valueToFind = _testData[50000];

            var sw = Stopwatch.StartNew();
            bool found = list.Contains(valueToFind);
            sw.Stop();

            Assert.True(found);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureGetByIndex(List<int> list)
        {
            int index = Math.Min(50000, list.Count - 1);

            var sw = Stopwatch.StartNew();
            int element = list[index];
            sw.Stop();

            Assert.Equal(list[index], element);
            return sw.Elapsed.TotalMilliseconds;
        }
    }
}