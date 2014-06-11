using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TimeSeriesCollection;

namespace Runner
{
    class Program
    {
        private const int ForecastingPeriod = 2;
        private const int Radius = 3;

        static void Main(string[] args)
        {
            var rawSeries = ReadSeries("data/input.txt");

            var specialPointsGroups = ReadPoints("data/points.txt").Select(x => x.ToList()).ToList();


            var series = rawSeries.Truncate(v => !v.HasValue).Interpolate().ToList();
            Write("data/clean.txt", series);


            var subSeries = new List<double>();
            var smoothedSeries = new List<double>();
            foreach (var value in series)
            {
                subSeries.Add(value);
                smoothedSeries.Add(new EMAForecaster(subSeries, 0.4).Forecast(ForecastingPeriod).Last());
            }
            Write("data/smoothed.txt", smoothedSeries);


            var diffSeries = series.Zip(smoothedSeries, (o, s) => o - s).ToList();
            Write("data/diff.txt", diffSeries);


            var i = 0;
            foreach (var points in specialPointsGroups)
            {
                var f = diffSeries.CalculateF(points, Radius);
                var weighted = diffSeries.CalculateAddition(points, Radius, 0.5);

                Write(String.Format("data/F_{0}.txt", i), f.Series);
                Write(String.Format("data/WF_{0}.txt", i++), weighted.Series);
            }


            var added = smoothedSeries
                .CalculateAdditions(series, specialPointsGroups, radius: Radius, weight: 0.5).Take(100)
                .Aggregate<IEnumerable<TimeSeriesExtensions.Addition>, IEnumerable<double>>(smoothedSeries, (current, additions) => current.Add(additions, specialPointsGroups));
            Write("data/added.txt", added);
        }

        static IEnumerable<double?> ReadSeries(string filename)
        {
            return Read<double?>(filename, line =>
            {
                try
                {
                    return double.Parse(line);
                }
                catch (FormatException)
                {
                    return null;
                }
            });
        }

        static IEnumerable<IEnumerable<int>> ReadPoints(string filename)
        {
            return Read(filename, line => line.Split(' ').Select(int.Parse));
        }

        static IEnumerable<T> Read<T>(string filename, Func<string, T> parser)
        {
            var input = new StreamReader(filename);
            string line;
            while ((line = input.ReadLine()) != null)
            {
                yield return parser(line);
            }
            input.Close();
        }

        static void Write(string filename, IEnumerable<double> data)
        {
            var output = new StreamWriter(filename);
            foreach (var value in data)
                output.WriteLine(value);
            output.Close();
        }
    }
}
