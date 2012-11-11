using System;
using System.Collections.Generic;
using System.Text;
using Eval4.Core;

namespace Eval4
{
    
    public class CSharpEvaluator : Evaluator<CSharpEvaluator.CSharpCustomToken>
    {
        public enum CSharpCustomToken
        {
            Coalesce,
            None
        }
        
        protected internal override EvaluatorOptions Options
        {
            get
            {
                return EvaluatorOptions.BooleanValues
                    | EvaluatorOptions.CaseSensitive
                    //| EvaluatorOptions.DateTimeValues
                    | EvaluatorOptions.DoubleValues
                    | EvaluatorOptions.IntegerValues
                    | EvaluatorOptions.ObjectValues
                    | EvaluatorOptions.StringValues;
            }
        }

        public override bool UseParenthesisForArrays
        {
            get { return false; }
        }

        public override Token ParseToken()
        {
            switch (mCurChar)
            {
                case '%':
                    NextChar();
                    return new Token(TokenType.OperatorModulo);


                case '&':
                    NextChar();
                    if (mCurChar == '&')
                    {
                        NextChar();
                        return new Token(TokenType.OperatorAndAlso);
                    }
                    return new Token(TokenType.OperatorAnd);

                case '?':
                    NextChar();
                    if (mCurChar == '?')
                    {
                        NextChar();
                        return new Token(CSharpCustomToken.Coalesce);
                    }
                    else return new Token(TokenType.OperatorIf);

                case '=':
                    NextChar();
                    if (mCurChar == '=')
                    {
                        NextChar();
                        return new Token(TokenType.OperatorEQ);
                    }
                    return new Token(TokenType.OperatorAssign);

                case '!':
                    NextChar();
                    if (mCurChar == '=')
                    {
                        NextChar();
                        return new Token(TokenType.OperatorNE);
                    }
                    return new Token(TokenType.OperatorNot);

                case '^':
                    NextChar();
                    return new Token(TokenType.OperatorXor);

                case '|':
                    NextChar();
                    if (mCurChar == '|')
                    {
                        NextChar();
                        return new Token(TokenType.OperatorOrElse);
                    }
                    return new Token(TokenType.OperatorOr);
                case ':':
                    NextChar();
                    return new Token(TokenType.OperatorColon);

                default:
                    return base.ParseToken();

            }
        }

        public override Token CheckKeyword(string keyword)
        {
            {
                switch (keyword.ToString())
                {
                    case "true":
                        return new Token(TokenType.ValueTrue);

                    case "false":
                        return new Token(TokenType.ValueFalse);

                    default:
                        return base.CheckKeyword(keyword);
                }
            }
        }

        protected override void ParseRight(Token tk, int opPrecedence, IHasValue Acc, ref IHasValue valueLeft)
        {
            var tt = tk.Type;
            switch (tt)
            {
                case TokenType.OperatorIf:
                    NextToken();
                    IHasValue thenExpr = ParseExpr(null, 1);
                    if (!Expect(TokenType.OperatorColon, "Missing : in ? expression test ? valueIfTrue : valueIfFalse.", ref valueLeft))
                        return;
                    IHasValue elseExpr = ParseExpr(null, 1);
                    var t = typeof(OperatorIfExpr<>).MakeGenericType(thenExpr.ValueType);

                    valueLeft = (IHasValue)Activator.CreateInstance(t, valueLeft, thenExpr, elseExpr);
                    break;
                default:
                    base.ParseRight(tk, opPrecedence, Acc, ref valueLeft);
                    break;
            }
        }

        protected override int GetPrecedence(Token token, bool unary)
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
