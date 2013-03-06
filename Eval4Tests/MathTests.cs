using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.Tests
{
    [TestClass]
    public class MathTests : BaseTest<MathEvaluator>
    {
        [TestMethod, TestCategory("Math")]
        public void Math_Arithmetic()
        {
            TestFormula("3", 3.0);
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixLiteral()
        {
            TestFormula("[1,2,3;4,5,6]", new Matrix(new double[][]{
                new double[]{1,2,3},
                new double[]{4,5,6}
            }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixAdd()
        {
            TestFormula("[1,2,3]+[4,5,6]", new Matrix(new double[][] { new double[] { 5, 7, 9 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixSub()
        {
            TestFormula("[4,5,6]-[1,2,3]", new Matrix(new double[][] { new double[] { 3, 3, 3 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixMul()
        {
            TestFormula("[1,2,3]*[4;5;6]", new Matrix(new double[][] { new double[] { 1 * 4 + 2 * 5 + 3 * 6 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixScalarAdd()
        {
            TestFormula("[1,2,3]+1", new Matrix(new double[][] { new double[] { 2, 3, 4 } }));
            TestFormula("1+[1,2,3]", new Matrix(new double[][] { new double[] { 2, 3, 4 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixScalarSub()
        {
            TestFormula("[1,2,3]-1", new Matrix(new double[][] { new double[] { 0, 1, 2 } }));
            TestFormula("3-[1,2,3]", new Matrix(new double[][] { new double[] { 2, 1, 0 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixScalarPower()
        {
            TestFormula("[1,2,3]^2", new Matrix(new double[][] { new double[] { 1, 4, 9 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixScalarMul()
        {
            TestFormula("[1,2,3]*4", new Matrix(new double[][] { new double[] { 4, 8, 12 } }));
            TestFormula("4*[1,2,3]", new Matrix(new double[][] { new double[] { 4, 8, 12 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_MatrixScalarDiv()
        {
            TestFormula("[2,4,6]/2", new Matrix(new double[][] { new double[] { 1, 2, 3 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_ElementWiseAdd()
        {
            TestFormula("[1,2,3]+[2,3,4]", new Matrix(new double[][] { new double[] { 3, 5, 7 } }));
            TestFormula("[1;2;3].+[2;3;4]", new Matrix(new double[][] { new double[] { 3 }, new double[] { 5 }, new double[] { 7 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_ElementWiseSub()
        {
            TestFormula("[2;6;12]-[1;2;3]", new Matrix(new double[][] { new double[] { 1 }, new double[] { 4 }, new double[] { 9 } }));
            TestFormula("[4,5,6].-[2,3,4]", new Matrix(new double[][] { new double[] { 2, 2, 2 } }));
        }
        
        [TestMethod, TestCategory("Math")]
        public void Math_ElementWiseMul()
        {
            TestFormula("[1,2,3].*[2,3,4]", new Matrix(new double[][] { new double[] { 2, 6, 12 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_ElementWiseDiv()
        {
            TestFormula("[2;6;12]./[1;2;3]", new Matrix(new double[][] { new double[] { 2 }, new double[] { 3 }, new double[] { 4 } }));
        }

        [TestMethod, TestCategory("Math")]
        public void Math_ElementWisePower()
        {
            TestFormula("[1,2,3].^[1,2,3]", new Matrix(new double[][] { new double[] { 1, 4, 27 } }));
        }
    }
}
