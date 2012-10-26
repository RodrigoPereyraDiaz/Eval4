using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class TestArraysAndObjects
    {
        static VbEvaluator evVB;
        static CSharpEvaluator evCSharp;

        static TestArraysAndObjects()
        {
            evVB = new VbEvaluator();
            evCSharp = new CSharpEvaluator();
            InitEvaluator(evVB);
            InitEvaluator(evCSharp);

        }

        private static void InitEvaluator(Eval4.Core.Evaluator res)
        {
            res.SetVariable("pascal", new int[] { 1, 8, 28, 56, 70, 56, 28, 8, 1 });
            res.SetVariable("fibonacci", new int[] { 1, 1, 2, 3, 5, 8, 13, 21, 34 });
            res.SetVariable("mult", new int[,] { { 0, 0, 0, 0 }, { 0, 1, 2, 3 }, { 0, 2, 4, 6 }, { 0, 3, 6, 9 } });
            res.SetVariable("accounts", new Accounts());
        }

        public class Accounts
        {
            public double Credit { get { return 150.00; } }
            public double Vat { get { return 20.0; } }
            public byte ByteValue { get { return 123; } }
            public Single SingleValue { get { return 123; } }
            public Decimal DecimalValue { get { return 123; } }
            public readonly Int16 Int16Value = 123;
            public double CreditWithVat()
            {
                return AddVat(Vat, Credit);
            }

            public static double AddVat(double vat, double value)
            {
                return value * ((100 + vat) / 100.0);
            }

            public Decimal Sum(Int16 a, UInt32 b, Single c, decimal d)
            {
                return ((decimal)(a + b + c)) + d;
            }
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
        public void CheckArrays()
        {
            TestFormula("pascal[0]", 1);
            TestFormula("pascal[2]", 28);
            TestFormula("pascal[2]/2", 14.0);
            TestFormula("mult[1,0]", 0);
            TestFormula("mult[1,2]", 2);
            TestFormula("mult[2,3]", 6);
            TestFormula("mult[3,3]", 9);
        }

        [TestMethod]
        public void CheckMethod()
        {
            TestFormula("accounts.Credit", 150.00);
            TestFormula("accounts.Vat", 20.00);
            TestFormula("accounts.CreditWithVat", 180.0);
            TestFormula("accounts.AddVat(20,100)", 120.0);

            TestFormula("accounts.ByteValue", (byte)123);
            TestFormula("accounts.SingleValue", (Single)123);
            TestFormula("accounts.DecimalValue", (decimal)123);
            TestFormula("accounts.Int16Value", (Int16)123);

            TestFormula("accounts.ByteValue * 1.0", 123.0);
            TestFormula("accounts.SingleValue  * 1.0", 123.0);
            TestFormula("accounts.DecimalValue * 1.0", 123.0);
            TestFormula("accounts.Int16Value * 1.0", 123.0);
            TestFormula("accounts.Sum(1,2,3,4)", (decimal)10.0);

        }
    }
}
