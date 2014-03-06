using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TimeSeriesCollection;

namespace TimeSeriesTests
{
    class TimeSeriesTest
    {
        [TestFixture]
        class TheConstructor
        {
            [Test]
            public void ShouldAcceptValuesAndDates()
            {
                Assert.IsInstanceOf<TimeSeries>(new TimeSeries(new double?[]{}, new DateTime[] {}));
            }
        }
    }
}
