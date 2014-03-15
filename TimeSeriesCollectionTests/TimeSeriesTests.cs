using System;
using NUnit.Framework;
using TimeSeriesCollection;

namespace TimeSeriesCollectionTests
{
    namespace TimeSeriesTests
    {
        [TestFixture]
        public class Constructor
        {
            [Test]
            public void ShouldAcceptValuesAndDates()
            {
                Assert.IsInstanceOf<TimeSeries>(new TimeSeries(new double?[] {}, new DateTime[] {}));
            }
        }
    }
}