using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Eval4.Core
{
    class TypedExpressions
    {
        public static TypedExpr Create<P1, P2, T>(IHasValue p1, IHasValue p2, Func<P1, P2, T> func)
        {
            if (!CanConvert(p1, typeof(P1))) return null;
            if (!CanConvert(p2, typeof(P2))) return null;
            return new TypedExpr<P1, P2, T>(ChangeType<P1>(p1), ChangeType<P2>(p2), func);
        }

        private static bool CanConvert(IHasValue actual, Type targetType)
        {
            var actualType = actual.SystemType;
            if (targetType == actualType) return true;
            if (targetType.IsAssignableFrom(actualType)) return true;
            if (targetType == typeof(int)) return DelegatedExpr.IsIntOrSmaller(actualType);
            if (targetType == typeof(double)) return DelegatedExpr.IsDoubleOrSmaller(actualType);
            if (targetType == typeof(string)) return true;
            return false;
        }

        public static TypedExpr Create<P1, T>(IHasValue p1, Func<P1, T> func)
        {
            if(!CanConvert(p1,typeof(P1))) return null;
            return new TypedExpr<P1, T>(ChangeType<P1>(p1), func);
        }

        public static IHasValue<P> ChangeType<P>(IHasValue p)
        {
            if (!(p is IHasValue<P>))
            {                
                p = new ChangeTypeExpr<P>(p);
            }
            return (IHasValue<P>)p;
        }

        public static IHasValue ChangeType(IHasValue expr, Type newType)
        {
            if (expr.SystemType == newType) return (IHasValue)expr;

            var t = typeof(ChangeTypeExpr<>).MakeGenericType(newType);

            var i = (TypedExpr)Activator.CreateInstance(t);
            i.SetP1((IHasValue)expr);
            return i;
        }

        private static TypedExpr CompareToExpr<T>(IHasValue p1, IHasValue p2, TokenType tt)
        {

            switch (tt)
            {
                case TokenType.operator_gt:
                    return TypedExpressions.Create<T, T, bool>(p1, p2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) > 0; });
                case TokenType.operator_ge:
                    return TypedExpressions.Create<T, T, bool>(p1, p2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) >= 0; });
                case TokenType.operator_eq:
                    return TypedExpressions.Create<T, T, bool>(p1, p2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) == 0; });
                case TokenType.operator_le:
                    return TypedExpressions.Create<T, T, bool>(p1, p2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) <= 0; });
                case TokenType.operator_lt:
                    return TypedExpressions.Create<T, T, bool>(p1, p2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) < 0; });
                case TokenType.operator_ne:
                    return TypedExpressions.Create<T, T, bool>(p1, p2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) != 0; });
                default:
                    throw new InvalidOperationException();
            }
        }

        internal static IHasValue BinaryExpr(Parser parser, IHasValue ValueLeft, TokenType tt, IHasValue ValueRight)
        {
            var result = InternalBinaryExpr(parser, ValueLeft, tt, ValueRight);
            if (string.IsNullOrEmpty(result.ShortName))
            {
                result.ShortName = tt.ToString();
            }
            return result;
        }

        internal static TypedExpr InternalBinaryExpr(Parser parser, IHasValue ValueLeft, TokenType tt, IHasValue ValueRight)
        {
            Type v1Type = ValueLeft.SystemType;
            Type v2Type = ValueRight.SystemType;
            TypedExpr result = null;

            switch (tt)
            {
                case TokenType.operator_plus:
                    result = TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a + b; })
                        ?? TypedExpressions.Create<DateTime, double, DateTime>(ValueLeft, ValueRight, (a, b) => { return a.AddDays(b); })
                        ?? TypedExpressions.Create<double, DateTime, DateTime>(ValueLeft, ValueRight, (a, b) => { return b.AddDays(a); })
                        ?? TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a + b; })
                        ?? TypedExpressions.Create<string, string, string>(ValueLeft, ValueRight, (a, b) => { return a + b; });
                    break;
                case TokenType.operator_minus:
                    result = TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a - b; })
                        ?? TypedExpressions.Create<DateTime, double, DateTime>(ValueLeft, ValueRight, (a, b) => { return a.AddDays(-b); })
                        ?? TypedExpressions.Create<DateTime, DateTime, double>(ValueLeft, ValueRight, (a, b) => { return a.Subtract(b).TotalDays; })
                        ?? TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a - b; });
                    break;

                case TokenType.operator_mul:
                    result = TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a * b; })
                        ?? TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a * b; });
                    break;

                case TokenType.operator_div:
                    result = TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a / b; });
                    break;

                case TokenType.operator_mod:
                    result = TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a % b; })
                        ?? TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a % b; });
                    break;
                case TokenType.operator_and:
                    result = TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a & b; })
                        ?? TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a & b; });
                    break;

                case TokenType.operator_or:
                    result = TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a | b; })
                        ?? TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a | b; });
                    break;

                case TokenType.operator_xor:
                    result = TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a ^ b; })
                        ?? TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a ^ b; });
                    break;

                case TokenType.operator_andalso:
                    result = TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a && b; });
                    break;

                case TokenType.operator_orelse:
                    result = TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a || b; });
                    break;

                case TokenType.operator_eq:
                    result = TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; })
                        ?? TypedExpressions.Create<int, int, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; })
                        ?? TypedExpressions.Create<string, string, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; })
                        ?? TypedExpressions.Create<double, double, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; })
                        ?? TypedExpressions.Create<object, object, bool>(ValueLeft, ValueRight, (a, b) => { return a.Equals(b); });
                    break;
                case TokenType.operator_ne:
                    result = TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; })
                        ?? TypedExpressions.Create<int, int, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; })
                        ?? TypedExpressions.Create<string, string, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; })
                        ?? TypedExpressions.Create<double, double, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; })
                        ?? TypedExpressions.Create<object, object, bool>(ValueLeft, ValueRight, (a, b) => { return !a.Equals(b); });
                    break;
                case TokenType.operator_ge:
                case TokenType.operator_gt:
                case TokenType.operator_le:
                case TokenType.operator_lt:
                    result = CompareToExpr<int>(ValueLeft, ValueRight, tt)
                        ?? CompareToExpr<double>(ValueLeft, ValueRight, tt)
                        ?? CompareToExpr<bool>(ValueLeft, ValueRight, tt)
                        ?? CompareToExpr<string>(ValueLeft, ValueRight, tt)
                        ?? CompareToExpr<object>(ValueLeft, ValueRight, tt);
                    break;
            }
            if (result != null) return result;
            throw parser.NewParserException("Cannot apply the operator " + tt.ToString().Replace("operator_", "") + " on " + v1Type.ToString() + " and " + v2Type.ToString());
        }


        internal static IHasValue UnaryExpr(Parser parser, TokenType tt, IHasValue ValueLeft)
        {
            var v1Type = ValueLeft.SystemType;
            TypedExpr result = null;

            switch (tt)
            {
                case TokenType.operator_not:
                    result = TypedExpressions.Create<bool, bool>(ValueLeft, (a) => { return !a; })
                        ?? TypedExpressions.Create<int, int>(ValueLeft, (a) => { return ~a; });
                    break;
                case TokenType.operator_minus:
                    result =  TypedExpressions.Create<int, int>(ValueLeft, (a) => { return -a; })
                        ?? TypedExpressions.Create<double, double>(ValueLeft, (a) => { return -a; });
                    break;
                case TokenType.operator_plus:
                    if (DelegatedExpr.IsIntOrSmaller(v1Type)) return ValueLeft;
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type)) return ValueLeft;
                    break;
            }
            if (result != null) return result;
            throw parser.NewParserException("Invalid operator " + tt.ToString().Replace("operator_", ""));
        }
    }

    public interface ISetP1 : IHasValue
    {
        void SetP1(IHasValue p1);
    }

    public interface ISetP2 : IHasValue
    {
        void SetP2(IHasValue p2);
    }

    public class ChangeTypeExpr<T> : ISetP1, IHasValue<T>
    {
        internal IHasValue mP1;

        public ChangeTypeExpr()
        {
        }

        public ChangeTypeExpr(IHasValue p1)
        {
            mP1 = p1;
            mP1.ValueChanged += mP1_ValueChanged;
        }

        void mP1_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null) ValueChanged(sender, e);
        }

        public object ObjectValue
        {
            get { return Value; }
        }

        public event ValueChangedEventHandler ValueChanged;

        public Type SystemType
        {
            get { return typeof(T); }
        }

        public T Value
        {
            get
            {
                return (T)System.Convert.ChangeType(mP1.ObjectValue, typeof(T));
            }
        }

        public IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield return new Dependency("p1", mP1);
            }
        }


        public string ShortName
        {
            get { return "ChangeType"; }
        }


        public void SetP1(IHasValue p1)
        {
            mP1 = p1;
            p1.ValueChanged += mP1_ValueChanged;
        }
    }

    public abstract class TypedExpr : IHasValue, ISetP1, ISetP2
    {

        public abstract object ObjectValue { get; }

        public event ValueChangedEventHandler ValueChanged;

        public abstract Type SystemType { get; }

        public string ShortName { get; set; }

        public abstract IEnumerable<Dependency> Dependencies { get; }

        internal void RaiseValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null) ValueChanged(sender, e);
        }

        public abstract void SetP1(IHasValue newP1);
        public abstract void SetP2(IHasValue newP2);
    }

    public class TypedExpr<P1, P2, T> : TypedExpr, IHasValue<T>
    {
        private IHasValue<P1> mP1;
        private IHasValue<P2> mP2;
        private Func<P1, P2, T> mFunc;

        public TypedExpr(IHasValue<P1> p1, IHasValue<P2> p2, Func<P1, P2, T> func)
        {
            mP1 = p1;
            mP2 = p2;
            mFunc = func;
            mP1.ValueChanged += mP_ValueChanged;
            mP2.ValueChanged += mP_ValueChanged;
        }

        void mP_ValueChanged(object sender, EventArgs e)
        {
            base.RaiseValueChanged(sender, e);
        }


        public override object ObjectValue
        {
            get
            {
                return mFunc(mP1.Value, mP2.Value);
            }
        }

        public override Type SystemType
        {
            get { return typeof(T); }
        }

        public T Value
        {
            get { return mFunc(mP1.Value, mP2.Value); }
        }


        public override IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield return new Dependency("P1", mP1);
                yield return new Dependency("P2", mP2);
            }
        }

        public override void SetP1(IHasValue newP1)
        {
            mP1 = (IHasValue<P1>)newP1;
        }

        public override void SetP2(IHasValue newP2)
        {
            mP2 = (IHasValue<P2>)newP2;
        }
    }

    public class TypedExpr<P1, T> : TypedExpr, IHasValue<T>
    {
        private IHasValue<P1> mP1;
        private Func<P1, T> mFunc;

        public TypedExpr(IHasValue<P1> p1, Func<P1, T> func)
        {
            mP1 = p1;
            mFunc = func;
            mP1.ValueChanged += mP_ValueChanged;
        }

        void mP_ValueChanged(object Sender, EventArgs e)
        {
            if (ValueChanged != null) ValueChanged(Sender, e);
        }

        public event ValueChangedEventHandler ValueChanged;

        public T Value
        {
            get { return mFunc(mP1.Value); }
        }

        public override object ObjectValue
        {
            get { return mFunc(mP1.Value); }
        }

        public override Type SystemType
        {
            get { return typeof(T); }
        }

        public override IEnumerable<Dependency> Dependencies
        {
            get
            {
                yield return new Dependency("p1", mP1);
            }
        }

        public override void SetP1(IHasValue newP1)
        {
            mP1 = (IHasValue<P1>)newP1;
        }

        public override void SetP2(IHasValue newP2)
        {
            throw new NotImplementedException();
        }
    }

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

        public event ValueChangedEventHandler ValueChanged;

        public abstract IEnumerable<Dependency> Dependencies { get; }

        public abstract string ShortName { get; }
    }

    internal class ConstantExpr<T> : IHasValue<T>
    {
        private T mValue;

        public ConstantExpr(T value)
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

        public CallMethodExpr(IHasValue baseObject, MemberInfo method, List<IHasValue> @params)
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
                    mParams[i] = TypedExpressions.ChangeType(mParams[i], paramInfo[i].ParameterType);
                }
            }
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
