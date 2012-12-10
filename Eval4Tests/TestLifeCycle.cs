using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eval4.Core;

namespace Eval4.Tests
{
    [TestClass]
    public class TestLifeCycle
    {
        [TestMethod, TestCategory("Life Cycle")]
        public void CompleteTest()
        {
            var ev = new CSharpEvaluator();
            ev.SetVariable<double>("x", 10.0);

            using (var q = ev.Parse("x*2"))
            {
                Assert.AreEqual(q.ObjectValue, 20.0);
                ev.SetVariable("x", 11.0);
                Assert.AreEqual(q.ObjectValue, 22.0);
            }
            // the formula is disposed here (but probably still in the cache)
            ev.SetVariable("x", 10.0);

            using (var q = ev.Parse("x*2"))
            {
                // the formula has be recycled
                Assert.AreEqual(q.ObjectValue, 20.0);
                ev.SetVariable("x", 11.0);
                Assert.AreEqual(q.ObjectValue, 22.0);
            }
        }
    }
}
