using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.CSharpTests
{
    [TestClass]
    public class TestArraysAndObjects : BaseTest
    {
        static Accounts accountInstance = new Accounts();
        static int[] pascal = new int[] { 1, 8, 28, 56, 70, 56, 28, 8, 1 };
        
        public TestArraysAndObjects()
        {
            InitEvaluator(evVB);
            InitEvaluator(evCS);
        }

        private void InitEvaluator(Eval4.Core.Evaluator ev)
        {
            ev.SetVariable("pascal", pascal);
            ev.SetVariable("fibonacci", new int[] { 1, 1, 2, 3, 5, 8, 13, 21, 34 });
            ev.SetVariable("mult", new int[,] { { 0, 0, 0, 0 }, { 0, 1, 2, 3 }, { 0, 2, 4, 6 }, { 0, 3, 6, 9 } });
            ev.SetVariable("accounts", accountInstance);
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

        [TestMethod]
        public void CheckArrays()
        {
            TestCSFormula("pascal", pascal);
            TestCSFormula("pascal[0]", 1);
            TestCSFormula("pascal[2]", 28);
            TestCSFormula("pascal[2]*2", 56);
            TestCSFormula("mult[1,0]", 0);
            TestCSFormula("mult[1,2]", 2);
            TestCSFormula("mult[2,3]", 6);
            TestCSFormula("mult[3,3]", 9);

            TestVBFormula("pascal(0)", 1);
            TestVBFormula("pascal(2)", 28);
            TestVBFormula("pascal(2)*2", 56);
            TestVBFormula("mult(1,0)", 0);
            TestVBFormula("mult(1,2)", 2);
            TestVBFormula("mult(2,3)", 6);
            TestVBFormula("mult(3,3)", 9);
        }

        [TestMethod]
        public void CheckMethod()
        {
            TestVbAndCsFormula("accounts.Credit", 150.00);
            TestVbAndCsFormula("accounts.Vat", 20.00);
            TestVbAndCsFormula("accounts.CreditWithVat", 180.0);
            TestVbAndCsFormula("accounts.AddVat(20,100)", 120.0);

            TestVbAndCsFormula("accounts.ByteValue", (byte)123);
            TestVbAndCsFormula("accounts.SingleValue", (Single)123);
            TestVbAndCsFormula("accounts.DecimalValue", (decimal)123);
            TestVbAndCsFormula("accounts.Int16Value", (Int16)123);

            TestVbAndCsFormula("accounts.ByteValue * 1.0", 123.0);
            TestVbAndCsFormula("accounts.SingleValue  * 1.0", 123.0);
            //TestVbAndCsFormula("accounts.DecimalValue * 1.0", accountInstance.DecimalValue * 1.0);
            TestVbAndCsFormula("accounts.Int16Value * 1.0", 123.0);
            //TestVbAndCsFormula("accounts.Sum(1,2,3,4)", (decimal)10.0);

        }
    }
}
