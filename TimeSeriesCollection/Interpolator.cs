using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesCollection
{
    public class Interpolator
    {
        private readonly IEnumerable<double?> _series;

        public Interpolator(IEnumerable<double?> series)
        {
            _series = series;
        }

        public static IEnumerable<double> Interpolate(double from, double to, int quantity)
        {
            var result = new double[quantity];
            var index = 0;
            while (index++ < quantity)
                result[index - 1] = from + index*(to - from)/(quantity + 1);
            return result;
        }

        public IEnumerable<double> Calculate()
        {
            var nulls = 0;
            double? last = null;
            var interpolated = _series.Aggregate(new List<double>(), (list, x) =>
            {
                if (!x.HasValue)
                {
                    nulls++;
                    return list;
                }

                if (nulls > 0)
                {
                    if (!last.HasValue) throw new NullInSeriesException();
                    list.AddRange(Interpolate(last.Value, x.Value, nulls));
                    nulls = 0;
                }
                last = x;
                list.Add(x.Value);
                return list;
            });

            if (nulls > 0) throw new NullInSeriesException();
            return interpolated;
        }

        public class NullInSeriesException : Exception
        {
            public NullInSeriesException()
                : base("Found null in the series. First you need to truncate series.")
            {
            }

            public NullInSeriesException(string message) : base(message)
            {
            }

            public NullInSeriesException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}