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

        public Evaluator()
        {
            mEnvironmentFunctionsList = new List<object>();
            mVariableBag = new VariableBag(this.IsCaseSensitive);
            mEnvironmentFunctionsList.Add(mVariableBag);
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
            return NewToken(TokenType.Value_identifier, keyword);
        }

        //internal virtual int GetPrecedence(BaseParser parser, Token tk, bool unary)
        //{
        //    var tt = tk.Type;
        //    if (unary)
        //    {
        //        switch (tt)
        //        {
        //            case TokenType.operator_minus:
        //                tt = TokenType.unary_minus;
        //                break;
        //            case TokenType.operator_plus:
        //                tt = TokenType.unary_plus;
        //                break;
        //            case TokenType.operator_not:
        //                tt = TokenType.unary_not;
        //                break;
        //            case TokenType.operator_tilde:
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
                    return NewToken(TokenType.none);

                case '\0': //null:
                    return NewToken(TokenType.end_of_formula);

                case '-':
                    parser.NextChar();

                    return NewToken(TokenType.operator_minus);
                case '+':
                    parser.NextChar();
                    return NewToken(TokenType.operator_plus);
                case '*':
                    parser.NextChar();
                    return NewToken(TokenType.operator_mul);
                case '/':
                    parser.NextChar();
                    return NewToken(TokenType.operator_div);
                case '(':
                    parser.NextChar();
                    return NewToken(TokenType.open_parenthesis);
                case ')':
                    parser.NextChar();
                    return NewToken(TokenType.close_parenthesis);
                case '<':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.operator_le);
                    }
                    else if (parser.mCurChar == '>')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.operator_ne);
                    }
                    else
                    {
                        return NewToken(TokenType.operator_lt);
                    }
                case '>':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.operator_ge);
                    }
                    else
                    {
                        return NewToken(TokenType.operator_gt);
                    }
                case ',':
                    parser.NextChar();
                    return NewToken(TokenType.comma);
                case '.':
                    parser.NextChar();
                    if (parser.mCurChar >= '0' && parser.mCurChar <= '9') return parser.ParseNumber(afterDot: true);
                    else return NewToken(TokenType.dot);
                case '\'':
                case '"':
                    return parser.ParseString(true);
                case '[':
                    parser.NextChar();
                    return NewToken(TokenType.open_bracket);
                case ']':
                    parser.NextChar();
                    return NewToken(TokenType.close_bracket);
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
                case TokenType.operator_minus:
                case TokenType.operator_plus:
                case TokenType.operator_not:
                    // unary minus operator
                    parser.NextToken();
                    result = parser.ParseExpr(null, opPrecedence);
                    result = TypedExpressions.UnaryExpr(parser, token.Type, result);
                    return result;

                case TokenType.Value_identifier:
                    parser.ParseIdentifier(ref result);
                    return result;

                case TokenType.Value_true:
                    result = new ConstantExpr<bool>(true);
                    parser.NextToken();
                    return result;

                case TokenType.Value_false:
                    result = new ConstantExpr<bool>(false);
                    parser.NextToken();
                    return result;

                case TokenType.Value_string:
                    result = new ConstantExpr<string>(token.ValueString);
                    parser.NextToken();
                    return result;

                case TokenType.Value_number:
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

                case TokenType.Value_date:
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

                case TokenType.open_parenthesis:
                    parser.NextToken();
                    result = parser.ParseExpr(null, 0);
                    if (parser.mCurToken.Type == TokenType.close_parenthesis)
                    {
                        // good we eat the end parenthesis and continue ...
                        parser.NextToken();
                        return result;
                    }
                    else
                    {
                        throw parser.NewUnexpectedToken("End parenthesis not found");
                    }

                case TokenType.operator_if:
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
                case TokenType.operator_plus:
                case TokenType.operator_minus:
                case TokenType.operator_concat:
                case TokenType.operator_mul:
                case TokenType.operator_div:
                case TokenType.operator_or:
                case TokenType.operator_orelse:
                case TokenType.operator_and:
                case TokenType.operator_andalso:
                case TokenType.operator_xor:
                case TokenType.operator_mod:
                case TokenType.operator_ne:
                case TokenType.operator_gt:
                case TokenType.operator_ge:
                case TokenType.operator_eq:
                case TokenType.operator_le:
                case TokenType.operator_lt:
                    parser.NextToken();
                    ValueRight = parser.ParseExpr(ValueLeft, opPrecedence);
                    ValueLeft = TypedExpressions.BinaryExpr(parser, ValueLeft, tt, ValueRight);
                    return true;
                //case TokenType.operator_percent:
                //    parser.NextToken();
                //    ValueLeft = TypedExpr.BinaryExpr(parser, ValueLeft, tt, Acc);
                //    return true;
                default:
                    return false;
            }
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
            result.Type = TokenType.custom;
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
