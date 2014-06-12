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

            forecast = series.Aggregate(forecast,
                (current, value) => value*_smoothingConstant + current*(1 - _smoothingConstant));

            while (period-- > 0)
                yield return forecast;
        }
    }
}