using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Core
{
    class TypedExpr
    {
        public static TypedExpr<P1, P2, T> Create<P1, P2, T>(IExpr p1, IExpr p2, Func<P1, P2, T> func)
        {
            return new TypedExpr<P1, P2, T>(ChangeType<P1>(p1), ChangeType<P2>(p2), func);
        }

        public static TypedExpr<P1, T> Create<P1, T>(IExpr p1, Func<P1, T> func)
        {
            return new TypedExpr<P1, T>(ChangeType<P1>(p1), func);
        }
        
        private static IExpr<P> ChangeType<P>(IExpr p)
        {
            if (!(p is IExpr<P>))
            {
                p = new ChangeTypeExpr<P>(p);
            }
            return (IExpr<P>)p;
        }

        private static IExpr<bool> CompareToExpr<T>(IExpr p1, IExpr p2, TokenType tt)
        {
            var pp1 = ChangeType<T>(p1);
            var pp2 = ChangeType<T>(p2);

            switch (tt)
            {
                case TokenType.operator_gt:
                    return TypedExpr.Create<T, T, bool>(pp1, pp2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) > 0; });
                case TokenType.operator_ge:
                    return TypedExpr.Create<T, T, bool>(pp1, pp2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) >= 0; });
                case TokenType.operator_eq:
                    return TypedExpr.Create<T, T, bool>(pp1, pp2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) == 0; });
                case TokenType.operator_le:
                    return TypedExpr.Create<T, T, bool>(pp1, pp2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) <= 0; });
                case TokenType.operator_lt:
                    return TypedExpr.Create<T, T, bool>(pp1, pp2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) < 0; });
                case TokenType.operator_ne:
                    return TypedExpr.Create<T, T, bool>(pp1, pp2, (a, b) => { return ((IComparable<T>)a).CompareTo(b) != 0; });
                default:
                    throw new InvalidOperationException();
            }
        }


        internal static IExpr BinaryExpr(Parser parser, IExpr ValueLeft, TokenType tt, IExpr ValueRight)
        {
            Type v1Type = ValueLeft.SystemType;
            Type v2Type = ValueRight.SystemType;

            switch (tt)
            {
                case TokenType.operator_plus:
                    if (Expr.IsIntOrSmaller(v1Type) && Expr.IsIntOrSmaller(v2Type)) return TypedExpr.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a + b; });
                    else if (v1Type == typeof(DateTime) && Expr.IsDoubleOrSmaller(v2Type)) return TypedExpr.Create<DateTime, double, DateTime>(ValueLeft, ValueRight, (a, b) => { return a.AddDays(b); });
                    else if (Expr.IsDoubleOrSmaller(v1Type) && v2Type == typeof(DateTime)) return TypedExpr.Create<double, DateTime, DateTime>(ValueLeft, ValueRight, (a, b) => { return b.AddDays(a); });
                    else if (Expr.IsDoubleOrSmaller(v1Type) && Expr.IsDoubleOrSmaller(v2Type)) return TypedExpr.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a + b; });
                    else return TypedExpr.Create<string, string, string>(ValueLeft, ValueRight, (a, b) => { return a + b; });
                    break;
                case TokenType.operator_minus:
                    if (Expr.IsIntOrSmaller(v1Type) && Expr.IsIntOrSmaller(v2Type)) return TypedExpr.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a - b; });
                    else if (v1Type == typeof(DateTime) && Expr.IsDoubleOrSmaller(v2Type)) return TypedExpr.Create<DateTime, double, DateTime>(ValueLeft, ValueRight, (a, b) => { return a.AddDays(-b); });
                    else if (v1Type == typeof(DateTime) && v2Type == typeof(DateTime)) return TypedExpr.Create<DateTime, DateTime, double>(ValueLeft, ValueRight, (a, b) => { return a.Subtract(b).TotalDays; });
                    else if (Expr.IsDoubleOrSmaller(v1Type) && Expr.IsDoubleOrSmaller(v2Type)) return TypedExpr.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a - b; });
                    break;
                case TokenType.operator_mul:
                    if (Expr.IsIntOrSmaller(v1Type) && Expr.IsIntOrSmaller(v2Type)) return TypedExpr.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a * b; });
                    else if (Expr.IsDoubleOrSmaller(v1Type) && Expr.IsDoubleOrSmaller(v2Type)) return TypedExpr.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a * b; });
                    break;
                case TokenType.operator_integerdiv:
                    if (Expr.IsIntOrSmaller(v1Type) && Expr.IsIntOrSmaller(v2Type)) return TypedExpr.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a / b; });
                    break;
                case TokenType.operator_div:
                    return TypedExpr.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a / b; });
                
                case TokenType.operator_mod:
                    if (Expr.IsIntOrSmaller(v1Type) && Expr.IsIntOrSmaller(v2Type)) return TypedExpr.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a % b; });
                    else return TypedExpr.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a % b; });
                    break;
                case TokenType.operator_and:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpr.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a & b; });
                    return TypedExpr.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a & b; });
                
                case TokenType.operator_or:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpr.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a | b; });
                    return TypedExpr.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a | b; });
                
                case TokenType.operator_xor:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpr.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a ^ b; });
                    return TypedExpr.Create<int, int, int>(ValueLeft, ValueRight, (a, b) => { return a ^ b; });
                
                case TokenType.operator_andalso:
                    return TypedExpr.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a && b; });
                
                case TokenType.operator_orelse:
                    return TypedExpr.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a || b; });
                
                case TokenType.operator_eq:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpr.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; });
                    else if (Expr.IsIntOrSmaller(v1Type) && Expr.IsIntOrSmaller(v2Type)) return TypedExpr.Create<int, int, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; });
                    else if (v1Type == typeof(string) && v2Type == typeof(string)) return TypedExpr.Create<string, string, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; });
                    else if (v1Type == typeof(double) && v2Type == typeof(double)) return TypedExpr.Create<double, double, bool>(ValueLeft, ValueRight, (a, b) => { return a == b; });
                    else return TypedExpr.Create<object, object, bool>(ValueLeft, ValueRight, (a, b) => { return a.Equals(b); });
                
                case TokenType.operator_ne:
                    if (v1Type == typeof(bool) && v2Type == typeof(bool)) return TypedExpr.Create<bool, bool, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; });
                    else if (Expr.IsIntOrSmaller(v1Type) && Expr.IsIntOrSmaller(v2Type)) return TypedExpr.Create<int, int, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; });
                    else if (v1Type == typeof(string) && v2Type == typeof(string)) return TypedExpr.Create<string, string, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; });
                    else if (v1Type == typeof(double) && v2Type == typeof(double)) return TypedExpr.Create<double, double, bool>(ValueLeft, ValueRight, (a, b) => { return a != b; });
                    else return TypedExpr.Create<object, object, bool>(ValueLeft, ValueRight, (a, b) => { return !a.Equals(b); });
                
                case TokenType.operator_ge:
                case TokenType.operator_gt:
                case TokenType.operator_le:
                case TokenType.operator_lt:
                    if (Expr.IsIntOrSmaller(v1Type)) return CompareToExpr<int>(ValueLeft, ValueRight, tt);
                    else if (v1Type == typeof(bool)) return CompareToExpr<bool>(ValueLeft, ValueRight, tt);
                    else if (v1Type == typeof(double)) return CompareToExpr<double>(ValueLeft, ValueRight, tt);
                    else if (v1Type == typeof(string)) return CompareToExpr<string>(ValueLeft, ValueRight, tt);
                    else return CompareToExpr<object>(ValueLeft, ValueRight, tt);

                case TokenType.operator_percent:
                    return TypedExpr.Create<double, double, double>(ValueLeft, ValueRight, (a, b) => { return a * b / 100.0; });
            }
            
            
            throw parser.NewParserException("Cannot apply the operator " + tt.ToString().Replace("operator_", "") + " on " + v1Type.ToString() + " and " + v2Type.ToString());
        }

       
        internal static IExpr UnaryExpr(Parser parser, TokenType tt, IExpr ValueLeft)
        {
            var v1Type = ValueLeft.SystemType;

            switch (tt)
            {
                case TokenType.unary_not:
                case TokenType.operator_not:
                    if (v1Type == typeof(bool)) return TypedExpr.Create<bool, bool>(ValueLeft, (a) => { return !a; });
                    else if (Expr.IsIntOrSmaller(v1Type)) return TypedExpr.Create<int, int>(ValueLeft, (a) => { return ~a; });
                    break;
                case TokenType.unary_minus:
                case TokenType.operator_minus:
                    if (Expr.IsIntOrSmaller(v1Type)) return TypedExpr.Create<int, int>(ValueLeft, (a) => { return -a; });
                    else if (Expr.IsDoubleOrSmaller(v1Type)) return TypedExpr.Create<double, double>(ValueLeft, (a) => { return -a; });
                    break;
                case TokenType.unary_plus:
                case TokenType.operator_plus:
                    if (Expr.IsIntOrSmaller(v1Type)) return ValueLeft;
                    else if (Expr.IsDoubleOrSmaller(v1Type)) return ValueLeft;
                    break;
            }
            throw parser.NewParserException("Invalid operator " + tt.ToString().Replace("operator_", ""));
        }
    }

    public class ChangeTypeExpr<T> : IExpr<T>
    {
        IExpr mP1;

        public ChangeTypeExpr(IExpr p1)
        {
            mP1 = p1;
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
    }

    public class TypedExpr<P1, P2, T> : IExpr<T>
    {
        private IExpr<P1> mP1;
        private IExpr<P2> mP2;
        private Func<P1, P2, T> mFunc;

        public TypedExpr(IExpr<P1> p1, IExpr<P2> p2, Func<P1, P2, T> func)
        {
            mP1 = p1;
            mP2 = p2;
            mFunc = func;
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
            get { return mFunc(mP1.Value, mP2.Value); }
        }

    }

    public class TypedExpr<P1, T> : IExpr<T>
    {
        private IExpr<P1> mP1;
        private Func<P1, T> mFunc;

        public TypedExpr(IExpr<P1> p1, Func<P1, T> func)
        {
            mP1 = p1;
            mFunc = func;
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

    }

}
