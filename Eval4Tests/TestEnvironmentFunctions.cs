using System;
using Eval4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.Tests
{
    [TestClass]
    public class TestEnvironmentFunctions : BaseTest
    {
        public TestEnvironmentFunctions()
        {
            evVB.AddEnvironmentFunctions(typeof(EnvironmentFunctions));
        }


        [TestMethod]
        public void TestEnvironment()
        {
            TestVBFormula("avg(2,3,5)", 10 / 3.0);
        }
    }

    public static class EnvironmentFunctions
    {
        public static double avg(double a, double b, double c)
        {
            return (a + b + c) / 3.0;
        }
    }
}
