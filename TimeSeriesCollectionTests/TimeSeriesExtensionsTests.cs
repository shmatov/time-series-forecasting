﻿using System;
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
    }
}