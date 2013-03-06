using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        public void TestFormula<K>(string formula, K expectedResult)
        {
            var actualResult = ev.Eval(formula);
            Assert.AreEqual(expectedResult, actualResult, formula);
        }

    }
}
