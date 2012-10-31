using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eval4.Core
{
    public interface IExpr : IHasValue
    {
    }

    public interface IExpr<T> : IExpr, IHasValue<T>
    {
    }

    public abstract class Expr : IExpr
    {

        protected ValueDelegate mValueDelegate;
        protected delegate object ValueDelegate();

        public delegate void RunDelegate();


        protected Expr()
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
        
        public virtual string Description
        {
            get { return "Expr " + this.GetType().Name; }
        }

        public virtual string Name
        {
            get { return "Expr " + this.GetType().Name; }
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
    }

    internal class ImmediateExpr<T> : Expr
    {

        private T mValue;

        public ImmediateExpr(T value)
        {
            mValue = value;
        }

        public override object ObjectValue
        {
            get { return mValue; }
        }

        public override Type SystemType
        {
            get { return typeof(T); }
        }
    }

    internal class SystemTypeConvertExpr : Expr
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

    }

    public class GetVariableExpr : Expr
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
        public GetVariableExpr(IHasValue value)
        {
            mParam1 = value;
        }


        public override object ObjectValue
        {
            get { return mParam1.ObjectValue; }
        }

        public override System.Type SystemType
        {
            get { return mParam1.SystemType; }
        }

        private void mParam1_ValueChanged(object sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(sender, e);
        }

    }

    public class CallMethodExpr : Expr
    {

        private object mBaseObject;
        private IHasValue withEventsField_mBaseValue;
        public IHasValue mBaseValue
        {
            get { return withEventsField_mBaseValue; }
            set
            {
                if (withEventsField_mBaseValue != null)
                {
                    withEventsField_mBaseValue.ValueChanged -= mBaseVariable_ValueChanged;
                }
                withEventsField_mBaseValue = value;
                if (withEventsField_mBaseValue != null)
                {
                    withEventsField_mBaseValue.ValueChanged += mBaseVariable_ValueChanged;
                }
            }
            // for the events only
        }

        private object mBaseValueObject;
        private MemberInfo mMethod;
        private IHasValue[] mParams;

        private object[] mParamValues;
        private System.Type mResultSystemType;
        private IHasValue withEventsField_mResultValue;
        public IHasValue mResultValue
        {
            get { return withEventsField_mResultValue; }
            set
            {
                if (withEventsField_mResultValue != null)
                {
                    withEventsField_mResultValue.ValueChanged -= mResultVariable_ValueChanged;
                }
                withEventsField_mResultValue = value;
                if (withEventsField_mResultValue != null)
                {
                    withEventsField_mResultValue.ValueChanged += mResultVariable_ValueChanged;
                }
            }
            // just for some
        }

        internal CallMethodExpr(object baseObject, MemberInfo method, List<IExpr> @params)
        {
            if (@params == null)
                @params = new List<IExpr>();
            IHasValue[] newParams = @params.ToArray();
            object[] newParamValues = new object[@params.Count];

            foreach (IHasValue p in newParams)
            {
                p.ValueChanged += mParamsValueChanged;
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
            }
            else if (method is FieldInfo)
            {
                mResultSystemType = ((FieldInfo)method).FieldType;
                paramInfo = new ParameterInfo[] { };
                mValueDelegate = GetField;
            }

            for (int i = 0; i <= mParams.Length - 1; i++)
            {
                if (i < paramInfo.Length)
                {
                    var value = mParams[i];
                    if (ConvertToSystemType(ref value, paramInfo[i].ParameterType))
                    {
                        mParams[i] = value;
                    }
                }
            }

            if (typeof(IHasValue).IsAssignableFrom(mResultSystemType))
            {
                mResultValue = (IHasValue)InternalValue();
                if (mResultValue is IHasValue)
                {
                    mResultSystemType = ((IHasValue)mResultValue).SystemType;
                }
                else if (mResultValue == null)
                {
                    mResultSystemType = typeof(Object);
                }
                else
                {
                    object v = mResultValue.ObjectValue;
                    if (v == null)
                    {
                        mResultSystemType = typeof(Object);
                    }
                    else
                    {
                        mResultSystemType = v.GetType();
                    }
                }
            }
            else
            {
                mResultSystemType = SystemType;
            }
        }

        protected static internal IExpr GetNew(Parser parser, object baseObject, MemberInfo method, List<IExpr> @params)
        {
            IExpr o = null;
            o = new CallMethodExpr(baseObject, method, @params);
            return o;
        }

        private object GetProperty()
        {
            object res = ((PropertyInfo)mMethod).GetValue(mBaseValueObject, mParamValues);
            return res;
        }

        private object GetMethod()
        {
            object res = ((MethodInfo)mMethod).Invoke(mBaseValueObject, mParamValues);
            return res;
        }

        private object GetField()
        {
            object res = ((FieldInfo)mMethod).GetValue(mBaseValueObject);
            return res;
        }

        private object InternalValue()
        {
            for (int i = 0; i <= mParams.Length - 1; i++)
            {
                mParamValues[i] = System.Convert.ChangeType(mParams[i].ObjectValue, mParams[i].SystemType);
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
            return base.mValueDelegate();
        }

        public override object ObjectValue
        {
            get
            {
                object res = InternalValue();
                if (res is IHasValue)
                {
                    mResultValue = (IHasValue)res;
                    res = mResultValue.ObjectValue;
                }
                return res;
            }
        }

        public override System.Type SystemType
        {
            get { return mResultSystemType; }
        }

        private void mParamsValueChanged(object sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(sender, e);
        }

        private void mBaseVariable_ValueChanged(object sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(sender, e);
        }

        private void mResultVariable_ValueChanged(object sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(sender, e);
        }
    }

    public class GetArrayEntryExpr : Expr
    {

        private IExpr withEventsField_mArray;
        public IExpr mArray
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

        public GetArrayEntryExpr(IExpr array, List<IExpr> @params)
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

    }

    public class OperatorIfExpr : Expr
    {
        private Type mSystemType;
        private IExpr ifExpr;
        private IExpr thenExpr;
        private IExpr elseExpr;

        public OperatorIfExpr(IExpr ifExpr, IExpr thenExpr, IExpr elseExpr)
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


    }
}
