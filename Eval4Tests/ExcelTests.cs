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
            ev.SetCell("A3", "3");

            ev.SetCell("B1", "a");
            ev.SetCell("B2", "b");
            ev.SetCell("B3", "c");
        }

        [TestMethod, TestCategory("Excel")]
        public void Excel_ReadCell()
        {
            // A1 return a cell for now.
            TestFormula("A1.valueObject", 1.0);
        }

        [TestMethod, TestCategory("Excel")]
        public void Excel_SimpleAddition()
        {
            TestFormula("SUM(A1,A2)", 3.0);
        }

        [TestMethod, TestCategory("Excel")]
        public void Excel_RangeAddition()
        {
            TestFormula("SUM(A1:A2)", 3.0);
        }

        [TestMethod, TestCategory("Excel")]
        public void Excel_Average()
        {
            TestFormula("AVERAGE(A1:A2)", 1.5);
        }

        [TestMethod, TestCategory("Excel")]
        public void Excel_VLookup()
        {
            // vlookup return a cell
            TestFormula("VLOOKUP(2,A1:B3,2,FALSE).valueObject", "b");
        }

        [TestMethod, TestCategory("Excel")]
        public void Excel_VLookup_with_range_lookup()
        {
            // vlookup return a cell
            TestFormula("VLOOKUP(2.2,A1:C3,2,TRUE).valueObject", "b");
        }
    }
}
