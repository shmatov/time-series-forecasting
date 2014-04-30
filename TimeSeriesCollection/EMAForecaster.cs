using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesCollection
{
    class EMAForecaster
    {
        private readonly IEnumerable<double> _series;

        public EMAForecaster(IEnumerable<double> series)
        {
            _series = series;
        }

        public IEnumerable<double> Forecast(int period)
        {
            throw new NotImplementedException();
        }
    }
}
