using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class VbTests
    {
        static VbEvaluator ev;

        static VbTests()
        {
            ev = new VbEvaluator();
        }

        [TestMethod]
        public void TestArithmetic()
        {
            TestFormula("3", 3);
            TestFormula("3 + 2", 5);
            TestFormula("3 - 2", 1);
            TestFormula("3 * 2", 6);
            TestFormula("3 / 2", 3 / 2);
            TestFormula("1*2*3*4*5*6*7*8*9", 1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9);
        }

        [TestMethod]
        public void TestPercent()
        {
            TestFormula("200 + 5%", 210);
            TestFormula("200 - 5%", 190);
        }

        [TestMethod]
        public void TestAnd()
        {
            TestFormula("false and false", false);
            TestFormula("false and true", false);
            TestFormula("true and false", false);
            TestFormula("true and true", true);
        }

        [TestMethod]
        public void TestOr()
        {
            TestFormula("false or false", false);
            TestFormula("false or true", true);
            TestFormula("true or false", true);
            TestFormula("true or true", true);
        }

        [TestMethod]
        public void TestNot()
        {
            TestFormula("not false", true);
            TestFormula("not true", false);
            TestFormula("not not false", false);
            TestFormula("false or not false", true);
        }

        [TestMethod]
        public void TestIf()
        {
            TestFormula("if(false,1,0)", 0);
            TestFormula("if(true,1,0)", 1);
        }

        [TestMethod]
        public void TestXor()
        {
            TestFormula("false xor false", false);
            TestFormula("false xor true", true);
            TestFormula("true xor false", true);
            TestFormula("true xor true", false);
        }

        static DateTime christmas = new DateTime(2012, 12, 25);

        [TestMethod]
        public void TestDate()
        {
            TestFormula("#2012/12/25#", christmas);
        }

        [TestMethod]
        public void TestDateArithmetic()
        {
            TestFormula("#2012/12/25# + 1", christmas.AddDays(1));
            TestFormula("1 + #2012/12/25#", christmas.AddDays(1));
            TestFormula("#2012/12/25# - 1", christmas.AddDays(-1));
        }

        [TestMethod]
        public void TestTimeSpan()
        {
            TestFormula("#2012/12/25# - #2013/01/01#", new TimeSpan(-7,0,0,0));
        }

        [TestMethod]
        public void StringConcat()
        {
            TestFormula("\"ABC\" + \"DEF\"", "ABCDEF");
        }

        [TestMethod]
        public void TestNumericEquality()
        {
            TestFormula("5=5", true);
            TestFormula("5>4", true);
            TestFormula("5>=4", true);
            TestFormula("5>=5", true);
            TestFormula("5<6", true);
            TestFormula("5<=6", true);
            TestFormula("6<=6", true);
            TestFormula("5<>6", true);

            TestFormula("5=6", false);
            TestFormula("4>5", false);
            TestFormula("6<5", false);
            TestFormula("4>=5", false);
            TestFormula("6<=5", false);
            TestFormula("5<>5", false);
        }

        [TestMethod]
        public void TestWordStrings()
        {
            TestFormula("\"A\"", "A");
            TestFormula("‘A’", "A");
            TestFormula("“A”", "A");
        }

        [TestMethod]
        public void TestStringEquality()
        {
            TestFormula("\"A\"=\"A\"", true);
            TestFormula("\"B\">\"A\"", true);
            TestFormula("\"A\"<\"B\"", true);
            TestFormula("\"B\">=\"A\"", true);
            TestFormula("\"A\"<=\"B\"", true);
            TestFormula("\"A\"<>\"B\"", true);

            TestFormula("\"A\">\"B\"", false);
            TestFormula("\"B\"<\"A\"", false);
            TestFormula("\"A\">=\"B\"", false);
            TestFormula("\"B\"<=\"A\"", false);
            TestFormula("\"A\"<>\"A\"", false);
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxError))]
        public void TestImpossibleOperator()
        {
            TestFormula("5-\"A\"", "5-'A' (impossible)");
        }

        public void TestFormula<T>(string formula, T expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

        [TestMethod]
        public void TestUnary()
        {
            TestFormula("-5 + 10", (-5 + 10));
            TestFormula("5 * -5", (5 * -5));
            TestFormula("3 - -5 * 4", 3 - -5 * 4);
            TestFormula("10 - -5", (10 - -5));
            TestFormula("1+-2*-3", (1 + -2 * -3));
            TestFormula("1+-2*3", (1 + -2 * 3));
        }


    }
}
