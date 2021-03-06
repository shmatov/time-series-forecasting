﻿using System;
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

        public static IEnumerable<T> ExtendRight<T>(this IEnumerable<T> series, int period)
        {
            var last = default(T);
            var enumerator = series.GetEnumerator();
            while (enumerator.MoveNext()) yield return last = enumerator.Current;
            while (period-- > 0) yield return last;
        }

        public static IEnumerable<T> ShiftRight<T>(this IEnumerable<T> series, int period)
        {
            foreach (var value in series)
            {
                while (period-- > 0)
                    yield return value;
                yield return value;
            }
        }

        public static IEnumerable<double> EMA(this IEnumerable<double> series, double alpha)
        {
            double? ema = null;
            foreach (var value in series)
            {
                ema = ema.HasValue ? alpha*value + (1 - alpha)*ema : value;
                yield return ema.Value;
            }
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
                if (i == 0) i = 1;
                return x*i/radius;
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
            IEnumerable<int> pointsIndices)
        {
            var offsets = Enumerable.Range(-1*f.Radius, 2*f.Radius + 1);

            var additions = pointsIndices.SelectMany(x =>
                offsets.Select(offset => x + offset)
                    .Zip(f.Series, (index, value) => new {index, value})
                );

            return series.Select((value, index) => new {index, value})
                .GroupJoin(additions, x => x.index, x => x.index, (s, add) => s.value + add.Sum(x => x.value));
        }


        /// <summary>
        ///     Добавление к ряду добавок в задданых точках.
        /// </summary>
        public static IEnumerable<double> Add(this IEnumerable<double> series, IEnumerable<Addition> additions,
            IEnumerable<IEnumerable<int>> pointsIndices)
        {
            return additions
                .Zip(pointsIndices, (addition, indices) => new {addition, indices})
                .Aggregate(series, (current, x) => current.Add(x.addition, x.indices));
        }


        /// <summary>
        ///     Итеративный алгоритм вычисления взвешенных добавок.
        /// </summary>
        public static IEnumerable<IEnumerable<Addition>> CalculateAdditions(
            this IEnumerable<double> series,
            IEnumerable<double> goalSeries,
            IEnumerable<IEnumerable<int>> pointsIndices,
            IEnumerable<int> radiuses,
            double weight)
        {
            var indicesGroups = pointsIndices.Select(x => x.ToList()).ToList();
            var seriesList = series.ToList();
            var radiusesList = radiuses.ToList();
            var goalSeriesList = goalSeries.ToList();

            while (true)
            {
                var diffList = goalSeriesList.Zip(seriesList, (o, x) => o - x).ToList();

                var additionAccumualtor = Enumerable.Repeat<double>(0, seriesList.Count);
                var additions = new List<Addition>();
                foreach (var group in indicesGroups.Zip(radiusesList, (indices, r) => new {Indices = indices, Radius = r}))
                {
                    var addition = diffList.CalculateAddition(group.Indices, group.Radius, weight);
                    additions.Add(addition);
                    additionAccumualtor = additionAccumualtor.Add(addition, group.Indices);
                }
                seriesList = seriesList.Zip(additionAccumualtor, (x, a) => x + a).ToList();
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