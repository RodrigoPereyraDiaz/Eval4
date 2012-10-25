using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class CSharpTests
    {
        static CSharpEvaluator ev;

        static CSharpTests()
        {
            ev = new Eval4.CSharpEvaluator();
        }

        public void TestFormula<T>(string formula, T expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

        [TestMethod]
        public void TestModulo()
        {
            TestFormula("23 % 10", 3);
        }

        [TestMethod]
        public void TestBitwiseAnd()
        {
            TestFormula("3 & 254", 2);
        }

        [TestMethod]
        public void TestBooleanAnd()
        {
            TestFormula("false && true", false);
        }

        [TestMethod]
        public void TestEqOperator()
        {
            TestFormula("1 == 2", false);
        }

        [TestMethod]
        public void TestNEOperator()
        {
            TestFormula("1 != 2", true);
        }

        [TestMethod]
        public void TestNotOperator()
        {
            TestFormula("! true", false);
        }

        [TestMethod]
        public void TestBitwiseOr()
        {
            TestFormula("1 | 255", 255);
        }
        
        [TestMethod]
        public void TestBooleanOr()
        {
            TestFormula("false || true", true);
        }

        [TestMethod]
        public void TestIf()
        {
            TestFormula("true ?  1:0", 1.0);
            TestFormula("false ?  1:0", 0.0);
        }
    }
}
