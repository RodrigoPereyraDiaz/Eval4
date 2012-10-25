using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class TestEnvironmentFunctions
    {
        static VbEvaluator ev;

        static TestEnvironmentFunctions()
        {
            ev = new VbEvaluator();
            ev.AddEnvironmentFunctions(typeof(EnvironmentFunctions));
        }


        public void TestFormula<T>(string formula, T expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

        [TestMethod]
        public void TestEnvironment()
        {
            TestFormula("avg(2,3,5)", 10 / 3.0);
        }
    }

    public static class EnvironmentFunctions
    {
        public static double avg(double a, double b, double c)
        {
            return (a + b + c) / 3.0;
        }
    }
}
