using System;
using System.Collections.Generic;
using System.Text;

namespace Eval4.Core
{
    public abstract class Evaluator
    {
        internal List<object> mEnvironmentFunctionsList;
        public bool RaiseVariableNotFoundException;
        protected Dictionary<string, VariableBase> mVariableBag;
        public abstract int GetPrecedence(Token token, bool unary);
        internal Dictionary<TokenType, List<Declaration>> mBinaryDeclarations;
        internal Dictionary<TokenType, List<Declaration>> mUnaryDeclarations;
        internal Dictionary<TypePair, Declaration> mImplicitCasts;
        internal Dictionary<TypePair, Declaration> mExplicitCasts;

        public Evaluator()
        {
            mEnvironmentFunctionsList = new List<object>();
            //CompileTypeHandlers(GetTypeHandlers());
            mVariableBag = new Dictionary<string, VariableBase>((this.Options & EvaluatorOptions.CaseSensitive) != 0 ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase);
            mUnaryDeclarations = new Dictionary<TokenType, List<Declaration>>();
            mBinaryDeclarations = new Dictionary<TokenType, List<Declaration>>();
            mImplicitCasts = new Dictionary<TypePair, Declaration>();
            mExplicitCasts = new Dictionary<TypePair, Declaration>();

            DeclareOperators();
        }

        protected virtual void DeclareOperators()
        {
            int option = 1;
            while (option > 0)
            {
                DeclareOperators((EvaluatorOptions)option);
                option <<= 1;
            }
        }

        protected virtual void DeclareOperators(EvaluatorOptions option)
        {
            switch (option)
            {
                case Evaluator.EvaluatorOptions.BooleanLogic: AddBoolOperators();
                    break;
                case Evaluator.EvaluatorOptions.IntegerValues: AddIntOperators();
                    break;
                case Evaluator.EvaluatorOptions.DoubleValues: AddDoubleOperators();
                    break;
                case Evaluator.EvaluatorOptions.DateTimeValues: AddDateTimeOperators();
                    break;
                case Evaluator.EvaluatorOptions.StringValues: AddStringOperators();
                    break;
                case Evaluator.EvaluatorOptions.ObjectValues: AddObjectOperators();
                    break;
            }
        }

        internal protected enum EvaluatorOptions
        {
            CaseSensitive = 1,
            BooleanLogic = 2,
            IntegerValues = 4,
            DoubleValues = 8,
            DateTimeValues = 16,
            StringValues = 32,
            ObjectValues = 64
        }

        internal class TypePair : IEquatable<TypePair>
        {
            public Type Actual;
            public Type Target;
            public override string ToString()
            {
                return Actual.Name + "=>" + Target.Name;
            }

            public bool Equals(TypePair other)
            {
                return other.Actual == this.Actual && other.Target == this.Target;
            }

            public override int GetHashCode()
            {
                return Actual.GetHashCode() ^ Target.GetHashCode();
            }
        }

        internal class Declaration
        {
            internal TokenType tk;
            internal Delegate dlg;
            internal Type P1;
            internal Type P2;
            internal Type T;
        }

        protected void AddUnaryOperation<P1, T>(TokenType tokenType, Func<P1, T> func)
        {
            ProcessDeclaration(new Declaration() { tk = tokenType, dlg = func, P1 = typeof(P1), T = typeof(T) });
        }

        protected void AddBinaryOperation<P1, P2, T>(TokenType tokenType, Func<P1, P2, T> func)
        {
            ProcessDeclaration(new Declaration() { tk = tokenType, dlg = func, P1 = typeof(P1), P2 = typeof(P2), T = typeof(T) });
        }

        protected void AddImplicitCast<P1, T>(Func<P1, T> func)
        {
            ProcessDeclaration(new Declaration() { tk = TokenType.ImplicitCast, dlg = func, P1 = typeof(P1), T = typeof(T) });
        }

        protected void AddExplicitCast<P1, T>(Func<P1, T> func)
        {
            ProcessDeclaration(new Declaration() { tk = TokenType.ExplicitCast, dlg = func, P1 = typeof(P1), T = typeof(T) });
        }

