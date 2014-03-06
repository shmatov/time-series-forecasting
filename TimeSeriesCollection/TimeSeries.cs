using System;

namespace TimeSeriesCollection
{
    public class TimeSeries
    {
        public double?[] Values { get; private set; }
        public DateTime[] Dates { get; private set; }

        public TimeSeries(double?[] values, DateTime[] dates)
        {
            Values = values;
            Dates = dates;
        }
    }
}
