using System.Collections.Immutable;
using System.Diagnostics;
using Xunit;

namespace Lab02
{
    public class ImmutableListPerformanceTests : CollectionPerformanceTestBase
    {
        [Fact]
        public void ImmutableList_Performance()
        {
            Console.WriteLine("\n--- ImmutableList<int> ---");

            var immutableList = CreateAndFillImmutableList(out var addRangeMs);
            Console.WriteLine($"AddRange: {addRangeMs:F2} ms");

            var insertBeginningMs = MeasureInsertBeginning(ref immutableList);
            Console.WriteLine($"InsertBeginning: {insertBeginningMs:F4} ms");

            var insertMiddleMs = MeasureInsertMiddle(ref immutableList);
            Console.WriteLine($"InsertMiddle: {insertMiddleMs:F4} ms");

            var removeBeginningMs = MeasureRemoveBeginning(ref immutableList);
            Console.WriteLine($"RemoveBeginning: {removeBeginningMs:F4} ms");

            var removeEndMs = MeasureRemoveEnd(ref immutableList);
            Console.WriteLine($"RemoveEnd: {removeEndMs:F4} ms");

            var removeMiddleMs = MeasureRemoveMiddle(ref immutableList);
            Console.WriteLine($"RemoveMiddle: {removeMiddleMs:F4} ms");

            var searchMs = MeasureSearchByValue(immutableList);
            Console.WriteLine($"SearchByValue: {searchMs:F4} ms");

            var getByIndexMs = MeasureGetByIndex(immutableList);
            Console.WriteLine($"GetByIndex: {getByIndexMs:F6} ms");
        }

        private ImmutableList<int> CreateAndFillImmutableList(out double elapsedMs)
        {
            var immutableList = ImmutableList.Create<int>();

            var sw = Stopwatch.StartNew();
            immutableList = immutableList.AddRange(_testData);
            sw.Stop();

            elapsedMs = sw.Elapsed.TotalMilliseconds;
            Assert.Equal(CollectionSize, immutableList.Count);
            return immutableList;
        }

        private static double MeasureInsertBeginning(ref ImmutableList<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list = list.Insert(0, -1);
            sw.Stop();

            Assert.Equal(before + 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureInsertMiddle(ref ImmutableList<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list = list.Insert(list.Count / 2, -2);
            sw.Stop();

            Assert.Equal(before + 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveBeginning(ref ImmutableList<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list = list.RemoveAt(0);
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveEnd(ref ImmutableList<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list = list.RemoveAt(list.Count - 1);
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureRemoveMiddle(ref ImmutableList<int> list)
        {
            int before = list.Count;

            var sw = Stopwatch.StartNew();
            list = list.RemoveAt(list.Count / 2);
            sw.Stop();

            Assert.Equal(before - 1, list.Count);
            return sw.Elapsed.TotalMilliseconds;
        }

        private double MeasureSearchByValue(ImmutableList<int> list)
        {
            int valueToFind = _testData[50000];

            var sw = Stopwatch.StartNew();
            bool found = list.Contains(valueToFind);
            sw.Stop();

            Assert.True(found);
            return sw.Elapsed.TotalMilliseconds;
        }

        private static double MeasureGetByIndex(ImmutableList<int> list)
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