        public void AddBoolOperators()
        {
            AddBinaryOperation<bool, bool, bool>(TokenType.OperatorAnd, (a, b) => { return a & b; });
            AddBinaryOperation<bool, bool, bool>(TokenType.OperatorOr, (a, b) => { return a | b; });
            AddBinaryOperation<bool, bool, bool>(TokenType.OperatorXor, (a, b) => { return a ^ b; });
            AddBinaryOperation<bool, bool, bool>(TokenType.OperatorAndAlso, (a, b) => { return a && b; });
            AddBinaryOperation<bool, bool, bool>(TokenType.OperatorOrElse, (a, b) => { return a || b; });
            AddBinaryOperation<bool, bool, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
            AddBinaryOperation<bool, bool, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
            AddUnaryOperation<bool, bool>(TokenType.OperatorNot, (a) => { return !a; });

            AddExplicitCast<bool, int>((a) => { return (a ? 1 : 0); });
        }

        public void AddIntOperators()
        {
            AddBinaryOperation<int, int, int>(TokenType.OperatorPlus, (a, b) => { return a + b; });
            AddBinaryOperation<int, int, int>(TokenType.OperatorMinus, (a, b) => { return a - b; });
            AddBinaryOperation<int, int, int>(TokenType.OperatorMultiply, (a, b) => { return a * b; });
            AddBinaryOperation<int, int, int>(TokenType.OperatorDivide, (a, b) => { return a / b; });
            AddBinaryOperation<int, int, int>(TokenType.OperatorModulo, (a, b) => { return a % b; });
            AddBinaryOperation<int, int, int>(TokenType.OperatorAnd, (a, b) => { return a & b; });
            AddBinaryOperation<int, int, int>(TokenType.OperatorOr, (a, b) => { return a | b; });
            AddBinaryOperation<int, int, int>(TokenType.OperatorXor, (a, b) => { return a ^ b; });
            //AddOperation<int, int, int>(TokenType.Operator_andalso, (a, b) => { return a && b; });
            //AddOperation<int, int, int>(TokenType.Operator_orelse, (a, b) => { return a || b; });
            AddBinaryOperation<int, int, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
            AddBinaryOperation<int, int, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
            AddBinaryOperation<int, int, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
            AddBinaryOperation<int, int, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
            AddBinaryOperation<int, int, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
            AddBinaryOperation<int, int, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

            AddUnaryOperation<int, int>(TokenType.OperatorNot, (a) => { return ~a; });
            AddUnaryOperation<int, int>(TokenType.OperatorMinus, (a) => { return -a; });
            AddUnaryOperation<int, int>(TokenType.OperatorPlus, (a) => { return a; });

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

        public void AddDoubleOperators()
        {
            AddBinaryOperation<double, double, double>(TokenType.OperatorPlus, (a, b) => { return a + b; });
            AddBinaryOperation<double, double, double>(TokenType.OperatorMinus, (a, b) => { return a - b; });
            AddBinaryOperation<double, double, double>(TokenType.OperatorMultiply, (a, b) => { return a * b; });
            AddBinaryOperation<double, double, double>(TokenType.OperatorDivide, (a, b) => { return a / b; });
            //AddOperation<double, double, double>(TokenType.Operator_and, (a, b) => { return a & b; });
            //AddOperation<double, double, double>(TokenType.Operator_or, (a, b) => { return a | b; });
            //AddOperation<double, double, double>(TokenType.Operator_xor, (a, b) => { return a ^ b; });
            //AddOperation<double, double, double>(TokenType.Operator_andalso, (a, b) => { return a && b; });
            //AddOperation<double, double, double>(TokenType.Operator_orelse, (a, b) => { return a || b; });
            AddBinaryOperation<double, double, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
            AddBinaryOperation<double, double, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
            AddBinaryOperation<double, double, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
            AddBinaryOperation<double, double, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
            AddBinaryOperation<double, double, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
            AddBinaryOperation<double, double, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

            //AddOperation<double, double>(TokenType.Operator_not, (a) => { return ~a; });
            AddUnaryOperation<double, double>(TokenType.OperatorMinus, (a) => { return -a; });
            AddUnaryOperation<double, double>(TokenType.OperatorPlus, (a) => { return a; });

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

        public void AddDateTimeOperators()
        {
            AddBinaryOperation<DateTime, TimeSpan, DateTime>(TokenType.OperatorPlus, (a, b) => { return a.Add(b); });
            AddBinaryOperation<TimeSpan, DateTime, DateTime>(TokenType.OperatorPlus, (a, b) => { return b.Add(a); });
            AddBinaryOperation<DateTime, TimeSpan, DateTime>(TokenType.OperatorMinus, (a, b) => { return a.Subtract(b); });
            AddBinaryOperation<DateTime, DateTime, TimeSpan>(TokenType.OperatorMinus, (a, b) => { return a.Subtract(b); });
            AddBinaryOperation<DateTime, DateTime, bool>(TokenType.OperatorEQ, (a, b) => { return a.Ticks == b.Ticks; });
            AddBinaryOperation<DateTime, DateTime, bool>(TokenType.OperatorNE, (a, b) => { return a.Ticks != b.Ticks; });
            AddBinaryOperation<DateTime, DateTime, bool>(TokenType.OperatorGE, (a, b) => { return a.Ticks >= b.Ticks; });
            AddBinaryOperation<DateTime, DateTime, bool>(TokenType.OperatorGT, (a, b) => { return a.Ticks > b.Ticks; });
            AddBinaryOperation<DateTime, DateTime, bool>(TokenType.OperatorLE, (a, b) => { return a.Ticks <= b.Ticks; });
            AddBinaryOperation<DateTime, DateTime, bool>(TokenType.OperatorLT, (a, b) => { return a.Ticks < b.Ticks; });

            AddBinaryOperation<TimeSpan, TimeSpan, TimeSpan>(TokenType.OperatorPlus, (a, b) => { return a + b; });
            AddBinaryOperation<TimeSpan, TimeSpan, TimeSpan>(TokenType.OperatorMinus, (a, b) => { return a - b; });
            AddBinaryOperation<TimeSpan, double, TimeSpan>(TokenType.OperatorMultiply, (a, b) => { return TimeSpan.FromTicks((long)(a.Ticks * b)); });
            AddBinaryOperation<double, TimeSpan, TimeSpan>(TokenType.OperatorMultiply, (a, b) => { return TimeSpan.FromTicks((long)(a * b.Ticks)); });
            AddBinaryOperation<TimeSpan, double, TimeSpan>(TokenType.OperatorDivide, (a, b) => { return TimeSpan.FromTicks((long)(a.Ticks / b)); });
            AddBinaryOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
            AddBinaryOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
            AddBinaryOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
            AddBinaryOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
            AddBinaryOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
            AddBinaryOperation<TimeSpan, TimeSpan, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

            AddUnaryOperation<TimeSpan, TimeSpan>(TokenType.OperatorMinus, (a) => { return -a; });
            AddUnaryOperation<TimeSpan, TimeSpan>(TokenType.OperatorPlus, (a) => { return a; });

            AddImplicitCast<int, TimeSpan>((a) => { return TimeSpan.FromDays(a); });
            AddImplicitCast<double, TimeSpan>((a) => { return TimeSpan.FromDays(a); });
        }

        public void AddStringOperators()
        {
            AddBinaryOperation<string, string, string>(TokenType.OperatorPlus, (a, b) => { return a + b; });
            AddBinaryOperation<string, int, string>(TokenType.OperatorMultiply, (a, b) => { return RepeatString(a, b); });
            AddBinaryOperation<int, string, string>(TokenType.OperatorMultiply, (a, b) => { return RepeatString(b, a); });
            AddBinaryOperation<string, string, bool>(TokenType.OperatorEQ, (a, b) => { return CompareString(a, b) == 0; });
            AddBinaryOperation<string, string, bool>(TokenType.OperatorNE, (a, b) => { return CompareString(a, b) != 0; });
            AddBinaryOperation<string, string, bool>(TokenType.OperatorGE, (a, b) => { return CompareString(a, b) >= 0; });
            AddBinaryOperation<string, string, bool>(TokenType.OperatorGT, (a, b) => { return CompareString(a, b) > 0; });
            AddBinaryOperation<string, string, bool>(TokenType.OperatorLE, (a, b) => { return CompareString(a, b) <= 0; });
            AddBinaryOperation<string, string, bool>(TokenType.OperatorLT, (a, b) => { return CompareString(a, b) < 0; });

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

        private static string ToString(object a)
        {
            if (a == null) return null;
            return a.ToString();
        }

        private static int CompareString(string a, string b)
        {
            return String.Compare(a, b);
        }

        private static string RepeatString(string a, int b)
        {
            StringBuilder sb = new StringBuilder(a.Length * b);
            for (int i = 0; i < b; i++) sb.Append(a);
            return sb.ToString();
        }

        public void AddObjectOperators()
        {
            AddBinaryOperation<IComparable, object, bool>(TokenType.OperatorEQ, (a, b) => { return Compare(a, b) == 0; });
            AddBinaryOperation<IComparable, object, bool>(TokenType.OperatorNE, (a, b) => { return Compare(a, b) != 0; });
            AddBinaryOperation<IComparable, object, bool>(TokenType.OperatorGE, (a, b) => { return Compare(a, b) >= 0; });
            AddBinaryOperation<IComparable, object, bool>(TokenType.OperatorGT, (a, b) => { return Compare(a, b) > 0; });
            AddBinaryOperation<IComparable, object, bool>(TokenType.OperatorLE, (a, b) => { return Compare(a, b) <= 0; });
            AddBinaryOperation<IComparable, object, bool>(TokenType.OperatorLT, (a, b) => { return Compare(a, b) < 0; });

            AddBinaryOperation<object, IComparable, bool>(TokenType.OperatorEQ, (a, b) => { return Compare(a, b) == 0; });
            AddBinaryOperation<object, IComparable, bool>(TokenType.OperatorNE, (a, b) => { return Compare(a, b) != 0; });
            AddBinaryOperation<object, IComparable, bool>(TokenType.OperatorGE, (a, b) => { return Compare(a, b) >= 0; });
            AddBinaryOperation<object, IComparable, bool>(TokenType.OperatorGT, (a, b) => { return Compare(a, b) > 0; });
            AddBinaryOperation<object, IComparable, bool>(TokenType.OperatorLE, (a, b) => { return Compare(a, b) <= 0; });
            AddBinaryOperation<object, IComparable, bool>(TokenType.OperatorLT, (a, b) => { return Compare(a, b) < 0; });

            AddBinaryOperation<object, object, bool>(TokenType.OperatorEQ, (a, b) => { return Equal(a, b); });
            AddBinaryOperation<object, object, bool>(TokenType.OperatorNE, (a, b) => { return !Equal(a, b); });
        }

        private static int Compare(IComparable a, object b)
        {
            if (a == null) return (b == null ? 0 : -1);
            return a.CompareTo(b);
        }

        private static int Compare(object a, IComparable b)
        {
            if (b == null) return (a == null ? 0 : 1);
            return b.CompareTo(a);
        }

        private static bool Equal(object a, object b)
        {
            if (a == null) return (b == null);
            return a.Equals(b);
        }

        private void ProcessDeclaration(Declaration decl)
        {
            var tk = decl.tk;
            if (tk == TokenType.ImplicitCast || tk == TokenType.ExplicitCast)
            {
                var typePair = new TypePair() { Actual = decl.P1, Target = decl.T };
                Declaration curDecl;
                var casts = (tk == TokenType.ImplicitCast ? mImplicitCasts : mExplicitCasts);
                if (!casts.TryGetValue(typePair, out curDecl) ||
                    (curDecl.tk == TokenType.ExplicitCast && decl.tk == TokenType.ImplicitCast))
                {
                    casts[typePair] = decl;
                }
            }
            else
            {
                var declarations = (decl.P2 == null ? mUnaryDeclarations : mBinaryDeclarations);
                List<Declaration> tokenDeclarations;
                if (!declarations.TryGetValue(tk, out tokenDeclarations))
                {
                    tokenDeclarations = new List<Declaration>();
                    declarations[tk] = tokenDeclarations;
                }
                tokenDeclarations.Add(decl);
            }
        }

        public abstract Token NewToken();

        public Token NewToken(TokenType type, string value = null)
        {
            var result = NewToken();
            result.Type = type;
            if (value != null) result.ValueString = value;
            return result;
        }

        abstract internal protected EvaluatorOptions Options { get; }

        public void AddEnvironmentFunctions(object o)
        {
            if (o is Type)
            {
                // fine
            }
            else if (o is IHasValue)
            {
                // fine
            }
            else
            {
                var t = typeof(ConstantExpr<>).MakeGenericType(o.GetType());
                o = Activator.CreateInstance(t, o);
            }

            if (!mEnvironmentFunctionsList.Contains(o))
            {
                mEnvironmentFunctionsList.Add(o);
            }
        }

        public void RemoveEnvironmentFunctions(object o)
        {
            if (mEnvironmentFunctionsList.Contains(o))
            {
                mEnvironmentFunctionsList.Remove(o);
            }
        }

        public IHasValue Parse(string formula, bool stringTemplate = false)
        {
            if (formula == null)
                formula = string.Empty;

            IHasValue parsed;
            var dict = (stringTemplate ? mTemplates : mExpressions);
            if (!dict.TryGetValue(formula, out parsed))
            {
                var p = new Parser(this, formula, stringTemplate);
                parsed = p.ParsedExpression;
                dict[formula] = parsed;
                if (dict.Count > 100) dict.Clear(); //  I know this is crude
            }
            return parsed;
        }


        public static string ConvertToString(object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            else if (value == null)
            {
                return string.Empty;
            }
            else if (value is DateTime)
            {
                DateTime d = (DateTime)value;
                if (d.TimeOfDay.TotalMilliseconds > 0)
                {
                    return d.ToString();
                }
                else
                {
                    return d.ToShortDateString();
                }
            }
            else if (value is decimal)
            {
                decimal d = (decimal)value;
                if ((d % 1) != 0)
                {
                    return d.ToString("#,##0.00");
                }
                else
                {
                    return d.ToString("#,##0");
                }
            }
            else if (value is double)
            {
                double d = (double)value;
                if ((d % 1) != 0)
                {
                    return d.ToString("#,##0.00");
                }
                else
                {
                    return d.ToString("#,##0");
                }
            }
            else if (value is object)
            {
                return value.ToString();
            }
            return null;
        }

        Dictionary<string, IHasValue> mExpressions = new Dictionary<string, IHasValue>();
        Dictionary<string, IHasValue> mTemplates = new Dictionary<string, IHasValue>();

        public object Eval(string formula)
        {
            IHasValue parsed = Parse(formula);
            return parsed.ObjectValue;
        }

        public string EvalTemplate(string template)
        {
            IHasValue parsed = ParseTemplate(template);
            return parsed.ObjectValue.ToString();
        }

        public IHasValue ParseTemplate(string template)
        {
            IHasValue parsed = Parse(template, stringTemplate: true);
            return parsed;
        }


        public void SetVariable<T>(string variableName, T variableValue)
        {
            VariableBase variable;
            if (mVariableBag.TryGetValue(variableName, out variable))
            {
                variable.ObjectValue = variableValue;
            }
            else
            {
                variable = new Variable<T>(variableValue, variableName);
                mVariableBag[variableName] = variable;
            }
        }

        public void SetVariableFunctions(string variableName, Type type)
        {
            SetVariable(variableName, new StaticFunctionsWrapper(type));
        }

        public void DeleteVariable(string variableName)
        {
            mVariableBag.Remove(variableName);
        }

        public abstract bool UseParenthesisForArrays { get; }

        public virtual Token CheckKeyword(string keyword)
        {
            return NewToken(TokenType.ValueIdentifier, keyword);
        }

        //internal virtual int GetPrecedence(BaseParser parser, Token tk, bool unary)
        //{
        //    var tt = tk.Type;
        //    if (unary)
        //    {
        //        switch (tt)
        //        {
        //            case TokenType.Operator_minus:
        //                tt = TokenType.unary_minus;
        //                break;
        //            case TokenType.Operator_plus:
        //                tt = TokenType.unary_plus;
        //                break;
        //            case TokenType.Operator_not:
        //                tt = TokenType.unary_not;
        //                break;
        //            case TokenType.Operator_tilde:
        //                tt = TokenType.unary_tilde;
        //                break;
        //        }
        //    }

        //}


        public virtual Token ParseToken(Parser parser)
        {
            switch (parser.mCurChar)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    parser.NextChar();
                    return NewToken(TokenType.Undefined);

                case '\0': //null:
                    return NewToken(TokenType.Eof);

                case '-':
                    parser.NextChar();

                    return NewToken(TokenType.OperatorMinus);
                case '+':
                    parser.NextChar();
                    return NewToken(TokenType.OperatorPlus);
                case '*':
                    parser.NextChar();
                    return NewToken(TokenType.OperatorMultiply);
                case '/':
                    parser.NextChar();
                    return NewToken(TokenType.OperatorDivide);
                case '(':
                    parser.NextChar();
                    return NewToken(TokenType.OpenParenthesis);
                case ')':
                    parser.NextChar();
                    return NewToken(TokenType.CloseParenthesis);
                case '<':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.OperatorLE);
                    }
                    else if (parser.mCurChar == '>')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.OperatorNE);
                    }
                    else
                    {
                        return NewToken(TokenType.OperatorLT);
                    }
                case '>':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.OperatorGE);
                    }
                    else
                    {
                        return NewToken(TokenType.OperatorGT);
                    }
                case ',':
                    parser.NextChar();
                    return NewToken(TokenType.Comma);
                case '.':
                    parser.NextChar();
                    if (parser.mCurChar >= '0' && parser.mCurChar <= '9') return parser.ParseNumber(afterDot: true);
                    else return NewToken(TokenType.Dot);
                case '\'':
                case '"':
                    return parser.ParseString(true);
                case '[':
                    parser.NextChar();
                    return NewToken(TokenType.OpenBracket);
                case ']':
                    parser.NextChar();
                    return NewToken(TokenType.CloseBracket);
                default:
                    if (parser.mCurChar >= '0' && parser.mCurChar <= '9') return parser.ParseNumber();
                    else if (IsIdentifierFirstLetter(parser.mCurChar)) return parser.ParseIdentifierOrKeyword();
                    break;
            }
            throw parser.NewParserException("Unexpected character " + parser.mCurChar);
        }

        public virtual IHasValue ParseLeft(Parser parser, Token token, int precedence)
        {

            IHasValue result = null;
            List<Declaration> declarations;
            if (mUnaryDeclarations.TryGetValue(token.Type, out declarations))
            {
                var tt = token;
                var opPrecedence = GetPrecedence(tt, true);
                parser.NextToken();
                var ValueRight = parser.ParseExpr(null, opPrecedence);

                foreach (var decl in declarations)
                {
                    Declaration cast1;
                    if (CanCast(ValueRight.SystemType, decl.P1, out cast1))
                    {
                        ApplyMethod(ref ValueRight, decl.dlg);
                        return ValueRight;
                    }

                }
            }
            return ParseLeft2(parser, token, ref result);
        }

        private static IHasValue ParseLeft2(Parser parser, Token token, ref IHasValue result)
        {
            switch (token.Type)
            {
                case TokenType.ValueIdentifier:
                    parser.ParseIdentifier(ref result);
                    return result;

                case TokenType.ValueTrue:
                    result = new ConstantExpr<bool>(true);
                    parser.NextToken();
                    return result;

                case TokenType.ValueFalse:
                    result = new ConstantExpr<bool>(false);
                    parser.NextToken();
                    return result;

                case TokenType.ValueString:
                    result = new ConstantExpr<string>(token.ValueString);
                    parser.NextToken();
                    return result;

                case TokenType.ValueNumber:
                    string valueString = token.ValueString;
                    int intValue;
                    double doubleValue;
                    if (int.TryParse(valueString, out intValue))
                    {
                        result = new ConstantExpr<int>(intValue);
                    }
                    else if (double.TryParse(valueString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out doubleValue))
                    {
                        result = new ConstantExpr<double>(doubleValue);
                    }
                    else
                    {
                        throw parser.NewParserException(string.Format("Invalid number {0}", parser.mCurToken.ValueString));
                    }
                    parser.NextToken();
                    return result;

                case TokenType.ValueDate:
                    try
                    {
                        result = new ConstantExpr<DateTime>(DateTime.Parse(parser.mCurToken.ValueString));
                        parser.NextToken();
                        return result;
                    }
                    catch (Exception) // ex)
                    {
                        throw parser.NewParserException(string.Format("Invalid date {0}, it should be #DD/MM/YYYY hh:mm:ss#", parser.mCurToken.ValueString));
                    }

                case TokenType.OpenParenthesis:
                    parser.NextToken();
                    result = parser.ParseExpr(null, 0);
                    if (parser.mCurToken.Type == TokenType.CloseParenthesis)
                    {
                        // good we eat the end parenthesis and continue ...
                        parser.NextToken();
                        return result;
                    }
                    else
                    {
                        throw parser.NewUnexpectedToken("End parenthesis not found");
                    }

                case TokenType.OperatorIf:
                    // first check functions
                    List<IHasValue> parameters = null;
                    // parameters... 
                    parser.NextToken();
                    bool brackets = false;
                    parameters = parser.ParseParameters(ref brackets);
                    var t = typeof(OperatorIfExpr<>).MakeGenericType(parameters[1].SystemType);
                    return (IHasValue)Activator.CreateInstance(t, parameters[0], parameters[1], parameters[2]);
            }
            throw parser.NewUnexpectedToken();
        }


        internal virtual void ParseRight(Parser parser, Token tk, int opPrecedence, IHasValue acc, ref IHasValue valueLeft)
        {
            var tt = tk.Type;
            IHasValue valueRight;
            parser.NextToken();
            valueRight = parser.ParseExpr(valueLeft, opPrecedence);
            var leftType = valueLeft.SystemType;
            var rightType = valueRight.SystemType;
            List<Declaration> declarations;
            if (this.mBinaryDeclarations.TryGetValue(tt, out declarations))
            {
                foreach (var decl in declarations)
                {
                    if (ApplyMethod(ref valueLeft, valueRight, decl.dlg)) return;
                }
            }
            throw parser.NewParserException(string.Format("Cannot find operation {0} {1} {2}", valueLeft.SystemType, tt, valueRight.SystemType));
        }

        protected bool ApplyMethod(ref IHasValue valueLeft, IHasValue valueRight, Delegate dlg)
        {
            Declaration cast1, cast2;
            var parameters = dlg.Method.GetParameters();
            Type p1 = parameters.Length > 0 ? parameters[0].ParameterType : null;
            Type p2 = parameters.Length > 1 ? parameters[1].ParameterType : null;

            if (CanCast(valueLeft.SystemType, p1, out cast1) && CanCast(valueRight.SystemType, p2, out cast2))
            {
                if (cast1 != null)
                {
                    var c1 = typeof(NewTypedExpr<,>).MakeGenericType(cast1.P1, cast1.T);
                    valueLeft = (IHasValue)Activator.CreateInstance(c1, valueLeft, cast1.dlg);
                }
                if (cast2 != null)
                {
                    var c2 = typeof(NewTypedExpr<,>).MakeGenericType(cast2.P1, cast2.T);
                    valueRight = (IHasValue)Activator.CreateInstance(c2, valueRight, cast2.dlg);
                }

                var x = typeof(NewTypedExpr<,,>).MakeGenericType(valueLeft.SystemType, valueRight.SystemType, dlg.Method.ReturnType);
                valueLeft = (IHasValue)Activator.CreateInstance(x, valueLeft, valueRight, dlg);
                return true;
            }
            return false;
        }


        protected bool ApplyMethod(ref IHasValue valueLeft, Delegate dlg)
        {
            Declaration cast1;
            var parameters = dlg.Method.GetParameters();
            Type p1 = parameters.Length > 0 ? parameters[0].ParameterType : null;

            if (CanCast(valueLeft.SystemType, p1, out cast1))
            {
                if (cast1 != null)
                {
                    var c1 = typeof(NewTypedExpr<,>).MakeGenericType(cast1.P1, cast1.T);
                    valueLeft = (IHasValue)Activator.CreateInstance(c1, valueLeft, cast1.dlg);
                }

                var x = typeof(NewTypedExpr<,>).MakeGenericType(valueLeft.SystemType, dlg.Method.ReturnType);
                valueLeft = (IHasValue)Activator.CreateInstance(x, valueLeft, dlg);
                return true;
            }
            return false;
        }

        //protected IHasValue CreateIHasValue(IHasValue ValueLeft, Declaration cast1, Declaration decl)
        //{
        //    if (cast1 != null)
        //    {
        //        var c1 = typeof(NewTypedExpr<,>).MakeGenericType(cast1.P1, cast1.T);
        //        ValueLeft = (IHasValue)Activator.CreateInstance(c1, ValueLeft, cast1.dlg);
        //    }
        //    var x = typeof(NewTypedExpr<,>).MakeGenericType(decl.P1, decl.T);
        //    return (IHasValue)Activator.CreateInstance(x, ValueLeft, decl.dlg);

        //}

        internal class NewTypedExpr<P1, T> : IHasValue<T>
        {
            private IHasValue<P1> mP1;
            private Func<P1, T> mFunc;

            public NewTypedExpr(IHasValue<P1> p1, Func<P1, T> func)
            {
                System.Diagnostics.Debug.Assert(func != null);
                mP1 = p1;
                mFunc = func;
            }


            public T Value
            {
                get { return mFunc(mP1.Value); }
            }

            public object ObjectValue
            {
                get { return mFunc(mP1.Value); }
            }

            public event ValueChangedEventHandler ValueChanged;

            public Type SystemType
            {
                get { return typeof(T); }
            }

            public string ShortName
            {
                get { return "NewTypedExpr"; }
            }

            public IEnumerable<Dependency> Dependencies
            {
                get
                {
                    yield return new Dependency("p1", mP1);
                }
            }
        }

        internal class NewTypedExpr<P1, P2, T> : IHasValue<T>
        {
            private IHasValue<P1> mP1;
            private IHasValue<P2> mP2;
            private Func<P1, P2, T> mFunc;

            public NewTypedExpr(IHasValue<P1> p1, IHasValue<P2> p2, Func<P1, P2, T> func)
            {
                mP1 = p1;
                mP2 = p2;
                mFunc = func;
            }


            public T Value
            {
                get { return mFunc(mP1.Value, mP2.Value); }
            }

            public object ObjectValue
            {
                get { return mFunc(mP1.Value, mP2.Value); }
            }

            public event ValueChangedEventHandler ValueChanged;

            public Type SystemType
            {
                get { return typeof(T); }
            }

            public string ShortName
            {
                get { return "NewTypedExpr"; }
            }

            public IEnumerable<Dependency> Dependencies
            {
                get
                {
                    yield return new Dependency("p1", mP1);
                    yield return new Dependency("p2", mP2);
                }
            }
        }

        private bool CanCast(Type type1, Type type2, out Declaration cast)
        {
            cast = null;
            if (type2 == null) return false;
            if (type1 == type2 || type2.IsAssignableFrom(type1)) return true;
            Declaration decl;
            if (mImplicitCasts.TryGetValue(new TypePair() { Actual = type1, Target = type2 }, out decl))
            {
                cast = decl;
                return true;
            }
            return false;
        }

        internal virtual void CleanUpCharacter(ref char mCurChar)
        {
            // do nothing
        }


        protected virtual bool IsIdentifierFirstLetter(char mCurChar)
        {
            return (mCurChar >= 'a' && mCurChar <= 'z') || (mCurChar >= 'A' && mCurChar <= 'Z') || (mCurChar >= 'A' && mCurChar <= 'Z') || (mCurChar >= 128) || (mCurChar == '_');
        }

        internal protected virtual bool IsIdentifierLetter(char mCurChar)
        {
            return (mCurChar >= '0' && mCurChar <= '9') || (mCurChar >= 'a' && mCurChar <= 'z') || (mCurChar >= 'A' && mCurChar <= 'Z') || (mCurChar >= 'A' && mCurChar <= 'Z') || (mCurChar >= 128) || (mCurChar == '_');
        }

        internal FindVariableEventArgs RaiseFindVariable(string variableName)
        {
            var result = new FindVariableEventArgs(variableName);
            if (FindVariable != null) FindVariable(this, result);
            return result;
        }

        public event EventHandler<FindVariableEventArgs> FindVariable;
    }

    public class FindVariableEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public object Value { get; set; }
        public Type Type { get; set; }

        public FindVariableEventArgs(string name)
        {
            Name = name;
        }

        public bool Handled { get; set; }
    }
    public abstract class Evaluator<T> : Evaluator
    {
        public abstract int GetPrecedence(Token<T> token, bool unary);

        public override int GetPrecedence(Token token, bool unary)
        {
            return this.GetPrecedence((Token<T>)token, unary);
        }

        public Token NewToken(T customTokenType)
        {
            var result = new Token<T>();
            result.Type = TokenType.Custom;
            result.CustomType = customTokenType;
            return result;
        }

        public override Token NewToken()
        {
            return new Token<T>();
        }

        //internal void AddImplicitCast<T1, T2>(Func<Cell, double> func)
        //{
        //    throw new NotImplementedException();
        //}

        //internal void AddExplicitCast<T1, T2>(Func<Cell, double> func)
        //{
        //    throw new NotImplementedException();
        //}
    }
    public class StaticFunctionsWrapper
    {
        public Type type;

        public StaticFunctionsWrapper(Type type)
        {
            // TODO: Complete member initialization
            this.type = type;
        }
    }
}
