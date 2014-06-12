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
            _specialPointsIndices = specialPointsIndices;
            _series = series.ToList();
            _offsets = Enumerable.Range(-1*radius, 2*radius + 1).ToList();
        }

        private IEnumerable<IEnumerable<int>> SpecialPointsRegions
        {
            get { return _specialPointsIndices.Select(x => _offsets.Select(offset => x + offset)); }
        }

        private IEnumerable<List<double>> PointsGroups
        {
            get { return Enumerable.Range(0, _offsets.Count).Select(_ => new List<double>()).ToList(); }
        }

        private List<double> AddIfSeriesHasIndex(int index, List<double> group)
        {
            if (index >= 0 && index < _series.Count)
                group.Add(_series[index]);
            return group;
        }

        private IEnumerable<List<double>> AggregateIntoGroups(IEnumerable<List<double>> groups, IEnumerable<int> indices)
        {
            return groups.Zip(indices, (group, index) => new {group, index})
                .Select(x => AddIfSeriesHasIndex(x.index, x.group));
        }

        public IEnumerable<double> Calculate()
        {
            return SpecialPointsRegions.Aggregate(PointsGroups, AggregateIntoGroups).Select(group => group.Average());
        }
    }
}