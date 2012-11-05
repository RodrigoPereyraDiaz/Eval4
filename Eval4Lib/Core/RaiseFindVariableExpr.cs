using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Core
{
    class GetVariableFromBag<T> : IHasValue<T>
    {
        private string mVariableName;
        private Evaluator mEvaluator;
        public event ValueChangedEventHandler ValueChanged;
        private Variable<T> mVariable;

        public GetVariableFromBag(Evaluator evaluator, string variableName)
        {
            mEvaluator = evaluator;
            mVariableName = variableName;
            mVariable = (Variable<T>)mEvaluator.mVariableBag[mVariableName];
        }

        public T Value
        {
            get
            {
                return mVariable.Value;
            }
        }

        public object ObjectValue
        {
            get
            {
                return mVariable.Value;
            }
        }


        public Type SystemType
        {
            get { return typeof(T); }
        }

        public string ShortName
        {
            get { return "GetVariableFromBag"; }
        }

        public IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield break;
            }
        }
    }

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
