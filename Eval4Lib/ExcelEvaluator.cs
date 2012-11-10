using Eval4.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Excel
{
    public enum ExcelToken
    {
        None
    }

    public class Range
    {
    }

    public class Cell
    {
        private IEvaluator mEv;
        private string mFormula;
        private string mName;
        private IHasValue mValue;
        public Exception Exception;

        public Cell(IEvaluator ev, int col, int row)
        {
            mName = GetCellName(col + 1, row + 1);
            this.mEv = ev;
            ev.SetVariable(mName, this);
        }

        public static string GetColName(int x)
        {
            string result = string.Empty;
            if (x <= 26)
            {
                result = ((char)(64 + x)).ToString();
            }
            else if (x <= 26 * 26)
            {
                var x1 = ((x - 1) / 26);
                var x2 = 1 + ((x - 1) % 26);

                result = ((char)(64 + x1)).ToString() + ((char)(64 + x2)).ToString();
            }
            return result;
        }

        public static string GetCellName(int x, int y)
        {
            var col = GetColName(x);
            var cell = col + y.ToString();
            return cell;
        }


        public string Formula
        {
            get
            {
                return mFormula;
            }
            set
            {
                mFormula = value;
                Exception = null;
                mValue = null;
                var firstChar = string.IsNullOrEmpty(mFormula) ? '\0' : mFormula[0];
                if ((firstChar >= '0' && firstChar <= '9') || firstChar == '.' || firstChar == '+' || firstChar == '-' || firstChar == '=')
                {
                    try
                    {
                        mValue = mEv.Parse((firstChar == '=' ? mFormula.Substring(1) : mFormula));
                    }
                    catch (Exception ex)
                    {
                        Exception = ex;
                    }
                }
            }
        }


        public override string ToString()
        {
            if (Exception != null) return Exception.Message;
            else if (mValue != null)
            {
                object val = mValue.ObjectValue;
                if (val is double) return ((double)val).ToString("#,##0.00");
                else return val.ToString();
            }
            else
            {
                if (mFormula != null && mFormula.StartsWith("'")) return mFormula.Substring(1);
                else return mFormula;
            }
        }

        public object ValueObject
        {
            get {
                if (mValue != null) return mValue.ObjectValue;
                else return mFormula;
            }
        }

    }

    public class ExcelSheet
    {
        
    }

    public class ExcelEvaluator : Evaluator<ExcelToken>
    {

        protected internal override EvaluatorOptions Options
        {
            get
            {
                return EvaluatorOptions.BooleanLogic
                    | EvaluatorOptions.CaseSensitive
                    | EvaluatorOptions.DoubleValues
                    //| EvaluatorOptions.IntegerValues
                    | EvaluatorOptions.ObjectValues
                    | EvaluatorOptions.StringValues;
            }
        }

        protected override void DeclareOperators()
        {
            base.DeclareOperators();
            base.AddImplicitCast<Cell, double>((a) =>
            {
                var value = a.ValueObject;
                if (value is double) return (double)value;
                if (value is bool) return ((bool)value) ? 1 : 0;
                if (value is DateTime) return ((DateTime)value).Subtract(EPOCH).TotalDays;
                return double.NaN;
            });
        }
        static DateTime EPOCH = new DateTime(1900, 1, 1);

        public override bool UseParenthesisForArrays
        {
            get { return false; }
        }

        public override IHasValue ParseUnaryExpression(Token<ExcelToken> token, int precedence)
        {
            switch (token.Type)
            {
                default:
                    return base.ParseUnaryExpression(token, precedence);
            }

        }
        public override Token<ExcelToken> ParseToken()
        {
            switch (mCurChar)
            {
                case '%':
                    NextChar();
                    return NewToken(TokenType.OperatorModulo);

                case '&':
                    NextChar();
                    if (mCurChar == '&')
                    {
                        NextChar();
                        return NewToken(TokenType.OperatorAndAlso);
                    }
                    return NewToken(TokenType.OperatorAnd);

                case '?':
                    NextChar();
                    return NewToken(TokenType.OperatorIf);

                case '=':
                    NextChar();
                    if (mCurChar == '=')
                    {
                        NextChar();
                        return NewToken(TokenType.OperatorEQ);
                    }
                    return NewToken(TokenType.OperatorAssign);

                case '!':
                    NextChar();
                    if (mCurChar == '=')
                    {
                        NextChar();
                        return NewToken(TokenType.OperatorNE);
                    }
                    return NewToken(TokenType.OperatorNot);

                case '^':
                    NextChar();
                    return NewToken(TokenType.OperatorXor);

                case '|':
                    NextChar();
                    if (mCurChar == '|')
                    {
                        NextChar();
                        return NewToken(TokenType.OperatorOrElse);
                    }
                    return NewToken(TokenType.OperatorOr);
                case ':':
                    NextChar();
                    return NewToken(TokenType.OperatorColon);
                default:
                    return base.ParseToken();

            }
        }

        public override Token<ExcelToken> CheckKeyword(string keyword)
        {
            {
                switch (keyword.ToString())
                {
                    case "true":
                        return NewToken(TokenType.ValueTrue);

                    case "false":
                        return NewToken(TokenType.ValueFalse);

                    default:
                        return base.CheckKeyword(keyword);
                }
            }
        }

        protected override void ParseRight(Token<ExcelToken> tk, int opPrecedence, IHasValue Acc, ref IHasValue valueLeft)
        {
            var tt = tk.Type;
            switch (tt)
            {
                case TokenType.OperatorIf:
                    NextToken();
                    IHasValue thenExpr = ParseExpr(null, 0);
                    if (!Expect(TokenType.OperatorColon, "Missing : in ? expression test ? valueIfTrue : valueIfFalse.", ref valueLeft))
                        return;
                    IHasValue elseExpr = ParseExpr(null, 0);
                    var t = typeof(OperatorIfExpr<>).MakeGenericType(thenExpr.ValueType);

                    valueLeft = (IHasValue)Activator.CreateInstance(t, valueLeft, thenExpr, elseExpr);
                    break;
                default:
                    base.ParseRight(tk, opPrecedence, Acc, ref valueLeft);
                    break;
            }
        }

        protected override int GetPrecedence(Token<ExcelToken> token, bool unary)
        {
            var tt = token.Type;
            //http://msdn.microsoft.com/en-us/library/aa691323(v=vs.71).aspx
            switch (tt)
            {
                case TokenType.Dot:
                case TokenType.OpenParenthesis:
                case TokenType.OpenBracket:
                case TokenType.New:

                    // 	Primary	
                    //x.y  f(x)  a[x]  x++  x--  new
                    //typeof  checked  unchecked
                    return 15;

                case TokenType.OperatorPlus:
                case TokenType.OperatorMinus:
                    return (unary ? 14 : 12);

                case TokenType.OperatorNot:
                case TokenType.OperatorTilde:
                    // 	Unary	
                    //+  -  !  ~  ++x  --x  (T)x
                    return 14;

                case TokenType.OperatorMultiply:
                case TokenType.OperatorDivide:
                case TokenType.OperatorModulo:
                    // 	Multiplicative	
                    //*  /  %
                    return 13;

                //case TokenType.Operator_plus:
                //case TokenType.Operator_minus:
                // 	Additive	
                //  +  -
                //  return 12;

                case TokenType.ShiftLeft:
                case TokenType.ShiftRight:
                    // 	Shift	
                    //<<  >>
                    return 11;

                case TokenType.OperatorLT:
                case TokenType.OperatorLE:
                case TokenType.OperatorGE:
                case TokenType.OperatorGT:
                    // 	Relational and type testing	
                    //<  >  <=  >=  is  as
                    return 10;

                case TokenType.OperatorEQ:
                case TokenType.OperatorNE:
                    // 	Equality	
                    //==  !=
                    return 9;

                case TokenType.OperatorAnd:
                    // 	Logical AND	
                    //&
                    return 8;

                case TokenType.OperatorXor:
                    // 	Logical XOR	
                    //^
                    return 7;

                case TokenType.OperatorOr:
                    // 	Logical OR	
                    //|
                    return 6;

                case TokenType.OperatorAndAlso:
                    // 	Conditional AND	
                    //&&
                    return 5;
                case TokenType.OperatorOrElse:
                    // 	Conditional OR	
                    //||
                    return 4;
                case TokenType.OperatorIf:
                    // 	Conditional	
                    //?:
                    return 3;
                case TokenType.OperatorAssign:
                    // 	Assignment	
                    //=  *=  /=  %=  +=  -=  <<=  >>=  &=  ^=  |=
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
