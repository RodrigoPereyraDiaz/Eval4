using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4
{
    public class BaseTest
    {
        public VbEvaluator evVB;
        public CSharpEvaluator evCS;
        public MathEvaluator evMath;

        public BaseTest()
        {
            evVB = new VbEvaluator();
            evCS = new CSharpEvaluator();
            evMath = new MathEvaluator();
        }

        public void TestVBFormula<T>(string formula, T expected)
        {
            TestFormula(evVB, formula, expected);
        }

        public void TestMath<T>(string formula, T expected)
        {
            TestFormula(evMath, formula, expected);
        }

        public void TestCSFormula(string formula, object expected)
        {
            TestFormula(evCS, formula, expected);
        }

        public void TestVbAndCsFormula(string formula, object expected)
        {
            TestFormula(evVB, formula, expected);
            TestFormula(evCS, formula, expected);
        }
       
        public void TestFormula<T>(Eval4.Core.Evaluator ev, string formula, T expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

        private void TestTemplate(Core.Evaluator ev, string formula, string expectedResult)
        {
            string result = ev.EvalTemplate(formula);
            Assert.AreEqual(expectedResult, result, "Template " + formula);
        }
    }
}
