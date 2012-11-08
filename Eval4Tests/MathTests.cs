using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.Tests
{
    [TestClass]
    public class OctaveTests : BaseTest
    {
        [TestMethod]
        public void TestArithmetic()
        {
       //     TestMath("3", 3);
        }

        //[TestMethod]
        //public void TestPercent()
        //{
        //    TestMath("200 + 5%", 210.0);
        //    TestMath("200 - 5%", 190.0);
        //}

        //[TestMethod]
        //public void TestAnd()
        //{
        //    TestMath("false and false", false);
        //    TestMath("false and true", false);
        //    TestMath("true and false", false);
        //    TestMath("true and true", true);
        //}

        //[TestMethod]
        //public void TestOr()
        //{
        //    TestMath("false or false", false);
        //    TestMath("false or true", true);
        //    TestMath("true or false", true);
        //    TestMath("true or true", true);
        //}

        //[TestMethod]
        //public void TestNot()
        //{
        //    TestMath("not false", true);
        //    TestMath("not true", false);
        //    TestMath("not not false", false);
        //    TestMath("false or not false", true);
        //}

        //[TestMethod]
        //public void TestIf()
        //{
        //    TestMath("if(false,1,0)", 0);
        //    TestMath("if(true,1,0)", 1);
        //}

        //[TestMethod]
        //public void TestXor()
        //{
        //    TestMath("false xor false", false);
        //    TestMath("false xor true", true);
        //    TestMath("true xor false", true);
        //    TestMath("true xor true", false);
        //}

        //static DateTime christmas = new DateTime(2012, 12, 25);

        //[TestMethod]
        //public void TestDate()
        //{
        //    TestMath("#2012/12/25#", christmas);
        //}

        //[TestMethod]
        //public void TestDateArithmetic()
        //{
        //    TestMath("#2012/12/25# + 1", christmas.AddDays(1));
        //    TestMath("1 + #2012/12/25#", christmas.AddDays(1));
        //    TestMath("#2012/12/25# - 1", christmas.AddDays(-1));
        //}

        //[TestMethod]
        //public void TestTimeSpan()
        //{
        //    TestMath("#2012/12/25# - #2013/01/01#", new TimeSpan(-7, 0, 0, 0));
        //}

        //[TestMethod]
        //public void StringConcat()
        //{
        //    TestMath("\"ABC\" + \"DEF\"", "ABCDEF");
        //}

        //[TestMethod]
        //public void TestNumericEquality()
        //{
        //    TestMath("5=5", true);
        //    TestMath("5>4", true);
        //    TestMath("5>=4", true);
        //    TestMath("5>=5", true);
        //    TestMath("5<6", true);
        //    TestMath("5<=6", true);
        //    TestMath("6<=6", true);
        //    TestMath("5<>6", true);

        //    TestMath("5=6", false);
        //    TestMath("4>5", false);
        //    TestMath("6<5", false);
        //    TestMath("4>=5", false);
        //    TestMath("6<=5", false);
        //    TestMath("5<>5", false);
        //}

        //[TestMethod]
        //public void TestWordStrings()
        //{
        //    TestMath("\"A\"", "A");
        //    TestMath("‘A’", "A");
        //    TestMath("“A”", "A");
        //}

        //[TestMethod]
        //public void TestStringEquality()
        //{
        //    TestMath("\"A\"=\"A\"", true);
        //    TestMath("\"B\">\"A\"", true);
        //    TestMath("\"A\"<\"B\"", true);
        //    TestMath("\"B\">=\"A\"", true);
        //    TestMath("\"A\"<=\"B\"", true);
        //    TestMath("\"A\"<>\"B\"", true);

        //    TestMath("\"A\">\"B\"", false);
        //    TestMath("\"B\"<\"A\"", false);
        //    TestMath("\"A\">=\"B\"", false);
        //    TestMath("\"B\"<=\"A\"", false);
        //    TestMath("\"A\"<>\"A\"", false);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(SyntaxError))]
        //public void TestImpossibleOperator()
        //{
        //    TestMath("5-\"A\"", "5-'A' (impossible)");
        //}

        //[TestMethod]
        //public void TestUnary()
        //{
        //    TestMath("-5 + 10", (-5 + 10));
        //    TestMath("5 * -5", (5 * -5));
        //    TestMath("3 - -5 * 4", 3 - -5 * 4);
        //    TestMath("10 - -5", (10 - -5));
        //    TestMath("1+-2*-3", (1 + -2 * -3));
        //    TestMath("1+-2*3", (1 + -2 * 3));
        //}


    }
}
