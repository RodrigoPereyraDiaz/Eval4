using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.Tests
{
    [TestClass]
    public class VbAndCsTests : BaseTest
    {
        [TestMethod]
        public void TestNumberLiterals()
        {
            TestVbAndCsFormula("1.5", 1.5);
            TestVbAndCsFormula("0.5", 0.5);
            TestVbAndCsFormula(".5", .5);
            TestVbAndCsFormula("-.5", -.5);
            TestVbAndCsFormula("-0.5", -0.5);
        }

        [TestMethod]
        public void TestPriorities()
        {
            TestVbAndCsFormula("-1.5*-2.5", -1.5 * -2.5);
        }

        [TestMethod]
        public void TestTemplate()
        {
            //   TestTemplate("<p>Hello</p>", "<p>Hello</p>");
        }

    }
}
