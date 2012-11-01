using System;
using System.Collections.Generic;
using System.Text;
using Eval4.Core;

namespace Eval4
{
    public class VbToken : Token
    {
        public override int GetPrecedence(bool unary)
        {
            var tt = Type;
            //if (unary)
            //{
            //    switch (tt)
            //    {
            //        case TokenType.operator_plus:
            //            tt = TokenType.unary_plus;
            //            break;
            //        case TokenType.operator_minus:
            //            tt = TokenType.unary_minus;
            //            break;
            //    }
            //}
            // http://msdn.microsoft.com/en-us/library/fw84t893(v=vs.80).aspx

            switch (tt)
            {
                case TokenType.open_parenthesis:
                    return 16;

                case TokenType.exponent:
                    //Exponentiation (^)
                    return 15;

                case TokenType.operator_minus:
                case TokenType.operator_plus:
                    //Unary identity and negation (+, –)
                    return (unary ? 14 : 9);

                case TokenType.operator_percent:
                    // the percent operator is something I created 
                    // it allows formula like 10 + 5% 
                    return 13;

                case TokenType.operator_mul:
                case TokenType.operator_div:
                    //Multiplication and floating-point division (*, /)
                    return 12;

                case TokenType.backslash:
                    //Integer division (\)
                    return 11;

                case TokenType.operator_mod:
                    //Modulus arithmetic (Mod)
                    return 10;

                //case TokenType.operator_plus:
                //    //Addition and subtraction (+, –), string concatenation (+)
                //    return 9;

                case TokenType.operator_concat:
                    //String concatenation (&)
                    return 8;

                case TokenType.shift_left:
                    //Arithmetic bit shift (<<, >>)
                    return 7;

                case TokenType.operator_eq:
                case TokenType.operator_ne:
                case TokenType.operator_ge:
                case TokenType.operator_gt:
                case TokenType.operator_le:
                case TokenType.operator_lt:
                    //All comparison operators (=, <>, <, <=, >, >=, Is, IsNot, Like, TypeOf...Is)
                    return 6;

                case TokenType.operator_not:
                    //Negation (Not)
                    return 5;

                case TokenType.operator_and:
                case TokenType.operator_andalso:
                    //Conjunction (And, AndAlso)
                    return 4;

                case TokenType.operator_or:
                case TokenType.operator_orelse:
                    //Inclusive disjunction (Or, OrElse)
                    return 3;

                case TokenType.operator_xor:
                    //Exclusive disjunction (Xor)
                    return 2;

                default:
                    return 1;
            }
        }
    }
    public class VbEvaluator : Core.Evaluator
    {
        protected internal override bool IsCaseSensitive
        {
            get { return false; }
        }

        public override bool UseParenthesisForArrays
        {
            get { return true; }
        }

        public override Token ParseToken(BaseParser parser, bool unary)
        {
            switch (parser.mCurChar)
            {
                case '%':
                    parser.NextChar();
                    return NewToken(TokenType.operator_percent);

                case '=':
                    parser.NextChar();
                    return NewToken(TokenType.operator_eq);

                case '#':
                    return parser.ParseDate();

                case '&':
                    parser.NextChar();
                    return NewToken(TokenType.operator_concat);

                default:
                    return base.ParseToken(parser, unary);

            }
        }

        public override Token CheckKeyword(string keyword)
        {
            switch (keyword.ToString())
            {
                case "and":
                    return NewToken(TokenType.operator_and);

                case "andalso":
                    return NewToken(TokenType.operator_andalso);

                case "or":
                    return NewToken(TokenType.operator_or);

                case "orelse":
                    return NewToken(TokenType.operator_orelse);

                case "xor":
                    return NewToken(TokenType.operator_xor);

                case "not":
                    return NewToken(TokenType.operator_not);

                case "true":
                case "yes":
                    return NewToken(TokenType.Value_true);

                case "if":
                    return NewToken(TokenType.operator_if);

                case "false":
                case "no":
                    return NewToken(TokenType.Value_false);

                default:
                    return base.CheckKeyword(keyword);
            }
        }

        // Single Quote-like Characters
        const char APOSTROPHE = (char)39;
        const char GRAVE_ACCENT = (char)96;
        const char LEFT_SINGLE_QUOTATION_MARK = (char)8216;
        const char RIGHT_SINGLE_QUOTATION_MARK = (char)8217;
        const char PRIME = (char)8242;
        const char COMBINING_ACUTE_ACCENT = (char)769;
        const char COMBINING_GRAVE_ACCENT = (char)768;

        // Double Quote-like Characters
        const char QUOTATION_MARK = (char)34;
        const char LEFT_DOUBLE_QUOTATION_MARK = (char)8220;
        const char RIGHT_DOUBLE_QUOTATION_MARK = (char)8221;
        const char DOUBLE_PRIME = (char)8243;

        // Dashes
        const char DITTO_MARK = (char)12291;
        const char HYPHEN_MINUS = (char)45;
        const char HYPHEN = (char)8208;
        const char EN_DASH = (char)8211;
        const char EM_DASH = (char)8212;
        const char MINUS_SIGN = (char)8722;
        const char SOFT_HYPHEN = (char)173;
        const char NON_BREAKING_HYPHEN = (char)8209;
        const char HYPHEN_BULLET = (char)8259;

        // Spaces
        const char SPACE = (char)32;
        const char NO_BREAK_SPACE = (char)160;
        const char EM_SPACE = (char)8195;
        const char EN_SPACE = (char)8194;


        internal override void CleanUpCharacter(ref char mCurChar)
        {
            // Some editors change the quotes (notably MS Word)
            // also copying code from a PDF will sometime fail
            // the switch below fix common issues
            switch (mCurChar)
            {
                // Single Quote-like Characters
                case GRAVE_ACCENT:
                case LEFT_SINGLE_QUOTATION_MARK:
                case RIGHT_SINGLE_QUOTATION_MARK:
                case PRIME:
                case COMBINING_ACUTE_ACCENT:
                case COMBINING_GRAVE_ACCENT:
                    mCurChar = APOSTROPHE;
                    break;
                // Double Quote-like Characters
                case LEFT_DOUBLE_QUOTATION_MARK:
                case RIGHT_DOUBLE_QUOTATION_MARK:
                case DOUBLE_PRIME:
                    mCurChar = QUOTATION_MARK;
                    break;
                // Dashes
                case DITTO_MARK:
                case HYPHEN:
                case EN_DASH:
                case EM_DASH:
                case MINUS_SIGN:
                case SOFT_HYPHEN:
                case NON_BREAKING_HYPHEN:
                case HYPHEN_BULLET:
                    mCurChar = HYPHEN_MINUS;
                    break;
            }
        }

        //internal override int GetPrecedence(BaseParser parser, Token tk, bool unary)
        //{
        //    return 0;
        //}

        public override Token NewToken()
        {
            return new VbToken();
        }
    }
}
