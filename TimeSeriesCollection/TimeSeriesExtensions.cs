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

        public static IEnumerable<double> CalculateF(this IEnumerable<double> series,
            IEnumerable<int> specialPointsIndices, int radius = 3)
        {
            return new SpecialPointsAverageCalculator(series, specialPointsIndices, radius).Calculate();
        }
    }
}