using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Eval4.Core
{
    public abstract class DelegatedExpr : IHasValue
    {

        protected ValueDelegate mValueDelegate;
        protected delegate object ValueDelegate();

        public delegate void RunDelegate();


        protected DelegatedExpr()
        {
        }

        protected void RaiseEventValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(sender, e);
            }
        }

        public abstract Type SystemType { get; }

        public static bool IsDoubleOrSmaller(Type v1Type)
        {
            return v1Type == typeof(sbyte) || v1Type == typeof(Int16) || v1Type == typeof(Int32) || v1Type == typeof(Int64)
                || v1Type == typeof(byte) || v1Type == typeof(UInt16) || v1Type == typeof(UInt32) || v1Type == typeof(UInt64)
                || v1Type == typeof(Single) || v1Type == typeof(double) || v1Type == typeof(Decimal);
        }

        public static bool IsIntOrSmaller(Type v1Type)
        {
            return v1Type == typeof(sbyte) || v1Type == typeof(Int16) || v1Type == typeof(Int32)
                || v1Type == typeof(byte) || v1Type == typeof(UInt16);
        }

        public virtual object ObjectValue
        {
            get { return mValueDelegate(); }
        }

        System.Type IHasValue.SystemType
        {
            get { return SystemType; }
        }

        protected static bool ConvertToSystemType(ref IHasValue param1, Type SystemType)
        {
            if (param1.SystemType != SystemType)
            {
                if (SystemType == typeof(object))
                {
                    //ignore
                }
                else
                {
                    param1 = new SystemTypeConvertExpr(param1, SystemType);
                    return true;
                }
            }
            return false;
        }

        internal static double ToDouble(object o)
        {
            if (o == null) return 0.0;
            if (o is double) return (double)o;

            return System.Convert.ToDouble(o);
        }

        internal static string ToString(object o)
        {
            if (o == null) return string.Empty;

            return System.Convert.ToString(o);
        }

        public event ValueChangedEventHandler ValueChanged;

        public abstract IEnumerable<Dependency> Dependencies { get; }


        public abstract string ShortName { get; }
    }

    internal class ImmediateExpr<T> : IHasValue<T>
    {
        private T mValue;

        public ImmediateExpr(T value)
        {
            mValue = value;
        }

        public object ObjectValue
        {
            get { return mValue; }
        }

        public event ValueChangedEventHandler ValueChanged;

        public Type SystemType
        {
            get { return typeof(T); }
        }

        public string ShortName
        {
            get { return "Literal"; }
        }

        public IEnumerable<Dependency> Dependencies
        {
            get { return Dependency.None; }
        }

        public T Value
        {
            get { return mValue; }
        }
    }

    internal class SystemTypeConvertExpr : DelegatedExpr
    {
        private IHasValue withEventsField_mParam1;
        public IHasValue mParam1
        {
            get { return withEventsField_mParam1; }
            set
            {
                if (withEventsField_mParam1 != null)
                {
                    withEventsField_mParam1.ValueChanged -= mParam1_ValueChanged;
                }
                withEventsField_mParam1 = value;
                if (withEventsField_mParam1 != null)
                {
                    withEventsField_mParam1.ValueChanged += mParam1_ValueChanged;
                }
            }
        }
        private Type mSystemType = typeof(object);

        public SystemTypeConvertExpr(IHasValue param1, System.Type Type)
        {
            mParam1 = param1;
            mValueDelegate = CType;
            mSystemType = Type;
        }

        private object CType()
        {
            return System.Convert.ChangeType(mParam1.ObjectValue, mSystemType);
        }

        public override Type SystemType
        {
            get { return mSystemType; }
        }

        private void mParam1_ValueChanged(object sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(sender, e);
        }


        public override IEnumerable<Dependency> Dependencies
        {
            get { throw new NotImplementedException(); }
        }

        public override string ShortName
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class CallMethodExpr<T> : IHasValue<T>
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

        public CallMethodExpr(object baseObject, MemberInfo method, List<IHasValue> @params)
        {
            if (@params == null)
                @params = new List<IHasValue>();
            IHasValue[] newParams = @params.ToArray();
            object[] newParamValues = new object[@params.Count];

            foreach (IHasValue p in newParams)
            {
                p.ValueChanged += p_ValueChanged;
            }
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
                mValueDelegate = GetField;

                //object res = ((FieldInfo)mMethod).GetValue(mBaseValueObject);
                //var f = Expression.Field(Expression.Constant(baseObject), (FieldInfo)method);


                //     System.Linq.Expressions.Expression<Func<int, bool>> expr = i => i < 5;
                //     // Compile the expression tree into executable code.
                //     Func<int, bool> deleg = expr.Compile();
                //     // Invoke the method and print the output.
                //     Console.WriteLine("deleg(4) = {0}", deleg(4));
                //var expr = (Expression<Func<T>>)f;
                //mValueDelegate = expr.Compile();
            }

            for (int i = 0; i <= mParams.Length - 1; i++)
            {
                if (i < paramInfo.Length)
                {
                    mParams[i] = TypedExpressions.ChangeType(mParams[i], paramInfo[i].ParameterType);
                }
            }

            //if (typeof(IHasValue).IsAssignableFrom(mResultSystemType))
            //{
            //    mResultValue = (IHasValue)InternalValue();
            //    if (mResultValue is IHasValue)
            //    {
            //        mResultSystemType = ((IHasValue)mResultValue).SystemType;
            //    }
            //    else if (mResultValue == null)
            //    {
            //        mResultSystemType = typeof(Object);
            //    }
            //    else
            //    {
            //        object v = mResultValue.ObjectValue;
            //        if (v == null)
            //        {
            //            mResultSystemType = typeof(Object);
            //        }
            //        else
            //        {
            //            mResultSystemType = v.GetType();
            //        }
            //    }
            //}
            //else
            //{
            //    mResultSystemType = SystemType;
            //}
        }

        void p_ValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public IHasValue mBaseValue
        {
            get { return withEventsField_mBaseValue; }
            set
            {
                if (withEventsField_mBaseValue != null)
                {
                    withEventsField_mBaseValue.ValueChanged -= withEventsField_mBaseValue_ValueChanged;
                }
                withEventsField_mBaseValue = value;
                if (withEventsField_mBaseValue != null)
                {
                    withEventsField_mBaseValue.ValueChanged += withEventsField_mBaseValue_ValueChanged;
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
                    withEventsField_mResultValue.ValueChanged -= withEventsField_mResultValue_ValueChanged;
                }
                withEventsField_mResultValue = value;
                if (withEventsField_mResultValue != null)
                {
                    withEventsField_mResultValue.ValueChanged += withEventsField_mResultValue_ValueChanged;
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

        private T GetField()
        {
            Recalc();
            object res = ((FieldInfo)mMethod).GetValue(mBaseValueObject);
            return (T)res;
        }

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
            //    return base.mValueDelegate();
        }

        //public override object ObjectValue
        //{
        //    get
        //    {
        //        object res = InternalValue();
        //        if (res is IHasValue)
        //        {
        //            mResultValue = (IHasValue)res;
        //            res = mResultValue.ObjectValue;
        //        }
        //        return res;
        //    }
        //}

        //public override System.Type SystemType
        //{
        //    get { return mResultSystemType; }
        //}

        //private void mParamsValueChanged(object sender, System.EventArgs e)
        //{
        //    base.RaiseEventValueChanged(sender, e);
        //}

        //private void mBaseVariable_ValueChanged(object sender, System.EventArgs e)
        //{
        //    base.RaiseEventValueChanged(sender, e);
        //}

        //private void mResultVariable_ValueChanged(object sender, System.EventArgs e)
        //{
        //    base.RaiseEventValueChanged(sender, e);
        //}

        //public override IEnumerable<Dependency> Dependencies
        //{
        //    get
        //    {

        //    }
        //}

        //public override string ShortName
        //{
        //    get { return "CallMethod"; }
        //}

        //public T Value
        //{
        //    get { throw new NotImplementedException(); }
        //}


        public event ValueChangedEventHandler ValueChanged;

        public T Value
        {
            get { return mValueDelegate(); }
        }

        public object ObjectValue
        {
            get { return this.mValueDelegate(); }
        }

        public Type SystemType
        {
            get { return typeof(T); }
        }

        public string ShortName
        {
            get { return "CallMethod"; }
        }

        public IEnumerable<Dependency> Dependencies
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

    public class GetArrayEntryExpr : DelegatedExpr
    {

        private IHasValue withEventsField_mArray;
        public IHasValue mArray
        {
            get { return withEventsField_mArray; }
            set
            {
                if (withEventsField_mArray != null)
                {
                    withEventsField_mArray.ValueChanged -= mBaseVariable_ValueChanged;
                }
                withEventsField_mArray = value;
                if (withEventsField_mArray != null)
                {
                    withEventsField_mArray.ValueChanged += mBaseVariable_ValueChanged;
                }
            }

        }
        private IHasValue[] mParams;
        private int[] mValues;
        private Type mResultSystemType;

        public GetArrayEntryExpr(IHasValue array, List<IHasValue> @params)
        {
            IHasValue[] newParams = @params.ToArray();
            int[] newValues = new int[@params.Count];

            mArray = array;
            mParams = newParams;
            mValues = newValues;
            mResultSystemType = array.SystemType.GetElementType();
        }

        public override object ObjectValue
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
                return res;
            }
        }

        public override System.Type SystemType
        {
            get { return mResultSystemType; }
        }

        private void mBaseVariable_ValueChanged(object sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(sender, e);
        }


        public override IEnumerable<Dependency> Dependencies
        {
            get { throw new NotImplementedException(); }
        }

        public override string ShortName
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class OperatorIfExpr : DelegatedExpr
    {
        private Type mSystemType;
        private IHasValue ifExpr;
        private IHasValue thenExpr;
        private IHasValue elseExpr;

        public OperatorIfExpr(IHasValue ifExpr, IHasValue thenExpr, IHasValue elseExpr)
        {
            this.ifExpr = ifExpr;
            this.thenExpr = thenExpr;
            this.elseExpr = elseExpr;
            mSystemType = thenExpr.SystemType;
        }

        public override object ObjectValue
        {
            get
            {
                object result;
                var test = System.Convert.ToBoolean(ifExpr.ObjectValue);
                result = test ? thenExpr.ObjectValue : elseExpr.ObjectValue;

                if (result != null && result.GetType() != mSystemType) result = System.Convert.ChangeType(result, mSystemType);
                return result;
            }
        }
        public override Type SystemType
        {
            get { return mSystemType; }
        }



        public override IEnumerable<Dependency> Dependencies
        {
            get { throw new NotImplementedException(); }
        }

        public override string ShortName
        {
            get { throw new NotImplementedException(); }
        }
    }
}
