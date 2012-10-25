using System;
using System.Collections.Generic;
using System.Text;
using Eval4.Core;

namespace Eval4
{
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

        public override TokenType ParseToken(Parser parser)
        {
            switch (parser.mCurChar)
            {
                case '%':
                    parser.NextChar();
                    return TokenType.operator_mod;

                case '&':
                    parser.NextChar();
                    if (parser.mCurChar == '&')
                    {
                        parser.NextChar();
                        return TokenType.operator_andalso;
                    }
                    return TokenType.operator_and;

                case '?':
                    parser.NextChar();
                    return TokenType.operator_if;

                case '=':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return TokenType.operator_eq;
                    }
                    return TokenType.operator_assign;

                case '!':
                    parser.NextChar();
                    if (parser.mCurChar == '=')
                    {
                        parser.NextChar();
                        return TokenType.operator_ne;
                    }
                    return TokenType.operator_not;

                case '^':
                    parser.NextChar();
                    return TokenType.operator_xor;

                case '|':
                    parser.NextChar();
                    if (parser.mCurChar == '|')
                    {
                        parser.NextChar();
                        return TokenType.operator_orelse;
                    }
                    return TokenType.operator_or;
                case ':':
                    parser.NextChar();
                    return TokenType.operator_colon;
                default:
                    return base.ParseToken(parser);

            }
        }

        public override TokenType CheckKeyword(string keyword)
        {
            {
                switch (keyword.ToString())
                {
                    case "true":
                        return TokenType.Value_true;

                    case "false":
                        return TokenType.Value_false;

                    default:
                        return base.CheckKeyword(keyword);
                }
            }
        }

        internal override bool ParseRight(Parser parser, TokenType tt, Precedence opPrecedence, Expr Acc, ref Expr ValueLeft)
        {
            switch (tt)
            {
                case TokenType.operator_if:
                    parser.NextToken();
                    Expr thenExpr = parser.ParseExpr(null, Precedence.None);
                    parser.Expect(TokenType.operator_colon, "Missing : in ? expression test ? valueIfTrue : valueIfFalse.");
                    Expr elseExpr = parser.ParseExpr(null, Precedence.None);
                    ValueLeft = new OperatorIfExpr(ValueLeft, thenExpr, elseExpr);
                    return true;
                default:
                    return base.ParseRight(parser, tt, opPrecedence, Acc, ref ValueLeft);
            }
        }
    }
}
