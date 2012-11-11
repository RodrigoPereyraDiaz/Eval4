using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Eval4.Core
{

    public abstract class Evaluator<T> : IEvaluator
    {
        internal List<object> mEnvironmentFunctionsList;
        public bool RaiseVariableNotFoundException;
        private Dictionary<string, IVariable> mVariableBag;
        internal Dictionary<TokenType, List<Declaration>> mBinaryDeclarations;
        internal Dictionary<TokenType, List<Declaration>> mUnaryDeclarations;
        internal Dictionary<TypePair, Declaration> mImplicitCasts;
        internal Dictionary<TypePair, Declaration> mExplicitCasts;
        Dictionary<string, IHasValue> mExpressions = new Dictionary<string, IHasValue>();
        Dictionary<string, IHasValue> mTemplates = new Dictionary<string, IHasValue>();
        protected string mString;
        protected int mLen;
        protected int mPos;
        public char mCurChar;
        public int startpos;
        public Token mCurToken;

        public Evaluator()
        {
            mEnvironmentFunctionsList = new List<object>();
            //CompileTypeHandlers(GetTypeHandlers());
            mVariableBag = new Dictionary<string, IVariable>((this.Options & EvaluatorOptions.CaseSensitive) != 0 ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase);
            mUnaryDeclarations = new Dictionary<TokenType, List<Declaration>>();
            mBinaryDeclarations = new Dictionary<TokenType, List<Declaration>>();
            mImplicitCasts = new Dictionary<TypePair, Declaration>();
            mExplicitCasts = new Dictionary<TypePair, Declaration>();

            DeclareOperators();
        }

        protected virtual void DeclareOperators()
        {
            int options = (int)this.Options;
            int curOption = 1;

            while (curOption > 0)
            {
                if ((options & curOption) != 0) DeclareOperators((EvaluatorOptions)curOption);
                curOption <<= 1;
            }
        }

        protected virtual void DeclareOperators(EvaluatorOptions option)
        {
            switch (option)
            {
                case EvaluatorOptions.BooleanLogic:
                    AddBinaryOperation<bool, bool, bool>(TokenType.OperatorAnd, (a, b) => { return a & b; });
                    AddBinaryOperation<bool, bool, bool>(TokenType.OperatorOr, (a, b) => { return a | b; });
                    AddBinaryOperation<bool, bool, bool>(TokenType.OperatorXor, (a, b) => { return a ^ b; });
                    AddBinaryOperation<bool, bool, bool>(TokenType.OperatorAndAlso, (a, b) => { return a && b; });
                    AddBinaryOperation<bool, bool, bool>(TokenType.OperatorOrElse, (a, b) => { return a || b; });
                    AddBinaryOperation<bool, bool, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
                    AddBinaryOperation<bool, bool, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
                    AddUnaryOperation<bool, bool>(TokenType.OperatorNot, (a) => { return !a; });

                    AddExplicitCast<bool, int>((a) => { return (a ? 1 : 0); });
                    break;

                case EvaluatorOptions.IntegerValues:
                    mHasIntegers = true;
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
                    break;

                case EvaluatorOptions.DoubleValues:
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
                    break;

                case EvaluatorOptions.DateTimeValues:
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
                    break;

                case EvaluatorOptions.StringValues:
                    AddBinaryOperation<string, string, string>(TokenType.OperatorPlus, (a, b) => { return a + b; });
                    AddBinaryOperation<string, int, string>(TokenType.OperatorMultiply, (a, b) => { return RepeatString(a, b); });
                    AddBinaryOperation<int, string, string>(TokenType.OperatorMultiply, (a, b) => { return RepeatString(b, a); });
                    AddBinaryOperation<string, string, bool>(TokenType.OperatorEQ, (a, b) => { return CompareString(a, b) == 0; });
                    AddBinaryOperation<string, string, bool>(TokenType.OperatorNE, (a, b) => { return CompareString(a, b) != 0; });
                    AddBinaryOperation<string, string, bool>(TokenType.OperatorGE, (a, b) => { return CompareString(a, b) >= 0; });
                    AddBinaryOperation<string, string, bool>(TokenType.OperatorGT, (a, b) => { return CompareString(a, b) > 0; });
                    AddBinaryOperation<string, string, bool>(TokenType.OperatorLE, (a, b) => { return CompareString(a, b) <= 0; });
                    AddBinaryOperation<string, string, bool>(TokenType.OperatorLT, (a, b) => { return CompareString(a, b) < 0; });

                    AddImplicitCast<byte, string>((a) => { return ConvertToString(a); });
                    AddImplicitCast<sbyte, string>((a) => { return ConvertToString(a); });
                    AddImplicitCast<short, string>((a) => { return ConvertToString(a); });
                    AddImplicitCast<ushort, string>((a) => { return ConvertToString(a); });
                    AddExplicitCast<uint, string>((a) => { return ConvertToString(a); });
                    AddExplicitCast<int, string>((a) => { return ConvertToString(a); });
                    AddExplicitCast<long, string>((a) => { return ConvertToString(a); });
                    AddExplicitCast<ulong, string>((a) => { return ConvertToString(a); });
                    AddExplicitCast<double, string>((a) => { return ConvertToString(a); });
                    AddExplicitCast<float, string>((a) => { return ConvertToString(a); });
                    AddExplicitCast<decimal, string>((a) => { return ConvertToString(a); });
                    break;

                case EvaluatorOptions.ObjectValues:
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
                    break;
            }
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

        //public Token new Token(TokenType type, object value = null)
        //{
        //    var result = new Token();
        //    result.Type = type;
        //    result.ValueObject = value;
        //    return result;
        //}

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
                parsed = InternalParse(formula, stringTemplate);
                dict[formula] = parsed;
                if (dict.Count > 100) dict.Clear(); //  I know this is crude
            }
            return parsed;
        }

        public virtual string ConvertToString(object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            else if (value == null)
            {
                return "<null>";
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
                return d.ToString("#,##0.000");
            }
            else if (value is double)
            {
                double d = (double)value;
                return d.ToString("#,##0.000");
            }
            else if (value is object)
            {
                return value.ToString();
            }
            return null;
        }

        public object Eval(string formula)
        {
            IHasValue parsed = InternalParse(formula);
            var sw = new System.IO.StringWriter();
            WriteDependencies(sw, "parsed", parsed);
            System.Diagnostics.Trace.WriteLine(sw.ToString());
            return parsed.ObjectValue;
        }

        public string EvalTemplate(string template)
        {
            IHasValue parsed = ParseTemplate(template);
            return parsed.ObjectValue.ToString();
        }

        public IHasValue<string> ParseTemplate(string template)
        {
            IHasValue<string> parsed = (IHasValue<string>)InternalParse(template, sourceIsTextTemplate: true);
            return parsed;
        }


        public void SetVariable<T>(string variableName, T variableValue)
        {
            IVariable variable;
            if (mVariableBag.TryGetValue(variableName, out variable))
            {
                variable.SetValue(variableValue);
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
            return new Token(TokenType.ValueIdentifier, keyword);
        }

        public virtual Token ParseToken()
        {
            switch (mCurChar)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    NextChar();
                    return new Token(TokenType.Undefined);

                case '\0': //null:
                    return new Token(TokenType.Eof);

                case '-':
                    NextChar();

                    return new Token(TokenType.OperatorMinus);
                case '+':
                    NextChar();
                    return new Token(TokenType.OperatorPlus);
                case '*':
                    NextChar();
                    return new Token(TokenType.OperatorMultiply);
                case '/':
                    NextChar();
                    return new Token(TokenType.OperatorDivide);
                case '(':
                    NextChar();
                    return new Token(TokenType.OpenParenthesis);
                case ')':
                    NextChar();
                    return new Token(TokenType.CloseParenthesis);
                case '<':
                    NextChar();
                    if (mCurChar == '=')
                    {
                        NextChar();
                        return new Token(TokenType.OperatorLE);
                    }
                    else if (mCurChar == '>')
                    {
                        NextChar();
                        return new Token(TokenType.OperatorNE);
                    }
                    else
                    {
                        return new Token(TokenType.OperatorLT);
                    }
                case '>':
                    NextChar();
                    if (mCurChar == '=')
                    {
                        NextChar();
                        return new Token(TokenType.OperatorGE);
                    }
                    else
                    {
                        return new Token(TokenType.OperatorGT);
                    }
                case ',':
                    NextChar();
                    return new Token(TokenType.Comma);
                case '.':
                    NextChar();
                    if (mCurChar >= '0' && mCurChar <= '9') return ParseNumber(afterDot: true);
                    else return new Token(TokenType.Dot);
                case '\'':
                case '"':
                    return ParseString(true);
                case '[':
                    NextChar();
                    return new Token(TokenType.OpenBracket);
                case ']':
                    NextChar();
                    return new Token(TokenType.CloseBracket);
                case ';':
                    NextChar();
                    return new Token(TokenType.SemiColon);
                default:
                    if (mCurChar >= '0' && mCurChar <= '9') return ParseNumber();
                    else if (IsIdentifierFirstLetter(mCurChar)) return ParseIdentifierOrKeyword();
                    break;
            }
            return new Token(TokenType.UnrecognisedCharacter, mCurChar.ToString());
        }

        public virtual IHasValue ParseUnaryExpression(Token token, int precedence)
        {
            IHasValue result = null;
            List<Declaration> declarations;
            if (mUnaryDeclarations.TryGetValue(token.Type, out declarations))
            {
                var tt = token;
                var opPrecedence = GetPrecedence(tt, true);
                NextToken();
                var ValueRight = ParseExpr(null, opPrecedence);

                foreach (var decl in declarations)
                {
                    Declaration cast1;
                    if (CanCast(ValueRight.ValueType, decl.P1, out cast1))
                    {
                        EmitDelegateExpr(ref ValueRight, decl.dlg);
                        return ValueRight;
                    }

                }
            }
            return ParseLeft(token, ref result);
        }

        protected abstract int GetPrecedence(Token tt, bool unary);

        protected virtual IHasValue ParseLeft(Token token, ref IHasValue result)
        {
            switch (token.Type)
            {
                case TokenType.ValueIdentifier:
                    ParseIdentifier(ref result);
                    return result;

                case TokenType.ValueTrue:
                    result = new ConstantExpr<bool>(true);
                    NextToken();
                    return result;

                case TokenType.ValueFalse:
                    result = new ConstantExpr<bool>(false);
                    NextToken();
                    return result;

                case TokenType.ValueString:
                    result = new ConstantExpr<string>(token.ValueString);
                    NextToken();
                    return result;

                case TokenType.ValueInteger:
                    int intValue;
                    if (int.TryParse(token.ValueString, out intValue))
                    {
                        if (mHasIntegers)
                            result = new ConstantExpr<int>(intValue);
                        else
                            result = new ConstantExpr<double>(intValue);
                    }
                    else
                    {
                        return NewSyntaxError(string.Format("Invalid number {0}", token.ValueString));
                    }
                    NextToken();
                    return result;

                case TokenType.ValueDecimal:
                    double doubleValue;
                    if (double.TryParse(token.ValueString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out doubleValue))
                    {
                        result = new ConstantExpr<double>(doubleValue);
                        NextToken();
                        return result;
                    }
                    else
                    {
                        return NewSyntaxError(string.Format("Invalid number {0}", token.ValueString));
                    }

                case TokenType.ValueDate:
                    DateTime dateTimeValue;

                    if (DateTime.TryParse(token.ValueString, out dateTimeValue))
                    {
                        result = new ConstantExpr<DateTime>(dateTimeValue);
                        NextToken();
                        return result;
                    }
                    else
                    {
                        return NewSyntaxError(string.Format("Invalid date {0}, it should be #DD/MM/YYYY hh:mm:ss#", token.ValueString));
                    }

                case TokenType.OpenParenthesis:
                    NextToken();
                    result = ParseExpr(null, 1);
                    if (mCurToken.Type == TokenType.CloseParenthesis)
                    {
                        // good we eat the end parenthesis and continue ...
                        NextToken();
                        return result;
                    }
                    else
                    {
                        return NewUnexpectedToken("End parenthesis not found");
                    }

                case TokenType.OperatorIf:
                    // first check functions
                    List<IHasValue> parameters = null;
                    // parameters... 
                    NextToken();
                    bool brackets = false;
                    IHasValue error = null;
                    if (!ParseParameters(ref brackets, ref parameters, ref error)) return error;
                    var t = typeof(OperatorIfExpr<>).MakeGenericType(parameters[1].ValueType);
                    return (IHasValue)Activator.CreateInstance(t, parameters[0], parameters[1], parameters[2]);
            }
            return NewUnexpectedToken();
        }

        protected virtual void ParseRight(Token tk, int opPrecedence, IHasValue acc, ref IHasValue valueLeft)
        {
            var tt = tk.Type;
            if (tt == TokenType.Dot)
            {
                NextToken();
                // is this the only parseright that don't need a value right?
                ParseIdentifier(ref valueLeft);
            }
            else
            {
                IHasValue valueRight;
                NextToken();
                valueRight = ParseExpr(valueLeft, opPrecedence);
                var leftType = valueLeft.ValueType;
                var rightType = valueRight.ValueType;
                List<Declaration> declarations;
                if (this.mBinaryDeclarations.TryGetValue(tt, out declarations))
                {
                    foreach (var decl in declarations)
                    {
                        if (EmitDelegateExpr(ref valueLeft, valueRight, decl.dlg)) return;
                    }
                }
                valueLeft = NewSyntaxError(string.Format("Cannot find operation {0} {1} {2}", valueLeft.ValueType, tt, valueRight.ValueType));
            }
        }

        protected bool EmitDelegateExpr(ref IHasValue valueLeft, IHasValue valueRight, Delegate dlg)
        {
            Declaration cast1, cast2;
            var parameters = dlg.Method.GetParameters();
            Type p1 = parameters.Length > 0 ? parameters[0].ParameterType : null;
            Type p2 = parameters.Length > 1 ? parameters[1].ParameterType : null;

            if (CanCast(valueLeft.ValueType, p1, out cast1) && CanCast(valueRight.ValueType, p2, out cast2))
            {
                if (cast1 != null)
                {
                    var c1 = typeof(DelegateExpr<,>).MakeGenericType(cast1.P1, cast1.T);
                    valueLeft = (IHasValue)Activator.CreateInstance(c1, valueLeft, cast1.dlg);
                }
                if (cast2 != null)
                {
                    var c2 = typeof(DelegateExpr<,>).MakeGenericType(cast2.P1, cast2.T);
                    valueRight = (IHasValue)Activator.CreateInstance(c2, valueRight, cast2.dlg);
                }

                var x = typeof(DelegateExpr<,,>).MakeGenericType(valueLeft.ValueType, valueRight.ValueType, dlg.Method.ReturnType);
                valueLeft = (IHasValue)Activator.CreateInstance(x, valueLeft, valueRight, dlg);
                return true;
            }
            return false;
        }

        protected bool EmitDelegateExpr(ref IHasValue valueLeft, Delegate dlg)
        {
            Declaration cast1;
            var parameters = dlg.Method.GetParameters();
            Type p1 = parameters.Length > 0 ? parameters[0].ParameterType : null;

            if (CanCast(valueLeft.ValueType, p1, out cast1))
            {
                if (cast1 != null)
                {
                    var c1 = typeof(DelegateExpr<,>).MakeGenericType(cast1.P1, cast1.T);
                    valueLeft = (IHasValue)Activator.CreateInstance(c1, valueLeft, cast1.dlg);
                }

                var x = typeof(DelegateExpr<,>).MakeGenericType(valueLeft.ValueType, dlg.Method.ReturnType);
                valueLeft = (IHasValue)Activator.CreateInstance(x, valueLeft, dlg);
                return true;
            }
            return false;
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

        private bool mHasIntegers;


        internal IHasValue<SyntaxError> NewSyntaxError(string msg, Exception ex = null)
        {
            msg += " " + " at position " + startpos;
            if ((ex != null))
            {
                msg += ". " + ex.Message;
            }
            return new ConstantExpr<SyntaxError>(new SyntaxError(msg, this.mString, this.mPos));
        }

        internal IHasValue<SyntaxError> NewUnexpectedToken(string msg = null)
        {
            if (String.IsNullOrEmpty(msg))
            {
                msg = "";
            }
            else
            {
                msg += "; ";
            }
            if (string.IsNullOrEmpty(mCurToken.ValueString))
            {
                //if (mCurChar == '\0') throw NewParserException(msg + "Unexpected end of formula.");
                return NewSyntaxError(msg + "Unexpected " + mCurToken.Type);
            }
            else
            {
                return NewSyntaxError(msg + "Unexpected " + mCurToken.Type + " \"" + mCurToken.ValueString + "\"");
            }
        }

        public void NextToken()
        {
            do
            {
                startpos = mPos;
                mCurToken = ParseToken();
                if (mCurToken.Type != TokenType.Undefined) return;
            } while (true);
        }

        internal void NextChar()
        {
            if (mPos < mLen)
            {
                mCurChar = mString[mPos];
                CleanUpCharacter(ref mCurChar);

                mPos += 1;
            }
            else
            {
                mCurChar = '\0';
            }
        }

        internal Token ParseNumber(bool afterDot = false)
        {
            var sb = new StringBuilder();
            if (!afterDot)
            {
                while (mCurChar >= '0' && mCurChar <= '9')
                {
                    sb.Append(mCurChar);
                    NextChar();
                }
                if (mCurChar == '.')
                {
                    afterDot = true;
                    NextChar();
                }
            }
            if (afterDot)
            {
                sb.Append('.');
                while (mCurChar >= '0' && mCurChar <= '9')
                {
                    sb.Append(mCurChar);
                    NextChar();
                }
                return new Token(TokenType.ValueDecimal, sb.ToString());
            }
            else
            {
                return new Token(TokenType.ValueInteger, sb.ToString());
            }
        }

        internal Token ParseIdentifierOrKeyword()
        {
            var sb = new StringBuilder();
            // we eat at least one character
            sb.Append(mCurChar);
            NextChar();

            while (IsIdentifierLetter(mCurChar))
            {
                sb.Append(mCurChar);
                NextChar();
            }
            mCurToken = CheckKeyword(sb.ToString());
            return mCurToken;
        }

        internal Token ParseString(bool InQuote)
        {
            var sb = new StringBuilder();
            char OriginalChar = '\0';
            if (InQuote)
            {
                OriginalChar = mCurChar;
                NextChar();
            }
            List<object> bits = new List<object>();

            while (mCurChar != '\0')
            {
                if (InQuote && mCurChar == OriginalChar)
                {
                    NextChar();
                    if (mCurChar == OriginalChar)
                    {
                        sb.Append(mCurChar);
                    }
                    else
                    {
                        //End of String
                        return new Token(TokenType.ValueString, sb.ToString());
                    }
                }
                else if (mCurChar == '%')
                {
                    NextChar();
                    if (mCurChar == '[')
                    {
                        NextChar();
                        System.Text.StringBuilder SaveValue = sb;
                        int SaveStartPos = startpos;
                        sb = new System.Text.StringBuilder();
                        this.NextToken();
                        // restart the tokenizer for the subExpr
                        object subExpr = null;
                        try
                        {
                            // subExpr = mParseExpr(0, ePriority.none)
                            if (subExpr == null)
                            {
                                sb.Append("<nothing>");
                            }
                            else
                            {
                                sb.Append(ConvertToString(subExpr));
                            }
                        }
                        catch (Exception ex)
                        {
                            // XML don't like < and >
                            sb.Append("[Error " + ex.Message + "]");
                        }
                        SaveValue.Append(sb.ToString());
                        sb = SaveValue;
                        startpos = SaveStartPos;
                    }
                    else
                    {
                        sb.Append('%');
                    }
                }
                else
                {
                    sb.Append(mCurChar);
                    NextChar();
                }
            }

            if (InQuote)
            {
                return new Token(TokenType.SyntaxError, "Incomplete string, missing " + OriginalChar + "; String started");
            }
            if (sb.Length > 0 && bits.Count > 0)
            {
                bits.Add(sb.ToString());
                sb.Length = 0;
                foreach (object o in bits)
                {
                    if (o is string) sb.Append(o as string);
                    else sb.Append((o as IHasValue).ObjectValue);
                }
            }
            return new Token(TokenType.ValueString, sb.ToString());
        }

        internal bool Expect(TokenType tokenType, string msg, ref IHasValue error)
        {
            if (mCurToken.Type == tokenType)
            {
                NextToken();
                return true;
            }
            else
            {
                error = NewUnexpectedToken(msg);
                return false;
            }
        }

        private IHasValue InternalParse(string source, bool sourceIsTextTemplate = false)
        {
            mString = source;
            mLen = source.Length;
            mPos = 0;
            // start the machine
            NextChar();
            NextToken();
            IHasValue res;
            if (sourceIsTextTemplate) res = ParseTemplate();
            else res = ParseExpr(null, -1);
            if (mCurToken.Type == TokenType.Eof)
            {
                if (res == null)
                    res = new ConstantExpr<string>(string.Empty);
                return res;
            }
            else
            {
                return NewUnexpectedToken();
            }
        }

        private IHasValue ParseTemplate()
        {
            var token = ParseString(false);
            return (IHasValue)token.Value;
        }

        internal IHasValue ParseExpr(IHasValue acc, int precedence)
        {
            IHasValue valueLeft = null;
            valueLeft = ParseUnaryExpression(mCurToken, precedence);
            if (valueLeft == null)
            {
                return NewUnexpectedToken("No Expression found");
            }
            while (typeof(IHasValue).IsAssignableFrom(valueLeft.ValueType))
            {
                valueLeft = (IHasValue)valueLeft.ObjectValue;
            }
            return ParseRight(acc, precedence, valueLeft);
        }

        protected IHasValue ParseRight(IHasValue acc, int precedence, IHasValue valueLeft)
        {
            while (true)
            {
                if (mCurToken.Type == TokenType.Eof || valueLeft.ValueType == typeof(SyntaxError)) return valueLeft;

                int opPrecedence = GetPrecedence(mCurToken, unary: false);
                if (precedence >= opPrecedence)
                {
                    // if on we have twice the same operator precedence it is more natural to calculate the left operator first
                    // ie 1+2+3-4 will be calculated ((1+2)+3)-4
                    return valueLeft;
                }
                else
                {
                    ParseRight(mCurToken, opPrecedence, acc, ref valueLeft);
                }
            }
        }

        protected bool EmitCallFunction(ref IHasValue valueLeft, string funcName, List<IHasValue> parameters, EvalMemberType callType, out IHasValue error)
        {
            IHasValue newExpr = null;

            if (valueLeft == null)
            {
                for (int i = mEnvironmentFunctionsList.Count - 1; i >= 0; i--)
                {
                    var environmentFunctions = mEnvironmentFunctionsList[i];
                    if (environmentFunctions is Type)
                    {
                        newExpr = GetMember(null, (Type)environmentFunctions, funcName, parameters, callType);
                    }
                    else if (environmentFunctions is IHasValue)
                    {
                        var hasValue = environmentFunctions as IHasValue;
                        newExpr = GetMember(hasValue, hasValue.ValueType, funcName, parameters, callType);
                    }
                    else
                    {
                        error = NewSyntaxError("Invalid Environment functions.");
                        return false;
                    }
                    if (newExpr != null) break;
                }
                if (newExpr == null && (parameters == null || parameters.Count == 0))
                {
                    IVariable variable;
                    if (mVariableBag.TryGetValue(funcName, out variable))
                    {
                        var t = typeof(GetVariableFromBag<>).MakeGenericType(variable.ValueType);
                        newExpr = (IHasValue)Activator.CreateInstance(t, this, funcName);
                    }
                    else
                    {
                        var value = LastChanceFindVariable(funcName);
                        if (value != null)
                        {
                            if (value is IHasValue) newExpr = (IHasValue)value;
                            else
                            {
                                var t = typeof(Variable<>).MakeGenericType(value.GetType());
                                newExpr = (IHasValue)Activator.CreateInstance(t, value, funcName);
                            }
                        }
                    }
                }
            }
            else if (valueLeft.ValueType == typeof(StaticFunctionsWrapper))
            {
                newExpr = GetMember(null, (valueLeft.ObjectValue as StaticFunctionsWrapper).type, funcName, parameters, callType);
            }
            else
            {
                newExpr = GetMember(valueLeft, valueLeft.ValueType, funcName, parameters, callType);
            }
            if ((newExpr != null))
            {
                valueLeft = newExpr;
                error = null;
                return true;
            }
            else
            {
                error = NewSyntaxError("No Variable or public method '" + funcName + "' was not found.");
                return false;
            }
        }

        protected virtual object LastChanceFindVariable(string funcName)
        {
            return null;
        }

        protected IHasValue GetMember(IHasValue @base, Type baseType, string funcName, List<IHasValue> parameters, EvalMemberType CallType)
        {
            MemberInfo mi = null;
            Type resultType;
            object[] casts;
            if (GetMemberInfo(baseType, isStatic: true, isInstance: @base != null, func: funcName, parameters: parameters, mi: out mi, resultType: out resultType, casts: out casts))
            {

                switch (mi.MemberType)
                {
                    case MemberTypes.Field:
                        if ((CallType & EvalMemberType.Field) == 0)
                            return NewSyntaxError("Unexpected Field");
                        resultType = (mi as FieldInfo).FieldType;
                        break;
                    case MemberTypes.Method:
                        if ((CallType & EvalMemberType.Method) == 0)
                            return NewSyntaxError("Unexpected Method");
                        resultType = (mi as MethodInfo).ReturnType;
                        break;
                    case MemberTypes.Property:
                        if ((CallType & EvalMemberType.Property) == 0)
                            return NewSyntaxError("Unexpected Property");
                        resultType = (mi as PropertyInfo).PropertyType;
                        break;
                    default:
                        return NewUnexpectedToken(mi.MemberType.ToString() + " members are not supported");
                }
                var t = typeof(CallMethodExpr<>).MakeGenericType(resultType);
                return (IHasValue)Activator.CreateInstance(t, @base, mi, parameters, casts);
            }
            //if (@base is IVariableBag && (parameters == null || parameters.Count == 0))
            //{
            //    IHasValue val = ((IVariableBag)@base).GetVariable(funcName);
            //    if ((val != null))
            //    {
            //        return val; // new GetVariableExpr(val);
            //    }
            //}
            return null;
        }

        protected bool GetMemberInfo(Type objType, bool isStatic, bool isInstance, string func, List<IHasValue> parameters, out MemberInfo mi, out Type resultType, out object[] casts)
        {
            BindingFlags bindingAttr = default(BindingFlags);
            bindingAttr = BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public | BindingFlags.InvokeMethod;
            if (isStatic) bindingAttr |= BindingFlags.Static;
            if (isInstance) bindingAttr |= BindingFlags.Instance;
            if ((this.Options & EvaluatorOptions.CaseSensitive) == 0)
            {
                bindingAttr = bindingAttr | BindingFlags.IgnoreCase;
            }
            MemberInfo[] mis = null;

            if (func == null)
            {
                mis = objType.GetDefaultMembers();
            }
            else
            {
                mis = objType.GetMember(func, bindingAttr);
            }


            // There is a bit of cooking here...
            // lets find the most acceptable Member
            int score = 0;
            int BestScore = 0;
            MemberInfo BestMember = null;
            object[] bestCasts = null;
            ParameterInfo[] plist = null;
            int idx = 0;

            mi = null;
            for (int i = 0; i <= mis.Length - 1; i++)
            {
                mi = mis[i];

                if (mi is MethodInfo)
                {
                    plist = ((MethodInfo)mi).GetParameters();
                }
                else if (mi is PropertyInfo)
                {
                    plist = ((PropertyInfo)mi).GetIndexParameters();
                }
                else if (mi is FieldInfo)
                {
                    plist = null;
                }
                score = 10;
                // by default
                idx = 0;
                if (plist == null)
                    plist = new ParameterInfo[] { };
                if (parameters == null)
                    parameters = new List<IHasValue>();

                ParameterInfo pi = null;
                var castList = new List<object>();

                for (int index = 0; index < plist.Length; index++)
                {
                    pi = plist[index];
                    if (idx < parameters.Count)
                    {
                        Delegate castDlg;
                        var thisScore = ParamCompatibility(parameters[idx], pi.ParameterType, out castDlg);
                        if (thisScore == 0
                            && pi.ParameterType.IsArray
                            && index == plist.Length - 1)
                        {
                            var elementType = pi.ParameterType.GetElementType();
                            thisScore = 10;
                            List<Delegate> entryCasts = new List<Delegate>();
                            while (idx < parameters.Count)
                            {
                                Delegate entryCast;
                                var entryScore = ParamCompatibility(parameters[idx], elementType, out entryCast);
                                entryCasts.Add(entryCast);
                                if (entryScore < thisScore) thisScore = entryScore;
                                idx++;
                            }
                            castList.Add(entryCasts.ToArray());
                        }
                        else castList.Add(castDlg);
                        score += thisScore;
                    }
                    else if (pi.IsOptional)
                    {
                        score += 10;
                    }
                    else
                    {
                        // unknown parameter
                        score = 0;
                    }
                    idx += 1;
                }
                if (score > BestScore)
                {
                    BestScore = score;
                    BestMember = mi;
                    bestCasts = castList.ToArray();
                }
            }
            mi = BestMember;
            casts = bestCasts;
            if (mi is MethodInfo) resultType = (mi as MethodInfo).ReturnType;
            else if (mi is PropertyInfo) resultType = (mi as PropertyInfo).PropertyType;
            else if (mi is FieldInfo) resultType = (mi as FieldInfo).FieldType;
            else resultType = null;
            return resultType != null;
        }

        protected int ParamCompatibility(IHasValue actual, Type expectedType, out Delegate castDlg)
        {
            castDlg = null;
            // This function returns a score 1 to 10 to this question
            // Can this Value fit into this type ?
            var actualType = actual.ValueType;
            if (actualType == expectedType || expectedType.IsAssignableFrom(actualType)) return 10;
            Declaration cast;
            if (mImplicitCasts.TryGetValue(new TypePair() { Actual = actualType, Target = expectedType }, out cast))
            {
                castDlg = cast.dlg;
                return 8;
            }
            if (expectedType == typeof(object)) return 6;
            if (expectedType == typeof(string))
            {
                castDlg = new Func<object, string>((o) => o.ToString());
                return 4;
            }
            if (mExplicitCasts.TryGetValue(new TypePair() { Actual = actualType, Target = expectedType }, out cast))
            {
                castDlg = cast.dlg;
                return 2;
            }
            return 0;
        }

        internal void ParseIdentifier(ref IHasValue valueLeft)
        {
            IHasValue error;
            // first check functions
            List<IHasValue> parameters = null;
            // parameters... 
            //Dim types As New ArrayList
            string func = mCurToken.ValueString;
            NextToken();
            bool isBrackets = false;
            Type resultType;
            if (!ParseParameters(ref isBrackets, ref parameters, ref valueLeft)) return;
            if ((parameters != null))
            {
                List<IHasValue> EmptyParameters = new List<IHasValue>();
                bool ParamsNotUsed = false;
                if (UseParenthesisForArrays)
                {
                    // in vb we don't know if it is array or not as we have only parenthesis
                    // so we try with parameters first
                    if (!EmitCallFunction(ref valueLeft, func, parameters, EvalMemberType.All, out error))
                    {
                        // and if not found we try as array or default member
                        if (!EmitCallFunction(ref valueLeft, func, EmptyParameters, EvalMemberType.All, out error))
                        {
                            valueLeft = error;
                            return;
                        }
                        ParamsNotUsed = true;
                    }
                }
                else
                {
                    if (isBrackets)
                    {
                        if (!EmitCallFunction(ref valueLeft, func, parameters, EvalMemberType.Property, out error))
                        {
                            if (!EmitCallFunction(ref valueLeft, func, EmptyParameters, EvalMemberType.All, out error))
                            {
                                valueLeft = error;
                                return;
                            }
                            ParamsNotUsed = true;
                        }
                    }
                    else
                    {
                        if (!EmitCallFunction(ref valueLeft, func, parameters, EvalMemberType.Field | EvalMemberType.Method, out error))
                        {
                            if (!EmitCallFunction(ref valueLeft, func, EmptyParameters, EvalMemberType.All, out error))
                            {
                                valueLeft = error;
                                return;
                            }
                            ParamsNotUsed = true;
                        }
                    }
                }
                // we found a function without parameters 
                // so our parameters must be default property or an array
                Type t = valueLeft.ValueType;
                if (ParamsNotUsed)
                {
                    if (t.IsArray)
                    {
                        if (parameters.Count == t.GetArrayRank())
                        {
                            var t2 = typeof(GetArrayEntryExpr<>).MakeGenericType(t.GetElementType());

                            valueLeft = (IHasValue)Activator.CreateInstance(t2, valueLeft, parameters);
                        }
                        else
                        {
                            valueLeft = NewSyntaxError("This array has " + t.GetArrayRank() + " dimensions");
                            return;
                        }
                    }
                    else
                    {
                        MemberInfo mi;
                        object[] casts;
                        if (GetMemberInfo(t, isStatic: true, isInstance: true, func: null, parameters: parameters, mi: out mi, resultType: out resultType, casts: out casts))
                        {
                            var t3 = typeof(CallMethodExpr<>).MakeGenericType(resultType);
                            valueLeft = (IHasValue)Activator.CreateInstance(t3, this, valueLeft, mi, parameters, casts);
                        }
                        else
                        {
                            valueLeft = NewSyntaxError("Parameters not supported here");
                            return;
                        }
                    }
                }
            }
            else
            {
                if (!EmitCallFunction(ref valueLeft, func, parameters, EvalMemberType.All, out error))
                {
                    valueLeft = error;
                    return;
                }
            }
        }

        internal bool ParseParameters(ref bool brackets, ref List<IHasValue> parameters, ref IHasValue error)
        {
            IHasValue valueleft = null;
            TokenType lClosing = default(TokenType);

            if (mCurToken.Type == TokenType.OpenParenthesis
                || (mCurToken.Type == TokenType.OpenBracket && !UseParenthesisForArrays))
            {
                switch (mCurToken.Type)
                {
                    case TokenType.OpenBracket:
                        lClosing = TokenType.CloseBracket;
                        brackets = true;
                        break;
                    case TokenType.OpenParenthesis:
                        lClosing = TokenType.CloseParenthesis;
                        break;
                }
                parameters = new List<IHasValue>();
                NextToken();  //eat the parenthesis
                do
                {
                    if (mCurToken.Type == lClosing)
                    {
                        // good we eat the end parenthesis and continue ...
                        NextToken();
                        return true;
                    }
                    valueleft = ParseExpr(null, 1);
                    parameters.Add(valueleft);

                    if (mCurToken.Type == lClosing)
                    {
                        // good we eat the end parenthesis and continue ...
                        NextToken();
                        return true;
                    }
                    else if (mCurToken.Type == TokenType.Comma)
                    {
                        NextToken();
                    }
                    else
                    {
                        error = NewUnexpectedToken(lClosing.ToString() + " not found");
                        return false;
                    }
                } while (true);
            }
            return true;
        }

        public static void WriteDependencies(System.IO.TextWriter tw, string name, Eval4.Core.IHasValue expr, string indent = null)
        {
            if (indent == null) indent = string.Empty;
            if (expr == null)
            {
                tw.WriteLine("{0} {1} type {2} ({3})", indent, name, "null","object");
                return;
            }
            else tw.WriteLine("{0} {1} type {2} ({3})", indent, name, expr.ShortName, expr.ValueType);
            int cpt = 0;
            foreach (var d in expr.Dependencies)
            {
                cpt++;
                WriteDependencies(tw, d.Name, d.Expr, indent + "  |");
            }
            if (cpt == 0)
            {
                tw.WriteLine("{0}  +--> {2}", indent, name, expr.ObjectValue);
            }
            else
            {
                tw.WriteLine("{0}  +--> {2} ({1})", indent, name, expr.ObjectValue);
            }
        }

        public Variable<T> GetVariable<T>(string variableName)
        {
            IVariable value;
            if (mVariableBag.TryGetValue(variableName, out value))
            {
                return value as Variable<T>;
            }
            return null;
        }


        IHasValue IEvaluator.Parse(string formula)
        {
            return this.Parse(formula);
        }



        void IEvaluator.SetVariable<T>(string variableName, T variableValue)
        {
            this.SetVariable(variableName, variableValue);
        }

        IHasValue<string> IEvaluator.ParseTemplate(string template)
        {
            return this.ParseTemplate(template);
        }

        object IEvaluator.Eval(string formula)
        {
            return this.Eval(formula);
        }

        string IEvaluator.EvalTemplate(string formula)
        {
            throw new NotImplementedException();
        }

        public class Token
        {
            public TokenType Type { get; set; }
            public T CustomType { get; set; }
            public Object Value { get; set; }

            public Token(TokenType tokenType, object value = null)
            {
                this.Type = tokenType;
                this.Value = value;
            }

            public Token(T customType, object value = null)
            {
                this.Type = TokenType.Custom;
                this.CustomType = customType;
                this.Value = value;
            }

            public String ValueString
            {
                get { return Value == null ? null : Value.ToString(); }
            }

            public override string ToString()
            {
                return (Type == TokenType.Custom ? CustomType.ToString() : Type.ToString())
                    + " " + (String.IsNullOrEmpty(ValueString) ? Value : ValueString);
            }

        }

    }

    public enum EvaluatorOptions
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

    class StaticFunctionsWrapper
    {
        public Type type;

        public StaticFunctionsWrapper(Type type)
        {
            // TODO: Complete member initialization
            this.type = type;
        }
    }

    [Flags()]
    public enum EvalMemberType
    {
        Field = 1,
        Method = 2,
        Property = 4,
        All = 7
    }

}
