using System;

namespace Lab02
{
    public abstract class CollectionPerformanceTestBase
    {
        protected const int CollectionSize = 100000;
        protected readonly List<int> _testData;

        protected CollectionPerformanceTestBase()
        {
            var random = new Random(42);
            _testData = Enumerable.Range(0, CollectionSize)
                .Select(_ => random.Next(1, 1000000))
                .ToList();
        }
    }
}