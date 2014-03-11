using System;

namespace TimeSeriesCollection
{
    public class TimeSeries
    {
        public TimeSeries(double?[] values, DateTime[] dates)
        {
            Values = values;
            Dates = dates;
        }

        public double?[] Values { get; private set; }
        public DateTime[] Dates { get; private set; }
    }
}