using NUnit.Framework;
using TimeSeriesCollection;

namespace TimeSeriesCollectionTests
{
    namespace SMACalculatorTests
    {
        [TestFixture]
        internal class Constructor
        {
            [Test]
            public void ShouldCreate()
            {
                Assert.IsInstanceOf<SMACalculator>(new SMACalculator(new double?[] {}, 1));
            }
        }

        [TestFixture]
        internal class CalculateMethod
        {
            private static readonly object[] AverageTestCase =
            {
                new[] {new double?[] {1, 2, 3}, new double?[] {1, 3, 5}},
                new[] {new double?[] {1, 3, 3, 3, 2}, new double?[] {1, 5, 3, 1, 2}}
            };

            private static readonly object[] WithNullsTestCase =
            {
                new[] {new double?[] {null}, new double?[] {null}},
                new[] {new double?[] {null, 1}, new double?[] {null, 1}},
                new[] {new double?[] {1, null, 2}, new double?[] {1, null, 3}}
            };

            private static readonly object[] WindowSizeTestCase =
            {
                new object[] {1, new double?[] {1, 2, 3, 4}, new double?[] {1, 2, 3, 4}},
                new object[] {2, new double?[] {1, 2, 3, 4}, new double?[] {1, 3, 3, 5}},
                new object[] {3, new double?[] {1, 2, 3, 4}, new double?[] {1, 3, 5, 4}},
                new object[] {4, new double?[] {1, 2, 3, 4}, new double?[] {1, 3, 5, 7}}
            };

            [Test]
            [TestCaseSource("AverageTestCase")]
            public void ShouldCalculateAverage(double?[] expected, double?[] data)
            {
                Assert.AreEqual(expected, new SMACalculator(data, 3).Calculate());
            }

            [Test]
            [TestCaseSource("WindowSizeTestCase")]
            public void ShouldDependOnWindowSize(int windowSize, double?[] expected, double?[] data)
            {
                Assert.AreEqual(expected, new SMACalculator(data, windowSize).Calculate());
            }

            [Test]
            [TestCaseSource("WithNullsTestCase")]
            public void ShouldSkipNulls(double?[] expected, double?[] data)
            {
                Assert.AreEqual(expected, new SMACalculator(data, 3).Calculate());
            }
        }
    }
}