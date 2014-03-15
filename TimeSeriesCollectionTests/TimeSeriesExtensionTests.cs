using System;
using System.Collections.Generic;
using NUnit.Framework;
using TimeSeriesCollection;

namespace TimeSeriesCollectionTests
{
    namespace TimeSeriesExtensionsTests
    {
        [TestFixture]
        public class AverageMethod
        {
            private static readonly object[] AverageTestCase =
            {
                new [] {new double?[] {1, 2, 3}, new double?[] {1, 3, 5}},
                new [] {new double?[] {1, 3, 3, 3, 2}, new double?[] {1, 5, 3, 1, 2}},
                new [] {new double?[] {1, null, 2}, new double?[] {1, null, 3}},
                new [] {new double?[] {null}, new double?[] {null}},
                new [] {new double?[] {1, null, null, 2}, new double?[] {1, null, null, 2}},
                new [] {new double?[] {null, 1}, new double?[] {null, 1}}
            };

            [Test]
            [TestCaseSource("AverageTestCase")]
            public void ShouldCalculateAverage(double?[] expected, double?[] data)
            {
                Assert.AreEqual(expected, data.Average());
            }

            [Test]
            public void ShouldReturnEnumerableOfNullableDoubles()
            {
                Assert.IsInstanceOf<IEnumerable<double?>>(new double?[] {}.Average());
            }
        }
    }
}