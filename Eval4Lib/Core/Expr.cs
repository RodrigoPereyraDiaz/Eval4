using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
namespace Eval4.Core
{
    public abstract class Expr : IHasValue, IObserver
    {
        internal List<IDisposable> mSubscribedTo = new List<IDisposable>();
        internal List<Subscription> mSubscribedBy = new List<Subscription>();

        public Expr(IHasValue p0, params IHasValue[] parameters)
        {
            if (p0 != null)
            {
                mSubscribedTo.Add(p0.Subscribe(this));
                if (parameters != null)
                {
                    foreach (IHasValue p1 in parameters)
                    {
                        mSubscribedTo.Add(p1.Subscribe(this));
                    }
                }
            }
        }

        public abstract object ObjectValue { get; }

        public IDisposable Subscribe(IObserver observer)
        {
            var result = new Subscription(this, observer);
            mSubscribedBy.Add(result);
            return result;
        }

        public abstract Type SystemType { get; }

        public abstract string ShortName { get; }

        public virtual IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield break;
            }
        }

        void IObserver.OnValueChanged()
        {
            RaiseValueChanged();
        }

        protected void RaiseValueChanged()
        {
            foreach (var subscription in mSubscribedBy)
            {
                subscription.mObserver.OnValueChanged();
            }
        }
    }

    public class Subscription : IDisposable
    {
        internal Expr mSource;
        internal IObserver mObserver;

        public Subscription(Expr source, IObserver observer)
        {
            mSource = source;
            mObserver = observer;
        }
        public void Dispose()
        {
            mSource.mSubscribedTo.Remove(this);
        }

    }

    public abstract class Expr<T> : Expr, IHasValue<T>
    {
        public abstract T Value { get; }

        public Expr(IHasValue p0, params IHasValue[] parameters)
            : base(p0, parameters)
        {
        }

        public override object ObjectValue
        {
            get { return Value; }
        }

        public override Type SystemType
        {
            get { return typeof(T); }
        }
    }

    internal class ConstantExpr<T> : Expr<T>
    {
        private T mValue;

        public ConstantExpr(T value)
            : base(null)
        {
            mValue = value;
        }

        //public event ValueChangedEventHandler ValueChanged;

        public override string ShortName
        {
            get { return "Literal"; }
        }

        public override T Value
        {
            get { return mValue; }
        }
    }

    public class CallMethodExpr<T> : Expr<T>
    {
        Func<T> mValueDelegate;
        private object mBaseObject;
        private IHasValue withEventsField_mBaseValue;
        private object mBaseValueObject;
        private MemberInfo mMethod;
        private IHasValue[] mParams;

        private object[] mParamValues;
        private System.Type mResultSystemType;
        private IHasValue withEventsField_mResultValue;

        public CallMethodExpr(IHasValue baseObject, MemberInfo method, List<IHasValue> @params, Delegate[] casts)
            : base(baseObject, @params == null ? null : @params.ToArray())
        {
            if (@params == null)
                @params = new List<IHasValue>();
            IHasValue[] newParams = @params.ToArray();
            object[] newParamValues = new object[@params.Count];

            mParams = newParams;
            mParamValues = newParamValues;
            mBaseObject = baseObject;
            mMethod = method;

            ParameterInfo[] paramInfo = null;
            if (method is PropertyInfo)
            {
                mResultSystemType = ((PropertyInfo)method).PropertyType;
                paramInfo = ((PropertyInfo)method).GetIndexParameters();
                mValueDelegate = GetProperty;
            }
            else if (method is MethodInfo)
            {
                var mi = (MethodInfo)method;
                mResultSystemType = mi.ReturnType;
                paramInfo = mi.GetParameters();
                mValueDelegate = GetMethod;
                //var dlg = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), (MethodInfo)mi);
                //mValueDelegate = dlg;
            }
            else if (method is FieldInfo)
            {
                mResultSystemType = ((FieldInfo)method).FieldType;
                paramInfo = new ParameterInfo[] { };
                Expression expr = null;
                if (baseObject != null)
                {
                    var expectedType = typeof(IHasValue<>).MakeGenericType(baseObject.SystemType);
                    expr = Expression.MakeMemberAccess(Expression.Constant(baseObject), expectedType.GetProperty("Value"));
                    expr = Expression.Field(expr, (FieldInfo)method);
                }
                else
                {
                    expr = Expression.Field(null, (FieldInfo)method);
                }
                var lambda = Expression.Lambda<Func<T>>(expr);
                mValueDelegate = lambda.Compile();
            }

            for (int i = 0; i <= mParams.Length - 1; i++)
            {
                if (i < paramInfo.Length)
                {
                    var sourceType = mParams[i].SystemType;
                    var targetType = paramInfo[i].ParameterType;
                    if (sourceType != targetType)
                    {
                        var c2 = typeof(NewTypedExpr<,>).MakeGenericType(sourceType, targetType);
                        mParams[i] = (IHasValue)Activator.CreateInstance(c2, mParams[i], casts[i]);
                    }
                }
            }
        }

        public IHasValue mBaseValue
        {
            get { return withEventsField_mBaseValue; }
            set
            {
                if (withEventsField_mBaseValue != null)
                {
                    //throw new NotImplementedException();
                    //withEventsField_mBaseValue.ValueChanged -= withEventsField_mBaseValue_ValueChanged;
                }
                withEventsField_mBaseValue = value;
                if (withEventsField_mBaseValue != null)
                {
                    //throw new NotImplementedException();
                    //withEventsField_mBaseValue.ValueChanged += withEventsField_mBaseValue_ValueChanged;
                }
            }
            // for the events only
        }

        void withEventsField_mBaseValue_ValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public IHasValue mResultValue
        {
            get { return withEventsField_mResultValue; }
            set
            {
                if (withEventsField_mResultValue != null)
                {
                    throw new NotImplementedException();
                    //withEventsField_mResultValue.ValueChanged -= withEventsField_mResultValue_ValueChanged;
                }
                withEventsField_mResultValue = value;
                if (withEventsField_mResultValue != null)
                {
                    throw new NotImplementedException();
                    //withEventsField_mResultValue.ValueChanged += withEventsField_mResultValue_ValueChanged;
                }
            }
            // just for some
        }

        void withEventsField_mResultValue_ValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private T GetProperty()
        {
            Recalc();
            object res = ((PropertyInfo)mMethod).GetValue(mBaseValueObject, mParamValues);
            return (T)res;
        }

        private T GetMethod()
        {
            Recalc();
            object res = ((MethodInfo)mMethod).Invoke(mBaseValueObject, mParamValues);
            return (T)res;
        }

        //private T GetField()
        //{
        //    Recalc();
        //    object res = ((FieldInfo)mMethod).GetValue(mBaseValueObject);
        //    return (T)res;
        //}

        private void Recalc()
        {
            for (int i = 0; i <= mParams.Length - 1; i++)
            {
                mParamValues[i] = mParams[i].ObjectValue;
            }
            if (mBaseObject is IHasValue)
            {
                mBaseValue = (IHasValue)mBaseObject;
                mBaseValueObject = mBaseValue.ObjectValue;
            }
            else
            {
                mBaseValueObject = mBaseObject;
            }
        }

        //public event ValueChangedEventHandler ValueChanged;

        public override T Value
        {
            get { return mValueDelegate(); }
        }

        public override string ShortName
        {
            get { return "CallMethod"; }
        }

        public override IEnumerable<Dependency> Dependencies
        {
            get
            {
                if (mBaseValue != null) yield return new Dependency("base", mBaseValue);
                for (int i = 0; i < mParams.Length; i++)
                {
                    yield return new Dependency("#" + i, mParams[i]);
                }
            }
        }

    }

    public class GetArrayEntryExpr<T> : Expr<T>
    {

        private IHasValue withEventsField_mArray;
        private IHasValue[] mParams;
        private int[] mValues;
        private Type mResultSystemType;

        public IHasValue mArray
        {
            get { return withEventsField_mArray; }
            set
            {
                if (withEventsField_mArray != null)
                {
                    //throw new NotImplementedException();
                    //withEventsField_mArray.ValueChanged -= mBaseVariable_ValueChanged;
                }
                withEventsField_mArray = value;
                if (withEventsField_mArray != null)
                {
                    //throw new NotImplementedException();
                    //withEventsField_mArray.ValueChanged += mBaseVariable_ValueChanged;
                }
            }

        }

        public GetArrayEntryExpr(IHasValue array, List<IHasValue> @params)
            : base(array, @params.ToArray())
        {
            IHasValue[] newParams = @params.ToArray();
            int[] newValues = new int[@params.Count];

            mArray = array;
            mParams = newParams;
            mValues = newValues;
            mResultSystemType = array.SystemType.GetElementType();
        }

        public override T Value
        {
            get
            {
                object res = null;
                Array arr = (Array)mArray.ObjectValue;

                for (int i = 0; i <= mValues.Length - 1; i++)
                {
                    mValues[i] = System.Convert.ToInt32(mParams[i].ObjectValue);
                }

                res = arr.GetValue(mValues);
                return (T)res;
            }
        }

        public System.Type SystemType
        {
            get { return typeof(T); }
        }

        private void mBaseVariable_ValueChanged(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
            //if (ValueChanged != null) ValueChanged(sender, e);
        }


        public override IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield return new Dependency("Array", mArray);
                for (int i = 0; i < mValues.Length; i++)
                {
                    yield return new Dependency("P" + i, mParams[i]);
                }
            }
        }

        public override string ShortName
        {
            get { return "ArrayEntry[]"; }
        }
    }

    public class OperatorIfExpr<T> : Expr<T>
    {
        private Type mSystemType;
        private IHasValue ifExpr;
        private IHasValue thenExpr;
        private IHasValue elseExpr;

        public OperatorIfExpr(IHasValue ifExpr, IHasValue thenExpr, IHasValue elseExpr)
            : base(ifExpr, thenExpr, elseExpr)
        {
            this.ifExpr = ifExpr;
            this.thenExpr = thenExpr;
            this.elseExpr = elseExpr;
            mSystemType = thenExpr.SystemType;
        }

        public override T Value
        {
            get
            {
                object result;
                var test = System.Convert.ToBoolean(ifExpr.ObjectValue);
                result = test ? thenExpr.ObjectValue : elseExpr.ObjectValue;

                if (result != null && result.GetType() != mSystemType) result = System.Convert.ChangeType(result, mSystemType);
                return (T)result;
            }
        }

        public override IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield return new Dependency("if", ifExpr);
                yield return new Dependency("then ", thenExpr);
                yield return new Dependency("else", elseExpr);

            }
        }

        public override string ShortName
        {
            get { return "OperatorIf"; }
        }

    }

    internal class NewTypedExpr<P1, T> : Expr<T>
    {
        private IHasValue<P1> mP1;
        private Func<P1, T> mFunc;

        public NewTypedExpr(IHasValue<P1> p1, Func<P1, T> func)
            : base(p1)
        {
            System.Diagnostics.Debug.Assert(func != null);
            mP1 = p1;
            mFunc = func;
        }


        public override T Value
        {
            get { return mFunc(mP1.Value); }
        }

        //public event ValueChangedEventHandler ValueChanged;

        public override string ShortName
        {
            get { return "NewTypedExpr"; }
        }

        public override IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield return new Dependency("p1", mP1);
            }
        }
    }

    internal class NewTypedExpr<P1, P2, T> : Expr<T>
    {
        private IHasValue<P1> mP1;
        private IHasValue<P2> mP2;
        private Func<P1, P2, T> mFunc;

        public NewTypedExpr(IHasValue<P1> p1, IHasValue<P2> p2, Func<P1, P2, T> func)
            : base(p1, p2)
        {
            mP1 = p1;
            mP2 = p2;
            mFunc = func;
        }


        public override T Value
        {
            get { return mFunc(mP1.Value, mP2.Value); }
        }

        public override string ShortName
        {
            get { return "NewTypedExpr"; }
        }

        public override IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield return new Dependency("p1", mP1);
                yield return new Dependency("p2", mP2);
            }
        }
    }

    class GetVariableFromBag<T> : Expr<T>
    {
        private string mVariableName;
        private Evaluator mEvaluator;
        //public event ValueChangedEventHandler ValueChanged;
        private Variable<T> mVariable;

        public GetVariableFromBag(Evaluator evaluator, string variableName)
            : base(null)
        {
            mEvaluator = evaluator;
            mVariableName = variableName;
            mVariable = (Variable<T>)mEvaluator.mVariableBag[mVariableName];
        }

        public override T Value
        {
            get
            {
                return mVariable.Value;
            }
        }

        public override string ShortName
        {
            get { return "GetVariableFromBag"; }
        }
    }

    class RaiseFindVariableExpr<T> : Expr<T>
    {
        private string mVariableName;
        private Evaluator mEvaluator;
        //public event ValueChangedEventHandler ValueChanged;
        private FindVariableEventArgs mFindVariableResult;

        public RaiseFindVariableExpr(Evaluator evaluator, string variableName)
            : base(null)
        {
            mEvaluator = evaluator;
            mVariableName = variableName;
        }

        public override T Value
        {
            get
            {
                mFindVariableResult = mEvaluator.RaiseFindVariable(mVariableName);
                return (T)mFindVariableResult.Value;
            }
        }

        public override string ShortName
        {
            get { return "FindVariable"; }
        }
    }

    public class Variable<T> : Expr<T>, IVariable
    {
        private T mValue;
        private string mVariableName;

        public Variable(T variableValue, string variableName)
            : base(null)
        {
            mValue = variableValue;
            mVariableName = variableName;
        }

        public override T Value
        {
            get { return mValue; }
        }

        public override string ShortName
        {
            get { return mVariableName; }
        }

        public void SetValue(object value)
        {
            mValue = (T)value;
            base.RaiseValueChanged();
        }
    }
}
