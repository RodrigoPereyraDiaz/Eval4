using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eval4.Core
{
    public abstract class Expr : IEvalValue
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

        private static bool IsNumeric(Type v1Type)
        {
            return v1Type == typeof(sbyte) || v1Type == typeof(Int16) || v1Type == typeof(Int32) || v1Type == typeof(Int64)
                || v1Type == typeof(byte) || v1Type == typeof(UInt16) || v1Type == typeof(UInt32) || v1Type == typeof(UInt64)
                || v1Type == typeof(Single) || v1Type == typeof(double) || v1Type == typeof(Decimal);
        }

        public bool CanReturn(Type type)
        {
            if (type == SystemType) return true;
            else if (IsNumeric(type)) return IsNumeric(SystemType);
            else if (type == typeof(string)) return true;
            else return false;
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

        System.Type IEvalValue.SystemType
        {
            get { return SystemType; }
        }

        protected internal Expr Convert(Parser parser, Expr param1, Type SystemType)
        {
            if (param1.SystemType != SystemType)
            {
                if (param1.CanReturn(SystemType))
                {
                    param1 = new ConvertExpr(parser, param1, SystemType);
                }
                else
                {
                    throw parser.NewParserException("Cannot convert " + param1.Name + " into " + SystemType);
                }
            }
            return param1;
        }

        protected static bool ConvertToSystemType(ref IEvalValue param1, Type SystemType)
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

        protected void SwapParams(ref Expr Param1, ref Expr Param2)
        {
            Expr swp = Param1;
            Param1 = Param2;
            Param2 = swp;
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

    //internal class VariableExpr : Expr
    //{

    //    private Variable withEventsField_mVariable;
    //    public Variable mVariable
    //    {
    //        get { return withEventsField_mVariable; }
    //        set
    //        {
    //            if (withEventsField_mVariable != null)
    //            {
    //                withEventsField_mVariable.ValueChanged -= mVariable_ValueChanged;
    //            }
    //            withEventsField_mVariable = value;
    //            if (withEventsField_mVariable != null)
    //            {
    //                withEventsField_mVariable.ValueChanged += mVariable_ValueChanged;
    //            }
    //        }

    //    }
    //    public VariableExpr(Variable variable)
    //    {
    //        mVariable = variable;
    //    }

    //    public override object ObjectValue
    //    {
    //        get { return mVariable; }
    //    }

    //    public override Type SystemType
    //    {
    //        get { return mVariable.SystemType; }
    //    }

    //    private void mVariable_ValueChanged(object Sender, System.EventArgs e)
    //    {
    //        base.RaiseEventValueChanged(Sender, e);
    //    }
    //}

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

    internal class UnaryExpr : Expr
    {

        private Expr withEventsField_mParam1;
        public Expr mParam1
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

        private Type mSystemType;

        public UnaryExpr(TokenType tt, Expr param1)
        {
            mParam1 = param1;
            var v1Type = mParam1.SystemType;

            switch (tt)
            {
                case TokenType.operator_not:
                    if (v1Type == typeof(bool))
                    {
                        mValueDelegate = BOOLEAN_NOT;
                        mSystemType = typeof(Boolean);
                    }
                    break;
                case TokenType.operator_minus:
                    if (mParam1.CanReturn(typeof(double)))
                    {
                        mValueDelegate = NUM_CHGSIGN;
                        mSystemType = typeof(double);
                    }
                    break;
            }
        }

        private object BOOLEAN_NOT()
        {
            return !(bool)mParam1.ObjectValue;
        }

        private object NUM_CHGSIGN()
        {
            return -ToDouble(mParam1.ObjectValue);
        }

        public override Type SystemType
        {
            get { return mSystemType; }
        }

        private void mParam1_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }
    }

    internal class ConvertExpr : Expr
    {
        private IEvalValue withEventsField_mParam1;
        public IEvalValue mParam1
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
        public ConvertExpr(Parser parser, IEvalValue param1, Type systemType)
        {
            mParam1 = param1;

            if (systemType == typeof(Boolean))
            {
                mValueDelegate = TBool;
                mSystemType = typeof(Boolean);
            }
            else if (systemType == typeof(DateTime))
            {
                mValueDelegate = TDate;
                mSystemType = typeof(DateTime);
            }
            else if (systemType == typeof(double))
            {
                mValueDelegate = TNum;
                mSystemType = typeof(double);
            }
            else if (systemType == typeof(int))
            {
                mValueDelegate = TInt;
                mSystemType = typeof(int);
            }
            else if (systemType == typeof(String))
            {
                mValueDelegate = TStr;
                mSystemType = typeof(String);
            }
            else
            {
                throw parser.NewParserException("Cannot convert " + param1.SystemType.Name + " to " + SystemType);
            }

        }

        private object TBool()
        {
            return System.Convert.ToBoolean(mParam1.ObjectValue);
        }

        private object TDate()
        {
            return System.Convert.ToDateTime(mParam1.ObjectValue);
        }

        private object TNum()
        {
            return System.Convert.ToDouble(mParam1.ObjectValue);
        }

        private object TInt()
        {
            return System.Convert.ToInt32(mParam1.ObjectValue);
        }

        private object TStr()
        {
            return System.Convert.ToString(mParam1.ObjectValue);
        }

        public override Type SystemType
        {
            get { return mSystemType; }
        }

        private void mParam1_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }
    }

    internal class SystemTypeConvertExpr : Expr
    {
        private IEvalValue withEventsField_mParam1;
        public IEvalValue mParam1
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

        public SystemTypeConvertExpr(IEvalValue param1, System.Type Type)
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

        private void mParam1_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }

    }

    internal class BinaryExpr : Expr
    {

        private Expr withEventsField_mParam1;
        public Expr mParam1
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
        private Expr withEventsField_mParam2;
        public Expr mParam2
        {
            get { return withEventsField_mParam2; }
            set
            {
                if (withEventsField_mParam2 != null)
                {
                    withEventsField_mParam2.ValueChanged -= mParam2_ValueChanged;
                }
                withEventsField_mParam2 = value;
                if (withEventsField_mParam2 != null)
                {
                    withEventsField_mParam2.ValueChanged += mParam2_ValueChanged;
                }
            }
        }

        private Type mSystemType;
        public BinaryExpr(Parser parser, Expr param1, TokenType tt, Expr param2)
        {
            mParam1 = param1;
            mParam2 = param2;

            Type v1Type = mParam1.SystemType;
            Type v2Type = mParam2.SystemType;

            switch (tt)
            {
                case TokenType.operator_plus:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double)))
                    {
                        mParam1 = Convert(parser, mParam1, typeof(double));
                        mParam2 = Convert(parser, mParam2, typeof(double));
                        mValueDelegate = NUM_PLUS_NUM;
                        mSystemType = typeof(double);
                    }
                    else if (mParam1.CanReturn(typeof(double)) && v2Type == typeof(DateTime))
                    {
                        mParam1 = param2;
                        mParam2 = Convert(parser, param1, typeof(double));
                        mValueDelegate = DATE_PLUS_NUM;
                        mSystemType = typeof(DateTime);
                    }
                    else if (v1Type == typeof(DateTime) && mParam2.CanReturn(typeof(double)))
                    {
                        mParam2 = Convert(parser, mParam2, typeof(double));
                        mValueDelegate = DATE_PLUS_NUM;
                        mSystemType = typeof(DateTime);
                    }
                    else if (mParam1.CanReturn(typeof(String)) && mParam2.CanReturn(typeof(String)))
                    {
                        param1 = Convert(parser, param1, typeof(String));
                        mValueDelegate = STR_CONCAT_STR;
                        mSystemType = typeof(String);
                    }
                    break;
                case TokenType.operator_minus:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double)))
                    {
                        mParam1 = Convert(parser, mParam1, typeof(double));
                        mParam2 = Convert(parser, mParam2, typeof(double));
                        mValueDelegate = NUM_MINUS_NUM;
                        mSystemType = typeof(double);
                    }
                    else if (v1Type == typeof(DateTime) && mParam2.CanReturn(typeof(double)))
                    {
                        mParam2 = Convert(parser, mParam2, typeof(double));
                        mValueDelegate = DATE_MINUS_NUM;
                        mSystemType = typeof(DateTime);
                    }
                    else if (v1Type == typeof(DateTime) && v2Type == typeof(DateTime))
                    {
                        mValueDelegate = DATE_MINUS_DATE;
                        mSystemType = typeof(double);
                    }
                    break;
                case TokenType.operator_mul:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double)))
                    {
                        mParam1 = Convert(parser, mParam1, typeof(double));
                        mParam2 = Convert(parser, mParam2, typeof(double));
                        mValueDelegate = NUM_MUL_NUM;
                        mSystemType = typeof(double);
                    }
                    break;
                case TokenType.operator_div:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double)))
                    {
                        mParam1 = Convert(parser, mParam1, typeof(double));
                        mParam2 = Convert(parser, mParam2, typeof(double));
                        mValueDelegate = NUM_DIV_NUM;
                        mSystemType = typeof(double);
                    }
                    break;
                case TokenType.operator_percent:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double)))
                    {
                        mParam1 = Convert(parser, mParam1, typeof(double));
                        mParam2 = Convert(parser, mParam2, typeof(double));
                        mValueDelegate = NUM_PERCENT_NUM;
                        mSystemType = typeof(double);
                    }
                    break;
                case TokenType.operator_mod:
                    mParam1 = Convert(parser, mParam1, typeof(Int32));
                    mParam2 = Convert(parser, mParam2, typeof(Int32));
                    mValueDelegate = INT_MOD_INT;
                    mSystemType = typeof(Int32);
                    break;
                case TokenType.operator_and:
                case TokenType.operator_or:
                case TokenType.operator_xor:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool))
                    {
                        switch (tt)
                        {
                            case TokenType.operator_or:
                                mValueDelegate = BOOL_OR_BOOL;
                                mSystemType = typeof(bool);
                                break;
                            case TokenType.operator_and:
                                mValueDelegate = BOOL_AND_BOOL;
                                mSystemType = typeof(bool);
                                break;
                            case TokenType.operator_xor:
                                mValueDelegate = BOOL_XOR_BOOL;
                                mSystemType = typeof(bool);
                                break;
                        }
                    }
                    else
                    {
                        mParam1 = Convert(parser, mParam1, typeof(Int32));
                        mParam2 = Convert(parser, mParam2, typeof(Int32));
                        switch (tt)
                        {
                            case TokenType.operator_or:
                                mValueDelegate = INT_OR_INT;
                                mSystemType = typeof(Int32);
                                break;
                            case TokenType.operator_and:
                                mValueDelegate = INT_AND_INT;
                                mSystemType = typeof(Int32);
                                break;
                            case TokenType.operator_xor:
                                mValueDelegate = INT_XOR_INT;
                                mSystemType = typeof(Int32);
                                break;
                        }
                    }
                    break;
                case TokenType.operator_andalso:
                case TokenType.operator_orelse:
                    mParam1 = Convert(parser, mParam1, typeof(Boolean));
                    mParam2 = Convert(parser, mParam2, typeof(Boolean));
                    switch (tt)
                    {
                        case TokenType.operator_orelse:
                            mValueDelegate = BOOL_ORELSE_BOOL;
                            mSystemType = typeof(Boolean);
                            break;
                        case TokenType.operator_andalso:
                            mValueDelegate = BOOL_ANDALSO_BOOL;
                            mSystemType = typeof(Boolean);
                            break;
                    }
                    break;
                
                case TokenType.operator_eq:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double))) mValueDelegate = NUM_EQ_NUM;
                    else mValueDelegate = STR_EQ_STR;
                    mSystemType = typeof(Boolean);
                    break;
                case TokenType.operator_ge:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double))) mValueDelegate = NUM_GE_NUM;
                    else mValueDelegate = STR_GE_STR;
                    mSystemType = typeof(Boolean);
                    break;
                case TokenType.operator_gt:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double))) mValueDelegate = NUM_GT_NUM;
                    else mValueDelegate = STR_GT_STR;
                    mSystemType = typeof(Boolean);
                    break;
                case TokenType.operator_le:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double))) mValueDelegate = NUM_LE_NUM;
                    else mValueDelegate = STR_LE_STR;
                    mSystemType = typeof(Boolean);
                    break;
                case TokenType.operator_lt:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double))) mValueDelegate = NUM_LT_NUM;
                    else mValueDelegate = STR_LT_STR;
                    mSystemType = typeof(Boolean);
                    break;
                case TokenType.operator_ne:
                    if (mParam1.CanReturn(typeof(double)) && mParam2.CanReturn(typeof(double))) mValueDelegate = NUM_NE_NUM;
                    else mValueDelegate = STR_NE_STR;
                    mSystemType = typeof(Boolean);
                    break;

            }

            if (mValueDelegate == null)
            {
                throw parser.NewParserException("Cannot apply the operator " + tt.ToString().Replace("operator_", "") + " on " + v1Type.ToString() + " and " + v2Type.ToString());
            }
        }

        private object INT_AND_INT()
        {
            return (int)mParam1.ObjectValue & (int)mParam2.ObjectValue;
        }

        private object BOOL_AND_BOOL()
        {
            return (bool)mParam1.ObjectValue & (bool)mParam2.ObjectValue;
        }

        private object BOOL_ANDALSO_BOOL()
        {
            return (bool)mParam1.ObjectValue && (bool)mParam2.ObjectValue;
        }

        private object INT_OR_INT()
        {
            return (int)mParam1.ObjectValue | (int)mParam2.ObjectValue;
        }

        private object BOOL_OR_BOOL()
        {
            return (bool)mParam1.ObjectValue | (bool)mParam2.ObjectValue;
        }

        private object BOOL_ORELSE_BOOL()
        {
            return (bool)mParam1.ObjectValue || (bool)mParam2.ObjectValue;
        }

        private object INT_XOR_INT()
        {
            return (int)mParam1.ObjectValue ^ (int)mParam2.ObjectValue;
        }

        private object BOOL_XOR_BOOL()
        {
            return (bool)mParam1.ObjectValue ^ (bool)mParam2.ObjectValue;
        }

        private object NUM_EQ_NUM()
        {
            return ToDouble(mParam1.ObjectValue) == ToDouble(mParam2.ObjectValue);
        }

        private object NUM_LT_NUM()
        {
            return ToDouble(mParam1.ObjectValue) < ToDouble(mParam2.ObjectValue);
        }

        private object NUM_GT_NUM()
        {
            return ToDouble(mParam1.ObjectValue) > ToDouble(mParam2.ObjectValue);
        }

        private object NUM_GE_NUM()
        {
            return ToDouble(mParam1.ObjectValue) >= ToDouble(mParam2.ObjectValue);
        }

        private object NUM_LE_NUM()
        {
            return ToDouble(mParam1.ObjectValue) <= ToDouble(mParam2.ObjectValue);
        }

        private object NUM_NE_NUM()
        {
            return ToDouble(mParam1.ObjectValue) != ToDouble(mParam2.ObjectValue);
        }

        private object NUM_PLUS_NUM()
        {
            return ToDouble(mParam1.ObjectValue) + ToDouble(mParam2.ObjectValue);
        }

        private object NUM_MUL_NUM()
        {
            return ToDouble(mParam1.ObjectValue) * ToDouble(mParam2.ObjectValue);
        }

        private object NUM_MINUS_NUM()
        {
            return ToDouble(mParam1.ObjectValue) - ToDouble(mParam2.ObjectValue);
        }

        private object DATE_PLUS_NUM()
        {
            return ((System.DateTime)mParam1.ObjectValue).AddDays(ToDouble(mParam2.ObjectValue));
        }

        private object DATE_MINUS_DATE()
        {
            return ((System.DateTime)mParam1.ObjectValue).Subtract((System.DateTime)mParam2.ObjectValue).TotalDays;
        }

        private object DATE_MINUS_NUM()
        {
            return ((System.DateTime)mParam1.ObjectValue).AddDays(-ToDouble(mParam2.ObjectValue));
        }

        private object STR_CONCAT_STR()
        {
            return mParam1.ObjectValue.ToString() + mParam2.ObjectValue.ToString();
        }

        private object NUM_DIV_NUM()
        {
            return ToDouble(mParam1.ObjectValue) / ToDouble(mParam2.ObjectValue);
        }

        private object NUM_PERCENT_NUM()
        {
            return ToDouble(mParam2.ObjectValue) * (ToDouble(mParam1.ObjectValue) / 100);
        }

        private object INT_MOD_INT()
        {
            return (int)(mParam1.ObjectValue) % (int)(mParam2.ObjectValue);
        }

        private object STR_EQ_STR()
        {
            return String.Compare(ToString(mParam1.ObjectValue), ToString(mParam2.ObjectValue)) == 0;
        }

        private object STR_LT_STR()
        {
            return String.Compare(ToString(mParam1.ObjectValue), ToString(mParam2.ObjectValue)) < 0;
        }

        private object STR_GT_STR()
        {
            return String.Compare(ToString(mParam1.ObjectValue), ToString(mParam2.ObjectValue)) > 0;
        }

        private object STR_GE_STR()
        {
            return String.Compare(ToString(mParam1.ObjectValue), ToString(mParam2.ObjectValue)) >= 0;
        }

        private object STR_LE_STR()
        {
            return String.Compare(ToString(mParam1.ObjectValue), ToString(mParam2.ObjectValue)) <= 0;
        }

        private object STR_NE_STR()
        {
            return String.Compare(ToString(mParam1.ObjectValue), ToString(mParam2.ObjectValue)) != 0;
        }

        public override Type SystemType
        {
            get { return mSystemType; }
        }

        private void mParam1_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }

        private void mParam2_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }
    }

    public class GetVariableExpr : Expr
    {

        private IEvalValue withEventsField_mParam1;
        public IEvalValue mParam1
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
        public GetVariableExpr(IEvalValue value)
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

        private void mParam1_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }

    }

    public class CallMethodExpr : Expr
    {

        private object mBaseObject;
        private IEvalValue withEventsField_mBaseValue;
        public IEvalValue mBaseValue
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
        private IEvalValue[] mParams;

        private object[] mParamValues;
        private System.Type mResultSystemType;
        private IEvalValue withEventsField_mResultValue;
        public IEvalValue mResultValue
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

        internal CallMethodExpr(object baseObject, MemberInfo method, List<Expr> @params)
        {
            if (@params == null)
                @params = new List<Expr>();
            IEvalValue[] newParams = @params.ToArray();
            object[] newParamValues = new object[@params.Count];

            foreach (IEvalValue p in newParams)
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

            if (typeof(IEvalValue).IsAssignableFrom(mResultSystemType))
            {
                mResultValue = (IEvalValue)InternalValue();
                if (mResultValue is IEvalValue)
                {
                    mResultSystemType = ((IEvalValue)mResultValue).SystemType;
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

        protected static internal Expr GetNew(Parser parser, object baseObject, MemberInfo method, List<Expr> @params)
        {
            Expr o = null;
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
            if (mBaseObject is IEvalValue)
            {
                mBaseValue = (IEvalValue)mBaseObject;
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
                if (res is IEvalValue)
                {
                    mResultValue = (IEvalValue)res;
                    res = mResultValue.ObjectValue;
                }
                return res;
            }
        }

        public override System.Type SystemType
        {
            get { return mResultSystemType; }
        }

        private void mParamsValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }

        private void mBaseVariable_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }

        private void mResultVariable_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }
    }

    public class GetArrayEntryExpr : Expr
    {

        private Expr withEventsField_mArray;
        public Expr mArray
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
        private IEvalValue[] mParams;
        private int[] mValues;
        private Type mResultSystemType;

        public GetArrayEntryExpr(Expr array, List<Expr> @params)
        {
            IEvalValue[] newParams = @params.ToArray();
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

        private void mBaseVariable_ValueChanged(object Sender, System.EventArgs e)
        {
            base.RaiseEventValueChanged(Sender, e);
        }

    }

    public class OperatorIfExpr : Expr
    {
        private Type mSystemType;
        private Expr ifExpr;
        private Expr thenExpr;
        private Expr elseExpr;

        public OperatorIfExpr(Expr ifExpr, Expr thenExpr, Expr elseExpr)
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
