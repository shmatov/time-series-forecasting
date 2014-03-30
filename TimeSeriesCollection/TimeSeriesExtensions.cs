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

        private static Func<int, double, double> BuildWeightFunction(int radius)
        {
            return (i, x) =>
            {
                i = i > radius ? 2*radius - i : i;
                return x*(i/(double) radius);
            };
        }

        public static IEnumerable<double> Weigh(this IEnumerable<double> series, int radius = 3)
        {
            var f = BuildWeightFunction(radius);
            return series.Select((value, index) => f(index, value));
        }

        public static IEnumerable<double> Interpolate(this IEnumerable<double?> series)
        {
            return new Interpolator(series).Calculate();
        }

        public static Addition CalculateF(this IEnumerable<double> series, IEnumerable<int> specialPointsIndices,
            int radius = 3)
        {
            var average = new SpecialPointsAverageCalculator(series, specialPointsIndices, radius).Calculate();
            return new Addition(average, radius);
        }

        public static IEnumerable<double> Add(this IEnumerable<double> series, Addition f, int[] specialPointsIndices)
        {
            var listSeries = series.ToList();
            var offsets = Enumerable.Range(-1*f.Radius, 2*f.Radius+1);
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