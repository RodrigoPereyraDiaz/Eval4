using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Core
{
    class RaiseFindVariableExpr<T> : IHasValue<T>
    {
        private string mVariableName;
        private Evaluator mEvaluator;
        public event ValueChangedEventHandler ValueChanged;
        private FindVariableEventArgs mFindVariableResult;

        public RaiseFindVariableExpr(Evaluator evaluator, string variableName)
        {
            mEvaluator = evaluator;
            mVariableName = variableName;
        }

        public T Value
        {
            get
            {
                mFindVariableResult = mEvaluator.RaiseFindVariable(mVariableName);
                return (T)mFindVariableResult.Value;
            }
        }

        public object ObjectValue
        {
            get
            {
                mFindVariableResult = mEvaluator.RaiseFindVariable(mVariableName);
                return mFindVariableResult.Value;
            }
        }


        public Type SystemType
        {
            get { return typeof(T); }
        }

        public string ShortName
        {
            get { return "FindVariable"; }
        }

        public IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield break;
            }
        }
    }
}
