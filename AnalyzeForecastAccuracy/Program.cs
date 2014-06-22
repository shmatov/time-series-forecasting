using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using TimeSeriesCollection;

namespace AnalyzeForecastAccuracy
{
    internal class Program
    {
        private const double EMAParameter = 0.6;
        private const int Period = 2;

        private static void Main(string[] args)
        {
            var fixturesPah = args.Length >= 1 ? args[0] : "fixtures";
            var reportsPath = args.Length == 2 ? args[1] : "reports";
            Run(Fixture.LoadFixtures(fixturesPah), reportsPath);
        }

        private static void Run(IEnumerable<Fixture> fixtures, string reportsPath)
        {
            foreach (var fixture in fixtures)
            {
                var indices = fixture.Points.Select(x => x.Indices);
                var radiuses = fixture.Points.Select(x => x.Radius);
                var forecast = ForecastSeries(fixture.Series, indices, radiuses, Period, fixture.StartForecastingFrom);

                var report = new Report();
                report.AddTimeSeries("series", fixture.Series, Color.DodgerBlue);
                //                report.AddTimeSeries("Shifted EMA", fixture.Series.EMA(EMAParameter).ShiftRight(Period), Color.Yellow);
                //                report.AddTimeSeries("Extended EMA", fixture.Series.EMA(EMAParameter).ExtendRight(Period), Color.Coral);
                //                report.AddTimeSeries("EMA", fixture.Series.EMA(EMAParameter), Color.MediumSeaGreen);
                report.AddTimeSeries("forecast", forecast, Color.OrangeRed, offset: fixture.StartForecastingFrom + Period);

                foreach (var points in fixture.Points)
                    report.AddPoints(points.Indices);

                report.AddDelimiter(fixture.StartForecastingFrom);

                report.SaveImage(Path.Combine(reportsPath, Path.ChangeExtension(fixture.Name, "png")), ChartImageFormat.Png);
            }
        }

        private static IEnumerable<double> ForecastSeries(IList<double> series, IEnumerable<IEnumerable<int>> indices, IEnumerable<int> radiuses, int period, int startForecastingFrom)
        {
            var indicesList = indices.Select(x => x.ToList()).ToList();
            var radiusesList = radiuses.ToList();

            var trainingSeries = series.Take(startForecastingFrom).ToList();
            foreach (var value in series.Skip(startForecastingFrom))
            {
                trainingSeries.Add(value);
                yield return Forecast(trainingSeries, indicesList, radiusesList, period);
            }
        }

        private static double Forecast(IList<double> series, IEnumerable<IEnumerable<int>> indices, IEnumerable<int> radiuses, int period)
        {
            var indicesList = indices.Select(x => x.ToList()).ToList();
            var smoothed = series.EMA(EMAParameter).ToList();
            var additionsGroups = smoothed.ShiftRight(period).CalculateAdditions(series, indicesList, radiuses, weight: 0.1).Take(100);
            var forecast = additionsGroups.Aggregate(smoothed.ExtendRight(period), (current, additions) => current.Add(additions, indicesList));
            return forecast.Last();
        }
    }
}