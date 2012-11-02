using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Core
{
    abstract class TypeHandler
    {
        protected internal abstract void DeclareOperations();

        protected void AddOperation<P1, T>(TokenType tokenType, Func<P1, T> func)
        {
        }

        protected void AddOperation<P1, P2, T>(TokenType tokenType, Func<P1, P2, T> func)
        {
        }

        protected void AddImplicitCast<P1, T>(Func<P1, T> func)
        {
        }

        protected void AddExplicitCast<P1, T>(Func<P1, T> func)
        {
        }
    }

    class BoolTypeHandler : TypeHandler
    {
        protected internal override void DeclareOperations()
        {
            AddOperation<bool, bool, bool>(TokenType.operator_and, (a, b) => { return a & b; });
            AddOperation<bool, bool, bool>(TokenType.operator_or, (a, b) => { return a | b; });
            AddOperation<bool, bool, bool>(TokenType.operator_xor, (a, b) => { return a ^ b; });
            AddOperation<bool, bool, bool>(TokenType.operator_andalso, (a, b) => { return a && b; });
            AddOperation<bool, bool, bool>(TokenType.operator_orelse, (a, b) => { return a || b; });
            AddOperation<bool, bool, bool>(TokenType.operator_eq, (a, b) => { return a == b; });
            AddOperation<bool, bool, bool>(TokenType.operator_ne, (a, b) => { return a != b; });
            AddOperation<bool, bool>(TokenType.operator_not, (a) => { return !a; });

            AddExplicitCast<bool, int>((a) => { return (a ? 1 : 0); });
        }

    }

    class DoubleTypeHandler : TypeHandler
    {
        protected internal override void DeclareOperations()
        {
            AddOperation<double, double, double>(TokenType.operator_plus, (a, b) => { return a + b; });
            AddOperation<double, double, double>(TokenType.operator_minus, (a, b) => { return a - b; });
            AddOperation<double, double, double>(TokenType.operator_mul, (a, b) => { return a * b; });
            AddOperation<double, double, double>(TokenType.operator_div, (a, b) => { return a / b; });
            //AddOperation<double, double, double>(TokenType.operator_and, (a, b) => { return a & b; });
            //AddOperation<double, double, double>(TokenType.operator_or, (a, b) => { return a | b; });
            //AddOperation<double, double, double>(TokenType.operator_xor, (a, b) => { return a ^ b; });
            //AddOperation<double, double, double>(TokenType.operator_andalso, (a, b) => { return a && b; });
            //AddOperation<double, double, double>(TokenType.operator_orelse, (a, b) => { return a || b; });
            AddOperation<double, double, bool>(TokenType.operator_eq, (a, b) => { return a == b; });
            AddOperation<double, double, bool>(TokenType.operator_ne, (a, b) => { return a != b; });
            AddOperation<double, double, bool>(TokenType.operator_ge, (a, b) => { return a >= b; });
            AddOperation<double, double, bool>(TokenType.operator_gt, (a, b) => { return a > b; });
            AddOperation<double, double, bool>(TokenType.operator_le, (a, b) => { return a <= b; });
            AddOperation<double, double, bool>(TokenType.operator_lt, (a, b) => { return a < b; });

            //AddOperation<double, double>(TokenType.operator_not, (a) => { return ~a; });
            AddOperation<double, double>(TokenType.operator_minus, (a) => { return -a; });
            AddOperation<double, double>(TokenType.operator_plus, (a) => { return a; });

            AddImplicitCast<byte, double>((a) => { return a; });
            AddImplicitCast<sbyte, double>((a) => { return a; });
            AddImplicitCast<short, double>((a) => { return a; });
            AddImplicitCast<ushort, double>((a) => { return a; });
            AddImplicitCast<int, double>((a) => { return a; });
            AddImplicitCast<uint, double>((a) => { return a; });
            AddImplicitCast<long, double>((a) => { return a; });
            AddImplicitCast<ulong, double>((a) => { return a; });
            AddImplicitCast<float, double>((a) => { return a; });

            AddExplicitCast<decimal, double>((a) => { return (int)a; });
        }

    }

    class IntTypeHandler : TypeHandler
    {
        protected internal override void DeclareOperations()
        {
            AddOperation<int, int, int>(TokenType.operator_plus, (a, b) => { return a + b; });
            AddOperation<int, int, int>(TokenType.operator_minus, (a, b) => { return a - b; });
            AddOperation<int, int, int>(TokenType.operator_mul, (a, b) => { return a * b; });
            AddOperation<int, int, int>(TokenType.operator_div, (a, b) => { return a / b; });
            AddOperation<int, int, int>(TokenType.operator_and, (a, b) => { return a & b; });
            AddOperation<int, int, int>(TokenType.operator_or, (a, b) => { return a | b; });
            AddOperation<int, int, int>(TokenType.operator_xor, (a, b) => { return a ^ b; });
            //AddOperation<int, int, int>(TokenType.operator_andalso, (a, b) => { return a && b; });
            //AddOperation<int, int, int>(TokenType.operator_orelse, (a, b) => { return a || b; });
            AddOperation<int, int, bool>(TokenType.operator_eq, (a, b) => { return a == b; });
            AddOperation<int, int, bool>(TokenType.operator_ne, (a, b) => { return a != b; });
            AddOperation<int, int, bool>(TokenType.operator_ge, (a, b) => { return a >= b; });
            AddOperation<int, int, bool>(TokenType.operator_gt, (a, b) => { return a > b; });
            AddOperation<int, int, bool>(TokenType.operator_le, (a, b) => { return a <= b; });
            AddOperation<int, int, bool>(TokenType.operator_lt, (a, b) => { return a < b; });

            AddOperation<int, int>(TokenType.operator_not, (a) => { return ~a; });
            AddOperation<int, int>(TokenType.operator_minus, (a) => { return -a; });
            AddOperation<int, int>(TokenType.operator_plus, (a) => { return a; });

            AddImplicitCast<byte, int>((a) => { return a; });
            AddImplicitCast<sbyte, int>((a) => { return a; });
            AddImplicitCast<short, int>((a) => { return a; });
            AddImplicitCast<ushort, int>((a) => { return a; });

            AddExplicitCast<uint, int>((a) => { return (int)a; });
            AddExplicitCast<long, int>((a) => { return (int)a; });
            AddExplicitCast<ulong, int>((a) => { return (int)a; });
            AddExplicitCast<double, int>((a) => { return (int)a; });
            AddExplicitCast<float, int>((a) => { return (int)a; });
            AddExplicitCast<decimal, int>((a) => { return (int)a; });
        }
    }

    class DateTimeTypeHandler : TypeHandler
    {
        protected internal override void DeclareOperations()
        {
            AddOperation<DateTime, TimeSpan, DateTime>(TokenType.operator_plus, (a, b) => { return a.Add(b); });
            AddOperation<DateTime, TimeSpan, DateTime>(TokenType.operator_minus, (a, b) => { return a.Subtract(b); });
            AddOperation<DateTime, DateTime, TimeSpan>(TokenType.operator_minus, (a, b) => { return a.Subtract(b); });
            AddOperation<DateTime, DateTime, bool>(TokenType.operator_eq, (a, b) => { return a.Ticks == b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.operator_ne, (a, b) => { return a.Ticks != b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.operator_ge, (a, b) => { return a.Ticks >= b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.operator_gt, (a, b) => { return a.Ticks > b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.operator_le, (a, b) => { return a.Ticks <= b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.operator_lt, (a, b) => { return a.Ticks < b.Ticks; });

            AddOperation<TimeSpan, TimeSpan, TimeSpan>(TokenType.operator_plus, (a, b) => { return a + b; });
            AddOperation<TimeSpan, TimeSpan, TimeSpan>(TokenType.operator_minus, (a, b) => { return a - b; });
            AddOperation<TimeSpan, double, TimeSpan>(TokenType.operator_mul, (a, b) => { return TimeSpan.FromTicks((long)(a.Ticks * b)); });
            AddOperation<double, TimeSpan, TimeSpan>(TokenType.operator_mul, (a, b) => { return TimeSpan.FromTicks((long)(a * b.Ticks)); });
            AddOperation<TimeSpan, double, TimeSpan>(TokenType.operator_div, (a, b) => { return TimeSpan.FromTicks((long)(a.Ticks / b)); });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.operator_eq, (a, b) => { return a == b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.operator_ne, (a, b) => { return a != b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.operator_ge, (a, b) => { return a >= b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.operator_gt, (a, b) => { return a > b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.operator_le, (a, b) => { return a <= b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.operator_lt, (a, b) => { return a < b; });

            AddOperation<TimeSpan, TimeSpan>(TokenType.operator_minus, (a) => { return -a; });
            AddOperation<TimeSpan, TimeSpan>(TokenType.operator_plus, (a) => { return a; });
            AddImplicitCast<double, TimeSpan>((a) => { return TimeSpan.FromDays(a); });
        }
    }

    class StringTypeHandler : TypeHandler
    {
        protected internal override void DeclareOperations()
        {
            AddOperation<string, string, string>(TokenType.operator_plus, (a, b) => { return a + b; });
            AddOperation<string, int, string>(TokenType.operator_mul, (a, b) => { return RepeatString(a, b); });
            AddOperation<int, string, string>(TokenType.operator_mul, (a, b) => { return RepeatString(b, a); });
            AddOperation<string, string, bool>(TokenType.operator_eq, (a, b) => { return CompareString(a, b) == 0; });
            AddOperation<string, string, bool>(TokenType.operator_ne, (a, b) => { return CompareString(a, b) != 0; });
            AddOperation<string, string, bool>(TokenType.operator_ge, (a, b) => { return CompareString(a, b) >= 0; });
            AddOperation<string, string, bool>(TokenType.operator_gt, (a, b) => { return CompareString(a, b) > 0; });
            AddOperation<string, string, bool>(TokenType.operator_le, (a, b) => { return CompareString(a, b) <= 0; });
            AddOperation<string, string, bool>(TokenType.operator_lt, (a, b) => { return CompareString(a, b) < 0; });

            AddImplicitCast<byte, string>((a) => { return ToString(a); });
            AddImplicitCast<sbyte, string>((a) => { return ToString(a); });
            AddImplicitCast<short, string>((a) => { return ToString(a); });
            AddImplicitCast<ushort, string>((a) => { return ToString(a); });
            AddExplicitCast<uint, string>((a) => { return ToString(a); });
            AddExplicitCast<int, string>((a) => { return ToString(a); });
            AddExplicitCast<long, string>((a) => { return ToString(a); });
            AddExplicitCast<ulong, string>((a) => { return ToString(a); });
            AddExplicitCast<double, string>((a) => { return ToString(a); });
            AddExplicitCast<float, string>((a) => { return ToString(a); });
            AddExplicitCast<decimal, string>((a) => { return ToString(a); });
        }

        private string ToString(object a)
        {
            if (a == null) return null;
            return a.ToString();
        }

        private int CompareString(string a, string b)
        {
            return String.Compare(a, b);
        }

        private static string RepeatString(string a, int b)
        {
            StringBuilder sb = new StringBuilder(a.Length * b);
            for (int i = 0; i < b; i++) sb.Append(a);
            return sb.ToString();
        }
    }

    class ObjectTypeHandler : TypeHandler
    {
        protected internal override void DeclareOperations()
        {
            AddOperation<IComparable, object, bool>(TokenType.operator_eq, (a, b) => { return Compare(a, b) == 0; });
            AddOperation<IComparable, object, bool>(TokenType.operator_ne, (a, b) => { return Compare(a, b) != 0; });
            AddOperation<IComparable, object, bool>(TokenType.operator_ge, (a, b) => { return Compare(a, b) >= 0; });
            AddOperation<IComparable, object, bool>(TokenType.operator_gt, (a, b) => { return Compare(a, b) > 0; });
            AddOperation<IComparable, object, bool>(TokenType.operator_le, (a, b) => { return Compare(a, b) <= 0; });
            AddOperation<IComparable, object, bool>(TokenType.operator_lt, (a, b) => { return Compare(a, b) < 0; });

            AddOperation<object, IComparable, bool>(TokenType.operator_eq, (a, b) => { return Compare(a, b) == 0; });
            AddOperation<object, IComparable, bool>(TokenType.operator_ne, (a, b) => { return Compare(a, b) != 0; });
            AddOperation<object, IComparable, bool>(TokenType.operator_ge, (a, b) => { return Compare(a, b) >= 0; });
            AddOperation<object, IComparable, bool>(TokenType.operator_gt, (a, b) => { return Compare(a, b) > 0; });
            AddOperation<object, IComparable, bool>(TokenType.operator_le, (a, b) => { return Compare(a, b) <= 0; });
            AddOperation<object, IComparable, bool>(TokenType.operator_lt, (a, b) => { return Compare(a, b) < 0; });

            AddOperation<object, object, bool>(TokenType.operator_eq, (a, b) => { return Equal(a, b); });
            AddOperation<object, object, bool>(TokenType.operator_ne, (a, b) => { return !Equal(a, b); });
        }

        private int Compare(IComparable a, object b)
        {
            if (a == null) return (b == null ? 0 : -1);
            return a.CompareTo(b);
        }

        private int Compare(object a, IComparable b)
        {
            if (b == null) return (a == null ? 0 : 1);
            return b.CompareTo(a);
        }

        private bool Equal(object a, object b)
        {
            if (a == null) return (b == null);
            return a.Equals(b);
        }
    }
   
}
