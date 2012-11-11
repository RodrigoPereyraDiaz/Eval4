using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eval4.Math;

namespace Eval4.Tests
{
    [TestClass]
    public class MathTests : BaseTest
    {
        [TestMethod]
        public void TestArithmetic()
        {
            TestMath("3", 3.0);
        }

        [TestMethod]
        public void TestMatrixLiteral()
        {
            TestMath("[1,2,3;4,5,6]", new Matrix(new double[][]{
                new double[]{1,2,3},
                new double[]{4,5,6}
            }));
        }

        [TestMethod]
        public void TestMatrixAdd()
        {
            TestMath("[1,2,3]+[4,5,6]", new Matrix(new double[][] { new double[] { 5, 7, 9 } }));
        }

        [TestMethod]
        public void TestMatrixSub()
        {
            TestMath("[4,5,6]-[1,2,3]", new Matrix(new double[][] { new double[] { 3, 3, 3 } }));
        }

        [TestMethod]
        public void TestMatrixMul()
        {
            TestMath("[1,2,3]*[4;5;6]", new Matrix(new double[][] { new double[] { 1 * 4 + 2 * 5 + 3 * 6 } }));
        }

        [TestMethod]
        public void TestMatrixScalarMul()
        {
            TestMath("[1,2,3]*4", new Matrix(new double[][] { new double[] { 4, 8, 12 } }));
        }

        [TestMethod]
        public void TestMatrixEntryWiseMul()
        {
            TestMath("[1,2,3].*[2,3,4]", new Matrix(new double[][] { new double[] { 2, 6, 12 } }));
        }

        [TestMethod]
        public void TestMatrixEntryWiseDiv()
        {
            TestMath("[2;6;12]./[1;2;3]", new Matrix(new double[][] { new double[] { 2 }, new double[] { 3 }, new double[] { 4 } }));
        }

    }
}
