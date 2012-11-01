using System;
using System.Collections.Generic;
using System.Text;
using Eval4.Core;

namespace Eval4
{
    public class CSharpToken : Token
    {
        public override int GetPrecedence(bool unary)
        {
            var tt = Type;
            //http://msdn.microsoft.com/en-us/library/aa691323(v=vs.71).aspx
            switch (tt)
            {
                case TokenType.dot:
                case TokenType.open_parenthesis:
                case TokenType.open_bracket:
                case TokenType.@new:

                    // 	Primary	
                    //x.y  f(x)  a[x]  x++  x--  new
                    //typeof  checked  unchecked
                    return 15;

                case TokenType.operator_plus:
                case TokenType.operator_minus:
                    return (unary ? 14 : 12);

                case TokenType.operator_not:
                case TokenType.operator_tilde:
                    // 	Unary	
                    //+  -  !  ~  ++x  --x  (T)x
                    return 14;

                case TokenType.operator_mul:
                case TokenType.operator_div:
                case TokenType.operator_mod:
                    // 	Multiplicative	
                    //*  /  %
                    return 13;

                //case TokenType.operator_plus:
                //case TokenType.operator_minus:
                // 	Additive	
                //  +  -
                //  return 12;

                case TokenType.shift_left:
                case TokenType.shift_right:
                    // 	Shift	
                    //<<  >>
                    return 11;

                case TokenType.operator_lt:
                case TokenType.operator_le:
                case TokenType.operator_ge:
                case TokenType.operator_gt:
                    // 	Relational and type testing	
                    //<  >  <=  >=  is  as
                    return 10;

                case TokenType.operator_eq:
                case TokenType.operator_ne:
                    // 	Equality	
                    //==  !=
                    return 9;

                case TokenType.operator_and:
                    // 	Logical AND	
                    //&
                    return 8;

                case TokenType.operator_xor:
                    // 	Logical XOR	
                    //^
                    return 7;

                case TokenType.operator_or:
                    // 	Logical OR	
                    //|
                    return 6;

                case TokenType.operator_andalso:
                    // 	Conditional AND	
                    //&&
                    return 5;
                case TokenType.operator_orelse:
                    // 	Conditional OR	
                    //||
                    return 4;
                case TokenType.operator_if:
                    // 	Conditional	
                    //?:
                    return 3;
                case TokenType.operator_assign:
                    // 	Assignment	
                    //=  *=  /=  %=  +=  -=  <<=  >>=  &=  ^=  |=
                    return 2;
                default:
                    return 1;
            }
        }
    }

    public class CSharpEvaluator : Core.Evaluator
    {
        protected internal override bool IsCaseSensitive
        {
            get { return true; }
        }

        public override bool UseParenthesisForArrays
        {
            get { return false; }
        }

        public override Token ParseToken(BaseParser parser)
        {
            switch (parser.mCurChar)
            {
                case '%':
                    parser.NextChar();
                    return NewToken(TokenType.operator_mod);

                case '&':
                    parser.NextChar();
                    if (parser.mCurChar == '&')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.operator_andalso);
                    }
                    return NewToken(TokenType.operator_and);

                case '?':
                    parser.NextChar();
                    return NewToken(TokenType.operator_if);

                case '=':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.operator_eq);
                    }
                    return NewToken(TokenType.operator_assign);

                case '!':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.operator_ne);
                    }
                    return NewToken(TokenType.operator_not);

                case '^':
                    parser.NextChar();
                    return NewToken(TokenType.operator_xor);

                case '|':
                    parser.NextChar();
                    if (parser.mCurChar == '|')
                    {
                        parser.NextChar();
                        return NewToken(TokenType.operator_orelse);
                    }
                    return NewToken(TokenType.operator_or);
                case ':':
                    parser.NextChar();
                    return NewToken(TokenType.operator_colon);
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
                        return NewToken(TokenType.Value_true);

                    case "false":
                        return NewToken(TokenType.Value_false);

                    default:
                        return base.CheckKeyword(keyword);
                }
            }
        }

        internal override bool ParseRight(BaseParser parser, Token tk, int opPrecedence, IExpr Acc, ref IExpr ValueLeft)
        {
            var tt = tk.Type;
            switch (tt)
            {
                case TokenType.operator_if:
                    parser.NextToken();
                    IExpr thenExpr = parser.ParseExpr(null, 0);
                    parser.Expect(TokenType.operator_colon, "Missing : in ? expression test ? valueIfTrue : valueIfFalse.");
                    IExpr elseExpr = parser.ParseExpr(null, 0);
                    ValueLeft = new OperatorIfExpr(ValueLeft, thenExpr, elseExpr);
                    return true;
                default:
                    return base.ParseRight(parser, tk, opPrecedence, Acc, ref ValueLeft);
            }
        }

        public override Token NewToken()
        {
            return new CSharpToken();
        }
    }
}
