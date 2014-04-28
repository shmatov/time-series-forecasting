using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TimeSeriesCollection;

namespace TimeSeriesCollectionTests
{
    namespace TimeSeriesExtensionsTests
    {
        [TestFixture]
        internal class Truncate
        {
            [Test]
            public void ShouldTruncate()
            {
                Assert.AreEqual(new double?[] {1, null, 1, 1},
                    new double?[] {null, null, 1, null, 1, 1, null, null}.Truncate(x => !x.HasValue));
            }
        }

        [TestFixture]
        internal class Weigh
        {
            [TestCase(2, new double[] {0, 1, 2, 1, 0}, new double[] {2, 2, 2, 2, 2})]
            public void ShouldWeigh(int radius, double[] expected, double[] actual)
            {
                var addition = new TimeSeriesExtensions.Addition(actual, radius);
                Assert.AreEqual(expected, addition.Weight().Series);
            }
        }

        [TestFixture]
        internal class CalculateF
        {
            [Test]
            public void ShouldCalculateF()
            {
                var series = new double[] {1, 2, 3, 2, 1, 3, 4, 5, 4, 3};
                var expected = new double[] {2, 3, 4, 3, 2};
                Assert.AreEqual(expected, series.CalculateF(new[] {2, 7}, 2).Series);
            }
        }

        [TestFixture]
        internal class Add
        {
            [TestCase(
                new double[] {0, 0, 0, 0, 0, 0, 0},
                new double[] {1, 2, 3}, 1, new[] {1, 5},
                new double[] {1, 2, 3, 0, 1, 2, 3})]
            [TestCase(new double[] {0, 0, 0},
                new double[] { 1, 2, 3 }, 1, new[] { 0, 2 },
                new double[] { 2, 4, 2 })]
            public void ShouldAdd(IEnumerable<double> series, IEnumerable<double> addition, int radius,
                IEnumerable<int> points, IEnumerable<double> expected)
            {
                var additions = new TimeSeriesExtensions.Addition(addition, radius);
                Assert.AreEqual(expected, series.Add(additions, points));
            }
        }

        [TestFixture]
        internal class CalculateAdditions
        {
            [TestCase(new double[] {1, 2, 3, 2, 1, 3, 4, 5, 4, 3}, new[] {2, 7}, new double[] {0, 1.5, 4, 1.5, 0})]
            public void ShouldCalculateAdditions(IEnumerable<double> series, IEnumerable<int> points,
                IEnumerable<double> expected)
            {
                var actual = series.CalculateAddition(points, 2, 1).Series;
                Assert.AreEqual(expected, actual);
            }
        }
    }
}