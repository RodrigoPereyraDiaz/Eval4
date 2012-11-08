using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.Tests
{
    [TestClass]
    public class VbTests : BaseTest
    {
        [TestMethod]
        public void TestArithmetic()
        {
            TestVBFormula("3", 3);
            TestVBFormula("3 + 2", 5);
            TestVBFormula("3 - 2", 1);
            TestVBFormula("3 * 2", 6);
            TestVBFormula("3 / 2", 3 / 2.0);
            TestVBFormula("1*2*3*4*5*6*7*8*9", 1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9);
        }

        [TestMethod]
        public void TestPercent()
        {
            TestVBFormula("200 + 5%", 210.0);
            TestVBFormula("200 - 5%", 190.0);
        }

        [TestMethod]
        public void TestAnd()
        {
            TestVBFormula("false and false", false);
            TestVBFormula("false and true", false);
            TestVBFormula("true and false", false);
            TestVBFormula("true and true", true);
        }

        [TestMethod]
        public void TestOr()
        {
            TestVBFormula("false or false", false);
            TestVBFormula("false or true", true);
            TestVBFormula("true or false", true);
            TestVBFormula("true or true", true);
        }

        [TestMethod]
        public void TestNot()
        {
            TestVBFormula("not false", true);
            TestVBFormula("not true", false);
            TestVBFormula("not not false", false);
            TestVBFormula("false or not false", true);
        }

        [TestMethod]
        public void TestIf()
        {
            TestVBFormula("if(false,1,0)", 0);
            TestVBFormula("if(true,1,0)", 1);
        }

        [TestMethod]
        public void TestXor()
        {
            TestVBFormula("false xor false", false);
            TestVBFormula("false xor true", true);
            TestVBFormula("true xor false", true);
            TestVBFormula("true xor true", false);
        }

        static DateTime christmas = new DateTime(2012, 12, 25);

        [TestMethod]
        public void TestDate()
        {
            TestVBFormula("#2012/12/25#", christmas);
        }

        [TestMethod]
        public void TestDateArithmetic()
        {
            TestVBFormula("#2012/12/25# + 1", christmas.AddDays(1));
            TestVBFormula("1 + #2012/12/25#", christmas.AddDays(1));
            TestVBFormula("#2012/12/25# - 1", christmas.AddDays(-1));
        }

        [TestMethod]
        public void TestTimeSpan()
        {
            TestVBFormula("#2012/12/25# - #2013/01/01#", new TimeSpan(-7, 0, 0, 0));
        }

        [TestMethod]
        public void StringConcat()
        {
            TestVBFormula("\"ABC\" + \"DEF\"", "ABCDEF");
        }

        [TestMethod]
        public void TestNumericEquality()
        {
            TestVBFormula("5=5", true);
            TestVBFormula("5>4", true);
            TestVBFormula("5>=4", true);
            TestVBFormula("5>=5", true);
            TestVBFormula("5<6", true);
            TestVBFormula("5<=6", true);
            TestVBFormula("6<=6", true);
            TestVBFormula("5<>6", true);

            TestVBFormula("5=6", false);
            TestVBFormula("4>5", false);
            TestVBFormula("6<5", false);
            TestVBFormula("4>=5", false);
            TestVBFormula("6<=5", false);
            TestVBFormula("5<>5", false);
        }

        [TestMethod]
        public void TestWordStrings()
        {
            TestVBFormula("\"A\"", "A");
            TestVBFormula("‘A’", "A");
            TestVBFormula("“A”", "A");
        }

        [TestMethod]
        public void TestStringEquality()
        {
            TestVBFormula("\"A\"=\"A\"", true);
            TestVBFormula("\"B\">\"A\"", true);
            TestVBFormula("\"A\"<\"B\"", true);
            TestVBFormula("\"B\">=\"A\"", true);
            TestVBFormula("\"A\"<=\"B\"", true);
            TestVBFormula("\"A\"<>\"B\"", true);

            TestVBFormula("\"A\">\"B\"", false);
            TestVBFormula("\"B\"<\"A\"", false);
            TestVBFormula("\"A\">=\"B\"", false);
            TestVBFormula("\"B\"<=\"A\"", false);
            TestVBFormula("\"A\"<>\"A\"", false);
        }

        [TestMethod]
        //[ExpectedException(typeof(SyntaxError))]
        public void TestImpossibleOperator()
        {
            var actualResult = evVB.Eval("5-\"A\"");

            Assert.IsInstanceOfType(actualResult, typeof(SyntaxError));
            Assert.AreEqual((actualResult as SyntaxError).pos, 5);
        }

        [TestMethod]
        public void TestUnary()
        {
            TestVBFormula("-5 + 10", (-5 + 10));
            TestVBFormula("5 * -5", (5 * -5));
            TestVBFormula("3 - -5 * 4", 3 - -5 * 4);
            TestVBFormula("10 - -5", (10 - -5));
            TestVBFormula("1+-2*-3", (1 + -2 * -3));
            TestVBFormula("1+-2*3", (1 + -2 * 3));
        }


    }
}
