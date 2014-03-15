using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesCollection
{
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