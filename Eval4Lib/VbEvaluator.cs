using System;
using System.Collections.Generic;
using System.Text;
using Eval4.Core;

namespace Eval4
{    
    public class VbEvaluator : Evaluator<VbEvaluator.VbTokenType>
    {
        public enum VbTokenType
        {
            OperatorPercent,
            IntegerDiv
        }
        
        protected internal override EvaluatorOptions Options
        {
            get
            {
                return EvaluatorOptions.BooleanValues
                    //| EvaluatorOptions.CaseSensitive
                    //| EvaluatorOptions.DateTimeValues
                    | EvaluatorOptions.DoubleValues
                    | EvaluatorOptions.IntegerValues
                    | EvaluatorOptions.ObjectValues
                    | EvaluatorOptions.StringValues;
            }
        }

        public override bool UseParenthesisForArrays
        {
            get { return true; }
        }

        public override Token ParseToken()
        {
            //TODO:Check we advance
            switch (mCurChar)
            {
                case '%':
                    NextChar();
                    return new Token(VbTokenType.OperatorPercent);

                case '=':
                    NextChar();
                    return new Token(TokenType.OperatorEQ);

                case '#':
                    return ParseDate();

                case '&':
                    NextChar();
                    return new Token(TokenType.OperatorConcat);

                case '\\':
                    NextChar();
                    return new Token(VbTokenType.IntegerDiv);

                default:
                    return base.ParseToken();

            }
        }

        internal Token ParseDate()
        {
            var sb = new StringBuilder();
            NextChar();
            // eat the #
            while ((mCurChar >= '0' && mCurChar <= '9') || (mCurChar == '/') || (mCurChar == ':') || (mCurChar == ' '))
            {
                sb.Append(mCurChar);
                NextChar();
            }
            if (mCurChar != '#')
            {
                return new Token(TokenType.SyntaxError, "Missing character # at the end of the date literal.");
            }
            else
            {
                NextChar();
                DateTime ignoreResult;

                if (!DateTime.TryParse(sb.ToString(), out ignoreResult))
                {
                    return new Token(TokenType.SyntaxError, "Invalid date literal. Expcecting the format #yyyy/mm/dd#");
                }
            }
            return new Token(TokenType.ValueDate, sb.ToString());
        }



        public override Token CheckKeyword(string keyword)
        {
            switch (keyword.ToString())
            {
                case "and":
                    return new Token(TokenType.OperatorAnd);

                case "andalso":
                    return new Token(TokenType.OperatorAndAlso);

                case "or":
                    return new Token(TokenType.OperatorOr);

                case "orelse":
                    return new Token(TokenType.OperatorOrElse);

                case "xor":
                    return new Token(TokenType.OperatorXor);

                case "not":
                    return new Token(TokenType.OperatorNot);

                case "true":
                case "yes":
                    return new Token(TokenType.ValueTrue);

                case "if":
                    return new Token(TokenType.OperatorIf);

                case "false":
                case "no":
                    return new Token(TokenType.ValueFalse);

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

        //internal override int GetPrecedence(BaseToken tk, bool unary)
        //{
        //    return 0;
        //}


        protected override void ParseRight(Token tk, int opPrecedence, IHasValue Acc, ref IHasValue valueLeft)
        {
            switch (tk.Type)
            {
                case TokenType.Custom:

                    IHasValue valueRight;
                    switch (tk.CustomType)
                    {
                        case VbTokenType.OperatorPercent:
                            NextToken();
                            if (EmitDelegateExpr(ref valueLeft, Acc, new Func<double, double, double>((a, b) => a * b / 100))) return;
                            break;

                        case VbTokenType.IntegerDiv:
                            NextToken();
                            valueRight = ParseExpr(valueLeft, opPrecedence);
                            if (EmitDelegateExpr(ref valueLeft, valueRight, new Func<int, int, int>((a, b) => a / b))) return;
                            break;
                    }
                    break;

                case TokenType.OperatorDivide:
                    NextToken();
                    valueRight = ParseExpr(valueLeft, opPrecedence);
                    if (EmitDelegateExpr(ref valueLeft, valueRight, new Func<double, double, double>((a, b) => a / (double)b))) return;
                    break;
            }
            base.ParseRight(tk, opPrecedence, Acc, ref valueLeft);

        }

        protected override int GetPrecedence(Token token, bool unary)
        {
            var tt = token;
            //if (unary)
            //{
            //    switch (tt)
            //    {
            //        case TokenType.Operator_plus:
            //            tt = TokenType.unary_plus;
            //            break;
            //        case TokenType.Operator_minus:
            //            tt = TokenType.unary_minus;
            //            break;
            //    }
            //}
            // http://msdn.microsoft.com/en-us/library/fw84t893(v=vs.80).aspx

            switch (tt.Type)
            {
                case TokenType.OpenParenthesis:
                    return 16;

                case TokenType.Exponent:
                    //Exponentiation (^)
                    return 15;

                case TokenType.OperatorMinus:
                case TokenType.OperatorPlus:
                    //Unary identity and negation (+, –)
                    return (unary ? 14 : 9);

                case TokenType.Custom:
                    if (tt.CustomType == VbTokenType.OperatorPercent)
                    {
                        // the percent operator is something I created 
                        // it allows formula like 10 + 5% 
                        return 13;
                    }
                    else return 1;
                    break;
                case TokenType.OperatorMultiply:
                case TokenType.OperatorDivide:
                    //Multiplication and floating-point division (*, /)
                    return 12;

                case TokenType.BackSlash:
                    //Integer division (\)
                    return 11;

                case TokenType.OperatorModulo:
                    //Modulus arithmetic (Mod)
                    return 10;

                //case TokenType.Operator_plus:
                //    //Addition and subtraction (+, –), string concatenation (+)
                //    return 9;

                case TokenType.OperatorConcat:
                    //String concatenation (&)
                    return 8;

                case TokenType.ShiftLeft:
                    //Arithmetic bit shift (<<, >>)
                    return 7;

                case TokenType.OperatorEQ:
                case TokenType.OperatorNE:
                case TokenType.OperatorGE:
                case TokenType.OperatorGT:
                case TokenType.OperatorLE:
                case TokenType.OperatorLT:
                    //All comparison operators (=, <>, <, <=, >, >=, Is, IsNot, Like, TypeOf...Is)
                    return 6;

                case TokenType.OperatorNot:
                    //Negation (Not)
                    return 5;

                case TokenType.OperatorAnd:
                case TokenType.OperatorAndAlso:
                    //Conjunction (And, AndAlso)
                    return 4;

                case TokenType.OperatorOr:
                case TokenType.OperatorOrElse:
                    //Inclusive disjunction (Or, OrElse)
                    return 3;

                case TokenType.OperatorXor:
                    //Exclusive disjunction (Xor)
                    return 2;
                default:
                    return 0;
            }
        }
    }

}
