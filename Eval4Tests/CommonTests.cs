using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class CommonTests
    {
        static VbEvaluator evVB;
        static CSharpEvaluator evCSharp;

        static CommonTests()
        {
            evVB = new VbEvaluator();
            evCSharp = new CSharpEvaluator();
            InitEvaluator(evVB);
            InitEvaluator(evCSharp);

        }

        private static void InitEvaluator(Eval4.Core.Evaluator res)
        {
        }

        public void TestFormula<T>(Eval4.Core.Evaluator ev, string formula, T expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

        public void TestFormula<T>(string formula, T expectedResult)
        {
            TestFormula(evCSharp, formula, expectedResult);
            formula = formula.Replace("[", "(").Replace("]", ")");
            TestFormula(evVB, formula, expectedResult);
        }


        [TestMethod]
        public void TestDecimal()
        {
            TestFormula("1.5", 1.5);
            TestFormula("0.5", 0.5);
            TestFormula(".5", .5);
            TestFormula("-.5", -.5);
            TestFormula("-0.5", -0.5);
        }

        [TestMethod]
        public void TestPriorities()
        {
            TestFormula("-1.5*-2.5", -1.5 * -2.5);
        }
    }
}
