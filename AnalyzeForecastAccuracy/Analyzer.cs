using System;
using System.Collections.Generic;
using System.Linq;

namespace AnalyzeForecastAccuracy
{
    class Analyzer
    {
        private readonly IEnumerable<double> _expected;
        private readonly IEnumerable<double> _forecast;

        public Analyzer(IEnumerable<double> forecast, IEnumerable<double> expected)
        {
            _forecast = forecast;
            _expected = expected;
        }

        public IEnumerable<double> Errors()
        {
            return _forecast.Zip(_expected, (f, e) => e - f);
        }

        public double MeanAbsolutePercantageError()
        {
            var values = _forecast.Zip(_expected, (forecast, expected) => Math.Abs(forecast - expected)/expected).ToList();
            // TODO: division by zero
            return values.Sum() / values.Count * 100;
        }

        public double RootMeanSquareError()
        {
            var values = _forecast.Zip(_expected, (forecast, expected) => Math.Pow(forecast - expected, 2)).ToList();
            return Math.Sqrt(values.Sum() / values.Count);
        }
    }
}