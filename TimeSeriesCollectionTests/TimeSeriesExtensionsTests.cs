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
        class Truncate
        {
            [Test]
            public void ShouldTruncate()
            {
                Assert.AreEqual(new double?[]{1, null,  1, 1},
                    new double? [] {null, null, 1, null, 1, 1, null, null}.Truncate(x => !x.HasValue));
            }
        }

        [TestFixture]
        class Weigh
        {
            [TestCase(2, new double[] {0, 1, 2, 1, 0}, new double[] {2, 2, 2, 2, 2})]
            public void ShouldWeigh(int radius, double [] expected, double[] actual)
            {
                Assert.AreEqual(expected, actual.Weigh(radius));
            }
        }

        [TestFixture]
        class CalculateF
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
        class Add
        {
            [Test]
            public void ShouldAdd()
            {
                var series = new double[] {0, 0, 0, 0, 0, 0, 0};
                var addition = new TimeSeriesExtensions.Addition(new double[] {1, 2, 3}, 1);
                var expected = new double[] {1, 2, 3, 0, 1, 2, 3};
                Assert.AreEqual(expected, series.Add(addition, new[] {1, 5}));
            }
        }

        [TestFixture]
        class CalculateAddition
        {
            [Test]
            public void ShouldCalculateAddition()
            {
                var series = new double[] { 1, 2, 3, 2, 1, 3, 4, 5, 4, 3 };
                var expected = new double[] { 2, 3, 4, 3, 2 };
                Assert.AreEqual(expected, series.CalculateF(new[] { 2, 7 }, 2).Series);
            }

        }
    }
}
