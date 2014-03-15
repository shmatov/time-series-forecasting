using System.Collections.Generic;

namespace TimeSeriesCollection
{
    public static class TimeSeriesExtensions
    {
        public static IEnumerable<double?> Average(this IEnumerable<double?> series, int windowSize = 3)
        {
            return new SMACalculator(series, windowSize).Calculate();
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