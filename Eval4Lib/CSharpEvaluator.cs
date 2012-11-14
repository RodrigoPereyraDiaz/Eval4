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
                return EvaluatorOptions.CaseSensitive;
            }
        }

        protected override void DeclareOperators()
        {
            DeclareOperators(typeof(bool));
            DeclareOperators(typeof(int));
            DeclareOperators(typeof(uint));
            DeclareOperators(typeof(long));
            DeclareOperators(typeof(ulong));
            DeclareOperators(typeof(float));
            DeclareOperators(typeof(double));
            DeclareOperators(typeof(DateTime));
            DeclareOperators(typeof(string));
            DeclareOperators(typeof(object));
        }

        protected override void DeclareOperators(Type type)
        {

            if (type == typeof(float))
            {
                // C# supports float arithmetic
                AddBinaryOperation<float, float, float>(TokenType.OperatorPlus, (a, b) => { return a + b; });
                AddBinaryOperation<float, float, float>(TokenType.OperatorMinus, (a, b) => { return a - b; });
                AddBinaryOperation<float, float, float>(TokenType.OperatorMultiply, (a, b) => { return a * b; });
                AddBinaryOperation<float, float, float>(TokenType.OperatorDivide, (a, b) => { return a / b; });
                AddBinaryOperation<float, float, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
                AddBinaryOperation<float, float, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
                AddBinaryOperation<float, float, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
                AddBinaryOperation<float, float, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
                AddBinaryOperation<float, float, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
                AddBinaryOperation<float, float, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

                AddUnaryOperation<float, float>(TokenType.OperatorMinus, (a) => { return -a; });
                AddUnaryOperation<float, float>(TokenType.OperatorPlus, (a) => { return a; });

                AddImplicitCast<byte, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<sbyte, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<short, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<ushort, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<int, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<uint, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<long, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<ulong, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<float, float>((a) => { return a; }, CastCompatibility.NoLoss);
                AddExplicitCast<decimal, float>((a) => { return (int)a; }, CastCompatibility.PossibleLoss);
            }
            if (type == typeof(uint))
            {

                // C# supports ulong arithmetic
                AddBinaryOperation<uint, uint, uint>(TokenType.OperatorPlus, (a, b) => { return a + b; });
                AddBinaryOperation<uint, uint, uint>(TokenType.OperatorMinus, (a, b) => { return a - b; });
                AddBinaryOperation<uint, uint, uint>(TokenType.OperatorMultiply, (a, b) => { return a * b; });
                AddBinaryOperation<uint, uint, uint>(TokenType.OperatorDivide, (a, b) => { return a / b; });
                AddBinaryOperation<uint, uint, uint>(TokenType.OperatorModulo, (a, b) => { return a % b; });
                AddBinaryOperation<uint, uint, uint>(TokenType.OperatorAnd, (a, b) => { return a & b; });
                AddBinaryOperation<uint, uint, uint>(TokenType.OperatorOr, (a, b) => { return a | b; });
                AddBinaryOperation<uint, uint, uint>(TokenType.OperatorXor, (a, b) => { return a ^ b; });
                AddBinaryOperation<uint, uint, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
                AddBinaryOperation<uint, uint, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
                AddBinaryOperation<uint, uint, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
                AddBinaryOperation<uint, uint, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
                AddBinaryOperation<uint, uint, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
                AddBinaryOperation<uint, uint, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

                AddUnaryOperation<uint, uint>(TokenType.OperatorNot, (a) => { return ~a; });
                AddUnaryOperation<uint, long>(TokenType.OperatorMinus, (a) => { return -a; });
                AddUnaryOperation<uint, uint>(TokenType.OperatorPlus, (a) => { return a; });

                AddImplicitCast<byte, uint>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<ushort, uint>((a) => { return a; }, CastCompatibility.NoLoss);


                AddExplicitCast<sbyte, uint>((a) => { return (uint)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<short, uint>((a) => { return (uint)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<int, uint>((a) => { return (uint)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<long, uint>((a) => { return (uint)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<ulong, uint>((a) => { return (uint)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<double, uint>((a) => { return (uint)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<float, uint>((a) => { return (uint)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<decimal, uint>((a) => { return (uint)a; }, CastCompatibility.PossibleLoss);
            }
            if (type == typeof(long))
            {
                // C# supports integer, long and ulong arithmetic
                AddBinaryOperation<long, long, long>(TokenType.OperatorPlus, (a, b) => { return a + b; });
                AddBinaryOperation<long, long, long>(TokenType.OperatorMinus, (a, b) => { return a - b; });
                AddBinaryOperation<long, long, long>(TokenType.OperatorMultiply, (a, b) => { return a * b; });
                AddBinaryOperation<long, long, long>(TokenType.OperatorDivide, (a, b) => { return a / b; });
                AddBinaryOperation<long, long, long>(TokenType.OperatorModulo, (a, b) => { return a % b; });
                AddBinaryOperation<long, long, long>(TokenType.OperatorAnd, (a, b) => { return a & b; });
                AddBinaryOperation<long, long, long>(TokenType.OperatorOr, (a, b) => { return a | b; });
                AddBinaryOperation<long, long, long>(TokenType.OperatorXor, (a, b) => { return a ^ b; });
                AddBinaryOperation<long, long, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
                AddBinaryOperation<long, long, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
                AddBinaryOperation<long, long, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
                AddBinaryOperation<long, long, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
                AddBinaryOperation<long, long, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
                AddBinaryOperation<long, long, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

                AddUnaryOperation<long, long>(TokenType.OperatorNot, (a) => { return ~a; });
                AddUnaryOperation<long, long>(TokenType.OperatorMinus, (a) => { return -a; });
                AddUnaryOperation<long, long>(TokenType.OperatorPlus, (a) => { return a; });

                AddImplicitCast<byte, long>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<sbyte, long>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<short, long>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<ushort, long>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<int, long>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<uint, long>((a) => { return a; }, CastCompatibility.NoLoss);

                AddExplicitCast<ulong, long>((a) => { return (long)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<double, long>((a) => { return (long)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<float, long>((a) => { return (long)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<decimal, long>((a) => { return (long)a; }, CastCompatibility.PossibleLoss);
            }
            if (type == typeof(ulong))
            {

                // C# supports ulong arithmetic
                AddBinaryOperation<ulong, ulong, ulong>(TokenType.OperatorPlus, (a, b) => { return a + b; });
                AddBinaryOperation<ulong, ulong, ulong>(TokenType.OperatorMinus, (a, b) => { return a - b; });
                AddBinaryOperation<ulong, ulong, ulong>(TokenType.OperatorMultiply, (a, b) => { return a * b; });
                AddBinaryOperation<ulong, ulong, ulong>(TokenType.OperatorDivide, (a, b) => { return a / b; });
                AddBinaryOperation<ulong, ulong, ulong>(TokenType.OperatorModulo, (a, b) => { return a % b; });
                AddBinaryOperation<ulong, ulong, ulong>(TokenType.OperatorAnd, (a, b) => { return a & b; });
                AddBinaryOperation<ulong, ulong, ulong>(TokenType.OperatorOr, (a, b) => { return a | b; });
                AddBinaryOperation<ulong, ulong, ulong>(TokenType.OperatorXor, (a, b) => { return a ^ b; });
                AddBinaryOperation<ulong, ulong, bool>(TokenType.OperatorEQ, (a, b) => { return a == b; });
                AddBinaryOperation<ulong, ulong, bool>(TokenType.OperatorNE, (a, b) => { return a != b; });
                AddBinaryOperation<ulong, ulong, bool>(TokenType.OperatorGE, (a, b) => { return a >= b; });
                AddBinaryOperation<ulong, ulong, bool>(TokenType.OperatorGT, (a, b) => { return a > b; });
                AddBinaryOperation<ulong, ulong, bool>(TokenType.OperatorLE, (a, b) => { return a <= b; });
                AddBinaryOperation<ulong, ulong, bool>(TokenType.OperatorLT, (a, b) => { return a < b; });

                AddUnaryOperation<ulong, ulong>(TokenType.OperatorNot, (a) => { return ~a; });
                //AddUnaryOperation<ulong, ulong>(TokenType.OperatorMinus, (a) => { return -a; });
                AddUnaryOperation<ulong, ulong>(TokenType.OperatorPlus, (a) => { return a; });

                AddImplicitCast<byte, ulong>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<ushort, ulong>((a) => { return a; }, CastCompatibility.NoLoss);
                AddImplicitCast<uint, ulong>((a) => { return a; }, CastCompatibility.NoLoss);

                AddExplicitCast<sbyte, ulong>((a) => { return (ulong)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<short, ulong>((a) => { return (ulong)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<int, ulong>((a) => { return (ulong)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<long, ulong>((a) => { return (ulong)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<double, ulong>((a) => { return (ulong)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<float, ulong>((a) => { return (ulong)a; }, CastCompatibility.PossibleLoss);
                AddExplicitCast<decimal, ulong>((a) => { return (ulong)a; }, CastCompatibility.PossibleLoss);
            }
            base.DeclareOperators(type);
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
