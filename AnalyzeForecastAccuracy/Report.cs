using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace AnalyzeForecastAccuracy
{
    class Report : Chart
    {
        private readonly ChartArea _chartArea;
        public Report()
        {
            _chartArea = new ChartArea
            {
                AxisX =
                {
                    Minimum = 0, 
                    MajorGrid = {Interval = 1, LineColor = Color.LightSteelBlue}, 
                    MinorGrid = {Enabled = false}
                },
                AxisY = {MajorGrid = {LineColor = Color.LightSteelBlue}}
            };
     
            ChartAreas.Add(_chartArea);

            Size = new Size(1024, 768);
            Legends.Add(new Legend());
        }

        public void AddTimeSeries(string name, IEnumerable<double> series, Color? color = null, int offset = 0)
        {
            var timeSeries = new Series { Name = name, ChartType = SeriesChartType.Line, BorderWidth = 2};
            if (color.HasValue) timeSeries.Color = color.Value;
            foreach (var point in series)
                timeSeries.Points.AddXY(offset++, point);
            Series.Add(timeSeries);
        }

        public void AddPoints(IEnumerable<int> points, Color? color = null)
        {
            var chartPoints = new Series { ChartType = SeriesChartType.Point, MarkerSize = 7, IsVisibleInLegend = false};
            if (color.HasValue) chartPoints.Color = color.Value;
            foreach (var point in points) chartPoints.Points.AddXY(point, 0);
            Series.Add(chartPoints);
        }

        public void AddDelimiter(int x)
        {
            var delimiter = new StripLine
            {
                Interval = 0,
                IntervalOffset = x,
                StripWidth = 0,
                BorderWidth = 1,
                BorderColor = Color.Black,
            };
            _chartArea.AxisX.StripLines.Add(delimiter);
        }
    }
}