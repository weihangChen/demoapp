using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.TobeRemove;
using System.Collections.Generic;

namespace DemoApp.Tests
{
    [TestClass]
    public class RemoveMeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var data = new List<WhatEver> { WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.impression, WhatEver.click, WhatEver.click
            };

            var removeme = new RemoveMe();
            var countObj = removeme.GetCount(data);
            Assert.IsTrue(countObj.ClickCount == 13);
            Assert.IsTrue(countObj.ImpressionCount == 12);

        }
    }
}
