using Eval4.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4
{
    public enum JavascriptToken{
    }

    class JavascriptEvaluator : Evaluator<JavascriptToken>
    {
        protected internal override bool IsCaseSensitive
        {
            get { return true; }
        }

        public override bool UseParenthesisForArrays
        {
            get { return false; }
        }

        protected override List<TypeHandler> GetTypeHandlers()
        {
            return base.GetTypeHandlers();
        }

        public override Token ParseToken(Parser parser)
        {
            switch (parser.mCurChar)
            {
                case '%':
                    parser.NextChar();
                    return NewToken(TokenType.OperatorModulo);

                case '&':
                    parser.NextChar();
                    if (parser.mCurChar == '&')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.OperatorAndAlso);
                    }
                    return NewToken(TokenType.OperatorAnd);

                case '?':
                    parser.NextChar();
                    return NewToken(TokenType.OperatorIf);

                case '=':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.OperatorEQ);
                    }
                    return NewToken(TokenType.OperatorAssign);

                case '!':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.OperatorNE);
                    }
                    return NewToken(TokenType.OperatorNot);

                case '^':
                    parser.NextChar();
                    return NewToken(TokenType.OperatorXor);

                case '|':
                    parser.NextChar();
                    if (parser.mCurChar == '|')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.OperatorOrElse);
                    }
                    return NewToken(TokenType.OperatorOr);
                case ':':
                    parser.NextChar();
                    return NewToken(TokenType.OperatorColon);
                default:
                    return base.ParseToken(parser);

            }
        }

        public override Token CheckKeyword(string keyword)
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

        internal override void ParseRight(Parser parser, Token tk, int opPrecedence, IHasValue Acc, ref IHasValue valueLeft)
        {
            var tt = tk.Type;
            switch (tt)
            {
                case TokenType.OperatorIf:
                    parser.NextToken();
                    IHasValue thenExpr = parser.ParseExpr(null, 0);
                    parser.Expect(TokenType.OperatorColon, "Missing : in ? expression test ? valueIfTrue : valueIfFalse.");
                    IHasValue elseExpr = parser.ParseExpr(null, 0);
                    var t = typeof(OperatorIfExpr<>).MakeGenericType(thenExpr.SystemType);

                    valueLeft = (IHasValue)Activator.CreateInstance(t, valueLeft, thenExpr, elseExpr);
                    break;
                default:
                    base.ParseRight(parser, tk, opPrecedence, Acc, ref valueLeft);
                    break;
            }
        }

        public override int GetPrecedence(Token<JavascriptToken> token, bool unary)
        {
            var tt = token.Type;
            //https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Operators/Operator_Precedence
            //TODO this is still C# precednce for now
            switch (tt)
            {
                case TokenType.Dot:
                case TokenType.OpenBracket:
                case TokenType.New:
                    return 15;

                case TokenType.OpenParenthesis:
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
                case TokenType.OperatorColon:
                case TokenType.CloseParenthesis:
                case TokenType.CloseBracket:
                case TokenType.Comma:
                    return 0;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
