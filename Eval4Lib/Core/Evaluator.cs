using System;
using System.Collections.Generic;

namespace Eval4.Core
{
    public abstract class Evaluator
    {
        internal List<object> mEnvironmentFunctionsList;
        public bool RaiseVariableNotFoundException;
        protected VariableBag mVariableBag;
        
        public Evaluator()
        {
            mEnvironmentFunctionsList = new List<object>();
            mVariableBag = new VariableBag(this.IsCaseSensitive);
            mEnvironmentFunctionsList.Add(mVariableBag);
        }

        abstract internal protected bool IsCaseSensitive { get; }

        public void AddEnvironmentFunctions(object o)
        {
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

        public IExpr Parse(string str)
        {
            if (str == null)
                str = string.Empty;
            var p = new Parser(this, str);
            return p.ParsedExpression;
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

        Dictionary<string, IExpr> mExpressions = new Dictionary<string, IExpr>();

        public object Eval(string formula)
        {
            IExpr parsed;
            if (!mExpressions.TryGetValue(formula, out parsed))
            {
                parsed = Parse(formula);
                mExpressions[formula] = parsed;
                if (mExpressions.Count > 1000) mExpressions.Clear(); //  I know this is crude
            }
            return parsed.ObjectValue;
        }

        public void SetVariable<T>(string variableName, T variableValue)
        {
            mVariableBag.SetVariable(variableName, variableValue);
        }

        public void DeleteVariable(string variableName)
        {
            mVariableBag.DeleteVariable(variableName);
        }

        public abstract bool UseParenthesisForArrays { get; }

        public virtual Token CheckKeyword(string keyword)
        {
            return new Token(TokenType.Value_identifier, keyword);
        }

        internal virtual int GetPrecedence(BaseParser parser, Token tk, bool unary)
        {
            var tt = tk.Type;
            switch (tt)
            {
                case TokenType.unary_minus:
                case TokenType.unary_plus:
                    return 4;
                case TokenType.operator_mul:
                case TokenType.operator_div:
                    return 3;

                case TokenType.operator_plus:
                case TokenType.operator_minus:
                    return 2;

                default:
                    return 1;
            }
        }

        
        public virtual Token ParseToken(BaseParser parser)
        {
            switch (parser.mCurChar)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    parser.NextChar();
                    return new Token(TokenType.none);
                    
                case '\0': //null:
                    return new Token(TokenType.end_of_formula);

                case '-':
                    parser.NextChar();
                    return new Token(TokenType.operator_minus);
                case '+':
                    parser.NextChar();
                    return new Token(TokenType.operator_plus);
                case '*':
                    parser.NextChar();
                    return new Token(TokenType.operator_mul);
                case '/':
                    parser.NextChar();
                    return new Token(TokenType.operator_div);
                case '(':
                    parser.NextChar();
                    return new Token(TokenType.open_parenthesis);
                case ')':
                    parser.NextChar();
                    return new Token(TokenType.close_parenthesis);
                case '<':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return new Token(TokenType.operator_le);
                    }
                    else if (parser.mCurChar == '>')
                    {
                        parser.NextChar();
                        return new Token(TokenType.operator_ne);
                    }
                    else
                    {
                        return new Token(TokenType.operator_lt);
                    }
                case '>':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return new Token(TokenType.operator_ge);
                    }
                    else
                    {
                        return new Token(TokenType.operator_gt);
                    }
                case ',':
                    parser.NextChar();
                    return new Token(TokenType.comma);
                case '.':
                    parser.NextChar();
                    if (parser.mCurChar >= '0' && parser.mCurChar <= '9') return parser.ParseNumber(afterDot:true);
                    else return new Token(TokenType.dot);
                case '\'':
                case '"':
                    return parser.ParseString(true);
                case '[':
                    parser.NextChar();
                    return new Token(TokenType.open_bracket);
                case ']':
                    parser.NextChar();
                    return new Token(TokenType.close_bracket);
                default:
                    if (parser.mCurChar >= '0' && parser.mCurChar <= '9') return parser.ParseNumber();
                    else return parser.ParseIdentifierOrKeyword();
            }
            return new Token(TokenType.none);
        }

        public virtual IExpr ParseLeft(BaseParser parser, Token token, int opPrecedence)
        {

            IExpr result = null;
            switch (token.Type)
            {
                case TokenType.operator_minus:
                case TokenType.operator_plus:
                case TokenType.operator_not:
                    // unary minus operator
                    parser.NextToken();
                    result = parser.ParseExpr(null, opPrecedence);
                    result = TypedExpr.UnaryExpr(parser, token.Type, result);
                    return result;
                
                case TokenType.Value_identifier:
                    parser.ParseIdentifier(ref result);
                    return result;
                
                case TokenType.Value_true:
                    result = new ImmediateExpr<bool>(true);
                    parser.NextToken();
                    return result;
                
                case TokenType.Value_false:
                    result = new ImmediateExpr<bool>(false);
                    parser.NextToken();
                    return result;

                case TokenType.Value_string:
                    result = new ImmediateExpr<string>(token.Value);
                    parser.NextToken();
                    return result;
                
                case TokenType.Value_number:
                    string valueString = token.Value;
                    int intValue;
                    double doubleValue;
                    if (int.TryParse(valueString, out intValue))
                    {
                        result = new ImmediateExpr<int>(intValue);
                    }
                    else if (double.TryParse(valueString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out doubleValue))
                    {
                        result = new ImmediateExpr<double>(doubleValue);
                    }
                    else
                    {
                        throw parser.NewParserException(string.Format("Invalid number {0}", parser.mCurToken.Value));
                    }
                    parser.NextToken();
                    return result;
                
                case TokenType.Value_date:
                    try
                    {
                        result = new ImmediateExpr<DateTime>(DateTime.Parse(parser.mCurToken.Value));
                        parser.NextToken();
                        return result;
                    }
                    catch (Exception) // ex)
                    {
                        throw parser.NewParserException(string.Format("Invalid date {0}, it should be #DD/MM/YYYY hh:mm:ss#", parser.mCurToken.Value));
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
                    List<IExpr> parameters = null;
                    // parameters... 
                    parser.NextToken();
                    bool brackets = false;
                    parameters = parser.ParseParameters(ref brackets);
                    return new OperatorIfExpr(parameters[0], parameters[1], parameters[2]);
            }
            throw parser.NewUnexpectedToken();
        }


        internal virtual bool ParseRight(BaseParser parser, Token tk, int opPrecedence, IExpr Acc, ref IExpr ValueLeft)
        {
            var tt = tk.Type;
            IExpr ValueRight;
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
                    ValueLeft = TypedExpr.BinaryExpr(parser, ValueLeft, tt, ValueRight);
                    return true;
                case TokenType.operator_percent:
                    parser.NextToken();
                    ValueLeft = TypedExpr.BinaryExpr(parser, ValueLeft, tt, Acc);
                    return true;
                default:
                    return false;
            }
        }

        internal virtual void CleanUpCharacter(ref char mCurChar)
        {
        }

    }
}
