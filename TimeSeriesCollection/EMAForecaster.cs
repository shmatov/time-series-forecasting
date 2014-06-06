using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesCollection
{
    public class EMAForecaster
    {
        private readonly IEnumerable<double> _timeSeries;
        private readonly double _smoothingConstant;


        public EMAForecaster(IEnumerable<double> timeSeries, double smoothingConstant)
        {
            _timeSeries = timeSeries;
            _smoothingConstant = smoothingConstant;
        }

        public IEnumerable<double> Forecast(int period)
        {
            var series = _timeSeries.ToList();
            var forecast = series.First();

            yield return forecast;
            foreach (var value in series)
            {
                yield return forecast = value * _smoothingConstant + forecast * (1 - _smoothingConstant);
            }

            while (--period > 0)
                yield return forecast;
//            var v = forecast;
//            for (var step = 0; step < period - 1; step++)
//            {
//                var newForecast = v*_smoothingConstant + forecast*(1 - _smoothingConstant);
//                v = forecast;
//                yield return forecast = newForecast;
//            }
        }

//        public IEnumerable<double> Forecast(int period)
//        {
//            var values = _timeSeries.ToList();
//
//            var result = new List<double>(values);
//            while (period-- > 0)
//            {
//                var value = ForecastNext(new LinkedList<double>(values), _smoothingConstant);
//                result.Add(value);
//                values.Add(value);
//            }
//            return result;
//        }
//
//        private static double ForecastNext(LinkedList<double> values, double smoothingConstant)
//        {
//            if (values.Last == null) return 0;
//            var currentStep = values.Last.Value * smoothingConstant;
//            values.RemoveLast();
//            return currentStep + (1 - smoothingConstant) * ForecastNext(values, smoothingConstant);
//        }
    }
}
