using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eval4.Core;

namespace Eval4
{
    public class BaseTest<T> where T : IEvaluator, new()
    {
        protected T ev;

        public BaseTest()
        {
            ev = new T();
        }

        public void TestFormula<T>(string formula, T expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

    }
}
