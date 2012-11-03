using System;
using System.Collections.Generic;

namespace Eval4.Core
{
    public abstract class Evaluator
    {
        internal List<object> mEnvironmentFunctionsList;
        public bool RaiseVariableNotFoundException;
        protected VariableBag mVariableBag;
        public abstract int GetPrecedence(Token token, bool unary);
        internal Dictionary<TokenType, List<Declaration>> mDeclarations;
        internal Dictionary<TypePair, Declaration> mCasts;

        internal class TypePair
        {
            public Type Actual;
            public Type Target;
        }
        public Evaluator()
        {
            mEnvironmentFunctionsList = new List<object>();
            mVariableBag = new VariableBag(this.IsCaseSensitive);
            mEnvironmentFunctionsList.Add(mVariableBag);
            CompileTypeHandlers(GetTypeHandlers());
        }

        protected virtual List<TypeHandler> GetTypeHandlers()
        {
            var typeHandlers = new List<TypeHandler>();
            typeHandlers.Add(new IntTypeHandler());
            typeHandlers.Add(new DateTimeTypeHandler());
            typeHandlers.Add(new DoubleTypeHandler());
            typeHandlers.Add(new StringTypeHandler());
            typeHandlers.Add(new ObjectTypeHandler());
            return typeHandlers;
        }

        protected virtual void CompileTypeHandlers(List<TypeHandler> typeHandlers)
        {
            mDeclarations = new Dictionary<TokenType, List<Declaration>>();
            mCasts = new Dictionary<TypePair, Declaration>();
            foreach (var th in typeHandlers)
            {
                foreach (var decl in th.mDeclarations)
                {
                    var tk = decl.tk;
                    if (tk == TokenType.ImplicitCast || tk == TokenType.ExplicitCast)
                    {
                        var typePair = new TypePair() { Actual = decl.P1, Target = decl.T };
                        Declaration curDecl;
                        if (!mCasts.TryGetValue(typePair, out curDecl) ||
                            (curDecl.tk == TokenType.ExplicitCast && decl.tk == TokenType.ImplicitCast))
                        {
                            mCasts[typePair] = decl;
                        }
                    }
                    else
                    {
                        List<Declaration> tokenDeclarations;
                        if (!mDeclarations.TryGetValue(tk, out tokenDeclarations))
                        {
                            tokenDeclarations = new List<Declaration>();
                            mDeclarations[tk] = tokenDeclarations;
                        }
                        tokenDeclarations.Add(decl);
                    }
                }
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

        abstract internal protected bool IsCaseSensitive { get; }

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
            mVariableBag.SetVariable(variableName, variableValue);
        }

        public void SetVariableFunctions(string variableName, Type type)
        {
            SetVariable(variableName, new StaticFunctionsWrapper(type));
        }

        public void DeleteVariable(string variableName)
        {
            mVariableBag.DeleteVariable(variableName);
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
                    return NewToken(TokenType.None);

                case '\0': //null:
                    return NewToken(TokenType.EndOfFormula);

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
                    else return parser.ParseIdentifierOrKeyword();
            }
            throw new InvalidProgramException();
        }

        public virtual IHasValue ParseLeft(Parser parser, Token token, int precedence)
        {

            IHasValue result = null;
            int opPrecedence = GetPrecedence(token, unary: true);

            switch (token.Type)
            {
                case TokenType.OperatorMinus:
                case TokenType.OperatorPlus:
                case TokenType.OperatorNot:
                    // unary minus operator
                    parser.NextToken();
                    result = parser.ParseExpr(null, opPrecedence);
                    result = TypedExpressions.UnaryExpr(parser, token.Type, result);
                    return result;

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
                    return new OperatorIfExpr(parameters[0], parameters[1], parameters[2]);
            }
            throw parser.NewUnexpectedToken();
        }


        internal virtual bool ParseRight(Parser parser, Token tk, int opPrecedence, IHasValue Acc, ref IHasValue ValueLeft)
        {
            var tt = tk.Type;
            IHasValue ValueRight;
            switch (tt)
            {
                case TokenType.OperatorPlus:
                case TokenType.OperatorMinus:
                case TokenType.OperatorConcat:
                case TokenType.OperatorMultiply:
                case TokenType.OperatorDivide:
                case TokenType.OperatorOr:
                case TokenType.OperatorOrElse:
                case TokenType.OperatorAnd:
                case TokenType.OperatorAndAlso:
                case TokenType.OperatorXor:
                case TokenType.OperatorModulo:
                case TokenType.OperatorNE:
                case TokenType.OperatorGT:
                case TokenType.OperatorGE:
                case TokenType.OperatorEQ:
                case TokenType.OperatorLE:
                case TokenType.OperatorLT:
                    parser.NextToken();
                    ValueRight = parser.ParseExpr(ValueLeft, opPrecedence);
                                yield return new Dependency("p1", mP1);
        //ValueLeft = TypedExpressions.BinaryExpr(parser, ValueLeft, tt, ValueRight);
                    ValueLeft = FindOperation(ValueLeft, tt, ValueRight);
                    return true;
                //case TokenType.Operator_percent:
                //    parser.NextToken();
                //    ValueLeft = TypedExpr.BinaryExpr(parser, ValueLeft, tt, Acc);
                //    return true;
                default:
                    return false;
            }
        }

        private IHasValue FindOperation(IHasValue ValueLeft, TokenType tt, IHasValue ValueRight)
        {
            var leftType = ValueLeft.SystemType;
            var rightType = ValueRight.SystemType;
            List<Declaration> declarations;
            if (this.mDeclarations.TryGetValue(tt, out declarations))
            {
                foreach (var decl in declarations)
                {
                    Declaration cast1, cast2;
                    if (CanCast(ValueLeft.SystemType, decl.P1, out cast1) && CanCast(ValueRight.SystemType, decl.P2, out cast2))
                    {
                        return CreateIHasValue(ValueLeft, ValueRight, cast1, cast2, decl);
                    }
                }
            }
            return null;
        }

        private IHasValue CreateIHasValue(IHasValue ValueLeft, IHasValue ValueRight, Declaration cast1, Declaration cast2, Declaration decl)
        {
            var x = typeof(NewTypedExpr<,,>).MakeGenericType(decl.P1, decl.P2, decl.T);
            if (cast1 != null)
            {
            }
            if (cast2 != null)
            {
            }
            //        public TypedExpr(IHasValue<P1> p1, IHasValue<P2> p2, Func<P1, P2, T> func)

            return (IHasValue)Activator.CreateInstance(x, ValueLeft, ValueRight, decl.dlg);

        }

        private class NewTypedExpr<P1, P2, T> : IHasValue<T>
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
                get {
                    yield return new Dependency("p1", mP1);
                    yield return new Dependency("p2", mP2);
                }
            }
        }

        private bool CanCast(Type type1, Type type2, out Declaration cast)
        {
            cast = null;
            if (type1 == type2 || type2.IsAssignableFrom(type1)) return true;
            Declaration decl;
            if (mCasts.TryGetValue(new TypePair() { Actual = type1, Target = type2 }, out decl))
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
