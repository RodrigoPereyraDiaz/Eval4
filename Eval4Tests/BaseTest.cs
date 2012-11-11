using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eval4.Core;

namespace Eval4
{
    public class BaseTest
    {
        public VB.VbEvaluator evVB;
        public CSharp.CSharpEvaluator evCS;
        public Math.MathEvaluator evMath;

        public BaseTest()
        {
            evVB = new VB.VbEvaluator();
            evCS = new CSharp.CSharpEvaluator();
            evMath = new Math.MathEvaluator();
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
       
        public void TestFormula<T>(IEvaluator ev, string formula, T expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

        //private void TestTemplate(IEvaluator ev, string formula, string expectedResult)
        //{
        //    string result = ev.EvalTemplate(formula);
        //    Assert.AreEqual(expectedResult, result, "Template " + formula);
        //}
    }
}
