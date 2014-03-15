using System.Linq;
using NUnit.Framework;
using TimeSeriesCollection;

namespace TimeSeriesCollectionTests
{
    namespace InterpolatorTests
    {
        [TestFixture]
        public class Constructor
        {
            [Test]
            public void ShouldCreate()
            {
                Assert.IsInstanceOf<Interpolator>(new Interpolator(Enumerable.Empty<double?>()));
            }
        }

        [TestFixture]
        public class InterpolateStaticMethod
        {
            [Test]
            [TestCase(new double[] {2}, 1, 3, 1)]
            [TestCase(new double[] {2, 3}, 1, 4, 2)]
            [TestCase(new double[] {-1, 0, 1}, -2, 2, 3)]
            public void ShouldInterpolate(double[] expected, double from, double to, int quantity)
            {
                Assert.AreEqual(expected, Interpolator.Interpolate(from, to, quantity));
            }
        }

        [TestFixture]
        public class CalculateMethod
        {
            private static readonly object[] BadSeriesTestCase =
            {
                new double?[] {1, null},
                new double?[] {null, 1}
            };

            private static readonly object[] InterpolationTestCase =
            {
                new object[] {new double[] {}, new double?[] {}},
                new object[] {new double[] {1, 2}, new double?[] {1, 2}},
                new object[] {new double[] {1, 2, 3}, new double?[] {1, null, 3}},
                new object[] {new double[] {1, 2, 3, 4, 5}, new double?[] {1, null, 3, null, 5}}
            };

            [Test]
            [TestCaseSource("InterpolationTestCase")]
            public void ShouldInterpolate(double[] expected, double?[] data)
            {
                Assert.AreEqual(expected, new Interpolator(data).Calculate());
            }

            [Test]
            [TestCaseSource("BadSeriesTestCase")]
            public void ShouldThrowException(double?[] data)
            {
                Assert.Throws<Interpolator.NullInSeriesException>(() => new Interpolator(data).Calculate());
            }
        }
    }
}