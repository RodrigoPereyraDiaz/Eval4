using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.Tests
{
    [TestClass]
    public class VbTests : BaseTest<VbEvaluator>
    {
        public VbTests()
        {
            ev.SetVariable("A", 2);
            ev.SetVariable("B", 3);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_Arithmetic()
        {
            TestFormula("3", 3);
            TestFormula("3 + 2", 5);
            TestFormula("3 - 2", 1);
            TestFormula("3 * 2", 6);
            TestFormula("3 / 2", 3 / 2.0);
            // we we're testing a bit of spacing here as well
            TestFormula("1 * 2 *3 * 4 *5* 6*7*\t8*9", 1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_Percent()
        {
            TestFormula("200 + 5%", 210.0);
            TestFormula("200 - 5%", 190.0);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_And()
        {
            TestFormula("false and false", false);
            TestFormula("false and true", false);
            TestFormula("true and false", false);
            TestFormula("true and true", true);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_Or()
        {
            TestFormula("false or false", false);
            TestFormula("false or true", true);
            TestFormula("true or false", true);
            TestFormula("true or true", true);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_Not()
        {
            TestFormula("not false", true);
            TestFormula("not true", false);
            TestFormula("not not false", false);
            TestFormula("false or not false", true);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_If()
        {
            TestFormula("if(false,1,0)", 0);
            TestFormula("if(true,1,0)", 1);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_Xor()
        {
            TestFormula("false xor false", false);
            TestFormula("false xor true", true);
            TestFormula("true xor false", true);
            TestFormula("true xor true", false);
        }

        static DateTime christmas = new DateTime(2012, 12, 25);

        [TestMethod, TestCategory("VB")]
        public void VB_Date()
        {
            TestFormula("#2012/12/25#", christmas);
        }

        //[TestMethod, TestCategory("VB")]
        //public void VB_DateArithmetic()
        //{
        //    TestFormula("#2012/12/25# + 1", christmas.AddDays(1));
        //    TestFormula("1 + #2012/12/25#", christmas.AddDays(1));
        //    TestFormula("#2012/12/25# - 1", christmas.AddDays(-1));
        //}

        //[TestMethod, TestCategory("VB")]
        //public void VB_TimeSpan()
        //{
        //    TestFormula("#2012/12/25# - #2013/01/01#", new TimeSpan(-7, 0, 0, 0));
        //}

        [TestMethod, TestCategory("VB")]
        public void VB_StringConcat()
        {
            TestFormula("\"ABC\" + \"DEF\"", "ABCDEF");
        }

        [TestMethod, TestCategory("VB")]
        public void VB_NumericEquality()
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

        [TestMethod, TestCategory("VB")]
        public void VB_WordStrings()
        {
            TestFormula("\"A\"", "A");
            TestFormula("‘A’", "A");
            TestFormula("“A”", "A");
        }

        [TestMethod, TestCategory("VB")]
        public void VB_StringEquality()
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

        [TestMethod, TestCategory("VB")]
        //[ExpectedException(typeof(SyntaxError))]
        public void VB_ImpossibleOperator()
        {
            var actualResult = ev.Eval("5-\"A\"");

            Assert.IsInstanceOfType(actualResult, typeof(SyntaxError));
            Assert.AreEqual((actualResult as SyntaxError).pos, 5);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_Unary()
        {
            TestFormula("-5 + 10", (-5 + 10));
            TestFormula("5 * -5", (5 * -5));
            TestFormula("3 - -5 * 4", 3 - -5 * 4);
            TestFormula("10 - -5", (10 - -5));
            TestFormula("1+-2*-3", (1 + -2 * -3));
            TestFormula("1+-2*3", (1 + -2 * 3));
        }


        [TestMethod, TestCategory("VB")]
        public void VB_CheckVariables()
        {
            TestFormula("A", 2);
            TestFormula("B", 3);
        }

        [TestMethod, TestCategory("VB")]
        public void VB_CheckVariablesOperations()
        {
            TestFormula("A+B", 5);
            TestFormula("A-B", -1);
            TestFormula("A*B", 6);
        }

        [TestMethod, TestCategory("VB")]
        //[ExpectedException(typeof(Exception))]
        public void VB_CheckChangingVariables()
        {
            ev.SetVariable("C", 10);
            var parsed = ev.Parse("C*5");
            //throw new NotImplementedException();
            //parsed.ValueChanged += parsed_ValueChanged;
            Assert.AreEqual(parsed.ObjectValue, 50);
            ev.SetVariable("C", 5);
            Assert.AreEqual(parsed.ObjectValue, 25);
            //throw new NotImplementedException();
            //parsed.ValueChanged -= parsed_ValueChanged;
            ev.SetVariable("C", 10);
            //Assert.AreEqual(parsed.ObjectValue, 99.99);
        }
        //Accounts accountInstance = new Accounts();
        //int[] pascal = new int[] { 1, 8, 28, 56, 70, 56, 28, 8, 1 };

        //public TestArraysAndObjects()
        //{
        //    InitEvaluator(evVB);
        //    InitEvaluator(evCS);
        //}

        //private void VB_InitEvaluator(IEvaluator ev)
        //{
        //    ev.SetVariable("pascal", pascal);
        //    ev.SetVariable("fibonacci", new int[] { 1, 1, 2, 3, 5, 8, 13, 21, 34 });
        //    ev.SetVariable("mult", new int[,] { { 0, 0, 0, 0 }, { 0, 1, 2, 3 }, { 0, 2, 4, 6 }, { 0, 3, 6, 9 } });
        //    ev.SetVariable("accounts", accountInstance);
        //}

        //public class Accounts
        //{
        //    public double Credit { get { return 150.00; } }
        //    public double Vat { get { return 20.0; } }
        //    public byte ByteValue { get { return 123; } }
        //    public Single SingleValue { get { return 123; } }
        //    public Decimal DecimalValue { get { return 123; } }
        //    public readonly Int16 Int16Value = 123;
        //    public double CreditWithVat()
        //    {
        //        return AddVat(Vat, Credit);
        //    }

        //    public static double AddVat(double vat, double value)
        //    {
        //        return value * ((100 + vat) / 100.0);
        //    }
        //}

        //[TestMethod, TestCategory("VB")]
        //public void VB_AccessArrayVariables()
        //{
        //    TestCSFormula("pascal", pascal);
        //    TestCSFormula("pascal[0]", 1);
        //    TestCSFormula("pascal[2]", 28);
        //    TestCSFormula("pascal[2]*2", 56);
        //    TestCSFormula("mult[1,0]", 0);
        //    TestCSFormula("mult[1,2]", 2);
        //    TestCSFormula("mult[2,3]", 6);
        //    TestCSFormula("mult[3,3]", 9);

        //    TestVBFormula("pascal(0)", 1);
        //    TestVBFormula("pascal(2)", 28);
        //    TestVBFormula("pascal(2)*2", 56);
        //    TestVBFormula("mult(1,0)", 0);
        //    TestVBFormula("mult(1,2)", 2);
        //    TestVBFormula("mult(2,3)", 6);
        //    TestVBFormula("mult(3,3)", 9);
        //}

        //[TestMethod, TestCategory("VB")]
        //public void VB_AccessObjectMethodsAndFields()
        //{
        //    TestVbAndCsFormula("accounts.Credit", 150.00);
        //    TestVbAndCsFormula("accounts.Vat", 20.00);
        //    TestVbAndCsFormula("accounts.CreditWithVat", 180.0);
        //    TestVbAndCsFormula("accounts.AddVat(20,100)", 120.0);

        //    TestVbAndCsFormula("accounts.ByteValue", (byte)123);
        //    TestVbAndCsFormula("accounts.SingleValue", (Single)123);
        //    TestVbAndCsFormula("accounts.DecimalValue", (decimal)123);
        //    TestVbAndCsFormula("accounts.Int16Value", (Int16)123);

        //    TestVbAndCsFormula("accounts.ByteValue * 1.0", 123.0);
        //    TestVbAndCsFormula("accounts.SingleValue  * 1.0", 123.0);
        //    TestVbAndCsFormula("accounts.Int16Value * 1.0", 123.0);
        //    //TestVbAndCsFormula("accounts.DecimalValue * 1.0", accountInstance.DecimalValue * 1.0);
        //    //TestVbAndCsFormula("accounts.Sum(1,2,3,4)", (decimal)10.0);
        //}

        //[TestMethod, TestCategory("VB")]
        //public void VB_NumberLiterals()
        //{
        //    TestVbAndCsFormula("1.5", 1.5);
        //    TestVbAndCsFormula("0.5", 0.5);
        //    TestVbAndCsFormula(".5", .5);
        //    TestVbAndCsFormula("-.5", -.5);
        //    TestVbAndCsFormula("-0.5", -0.5);
        //}

        //[TestMethod, TestCategory("VB")]
        //public void VB_Priorities()
        //{
        //    TestVbAndCsFormula("-1.5*-2.5", -1.5 * -2.5);
        //}

        //[TestMethod, TestCategory("VB")]
        //public void VB_Template()
        //{
        //    TestTemplate("<p>Hello</p>", "<p>Hello</p>");
        //}
    }
}
