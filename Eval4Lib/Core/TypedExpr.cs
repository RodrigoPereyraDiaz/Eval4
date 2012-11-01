using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Core
{
    class TypedExpressions
    {
        public static TypedExpr<P1, P2, T> Create<P1, P2, T>(IHasValue p1, IHasValue p2, Func<P1, P2, T> func)
        {
            return new TypedExpr<P1, P2, T>(ChangeType<P1>(p1), ChangeType<P2>(p2), func);
        }

        public static TypedExpr<P1, T> Create<P1, T>(IHasValue p1, Func<P1, T> func)
        {
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

            var i = (IChangeTypeExpr)Activator.CreateInstance(t);
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
            var result = InternaBinaryExpr(parser, ValueLeft, tt, ValueRight);
            if (string.IsNullOrEmpty(result.ShortName))
            {
                result.ShortName = tt.ToString();
            }
            return result;
        }

        internal static TypedExpr InternaBinaryExpr(Parser parser, IHasValue ValueLeft, TokenType tt, IHasValue ValueRight)
        {
            Type v1Type = ValueLeft.SystemType;
            Type v2Type = ValueRight.SystemType;

            switch (tt)
            {
                case TokenType.operator_plus:
                    if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a + b; });
                    else if (v1Type == typeof(DateTime) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<DateTime, double, DateTime>(ValueLeft, ValueRight, (a, b) => { return a.AddDays(b); });
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && v2Type == typeof(DateTime)) return TypedExpressions.Create<double, DateTime, DateTime>(ValueLeft, ValueRight, (a, b) => { return b.AddDays(a); });
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a + b; });
                    else return TypedExpressions.Create<string, string, string>(ValueLeft, ValueRight, (a, b) => { return a + b; });

                case TokenType.operator_minus:
                    if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a - b; });
                    else if (v1Type == typeof(DateTime) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<DateTime, double, DateTime>(ValueLeft, ValueRight, (a, b) => { return a.AddDays(-b); });
                    else if (v1Type == typeof(DateTime) && v2Type == typeof(DateTime)) return TypedExpressions.Create<DateTime, DateTime, double>(ValueLeft, ValueRight, (a, b) => { return a.Subtract(b).TotalDays; });
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a - b; });
                    break;

                case TokenType.operator_mul:
                    if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a * b; });
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a * b; });
                    break;

                case TokenType.operator_div:
                    if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a / b; });
                    break;

                case TokenType.operator_mod:
                    if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a % b; });
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a % b; });
                    break;
                case TokenType.operator_and:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a & b; });
                    else if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a & b; });
                    break;

                case TokenType.operator_or:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a | b; });
                    else if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a | b; });
                    break;

                case TokenType.operator_xor:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a ^ b; });
                    else if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a ^ b; });
                    break;

                case TokenType.operator_andalso:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a && b; });
                    break;

                case TokenType.operator_orelse:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a || b; });
                    break;

                case TokenType.operator_eq:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; });
                    else if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; });
                    else if (v1Type == typeof(string) && v2Type == typeof(string)) return TypedExpressions.Create<string, string, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; });
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<double, double, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; });
                    else return TypedExpressions.Create<object, object, bool>(ValueLeft, ValueRight, (a, b) => { return a.Equals(b); });

                case TokenType.operator_ne:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpressions.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; });
                    else if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return TypedExpressions.Create<int, int, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; });
                    else if (v1Type == typeof(string) && v2Type == typeof(string)) return TypedExpressions.Create<string, string, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; });
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return TypedExpressions.Create<double, double, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; });
                    else return TypedExpressions.Create<object, object, bool>(ValueLeft, ValueRight, (a, b) => { return !a.Equals(b); });

                case TokenType.operator_ge:
                case TokenType.operator_gt:
                case TokenType.operator_le:
                case TokenType.operator_lt:
                    if (DelegatedExpr.IsIntOrSmaller(v1Type) && DelegatedExpr.IsIntOrSmaller(v2Type)) return CompareToExpr<int>(ValueLeft, ValueRight, tt);
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type) && DelegatedExpr.IsDoubleOrSmaller(v2Type)) return CompareToExpr<double>(ValueLeft, ValueRight, tt);
                    else if (v1Type == typeof(bool)) return CompareToExpr<bool>(ValueLeft, ValueRight, tt);
                    else if (v1Type == typeof(string)) return CompareToExpr<string>(ValueLeft, ValueRight, tt);
                    else return CompareToExpr<object>(ValueLeft, ValueRight, tt);

            }
            throw parser.NewParserException("Cannot apply the operator " + tt.ToString().Replace("operator_", "") + " on " + v1Type.ToString() + " and " + v2Type.ToString());
        }


        internal static IHasValue UnaryExpr(Parser parser, TokenType tt, IHasValue ValueLeft)
        {
            var v1Type = ValueLeft.SystemType;

            switch (tt)
            {
                case TokenType.operator_not:
                    if (v1Type == typeof(bool)) return TypedExpressions.Create<bool, bool>(ValueLeft, (a) => { return !a; });
                    else if (DelegatedExpr.IsIntOrSmaller(v1Type)) return TypedExpressions.Create<int, int>(ValueLeft, (a) => { return ~a; });
                    break;
                case TokenType.operator_minus:
                    if (DelegatedExpr.IsIntOrSmaller(v1Type)) return TypedExpressions.Create<int, int>(ValueLeft, (a) => { return -a; });
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type)) return TypedExpressions.Create<double, double>(ValueLeft, (a) => { return -a; });
                    break;
                case TokenType.operator_plus:
                    if (DelegatedExpr.IsIntOrSmaller(v1Type)) return ValueLeft;
                    else if (DelegatedExpr.IsDoubleOrSmaller(v1Type)) return ValueLeft;
                    break;
            }
            throw parser.NewParserException("Invalid operator " + tt.ToString().Replace("operator_", ""));
        }
    }

    public interface IChangeTypeExpr : IHasValue
    {
        void SetP1(IHasValue p1);
    }

    public class ChangeTypeExpr<T> : IChangeTypeExpr, IHasValue<T>
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
            p1.ValueChanged+=mP1_ValueChanged;
        }
    }

    public abstract class TypedExpr : IHasValue
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
    }

    public class TypedExpr<P1, T> : IHasValue<T>
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

        public object ObjectValue
        {
            get
            {
                return Value;
            }
        }

        public event ValueChangedEventHandler ValueChanged;

        public Type SystemType
        {
            get { return typeof(T); }
        }

        public T Value
        {
            get { return mFunc(mP1.Value); }
        }

        public IEnumerable<Dependency> Dependencies
        {
            get { throw new NotImplementedException(); }
        }


        public string ShortName
        {
            get { throw new NotImplementedException(); }
        }
    }

}
