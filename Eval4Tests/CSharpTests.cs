using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.Tests
{
    [TestClass]
    public class CSharpTests : BaseTest<CSharpEvaluator>
    {
        public class TestContext
        {
            // fields
            public byte @byte = 1;
            public sbyte @sbyte = 2;
            public short @short = 3;
            public int @int = 4;
            public uint @uint = 5;
            public long @long = 6;
            public ulong @ulong = 7;
            public float @float = 8;
            public double @double = 9;
            public decimal @decimal = 10;

            public static double AddVat(double vat, double value)
            {
                return value * ((100 + vat) / 100.0);
            }
        }


        public CSharpTests()
        {
            ev.SetVariable("context", new TestContext());
            ev.SetVariable("fibonacci", new int[] { 1, 1, 2, 3, 5, 8, 13, 21, 34 });
            ev.SetVariable("mult", new int[,] { { 0, 0, 0, 0 }, { 0, 1, 2, 3 }, { 0, 2, 4, 6 }, { 0, 3, 6, 9 } });

        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_FieldTypes()
        {
            var context = new TestContext();
            TestFormula("context.byte", context.@byte);
            TestFormula("context.decimal", context.@decimal);
            TestFormula("context.double", context.@double);
            TestFormula("context.float", context.@float);
            TestFormula("context.int", context.@int);
            TestFormula("context.long", context.@long);
            TestFormula("context.sbyte", context.@sbyte);
            TestFormula("context.short", context.@short);
            TestFormula("context.uint", context.@uint);
            TestFormula("context.ulong", context.@ulong);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_FieldCalculation()
        {
            var context = new TestContext();
            // those are returning int32 (like in C#)
            TestFormula("context.byte * 2", (int)context.@byte * 2);
            TestFormula("context.sbyte * 2", (int)context.@sbyte * 2);
            TestFormula("context.short * 2", (int)context.@short * 2);
            TestFormula("context.int * 2", (int)context.@int * 2);

            TestFormula("context.double * 2", context.@double * 2);

            // those return double not like in C# but I did not write all the types arithmetics

            // those types are not working like C# and revert return doubles
            TestFormula("context.uint * 2", (double)context.@uint * 2);
            TestFormula("context.long * 2", (double)context.@long * 2);
            TestFormula("context.ulong * 2", (double)context.@ulong * 2);
            TestFormula("context.float * 2", (double)context.@float * 2);

            // decimal are not supported
            TestFormula("context.decimal * 2", (double)context.@decimal * 2);

        }



        [TestMethod, TestCategory("C#")]
        public void CSharp_Modulo()
        {
            TestFormula("23 % 10", 3);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_BitwiseAnd()
        {
            TestFormula("3 & 254", 2);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_BooleanAnd()
        {
            TestFormula("false && true", false);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_EqOperator()
        {
            TestFormula("1 == 2", false);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_NEOperator()
        {
            TestFormula("1 != 2", true);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_NotOperator()
        {
            TestFormula("! true", false);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_BitwiseOr()
        {
            TestFormula("1 | 255", 255);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_BooleanOr()
        {
            TestFormula("false || true", true);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_If()
        {
            TestFormula("true ?  1:0", 1);
            TestFormula("false ?  1:0", 0);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_Numerics()
        {
            TestFormula("1", 1);
            TestFormula("-1", -1);
            TestFormula("1e0", 1e0);
            TestFormula("1e1", 1e1);
            TestFormula("1e-10", 1e-10);
            TestFormula("1.2345e-2", 1.2345e-2);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_AccessArrayVariables()
        {
            TestFormula("fibonacci[0]", 1);
            TestFormula("fibonacci[1]", 1);
            TestFormula("fibonacci[2]", 2);
            TestFormula("fibonacci[3]", 3);
            TestFormula("fibonacci[4]", 5);
            TestFormula("fibonacci[5]*2", 16);
            TestFormula("mult[1,0]", 0);
            TestFormula("mult[1,2]", 2);
            TestFormula("mult[2,3]", 6);
            TestFormula("mult[3,3]", 9);

        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_NumberLiterals()
        {
            TestFormula("1.5", 1.5);
            TestFormula("0.5", 0.5);
            TestFormula(".5", .5);
            TestFormula("-.5", -.5);
            TestFormula("-0.5", -0.5);
        }

        [TestMethod, TestCategory("C#")]
        public void CSharp_Priorities()
        {
            TestFormula("-1.5*-2.5", -1.5 * -2.5);
        }

        //[TestMethod, TestCategory("C#")]
        //public void CSharp_Template()
        //{
        //    //   TestTemplate("<p>Hello</p>", "<p>Hello</p>");
        //}    }
        //[TestClass]
        //public class TestEnvironmentFunctions //: BaseTest
        //{
        //public TestEnvironmentFunctions()
        //{
        //    evVB.AddEnvironmentFunctions(typeof(EnvironmentFunctions));
        //}


        //[TestMethod, TestCategory("C#")]
        //public void CSharp_Environment()
        //{
        //    TestVBFormula("avg(2,3,5)", 10 / 3.0);
        //}
        //}

        //public static class EnvironmentFunctions
        //{
        //    public static double avg(double a, double b, double c)
        //    {
        //        return (a + b + c) / 3.0;
        //    }
        //}
    }
}
