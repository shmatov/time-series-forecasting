using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesCollection
{
    public static class TimeSeriesExtensions
    {
        public static IEnumerable<double?> Average(this IEnumerable<double?> series)
        {
            return new SMACalculator(series).Calculate();
        }

        public static IEnumerable<double?> Interpolate(this IEnumerable<double?> series)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<double> CalculateF(this IEnumerable<double> series, IEnumerable<int> specialPointsIndices)
        {
            return new SpecialPointsAverageCalculator(series, specialPointsIndices).Calculate();
        }
    }

    internal class SMACalculator
    {
        private readonly int _movingWindowLength;
        private readonly IEnumerable<double?> _series;

        public SMACalculator(IEnumerable<double?> series)
        {
            _series = series;
            _movingWindowLength = 3;
        }

        public IEnumerable<double?> Calculate()
        {
            var movingWindow = new LinkedList<double?>();
            return _series.Aggregate(new List<double?>(),
                (results, element) =>
                {
                    movingWindow.AddLast(element);
                    if (movingWindow.Count > _movingWindowLength) movingWindow.RemoveFirst();

                    var average = element.HasValue
                        ? (double?) movingWindow.Where(x => x.HasValue).Average(x => x.Value)
                        : null;

                    results.Add(average);
                    return results;
                });
        }
    }

    internal class SpecialPointsAverageCalculator
    {
        private readonly int[] _offsets;
        private readonly List<double> _series;
        private readonly IEnumerable<int> _specialPointsIndices;

        public SpecialPointsAverageCalculator(IEnumerable<double> series, IEnumerable<int> specialPointsIndices)
        {
            _series = series.ToList();
            _specialPointsIndices = specialPointsIndices;
            _offsets = Enumerable.Range(-1*5, 5).ToArray();
        }

        private IEnumerable<IEnumerable<int>> SpecialPointsRegions
        {
            get { return _specialPointsIndices.Select(x => _offsets.Select(offset => x + offset)); }
        }

        private IEnumerable<List<double>> PointsGroups
        {
            get { return Enumerable.Range(0, _offsets.Length).Select(_ => new List<double>()).ToList(); }
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