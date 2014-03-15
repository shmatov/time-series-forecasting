using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesCollection
{
    public static class TimeSeriesExtensions
    {
        public static IEnumerable<double?> Average(this IEnumerable<double?> series, int windowSize = 3)
        {
            return new SMACalculator(series, windowSize).Calculate();
        }

        public static IEnumerable<double?> Interpolate(this IEnumerable<double?> series)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<double> CalculateF(this IEnumerable<double> series,
            IEnumerable<int> specialPointsIndices, int radius = 3)
        {
            return new SpecialPointsAverageCalculator(series, specialPointsIndices, radius).Calculate();
        }
    }

    internal class SMACalculator
    {
        private readonly int _movingWindowSize;
        private readonly IEnumerable<double?> _series;

        public SMACalculator(IEnumerable<double?> series, int movingWindowSize)
        {
            _series = series;
            _movingWindowSize = movingWindowSize;
        }

        public IEnumerable<double?> Calculate()
        {
            var movingWindow = new MovingWindow(_movingWindowSize);
            return _series.Aggregate(new List<double?>(),
                (results, element) =>
                {
                    movingWindow.Add(element);
                    results.Add(element.HasValue ? (double?) movingWindow.Average : null);
                    return results;
                });
        }

        private class MovingWindow
        {
            private readonly int _size;
            private int _hasValueCount;
            private double _sum;
            private readonly Queue<double?> _queue = new Queue<double?>();

            public double Average
            {
                get { return _sum/_hasValueCount; }
            }

            public MovingWindow(int size)
            {
                _size = size;
            }

            public void Add(double? element)
            {
                _queue.Enqueue(element);
                if (element.HasValue)
                {
                    _hasValueCount++;
                    _sum += element.Value;
                }

                if (_queue.Count > _size)
                {
                    element = _queue.Dequeue();
                    if (element.HasValue)
                    {
                        _hasValueCount--;
                        _sum -= element.Value;
                    }
                }
            }
        }
    }

    internal class SpecialPointsAverageCalculator
    {
        private readonly List<int> _offsets;
        private readonly List<double> _series;
        private readonly IEnumerable<int> _specialPointsIndices;

        public SpecialPointsAverageCalculator(IEnumerable<double> series, IEnumerable<int> specialPointsIndices,
            int radius)
        {
            _series = series.ToList();
            _specialPointsIndices = specialPointsIndices;
            _offsets = Enumerable.Range(-1*radius, radius).ToList();
        }

        private IEnumerable<IEnumerable<int>> SpecialPointsRegions
        {
            get { return _specialPointsIndices.Select(x => _offsets.Select(offset => x + offset)); }
        }

        private IEnumerable<List<double>> PointsGroups
        {
            get { return Enumerable.Range(0, _offsets.Count).Select(_ => new List<double>()).ToList(); }
        }

        private bool SeriesHasIndex(int index)
        {
            return index >= 0 && index < _series.Count;
        }

        private IEnumerable<List<double>> AggregateIntoGroups(IEnumerable<List<double>> groups, IEnumerable<int> indices)
        {
            return groups.Zip(indices, (group, index) => new {group, index}).Where(x => SeriesHasIndex(x.index))
                .Select(x =>
                {
                    x.group.Add(_series[x.index]);
                    return x.group;
                });
        }

        public IEnumerable<double> Calculate()
        {
            return SpecialPointsRegions.Aggregate(PointsGroups, AggregateIntoGroups)
                .Select(group => group.Average());
        }
    }
}