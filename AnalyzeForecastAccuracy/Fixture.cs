using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace AnalyzeForecastAccuracy
{
    class Fixture
    {
        public double[] Series { get; set; }
        public int StartForecastingFrom { get; set; }
        public PointsGroup[] Points { get; set; }
        public string Name { get; set; }

        internal class PointsGroup
        {
            public int Radius { get; set; }
            public int[] Indices { get; set; }
        }

        public static IEnumerable<Fixture> LoadFixtures(string fixturesPath)
        {
            return Directory.GetFiles(fixturesPath, "*.json").Select(path =>
            {
                var fixture = Load(path);
                fixture.Name = Path.GetFileNameWithoutExtension(path);
                return fixture;
            });
        }

        private static Fixture Load(string path)
        {
            var json = new StreamReader(path).ReadToEnd();
            return new JavaScriptSerializer().Deserialize<Fixture>(json);
        }
    }
}