using System.Diagnostics;
using Xunit;

namespace Lab02
{
    public class StackPerformanceTests : CollectionPerformanceTestBase
    {
        [Fact]
        public void Stack_Performance()
        {
            Console.WriteLine("\n--- Stack<int> ---");

            var stack = CreateAndFillStack(out var pushMs);
            Console.WriteLine($"Push: {pushMs:F2} ms");

            var (popMs, removed) = MeasurePop(stack);
            Console.WriteLine($"Pop: {popMs:F6} ms");

            var searchMs = MeasureSearchByValue(stack, removed);
            Console.WriteLine($"SearchByValue: {searchMs:F4} ms");
        }

        private Stack<int> CreateAndFillStack(out double elapsedMs)
        {
            var stack = new Stack<int>();

            var sw = Stopwatch.StartNew();
            foreach (var item in _testData) stack.Push(item);
            sw.Stop();

            elapsedMs = sw.Elapsed.TotalMilliseconds;
            Assert.Equal(CollectionSize, stack.Count);
            return stack;
        }

        private static (double elapsedMs, int removed) MeasurePop(Stack<int> stack)
        {
            int before = stack.Count;

            var sw = Stopwatch.StartNew();
            int removed = stack.Pop();
            sw.Stop();

            Assert.Equal(before - 1, stack.Count);
            return (sw.Elapsed.TotalMilliseconds, removed);
        }

        private double MeasureSearchByValue(Stack<int> stack, int removed)
        {
            int valueToFind = _testData[50000];

            var sw = Stopwatch.StartNew();
            bool found = stack.Contains(valueToFind);
            sw.Stop();

            Assert.True(found || removed == valueToFind);
            return sw.Elapsed.TotalMilliseconds;
        }
    }
}