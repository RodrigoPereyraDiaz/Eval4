using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class TestsWithVariables
    {
        static VbEvaluator ev;

        static TestsWithVariables()
        {
            ev = new VbEvaluator();
            ev.SetVariable("A", 2);
            ev.SetVariable("B", 3);
        }

        public void TestFormula<T>(string formula, T expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

        [TestMethod]
        public void CheckVariables()
        {
            TestFormula("A", 2);
            TestFormula("B", 3);
        }

        [TestMethod]
        public void CheckVariablesOperations()
        {
            TestFormula("A+B", 5);
            TestFormula("A-B", -1);
            TestFormula("A*B", 6);
        }

        [TestMethod]
        //[ExpectedException(typeof(Exception))]
        public void CheckChangingVariables()
        {
            ev.SetVariable("C", 10);
            var parsed = ev.Parse("C*5");
            parsed.ValueChanged += parsed_ValueChanged;
            Assert.AreEqual(parsed.ObjectValue, 50);
            ev.SetVariable("C", 5);
            Assert.AreEqual(parsed.ObjectValue, 25);
            parsed.ValueChanged -= parsed_ValueChanged;
            ev.SetVariable("C", 10);
            //Assert.AreEqual(parsed.ObjectValue, 99.99);
        }

        void parsed_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
