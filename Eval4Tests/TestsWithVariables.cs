using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class TestsWithVariables : BaseTest
    {

        public TestsWithVariables()
        {
            evVB.SetVariable("A", 2);
            evVB.SetVariable("B", 3);
        }

        [TestMethod]
        public void CheckVariables()
        {
            TestVBFormula("A", 2);
            TestVBFormula("B", 3);
        }

        [TestMethod]
        public void CheckVariablesOperations()
        {
            TestVBFormula("A+B", 5);
            TestVBFormula("A-B", -1);
            TestVBFormula("A*B", 6);
        }

        [TestMethod]
        //[ExpectedException(typeof(Exception))]
        public void CheckChangingVariables()
        {
            evVB.SetVariable("C", 10);
            var parsed = evVB.Parse("C*5");
            throw new NotImplementedException();
            //parsed.ValueChanged += parsed_ValueChanged;
            Assert.AreEqual(parsed.ObjectValue, 50);
            evVB.SetVariable("C", 5);
            Assert.AreEqual(parsed.ObjectValue, 25);
            throw new NotImplementedException();
            //parsed.ValueChanged -= parsed_ValueChanged;
            evVB.SetVariable("C", 10);
            //Assert.AreEqual(parsed.ObjectValue, 99.99);
        }

        void parsed_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
