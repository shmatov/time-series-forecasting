using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesCollection
{
    public static class TimeSeriesExtensions
    {
        public static double?[] Average(double?[] series)
        {
            return new SMACalculator(series).Calculate();
        }

        public static double[] Interpolate(double?[] series)
        {
            throw new NotImplementedException();
        }

        public static double[] CalculateF(double[] series, int[] specialPointsIndices)
        {
            return new SpecialPointsAverageCalculator(series, specialPointsIndices).Calculate();
        }
    }

    internal class SMACalculator
    {
        private readonly int _movingWindowLength;
        private readonly double?[] _series;

        public SMACalculator(double?[] series)
        {
            _series = series;
            _movingWindowLength = 3;
        }

        public double?[] Calculate()
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
                }).ToArray();
        }
    }

    internal class SpecialPointsAverageCalculator
    {
        private readonly int[] _offsets;
        private readonly double[] _series;
        private readonly int[] _specialPointsIndices;

        public SpecialPointsAverageCalculator(double[] series, int[] specialPointsIndices)
        {
            _series = series;
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
            return index >= 0 && index < _series.Length;
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

        public double[] Calculate()
        {
            return SpecialPointsRegions.Aggregate(PointsGroups, AggregateIntoGroups)
                .Select(group => group.Average())
                .ToArray();
        }
    }
}