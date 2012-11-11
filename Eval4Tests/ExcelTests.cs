using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eval4.Tests
{
    [TestClass]
    public class ExcelTests : BaseTest<ExcelEvaluator>
    {

        public ExcelTests()
        {
            ev.SetCell("A1", "1");
            ev.SetCell("A2", "2");
            ev.SetCell("B1", "a");
            ev.SetCell("B2", "b");
        }

        [TestMethod]
        public void Excel_ReadCell()
        {
            // A1 return a cell for now.
            TestFormula("A1.valueObject", 1.0);
        }

        [TestMethod]
        public void Excel_SimpleAddition()
        {
            TestFormula("SUM(A1,A2)", 3.0);
        }

        [TestMethod]
        public void Excel_RangeAddition()
        {
            TestFormula("SUM(A1:A2)", 3.0);
        }

        [TestMethod]
        public void Excel_Average()
        {
            TestFormula("AVERAGE(A1:A2)", 1.5);
        }

        [TestMethod]
        public void Excel_VLookup()
        {
            // vlookup return a cell
            TestFormula("VLOOKUP(2,A1:B2,2,FALSE).valueObject", "b");
        }

    }
}
