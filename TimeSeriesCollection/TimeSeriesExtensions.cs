using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesCollection
{
    public static class TimeSeriesExtensions
    {
        public class Addition
        {
            public int Radius;
            public IEnumerable<double> Series;

            public Addition(IEnumerable<double> series, int radius)
            {
                Series = series;
                Radius = radius;
            }
        }

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
                T current = enumerator.Current;
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


        /// <summary>
        ///     Вычисление добавочной функции для особых точек в заданном радиусе.
        /// </summary>
        public static Addition CalculateF(this IEnumerable<double> series, IEnumerable<int> pointsIndices,
            int radius)
        {
            var average =
                new SpecialPointsAverageCalculator(series, pointsIndices, radius).Calculate();
            return new Addition(average, radius);
        }


        private static Func<int, double, double> BuildWeightFunction(int radius)
        {
            return (i, x) =>
            {
                i = i > radius ? 2*radius - i : i;
                return x*(i/(double) radius);
            };
        }


        /// <summary>
        ///     Домножает добавку на "колокольчик".
        /// </summary>
        public static Addition Weight(this Addition addition)
        {
            var f = BuildWeightFunction(addition.Radius);
            var weighted = addition.Series.Select((value, index) => f(index, value));
            return new Addition(weighted, addition.Radius);
        }


        /// <summary>
        ///     Добавление к ряду добавки в заданных точках.
        /// </summary>
        public static IEnumerable<double> Add(this IEnumerable<double> series, Addition f,
            IList<int> pointsIndices)
        {
            var listSeries = series.ToList();
            var offsets = Enumerable.Range(-1*f.Radius, 2*f.Radius + 1);
            var additions = pointsIndices.SelectMany(x => offsets.Select(offset => x + offset)
                .Where(index => index >= 0 && index < listSeries.Count)
                .Zip(f.Series, (index, value) => new {index, value}));

            foreach (var addition in additions)
                listSeries[addition.index] += addition.value;
            return listSeries;
        }


        /// <summary>
        ///     Итеративный алгоритм вычисления взвешенных добавок.
        /// </summary>
        public static IEnumerable<IEnumerable<Addition>> CalculateAdditions(this IEnumerable<double> s,
            IEnumerable<IEnumerable<int>> pointsIndices, int radius, double weight)
        {
            var indicesGroups = pointsIndices.Select(x => x.ToList()).ToList();
            var series = s.ToList();

            while (true)
            {
                var seriesAddition = Enumerable.Repeat<double>(0, series.Count);
                var additions = new List<Addition>();
                foreach (var indices in indicesGroups)
                {
                    var addition = series.CalculateAddition(indices, radius, weight);
                    additions.Add(addition);
                    seriesAddition = seriesAddition.Add(addition, indices);
                }
                series = series.Zip(seriesAddition, (x, a) => x + a).ToList();
                yield return additions;
            }
        }


        /// <summary>
        ///     Вычисление взвешенной добавки.
        /// </summary>
        public static Addition CalculateAddition(this IEnumerable<double> series,
            IEnumerable<int> pointsIndices, int radius, double weight)
        {
            var weighted = series.CalculateF(pointsIndices, radius)
                .Weight()
                .Series.Select(x => x*weight);
            return new Addition(weighted, radius);
        }
    }
}