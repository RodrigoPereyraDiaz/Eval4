using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Core
{
    public enum TokenType
    {
        Undefined,
        Eof,
        OperatorPlus,
        OperatorMinus,
        OperatorMultiply,
        OperatorDivide,
        OperatorModulo,
        OperatorNE,
        OperatorGT,
        OperatorGE,
        OperatorEQ,
        OperatorLE,
        OperatorLT,
        OperatorAnd,
        OperatorAndAlso,
        OperatorOr,
        OperatorOrElse,
        OperatorXor,
        OperatorConcat,
        OperatorIf,
        OperatorColon,
        OperatorAssign,
        OperatorNot,
        OperatorTilde,
        ValueIdentifier,
        ValueTrue,
        ValueFalse,
        ValueInteger,
        ValueDecimal,
        ValueString,
        ValueDate,
        OpenBracket,
        CloseBracket,
        Comma,
        Dot,
        OpenParenthesis,
        CloseParenthesis,
        ShiftLeft,
        ShiftRight,
        New,
        BackSlash,
        Exponent,
        ImplicitCast,
        ExplicitCast,
        Custom,
        SyntaxError,
        SemiColon,
        UnrecognisedCharacter
    }

}
