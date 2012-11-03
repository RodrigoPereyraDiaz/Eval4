using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Core
{
    internal class Declaration
    {
        public TokenType tk;
        public Delegate dlg;
        public Type P1;
        public Type P2;
        public Type T;
    }


    public abstract class TypeHandler
    {
        internal List<Declaration> mDeclarations = new List<Declaration>();

        protected void AddOperation<P1, T>(TokenType tokenType, Func<P1, T> func)
        {
            mDeclarations.Add(new Declaration() { tk = tokenType, dlg = func, P1 = typeof(P1), T = typeof(T) });
        }

        protected void AddOperation<P1, P2, T>(TokenType tokenType, Func<P1, P2, T> func)
        {
            mDeclarations.Add(new Declaration() { tk = tokenType, dlg = func, P1 = typeof(P1), P2 = typeof(P2), T = typeof(T) });
        }

        protected void AddImplicitCast<P1, T>(Func<P1, T> func)
        {
            mDeclarations.Add(new Declaration() { tk = TokenType.ImplicitCast, dlg = func, P1 = typeof(P1), T = typeof(T) });
        }

        protected void AddExplicitCast<P1, T>(Func<P1, T> func)
        {
            mDeclarations.Add(new Declaration() { tk = TokenType.ExplicitCast, dlg = func, P1 = typeof(P1), T = typeof(T) });
        }
    }

    class BoolTypeHandler : TypeHandler
    {
        public BoolTypeHandler()
        {
            AddOperation<bool, bool, bool>(TokenType.OperatorAnd, (a, b) => { return a & b; });
            AddOperation<bool, bool, bool>(TokenType.OperatorOr, (a, b) => { return a | b; });
            AddOperation<bool, bool, bool>(TokenType.OperatorXor, (a, b) => { return a ^ b; });
            AddOperation<bool, bool, bool>(TokenType.OperatorAndAlso, (a, b) => { return a && b; });
            AddOperation<bool, bool, bool>(TokenType.OperatorOrElse, (a, b) => { return a || b; });
            AddOperation<bool, bool, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
            AddOperation<bool, bool, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
            AddOperation<bool, bool>(TokenType.OperatorNot, (a) => { return !a; });

            AddExplicitCast<bool, int>((a) => { return (a ? 1 : 0); });
        }

    }

    class IntTypeHandler : TypeHandler
    {
        public IntTypeHandler()
        {
            AddOperation<int, int, int>(TokenType.OperatorPlus, (a, b) => { return a + b; });
            AddOperation<int, int, int>(TokenType.OperatorMinus, (a, b) => { return a - b; });
            AddOperation<int, int, int>(TokenType.OperatorMultiply, (a, b) => { return a * b; });
            AddOperation<int, int, int>(TokenType.OperatorDivide, (a, b) => { return a / b; });
            AddOperation<int, int, int>(TokenType.OperatorModulo, (a, b) => { return a % b; });
            AddOperation<int, int, int>(TokenType.OperatorAnd, (a, b) => { return a & b; });
            AddOperation<int, int, int>(TokenType.OperatorOr, (a, b) => { return a | b; });
            AddOperation<int, int, int>(TokenType.OperatorXor, (a, b) => { return a ^ b; });
            //AddOperation<int, int, int>(TokenType.Operator_andalso, (a, b) => { return a && b; });
            //AddOperation<int, int, int>(TokenType.Operator_orelse, (a, b) => { return a || b; });
            AddOperation<int, int, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
            AddOperation<int, int, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
            AddOperation<int, int, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
            AddOperation<int, int, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
            AddOperation<int, int, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
            AddOperation<int, int, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

            AddOperation<int, int>(TokenType.OperatorNot, (a) => { return ~a; });
            AddOperation<int, int>(TokenType.OperatorMinus, (a) => { return -a; });
            AddOperation<int, int>(TokenType.OperatorPlus, (a) => { return a; });

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

    class DoubleTypeHandler : TypeHandler
    {
        public DoubleTypeHandler()
        {
            AddOperation<double, double, double>(TokenType.OperatorPlus, (a, b) => { return a + b; });
            AddOperation<double, double, double>(TokenType.OperatorMinus, (a, b) => { return a - b; });
            AddOperation<double, double, double>(TokenType.OperatorMultiply, (a, b) => { return a * b; });
            AddOperation<double, double, double>(TokenType.OperatorDivide, (a, b) => { return a / b; });
            //AddOperation<double, double, double>(TokenType.Operator_and, (a, b) => { return a & b; });
            //AddOperation<double, double, double>(TokenType.Operator_or, (a, b) => { return a | b; });
            //AddOperation<double, double, double>(TokenType.Operator_xor, (a, b) => { return a ^ b; });
            //AddOperation<double, double, double>(TokenType.Operator_andalso, (a, b) => { return a && b; });
            //AddOperation<double, double, double>(TokenType.Operator_orelse, (a, b) => { return a || b; });
            AddOperation<double, double, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
            AddOperation<double, double, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
            AddOperation<double, double, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
            AddOperation<double, double, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
            AddOperation<double, double, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
            AddOperation<double, double, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

            //AddOperation<double, double>(TokenType.Operator_not, (a) => { return ~a; });
            AddOperation<double, double>(TokenType.OperatorMinus, (a) => { return -a; });
            AddOperation<double, double>(TokenType.OperatorPlus, (a) => { return a; });

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

    class DateTimeTypeHandler : TypeHandler
    {
        public DateTimeTypeHandler()
        {
            AddOperation<DateTime, TimeSpan, DateTime>(TokenType.OperatorPlus, (a, b) => { return a.Add(b); });
            AddOperation<TimeSpan, DateTime, DateTime>(TokenType.OperatorPlus, (a, b) => { return b.Add(a); });
            AddOperation<DateTime, TimeSpan, DateTime>(TokenType.OperatorMinus, (a, b) => { return a.Subtract(b); });
            AddOperation<DateTime, DateTime, TimeSpan>(TokenType.OperatorMinus, (a, b) => { return a.Subtract(b); });
            AddOperation<DateTime, DateTime, bool>(TokenType.OperatorEQ, (a, b) => { return a.Ticks == b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.OperatorNE, (a, b) => { return a.Ticks != b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.OperatorGE, (a, b) => { return a.Ticks >= b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.OperatorGT, (a, b) => { return a.Ticks > b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.OperatorLE, (a, b) => { return a.Ticks <= b.Ticks; });
            AddOperation<DateTime, DateTime, bool>(TokenType.OperatorLT, (a, b) => { return a.Ticks < b.Ticks; });

            AddOperation<TimeSpan, TimeSpan, TimeSpan>(TokenType.OperatorPlus, (a, b) => { return a + b; });
            AddOperation<TimeSpan, TimeSpan, TimeSpan>(TokenType.OperatorMinus, (a, b) => { return a - b; });
            AddOperation<TimeSpan, double, TimeSpan>(TokenType.OperatorMultiply, (a, b) => { return TimeSpan.FromTicks((long)(a.Ticks * b)); });
            AddOperation<double, TimeSpan, TimeSpan>(TokenType.OperatorMultiply, (a, b) => { return TimeSpan.FromTicks((long)(a * b.Ticks)); });
            AddOperation<TimeSpan, double, TimeSpan>(TokenType.OperatorDivide, (a, b) => { return TimeSpan.FromTicks((long)(a.Ticks / b)); });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
            AddOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

            AddOperation<TimeSpan, TimeSpan>(TokenType.OperatorMinus, (a) => { return -a; });
            AddOperation<TimeSpan, TimeSpan>(TokenType.OperatorPlus, (a) => { return a; });

            AddImplicitCast<int, TimeSpan>((a) => { return TimeSpan.FromDays(a); });
            AddImplicitCast<double, TimeSpan>((a) => { return TimeSpan.FromDays(a); });
        }
    }

    class StringTypeHandler : TypeHandler
    {
        public StringTypeHandler()
        {
            AddOperation<string, string, string>(TokenType.OperatorPlus, (a, b) => { return a + b; });
            AddOperation<string, int, string>(TokenType.OperatorMultiply, (a, b) => { return RepeatString(a, b); });
            AddOperation<int, string, string>(TokenType.OperatorMultiply, (a, b) => { return RepeatString(b, a); });
            AddOperation<string, string, bool>(TokenType.OperatorEQ, (a, b) => { return CompareString(a, b) == 0; });
            AddOperation<string, string, bool>(TokenType.OperatorNE, (a, b) => { return CompareString(a, b) != 0; });
            AddOperation<string, string, bool>(TokenType.OperatorGE, (a, b) => { return CompareString(a, b) >= 0; });
            AddOperation<string, string, bool>(TokenType.OperatorGT, (a, b) => { return CompareString(a, b) > 0; });
            AddOperation<string, string, bool>(TokenType.OperatorLE, (a, b) => { return CompareString(a, b) <= 0; });
            AddOperation<string, string, bool>(TokenType.OperatorLT, (a, b) => { return CompareString(a, b) < 0; });

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
        public ObjectTypeHandler()
        {
            AddOperation<IComparable, object, bool>(TokenType.OperatorEQ, (a, b) => { return Compare(a, b) == 0; });
            AddOperation<IComparable, object, bool>(TokenType.OperatorNE, (a, b) => { return Compare(a, b) != 0; });
            AddOperation<IComparable, object, bool>(TokenType.OperatorGE, (a, b) => { return Compare(a, b) >= 0; });
            AddOperation<IComparable, object, bool>(TokenType.OperatorGT, (a, b) => { return Compare(a, b) > 0; });
            AddOperation<IComparable, object, bool>(TokenType.OperatorLE, (a, b) => { return Compare(a, b) <= 0; });
            AddOperation<IComparable, object, bool>(TokenType.OperatorLT, (a, b) => { return Compare(a, b) < 0; });

            AddOperation<object, IComparable, bool>(TokenType.OperatorEQ, (a, b) => { return Compare(a, b) == 0; });
            AddOperation<object, IComparable, bool>(TokenType.OperatorNE, (a, b) => { return Compare(a, b) != 0; });
            AddOperation<object, IComparable, bool>(TokenType.OperatorGE, (a, b) => { return Compare(a, b) >= 0; });
            AddOperation<object, IComparable, bool>(TokenType.OperatorGT, (a, b) => { return Compare(a, b) > 0; });
            AddOperation<object, IComparable, bool>(TokenType.OperatorLE, (a, b) => { return Compare(a, b) <= 0; });
            AddOperation<object, IComparable, bool>(TokenType.OperatorLT, (a, b) => { return Compare(a, b) < 0; });

            AddOperation<object, object, bool>(TokenType.OperatorEQ, (a, b) => { return Equal(a, b); });
            AddOperation<object, object, bool>(TokenType.OperatorNE, (a, b) => { return !Equal(a, b); });
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

