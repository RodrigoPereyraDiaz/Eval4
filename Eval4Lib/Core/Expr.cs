using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Eval4.Core
{

    // Expr is an implementation for IHasValue.
    // in Evaluator we should not be using Expr but only IHasValue
    public abstract class Expr : IHasValue, IObserver
    {
        internal List<ISubscription> mSubscribedBy = new List<ISubscription>();
        internal List<ISubscription> mSubscribedTo = new List<ISubscription>();
        protected bool mModified;
        internal bool mDisposed;
        
        public Expr()
        {
            mModified = true;
        }

        protected void Subscribe(string name, IEnumerable<IHasValue> list)
        {
            if (list == null) return;
            int cpt = 0;
            foreach (var p0 in list)
            {
                p0.Subscribe(this, name + (++cpt).ToString());
            }
        }

        public abstract object ObjectValue { get; }

        public ISubscription Subscribe(IObserver observer, string role)
        {
            if (mDisposed)
            {
                mDisposed = false;
                
            }
            
            var result = new Subscription(this, observer);
            //mDependencies.Add(new Dependency(role,
            mSubscribedBy.Add(result);

            var expr = observer as Expr;
            if (expr != null)
            {
                expr.mSubscribedTo.Add(result);
            }
            return result;
        }

        public abstract Type ValueType { get; }

        public abstract string ShortName { get; }

        void IObserver.OnValueChanged(IHasValue value)
        {
            RaiseValueChanged();
        }

        protected void RaiseValueChanged()
        {
            if (mModified) return;
            mModified = true;
            foreach (var subscription in mSubscribedBy)
            {
                subscription.Observer.OnValueChanged(this);
            }
        }

        public IEnumerable<ISubscription> Subscriptions
        {
            get
            {
                return mSubscribedTo.ToArray();
            }
        }

        private class Subscription : ISubscription
        {
            internal Expr mSourceExpr;
            internal IObserver mObserver;

            public Subscription(Expr source, IObserver observer)
            {
                mSourceExpr = source;
                mObserver = observer;
            }

            public void Dispose()
            {
                mSourceExpr.mSubscribedBy.Remove(this);
                if (mSourceExpr.mSubscribedBy.Count == 0)
                {
                    mSourceExpr.Dispose();
                }
            }

            public string Name
            {
                get { return string.Empty; }
            }

            public IHasValue Source
            {
                get { return mSourceExpr; }
            }

            public IObserver Observer
            {
                get { return mObserver; }
            }
        }

        public void Dispose()
        {
            if (!mDisposed)
            {
                mDisposed = true;
                foreach (var x in mSubscribedTo)
                {
                    x.Dispose();
                }
                //mSubscribedTo.Clear();
            }
        }

        public override string ToString()
        {
            var result=new StringBuilder();
            if (mModified) result.Append("* ");
            result.Append(ShortName);
            result.Append("(");
            bool first = true;
            foreach (var c in mSubscribedTo)
            {
                if (first) first = false;
                else result.Append(", ");
                result.Append(c.Source.ToString());
            }
            result.Append(")");
            return result.ToString();
        }

        internal void Recycle()
        {
            if (!mDisposed) return;
            mModified = true;
            mDisposed = false;
            foreach (var m in mSubscribedTo)
            {
                var sourceAsExpr = m.Source as Expr;
                if (sourceAsExpr != null)
                {
                    sourceAsExpr.mSubscribedBy.Add(m);
                    ((Expr)m.Source).Recycle();
                }
            }
        }
    }

    
    public abstract class Expr<T> : Expr, IHasValue<T>
    {
        private T mValue;

        public abstract T Value { get; }

        public sealed override object ObjectValue
        {
            //[System.Diagnostics.DebuggerStepThrough()]
            get {
                if (mModified)
                {
                    mModified = false;
                    mValue = Value;
                }
                return mValue;
            }
        }

        public override Type ValueType
        {
            //[System.Diagnostics.DebuggerStepThrough()]
            get { return typeof(T); }
        }

        T IHasValue<T>.Value
        {
            get
            {
                if (mModified)
                {
                    mModified = false;
                    mValue = this.Value;
                }
                return mValue;
            }
        }

    }

    internal class ConstantExpr<T> : Expr<T>
    {
        private T mValue;

        public ConstantExpr(T value)
        {
            mValue = value;
        }

        //public event ValueChangedEventHandler ValueChanged;

        public override string ShortName
        {
            get { return "Constant"; }
        }

        public override T Value
        {
            //[System.Diagnostics.DebuggerStepThrough()]
            get { return mValue; }
        }
    }

    public class ArrayBuilder<T> : Expr<T[]>
    {
        private IHasValue[] mEntries;
        private Delegate[] mCasts;
        private T[] mResult;

        public ArrayBuilder(IHasValue[] entries, object[] casts)
        {
            mEntries = entries;
            mCasts = casts.Cast<Delegate>().ToArray();
            mResult = new T[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                if (casts[i] != null)
                {

                }
                else entries[i].Subscribe(this, "#" + i);
            }
        }

        public override T[] Value
        {
            get
            {
                for (int i = 0; i < mEntries.Length; i++)
                {
                    var val = mEntries[i].ObjectValue;
                    var cast = mCasts[i];
                    mResult[i] = (cast == null ? (T)val : (T)cast.DynamicInvoke(val));
                }
                return mResult;
            }
        }

        public override string ShortName
        {
            get
            {
                return "ArrayBuilder";
            }
        }
    }


    public class CallMethodExpr<T> : Expr<T>
    {
        Func<T> mValueDelegate;
        private IHasValue mBaseObject;
        private object mBaseValueObject;
        private MemberInfo mMethod;
        private IHasValue[] mPreparedParams;

        private object[] mParamValues;
        private System.Type mResultSystemType;
        private string mShortName;

        public CallMethodExpr(IHasValue baseObject, MemberInfo method, List<IHasValue> @params, object[] casts, string shortName)
        {
            if (baseObject != null) baseObject.Subscribe(this, "baseObject");
            base.Subscribe("parameter", @params);

            if (@params == null)
                @params = new List<IHasValue>();

            var preparedParams = new List<IHasValue>();
            mBaseObject = baseObject;
            mMethod = method;
            mShortName = shortName;

            ParameterInfo[] paramInfo = null;
            if (method is PropertyInfo)
            {
                mResultSystemType = ((PropertyInfo)method).PropertyType;
                paramInfo = ((PropertyInfo)method).GetIndexParameters();
            }
            else if (method is MethodInfo)
            {
                var mi = (MethodInfo)method;
                mResultSystemType = mi.ReturnType;
                paramInfo = mi.GetParameters();
            }
            else if (method is FieldInfo)
            {
                mResultSystemType = ((FieldInfo)method).FieldType;
                paramInfo = new ParameterInfo[] { };
            }

            for (int i = 0; i < @params.Count; i++)
            {
                if (i < paramInfo.Length)
                {
                    var sourceType = @params[i].ValueType;
                    var targetType = paramInfo[i].ParameterType;
                    if (casts[i] == null)
                    {
                        preparedParams.Add(@params[i]);
                    }
                    else
                    {
                        if (casts[i].GetType().IsArray)
                        {
                            var c2 = typeof(ArrayBuilder<>).MakeGenericType(targetType.GetElementType());
                            preparedParams.Add((IHasValue)Activator.CreateInstance(c2, new object[] { @params.Skip(i).ToArray(), casts[i] }));
                            // we have consumed all parameters
                            break;
                        }
                        else
                        {
                            var c3 = typeof(DelegateExpr<,>).MakeGenericType(sourceType, targetType);
                            preparedParams.Add((IHasValue)Activator.CreateInstance(c3, @params[i], casts[i], @params[i].ShortName));
                        }
                    }
                }
            }

            mPreparedParams = preparedParams.ToArray();
            mParamValues = new object[mPreparedParams.Length];

            if (method is PropertyInfo)
            {
                mValueDelegate = EmitGetMethod(baseObject, ((PropertyInfo)method).GetGetMethod());
            }
            else if (method is MethodInfo)
            {
                mValueDelegate = EmitGetMethod(baseObject, (MethodInfo)method);
            }
            else if (method is FieldInfo)
            {
                EmitGetField(baseObject, method);
            }
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

        private void EmitGetField(IHasValue baseObject, MemberInfo method)
        {
            Expression expr = null;
            if (baseObject != null)
            {
                var expectedType = typeof(IHasValue<>).MakeGenericType(baseObject.ValueType);
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

        private Func<T> EmitGetMethod(IHasValue baseObject, MethodInfo mi)
        {
            var paramTypes = (from p in mPreparedParams select p.ValueType).ToArray();
            DynamicMethod meth = new DynamicMethod(
                "DynamicGetMethod",
                typeof(T),
                new Type[] { GetType() },
                GetType(),  // associate with a type
                true);
            ILGenerator il = meth.GetILGenerator();
            FieldInfo fiPreparedParams = GetType().GetField("mPreparedParams", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo miGetObjectValue = typeof(IHasValue).GetProperty("ObjectValue").GetGetMethod();
            if (!mi.IsStatic)
            {
                FieldInfo fiBaseObject = GetType().GetField("mBaseObject", BindingFlags.Instance | BindingFlags.NonPublic);
                il.Emit(OpCodes.Ldarg_0);         // this
                il.Emit(OpCodes.Ldfld, fiBaseObject); // this.mParams
                il.Emit(OpCodes.Callvirt, miGetObjectValue);
            }
            var methodParams = mi.GetParameters();

            for (int i = 0; i < mPreparedParams.Length; i++)
            {


                il.Emit(OpCodes.Ldarg_0);         // this
                il.Emit(OpCodes.Ldfld, fiPreparedParams); // this.mParams
                il.Emit(OpCodes.Ldc_I4, i);       // i
                il.Emit(OpCodes.Ldelem_Ref);      // this.mParams[i]
                il.Emit(OpCodes.Callvirt, miGetObjectValue);
                var preparedType = mPreparedParams[i].ValueType;
                if (preparedType == typeof(string))
                {
                    throw new NotImplementedException("check this");
                }
                if (preparedType.IsValueType && methodParams[i].ParameterType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, preparedType);
                }
            }
            il.Emit(OpCodes.Call, mi);
            il.Emit(OpCodes.Ret);

            Delegate dlg = meth.CreateDelegate(typeof(Func<T>), this);
            return (Func<T>)dlg;
        }

        private void Recalc()
        {
            for (int i = 0; i <= mPreparedParams.Length - 1; i++)
            {
                mParamValues[i] = mPreparedParams[i].ObjectValue;
            }
            mBaseValueObject = mBaseObject.ObjectValue;
        }

        public override T Value
        {
            //[System.Diagnostics.DebuggerStepThrough()]
            get { return mValueDelegate(); }
        }

        public override string ShortName
        {
            get { return mShortName; }
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
            : base()
        {
            array.Subscribe(this, "Array");
            base.Subscribe("index", @params);
            IHasValue[] newParams = @params.ToArray();
            int[] newValues = new int[@params.Count];

            mArray = array;
            mParams = newParams;
            mValues = newValues;
            mResultSystemType = array.ValueType.GetElementType();
        }

        public override T Value
        {
            get
            {
                object res = null;
                Array arr = (Array)mArray.ObjectValue;

                for (int i = 0; i <= mValues.Length - 1; i++)
                {
                    mValues[i] = Convert.ToInt32(mParams[i].ObjectValue);
                }

                res = arr.GetValue(mValues);
                return (T)res;
            }
        }

        public Type SystemType
        {
            get { return typeof(T); }
        }

        private void mBaseVariable_ValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            //if (ValueChanged != null) ValueChanged(sender, e);
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
        {
            this.ifExpr = ifExpr;
            if (ifExpr != null) 
                ifExpr.Subscribe(this, "if");
            this.thenExpr = thenExpr;
            if (thenExpr != null) 
                thenExpr.Subscribe(this, "then");
            this.elseExpr = elseExpr;
            if (elseExpr != null) 
                elseExpr.Subscribe(this, "else");
            mSystemType = thenExpr.ValueType;
        }

        public override T Value
        {
            get
            {
                object result;
                var test = Convert.ToBoolean(ifExpr.ObjectValue);
                result = test ? thenExpr.ObjectValue : elseExpr.ObjectValue;

                if (result != null && result.GetType() != mSystemType) result = System.Convert.ChangeType(result, mSystemType);
                return (T)result;
            }
        }

        public override string ShortName
        {
            get { return "OperatorIf"; }
        }

    }

    internal class DelegateExpr<P1, T> : Expr<T>
    {
        private IHasValue<P1> mP1;
        private Func<P1, T> mDelegate;
        private string mShortName;

        public DelegateExpr(IHasValue<P1> p1, Func<P1, T> dlg, string shortName)
        {
            mP1 = p1;
            if (p1 != null) p1.Subscribe(this, "p1");
            System.Diagnostics.Debug.Assert(dlg != null);
            mDelegate = dlg;
            mShortName = shortName;
        }


        public override T Value
        {
            //[System.Diagnostics.DebuggerStepThrough()]
            get { return mDelegate(mP1.Value); }
        }

        //public event ValueChangedEventHandler ValueChanged;

        public override string ShortName
        {
            get { return mShortName; }
        }

    }

    internal class DelegateExpr<P1, P2, T> : Expr<T>
    {
        private IHasValue<P1> mP1;
        private IHasValue<P2> mP2;
        private Func<P1, P2, T> mDelegate;
        private string mShortName;

        public DelegateExpr(IHasValue<P1> p1, IHasValue<P2> p2, Func<P1, P2, T> dlg, string shortName)
        {
            mP1 = p1;
            if (p1 != null) p1.Subscribe(this, "p1");
            mP2 = p2;
            if (p2 != null) p2.Subscribe(this, "p2");
            mDelegate = dlg;
            mShortName = shortName;
        }

        public override T Value
        {
            //[System.Diagnostics.DebuggerStepThrough()]
            get { return mDelegate(mP1.Value, mP2.Value); }
        }

        public override string ShortName
        {
            get { return mShortName; }
        }
    }

    class GetVariableFromBag<T> : Expr<T>
    {
        private string mVariableName;
        private IEvaluator mEvaluator;
        private Variable<T> mVariable;

        public GetVariableFromBag(IEvaluator evaluator, string variableName)
        {
            mEvaluator = evaluator;
            mVariableName = variableName;
            mVariable = mEvaluator.GetVariable<T>(mVariableName);
            if (mVariable != null) mVariable.Subscribe(this, "Variable " + variableName);
        }

        public override T Value
        {
            //[System.Diagnostics.DebuggerStepThrough()]
            get { return mVariable.Value; }
        }

        public override string ShortName
        {
            get { return "GetVar " + mVariableName; }
        }
    }

    public class Variable<T> : Expr<T>, IVariable
    {
        private T mValue;
        private string mVariableName;

        public Variable(T variableValue, string variableName)
        {
            mValue = variableValue;
            mVariableName = variableName;
        }

        public override T Value
        {
            //[System.Diagnostics.DebuggerStepThrough()]
            get { return mValue; }
        }

        public override string ShortName
        {
            get { return mVariableName; }
        }

        public void SetValue(object value)
        {
            mValue = (T)value;
            RaiseValueChanged();
        }
    }
}
