using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eval4.Core
{
    public class Parser
    {
        internal Evaluator mEvaluator;

        private string mString;
        private int mLen;
        private int mPos;
        internal char mCurChar;
        public int startpos;
        public TokenType type;

        public System.Text.StringBuilder Value = new System.Text.StringBuilder();
        private Expr mParsedExpression;

        internal SyntaxError NewParserException(string msg, Exception ex = null)
        {
            if (ex is SyntaxError)
            {
                msg += ". " + ex.Message;
            }
            else
            {
                msg += " " + " at position " + startpos;
                if ((ex != null))
                {
                    msg += ". " + ex.Message;
                }
            }
            return new SyntaxError(msg, this.mString, this.mPos);
        }

        internal SyntaxError NewUnexpectedToken(string msg = null)
        {
            if (String.IsNullOrEmpty(msg))
            {
                msg = "";
            }
            else
            {
                msg += "; ";
            }
            if (Value.Length == 0)
                throw NewParserException(msg + "Unexpected character '" + mCurChar + "'");
            else
                throw NewParserException(msg + "Unexpected " + type.ToString().Replace('_', ' ') + " : " + Value.ToString());
        }

        public void NextToken(bool unary = false)
        {
            Value.Length = 0;
            do
            {
                startpos = mPos;
                type = mEvaluator.ParseToken(this);
                if (type != TokenType.none)
                    return;
                NextChar();
            } while (true);
        }

        internal void NextChar()
        {
            if (mPos < mLen)
            {
                mCurChar = mString[mPos];
                mEvaluator.CleanUpCharacter(ref mCurChar);

                mPos += 1;
            }
            else
            {
                mCurChar = '\0';
            }
        }

        internal TokenType ParseNumber()
        {
            type = TokenType.Value_number;
            while (mCurChar >= '0' && mCurChar <= '9')
            {
                Value.Append(mCurChar);
                NextChar();
            }
            if (mCurChar == '.')
            {
                Value.Append(mCurChar);
                NextChar();
                while (mCurChar >= '0' && mCurChar <= '9')
                {
                    Value.Append(mCurChar);
                    NextChar();
                }
            }
            return type;
        }

        internal TokenType ParseIdentifierOrKeyword()
        {
            while ((mCurChar >= '0' && mCurChar <= '9') || (mCurChar >= 'a' && mCurChar <= 'z') || (mCurChar >= 'A' && mCurChar <= 'Z') || (mCurChar >= 'A' && mCurChar <= 'Z') || (mCurChar >= 128) || (mCurChar == '_'))
            {
                Value.Append(mCurChar);
                NextChar();
            }
            type = mEvaluator.CheckKeyword(Value.ToString());
            return type;
        }

        internal void ParseString(bool InQuote)
        {
            char OriginalChar = '\0';
            if (InQuote)
            {
                OriginalChar = mCurChar;
                NextChar();
            }

            while (mCurChar != '\0')
            {
                if (InQuote && mCurChar == OriginalChar)
                {
                    NextChar();
                    if (mCurChar == OriginalChar)
                    {
                        Value.Append(mCurChar);
                    }
                    else
                    {
                        //End of String
                        return;
                    }
                }
                else if (mCurChar == '%')
                {
                    NextChar();
                    if (mCurChar == '[')
                    {
                        NextChar();
                        System.Text.StringBuilder SaveValue = Value;
                        int SaveStartPos = startpos;
                        this.Value = new System.Text.StringBuilder();
                        this.NextToken();
                        // restart the tokenizer for the subExpr
                        object subExpr = null;
                        try
                        {
                            // subExpr = mParser.ParseExpr(0, ePriority.none)
                            if (subExpr == null)
                            {
                                this.Value.Append("<nothing>");
                            }
                            else
                            {
                                this.Value.Append(Evaluator.ConvertToString(subExpr));
                            }
                        }
                        catch (Exception ex)
                        {
                            // XML don't like < and >
                            this.Value.Append("[Error " + ex.Message + "]");
                        }
                        SaveValue.Append(Value.ToString());
                        Value = SaveValue;
                        startpos = SaveStartPos;
                    }
                    else
                    {
                        Value.Append('%');
                    }
                }
                else
                {
                    Value.Append(mCurChar);
                    NextChar();
                }
            }
            if (InQuote)
            {
                throw NewParserException("Incomplete string, missing " + OriginalChar + "; String started");
            }
        }

        internal void ParseDate()
        {
            NextChar();
            // eat the #
            while ((mCurChar >= '0' && mCurChar <= '9') || (mCurChar == '/') || (mCurChar == ':') || (mCurChar == ' '))
            {
                Value.Append(mCurChar);
                NextChar();
            }
            if (mCurChar != '#')
            {
                throw NewParserException("Invalid date should be #dd/mm/yyyy#");
            }
            else
            {
                NextChar();
            }
            type = TokenType.Value_date;
        }


        internal void Expect(TokenType tokenType, string msg)
        {
            if (type == tokenType)
            {
                NextToken();
            }
            else
            {
                throw NewUnexpectedToken(msg);
            }
        }

        public Expr ParsedExpression { get { return mParsedExpression; } }

        public Parser(Evaluator evaluator, string source)
        {
            mEvaluator = evaluator;
            mString = source;
            mLen = source.Length;
            mPos = 0;
            // start the machine
            NextChar();
            NextToken();
            Expr res = ParseExpr(null, 0);
            if (type == TokenType.end_of_formula)
            {
                if (res == null)
                    res = new ImmediateExpr<string>(string.Empty);
                mParsedExpression = res;
            }
            else
            {
                throw NewUnexpectedToken();
            }

        }

        internal Expr ParseExpr(Expr Acc, int precedence)
        {
            Expr ValueLeft = null;
            ValueLeft = ParseLeft();
            if (ValueLeft == null)
            {
                throw NewUnexpectedToken("No Expression found");
            }
            ParseDot(ref ValueLeft);
            return ParseRight(Acc, precedence, ValueLeft);
        }

        private Expr ParseRight(Expr Acc, int precedence, Expr ValueLeft)
        {
            while (true)
            {
                TokenType tt = default(TokenType);
                tt = type;
                int opPrecedence = mEvaluator.GetPrecedence(this, tt, unary: false);
                if (precedence >= opPrecedence)
                {
                    // if on we have twice the same operator precedence it is more natural to calculate the left operator first
                    // ie 1+2+3-4 will be calculated ((1+2)+3)-4
                    return ValueLeft;
                }
                else
                {
                    if (!mEvaluator.ParseRight(this, tt, opPrecedence, Acc, ref ValueLeft))
                    {
                        return ValueLeft;
                    }
                }
            }
        }

        private Expr ParseLeft()
        {
            Expr result = null;
            while (type != TokenType.end_of_formula)
            {
                int opPrecedence = mEvaluator.GetPrecedence(this, type, unary: true);
                // we ignore precedence here, not sure if it is valid
                result = mEvaluator.ParseLeft(this, type, opPrecedence);
                if (result != null) return result;
            }
            return null;
        }


        [Flags()]
        private enum CallType
        {
            field = 1,
            method = 2,
            property = 4,
            all = 7
        }

        private bool EmitCallFunction(ref Expr ValueLeft, string funcName, List<Expr> parameters, CallType CallType, bool ErrorIfNotFound)
        {
            Expr newExpr = null;
            if (ValueLeft == null)
            {
                foreach (object environmentFunctions in mEvaluator.mEnvironmentFunctionsList)
                {
                    if (environmentFunctions is Type)
                        newExpr = GetLocalFunction(null, (Type)environmentFunctions, funcName, parameters, CallType);
                    else newExpr = GetLocalFunction(environmentFunctions, environmentFunctions.GetType(), funcName, parameters, CallType);

                    if (newExpr != null) break;
                }
            }
            else
            {
                newExpr = GetLocalFunction(ValueLeft, ValueLeft.SystemType, funcName, parameters, CallType);
            }
            if ((newExpr != null))
            {
                ValueLeft = newExpr;
                return true;
            }
            else
            {
                if (ErrorIfNotFound)
                    throw NewParserException("Variable or method '" + funcName + "' was not found");
                return false;
            }
        }

        private Expr GetLocalFunction(object @base, Type baseType, string funcName, List<Expr> parameters, CallType CallType)
        {
            MemberInfo mi = null;
            mi = GetMemberInfo(baseType, isStatic: true, isInstance: @base != null, func: funcName, parameters: parameters);
            if ((mi != null))
            {
                switch (mi.MemberType)
                {
                    case MemberTypes.Field:
                        if ((CallType & CallType.field) == 0)
                            throw NewParserException("Unexpected Field");
                        break;
                    case MemberTypes.Method:
                        if ((CallType & CallType.method) == 0)
                            throw NewParserException("Unexpected Method");
                        break;
                    case MemberTypes.Property:
                        if ((CallType & CallType.property) == 0)
                            throw NewParserException("Unexpected Property");
                        break;
                    default:
                        throw NewUnexpectedToken(mi.MemberType.ToString() + " members are not supported");
                }

                return CallMethodExpr.GetNew(this, @base, mi, parameters);
            }
            if (@base is IVariableBag && (parameters == null || parameters.Count == 0))
            {
                IEvalValue val = ((IVariableBag)@base).GetVariable(funcName);
                if ((val != null))
                {
                    return new GetVariableExpr(val);
                }
            }
            return null;
        }

        private MemberInfo GetMemberInfo(Type objType, bool isStatic, bool isInstance, string func, List<Expr> parameters)
        {
            BindingFlags bindingAttr = default(BindingFlags);
            bindingAttr = BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public | BindingFlags.InvokeMethod;
            if (isStatic) bindingAttr |= BindingFlags.Static;
            if (isInstance) bindingAttr |= BindingFlags.Instance;
            if (this.mEvaluator.IsCaseSensitive == false)
            {
                bindingAttr = bindingAttr | BindingFlags.IgnoreCase;
            }
            MemberInfo[] mis = null;

            if (func == null)
            {
                mis = objType.GetDefaultMembers();
            }
            else
            {
                mis = objType.GetMember(func, bindingAttr);
            }


            // There is a bit of cooking here...
            // lets find the most acceptable Member
            int Score = 0;
            int BestScore = 0;
            MemberInfo BestMember = null;
            ParameterInfo[] plist = null;
            int idx = 0;

            MemberInfo mi = null;
            for (int i = 0; i <= mis.Length - 1; i++)
            {
                mi = mis[i];

                if (mi is MethodInfo)
                {
                    plist = ((MethodInfo)mi).GetParameters();
                }
                else if (mi is PropertyInfo)
                {
                    plist = ((PropertyInfo)mi).GetIndexParameters();
                }
                else if (mi is FieldInfo)
                {
                    plist = null;
                }
                Score = 10;
                // by default
                idx = 0;
                if (plist == null)
                    plist = new ParameterInfo[] { };
                if (parameters == null)
                    parameters = new List<Expr>();

                ParameterInfo pi = null;
                if (parameters.Count > plist.Length)
                {
                    Score = 0;
                }
                else
                {
                    for (int index = 0; index <= plist.Length - 1; index++)
                    {
                        pi = plist[index];
                        if (idx < parameters.Count)
                        {
                            Score += ParamCompatibility(parameters[idx], pi.ParameterType);
                        }
                        else if (pi.IsOptional)
                        {
                            Score += 10;
                        }
                        else
                        {
                            // unknown parameter
                            Score = 0;
                        }
                        idx += 1;
                    }
                }
                if (Score > BestScore)
                {
                    BestScore = Score;
                    BestMember = mi;
                }
            }
            return BestMember;
        }

        private static int ParamCompatibility(object value, Type type)
        {
            // This function returns a score 1 to 10 to this question
            // Can this Value fit into this type ?
            if (value == null)
            {
                if (object.ReferenceEquals(type, typeof(object)))
                    return 10;
                if (object.ReferenceEquals(type, typeof(string)))
                    return 8;
                return 5;
            }
            else if (object.ReferenceEquals(type, value.GetType()))
            {
                return 10;
            }
            else
            {
                return 5;
            }
        }

        private void ParseDot(ref Expr ValueLeft)
        {
            do
            {
                switch (type)
                {
                    case TokenType.dot:
                        NextToken();
                        break;
                    case TokenType.open_parenthesis:
                        break;
                    // fine this is either an array or a default property
                    default:
                        return;
                }
                ParseIdentifier(ref ValueLeft);
            } while (true);
        }

        internal void ParseIdentifier(ref Expr ValueLeft)
        {
            // first check functions
            List<Expr> parameters = null;
            // parameters... 
            //Dim types As New ArrayList
            string func = Value.ToString();
            NextToken();
            bool isBrackets = false;
            parameters = ParseParameters(ref isBrackets);
            if ((parameters != null))
            {
                List<Expr> EmptyParameters = new List<Expr>();
                bool ParamsNotUsed = false;
                if (mEvaluator.UseParenthesisForArrays)
                {
                    // in vb we don't know if it is array or not as we have only parenthesis
                    // so we try with parameters first
                    if (!EmitCallFunction(ref ValueLeft, func, parameters, CallType.all, ErrorIfNotFound: false))
                    {
                        // and if not found we try as array or default member
                        EmitCallFunction(ref ValueLeft, func, EmptyParameters, CallType.all, ErrorIfNotFound: true);
                        ParamsNotUsed = true;
                    }
                }
                else
                {
                    if (isBrackets)
                    {
                        if (!EmitCallFunction(ref ValueLeft, func, parameters, CallType.property, ErrorIfNotFound: false))
                        {
                            EmitCallFunction(ref ValueLeft, func, EmptyParameters, CallType.all, ErrorIfNotFound: true);
                            ParamsNotUsed = true;
                        }
                    }
                    else
                    {
                        if (!EmitCallFunction(ref ValueLeft, func, parameters, CallType.field | CallType.method, ErrorIfNotFound: false))
                        {
                            EmitCallFunction(ref ValueLeft, func, EmptyParameters, CallType.all, ErrorIfNotFound: true);
                            ParamsNotUsed = true;
                        }
                    }
                }
                // we found a function without parameters 
                // so our parameters must be default property or an array
                Type t = ValueLeft.SystemType;
                if (ParamsNotUsed)
                {
                    if (t.IsArray)
                    {
                        if (parameters.Count == t.GetArrayRank())
                        {
                            ValueLeft = new GetArrayEntryExpr(ValueLeft, parameters);
                        }
                        else
                        {
                            throw NewParserException("This array has " + t.GetArrayRank() + " dimensions");
                        }
                    }
                    else
                    {
                        MemberInfo mi = null;
                        mi = GetMemberInfo(t, isStatic: true, isInstance: true, func: null, parameters: parameters);
                        if ((mi != null))
                        {
                            ValueLeft = CallMethodExpr.GetNew(this, ValueLeft, mi, parameters);
                        }
                        else
                        {
                            throw NewParserException("Parameters not supported here");
                        }
                    }
                }
            }
            else
            {
                EmitCallFunction(ref ValueLeft, func, parameters, CallType.all, ErrorIfNotFound: true);
            }
        }

        internal List<Expr> ParseParameters(ref bool brackets)
        {
            List<Expr> parameters = null;
            Expr Valueleft = null;
            TokenType lClosing = default(TokenType);

            if (type == TokenType.open_parenthesis
                || (type == TokenType.open_bracket && !mEvaluator.UseParenthesisForArrays))
            {
                switch (type)
                {
                    case TokenType.open_bracket:
                        lClosing = TokenType.close_bracket;
                        brackets = true;
                        break;
                    case TokenType.open_parenthesis:
                        lClosing = TokenType.close_parenthesis;
                        break;
                }
                parameters = new List<Expr>();
                NextToken();
                //eat the parenthesis
                do
                {
                    if (type == lClosing)
                    {
                        // good we eat the end parenthesis and continue ...
                        NextToken();
                        return parameters;
                    }
                    Valueleft = ParseExpr(null, 0);
                    parameters.Add(Valueleft);

                    if (type == lClosing)
                    {
                        // good we eat the end parenthesis and continue ...
                        NextToken();
                        return parameters;
                    }
                    else if (type == TokenType.comma)
                    {
                        NextToken();
                    }
                    else
                    {
                        throw NewUnexpectedToken(lClosing.ToString() + " not found");
                    }
                } while (true);
            }
            return parameters;
        }

    }

    public enum ParserSyntax
    {
        CSharp,
        Vb
    }

    //public class VariableNotFoundException : Exception
    //{

    //    public readonly string VariableName;
    //    public VariableNotFoundException(string variableName, Exception innerException = null)
    //        : base(variableName + " was not found", null)
    //    {
    //        this.VariableName = variableName;
    //    }
    //}

    //public enum Precedence
    //{
    //    None,
    //    Xor,
    //    Or,
    //    And,
    //    Not,
    //    Equality,
    //    Arithmeticshift,
    //    Concat,
    //    Plusminus,
    //    Modulo,
    //    Integerdiv,
    //    Muldiv,
    //    Percent, // this is my own opertor here not vb
    //    Unaryminus,
    //    Exponent,
    //    Parenthesis,
    //    Undefined
    //}

    public enum TokenType
    {
        none,
        end_of_formula,
        operator_plus,
        operator_minus,
        operator_mul,
        operator_div,
        operator_percent,
        operator_mod,
        open_parenthesis,
        comma,
        dot,
        close_parenthesis,
        operator_ne,
        operator_gt,
        operator_ge,
        operator_eq,
        operator_le,
        operator_lt,
        operator_and,
        operator_andalso,
        operator_or,
        operator_orelse,
        operator_xor,
        operator_not,
        operator_concat,
        operator_if,
        operator_colon,
        operator_assign,
        Value_identifier,
        Value_true,
        Value_false,
        Value_number,
        Value_string,
        Value_date,
        open_bracket,
        close_bracket,
        unary_plus,
        unary_minus,
        shift_left,
        shift_right,
        @new,
        unary_not,
        unary_tilde,
        operator_tilde,
        backslash,
        exponent

    }

}
