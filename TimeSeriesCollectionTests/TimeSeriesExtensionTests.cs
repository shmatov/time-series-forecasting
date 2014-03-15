using System;
using NUnit.Framework;
using TimeSeriesCollection;

namespace TimeSeriesCollectionTests
{
    namespace TimeSeriesExtensionsTests
    {
        [TestFixture]
        public class AverageMethod
        {
            [Test]
            public void ShouldCalculateAverage()
            {
                var testCase = new []
                {
                    new {data=new double?[] {1, 3, 5}, expected=new double?[] {1, 2, 3}},
                    new {data=new double?[] { 1, 5, 3, 1, 2 }, expected=new double?[] { 1, 3, 3, 3, 2 }},
                    new {data=new double?[] {1, null, 3}, expected=new double?[] {1, null, 2}},
                    new {data=new double?[] {null}, expected=new double?[] {null}},
                    new {data=new double?[] {null, 1}, expected=new double?[] {null, 1}},
                    new {data=new double?[] {1, null, null, 2}, expected=new double?[] {1, null, null, 2}},
                };

                foreach (var test in testCase)
                    Assert.AreEqual(test.expected, test.data.Average());
            }

            [Test]
            public void ShouldReturnArrayOfNullableDoubles()
            {
                Assert.IsInstanceOf<double?[]>(new double?[] {}.Average());
            }
        }

        [TestFixture]
        public class InterpolateMethod
        {
            [Test]
            public void ShouldReturnArrayOfDoubles()
            {
                Assert.IsInstanceOf<double[]>(new double?[] {}.Interpolate());
            }
        }
    }
}