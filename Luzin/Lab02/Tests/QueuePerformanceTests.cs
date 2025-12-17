using System.Diagnostics;
using Xunit;

namespace Lab02
{
    public class QueuePerformanceTests : CollectionPerformanceTestBase
    {
        [Fact]
        public void Queue_Performance()
        {
            Console.WriteLine("\n--- Queue<int> ---");

            var queue = CreateAndFillQueue(out var enqueueMs);
            Console.WriteLine($"Enqueue: {enqueueMs:F2} ms");

            var (dequeueMs, removed) = MeasureDequeue(queue);
            Console.WriteLine($"Dequeue: {dequeueMs:F6} ms");

            var searchMs = MeasureSearchByValue(queue, removed);
            Console.WriteLine($"SearchByValue: {searchMs:F4} ms");
        }

        private Queue<int> CreateAndFillQueue(out double elapsedMs)
        {
            var queue = new Queue<int>();

            var sw = Stopwatch.StartNew();
            foreach (var item in _testData) queue.Enqueue(item);
            sw.Stop();

            elapsedMs = sw.Elapsed.TotalMilliseconds;
            Assert.Equal(CollectionSize, queue.Count);
            return queue;
        }

        private static (double elapsedMs, int removed) MeasureDequeue(Queue<int> queue)
        {
            int before = queue.Count;

            var sw = Stopwatch.StartNew();
            int removed = queue.Dequeue();
            sw.Stop();

            Assert.Equal(before - 1, queue.Count);
            return (sw.Elapsed.TotalMilliseconds, removed);
        }

        private double MeasureSearchByValue(Queue<int> queue, int removed)
        {
            int valueToFind = _testData[50000];

            var sw = Stopwatch.StartNew();
            bool found = queue.Contains(valueToFind);
            sw.Stop();

            Assert.True(found || removed == valueToFind);
            return sw.Elapsed.TotalMilliseconds;
        }
    }
}