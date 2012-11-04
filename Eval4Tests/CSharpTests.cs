using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class CSharpTests : BaseTest
    {

        [TestMethod]
        public void TestModulo()
        {
            TestCSFormula("23 % 10", 3);
        }
        
        [TestMethod]
        public void TestBitwiseAnd()
        {
            TestCSFormula("3 & 254", 2);
        }

        [TestMethod]
        public void TestBooleanAnd()
        {
            TestCSFormula("false && true", false);
        }

        [TestMethod]
        public void TestEqOperator()
        {
            TestCSFormula("1 == 2", false);
        }

        [TestMethod]
        public void TestNEOperator()
        {
            TestCSFormula("1 != 2", true);
        }

        [TestMethod]
        public void TestNotOperator()
        {
            TestCSFormula("! true", false);
        }

        [TestMethod]
        public void TestBitwiseOr()
        {
            TestCSFormula("1 | 255", 255);
        }

        [TestMethod]
        public void TestBooleanOr()
        {
            TestCSFormula("false || true", true);
        }

        [TestMethod]
        public void TestIf()
        {
            TestCSFormula("true ?  1:0", 1);
            TestCSFormula("false ?  1:0", 0);
        }
    }
}
