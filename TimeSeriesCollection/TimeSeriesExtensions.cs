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

        public static IEnumerable<T> Truncate<T>(this IEnumerable<T> series, Func<T, bool> predicate)
        {
            var buffer = new Queue<T>();
            var enumerator = series.SkipWhile(predicate).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (predicate.Invoke(current))
                {
                    buffer.Enqueue(current);
                }
                else
                {
                    while (buffer.Count > 0)
                    {
                        yield return buffer.Dequeue();
                    }
                    yield return current;
                }
            }
        }

        public static IEnumerable<double> Interpolate(this IEnumerable<double?> series)
        {
            return new Interpolator(series).Calculate();
        }

        public static Addition CalculateF(this IEnumerable<double> series,
            IEnumerable<int> specialPointsIndices, int radius = 3)
        {
            return new Addition(
                new SpecialPointsAverageCalculator(series, specialPointsIndices, radius).Calculate(),
                radius);
        }

        public static IEnumerable<double> Add(this IEnumerable<double> series, double[] f, int[] specialPointsIndices)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<double> Add(this IEnumerable<double> series, Addition f, int[] specialPointsIndices)
        {
            var listSeries = series.ToList();
            var offsets = Enumerable.Range(-1*f.Radius, f.Radius);
            var additions = specialPointsIndices.SelectMany(x => offsets.Select(offset => x + offset)
                .Where(index => index >= 0 && index < listSeries.Count)
                .Zip(f.Series, (index, value) => new {index, value}));
            
            foreach (var addition in additions)
                listSeries[addition.index] += addition.value;
            return listSeries;
        }

        public class Addition
        {
            public IEnumerable<double> Series;
            public int Radius;

            public Addition(IEnumerable<double> series, int radius)
            {
                Series = series;
                Radius = radius;
            }
        }
    }
}