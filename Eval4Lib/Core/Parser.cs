using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Eval4.Core
{
 
    public class Parser
    {
        internal Evaluator mEvaluator;

        protected string mString;
        protected int mLen;
        protected int mPos;
        public char mCurChar;
        public int startpos;
        public Token mCurToken;

        protected IHasValue mParsedExpression;

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
            if (string.IsNullOrEmpty(mCurToken.ValueString))
                throw NewParserException(msg + "Unexpected character '" + mCurChar + "'");
            else
                throw NewParserException(msg + "Unexpected " + mCurToken.ToString());
        }

        public void NextToken()
        {
            do
            {
                startpos = mPos;
                mCurToken = mEvaluator.ParseToken(this);
                if (mCurToken.Type != TokenType.None)
                    return;
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

        internal Token ParseNumber(bool afterDot = false)
        {
            var sb = new StringBuilder();
            if (!afterDot)
            {
                while (mCurChar >= '0' && mCurChar <= '9')
                {
                    sb.Append(mCurChar);
                    NextChar();
                }
                if (mCurChar == '.')
                {
                    afterDot = true;
                    NextChar();
                }
            }
            if (afterDot)
            {
                sb.Append('.');
                while (mCurChar >= '0' && mCurChar <= '9')
                {
                    sb.Append(mCurChar);
                    NextChar();
                }
            }
            return mEvaluator.NewToken(TokenType.ValueNumber, sb.ToString());
        }

        internal Token ParseIdentifierOrKeyword()
        {
            var sb = new StringBuilder();
            while ((mCurChar >= '0' && mCurChar <= '9') || (mCurChar >= 'a' && mCurChar <= 'z') || (mCurChar >= 'A' && mCurChar <= 'Z') || (mCurChar >= 'A' && mCurChar <= 'Z') || (mCurChar >= 128) || (mCurChar == '_'))
            {
                sb.Append(mCurChar);
                NextChar();
            }
            mCurToken = mEvaluator.CheckKeyword(sb.ToString());
            return mCurToken;
        }

        internal Token ParseString(bool InQuote)
        {
            var sb = new StringBuilder();
            char OriginalChar = '\0';
            if (InQuote)
            {
                OriginalChar = mCurChar;
                NextChar();
            }
            List<object> bits = new List<object>();

            while (mCurChar != '\0')
            {
                if (InQuote && mCurChar == OriginalChar)
                {
                    NextChar();
                    if (mCurChar == OriginalChar)
                    {
                        sb.Append(mCurChar);
                    }
                    else
                    {
                        //End of String
                        return mEvaluator.NewToken(TokenType.ValueString, sb.ToString());
                    }
                }
                else if (mCurChar == '%')
                {
                    NextChar();
                    if (mCurChar == '[')
                    {
                        NextChar();
                        System.Text.StringBuilder SaveValue = sb;
                        int SaveStartPos = startpos;
                        sb = new System.Text.StringBuilder();
                        this.NextToken();
                        // restart the tokenizer for the subExpr
                        object subExpr = null;
                        try
                        {
                            // subExpr = mParser.ParseExpr(0, ePriority.none)
                            if (subExpr == null)
                            {
                                sb.Append("<nothing>");
                            }
                            else
                            {
                                sb.Append(Evaluator.ConvertToString(subExpr));
                            }
                        }
                        catch (Exception ex)
                        {
                            // XML don't like < and >
                            sb.Append("[Error " + ex.Message + "]");
                        }
                        SaveValue.Append(sb.ToString());
                        sb = SaveValue;
                        startpos = SaveStartPos;
                    }
                    else
                    {
                        sb.Append('%');
                    }
                }
                else
                {
                    sb.Append(mCurChar);
                    NextChar();
                }
            }

            if (InQuote)
            {
                throw NewParserException("Incomplete string, missing " + OriginalChar + "; String started");
            }
            if (sb.Length > 0 && bits.Count>0) {
                bits.Add(sb.ToString());
                sb.Length = 0;
                foreach (object o in bits)
                {
                    if (o is string) sb.Append(o as string);
                    else sb.Append((o as IHasValue).ObjectValue);
                }
            }
            return mEvaluator.NewToken(TokenType.ValueString, sb.ToString());
        }

        internal void Expect(TokenType tokenType, string msg)
        {
            if (mCurToken.Type == tokenType)
            {
                NextToken();
            }
            else
            {
                throw NewUnexpectedToken(msg);
            }
        }

        public IHasValue ParsedExpression { get { return mParsedExpression; } }

        public Parser(Evaluator evaluator, string source, bool sourceIsTextTemplate=false)
        {
            mEvaluator = evaluator;
            mString = source;
            mLen = source.Length;
            mPos = 0;
            // start the machine
            NextChar();
            NextToken();
            IHasValue res;
            if (sourceIsTextTemplate) res = ParseTemplate();
            else res = ParseExpr(null, 0);
            if (mCurToken.Type == TokenType.EndOfFormula)
            {
                if (res == null)
                    res = new ConstantExpr<string>(string.Empty);
                mParsedExpression = res;
            }
            else
            {
                throw NewUnexpectedToken();
            }

        }

        private IHasValue ParseTemplate()
        {
            var token = ParseString(false);
            return (IHasValue)token.ValueObject;
        }

        internal IHasValue ParseExpr(IHasValue Acc, int precedence)
        {
            IHasValue ValueLeft = null;
            ValueLeft = ParseLeft(precedence);
            if (ValueLeft == null)
            {
                throw NewUnexpectedToken("No Expression found");
            }
            while (typeof(IHasValue).IsAssignableFrom(ValueLeft.SystemType))
            {
                ValueLeft = (IHasValue)ValueLeft.ObjectValue;
            }
            ParseDot(ref ValueLeft);
            return ParseRight(Acc, precedence, ValueLeft);
        }

        protected IHasValue ParseRight(IHasValue Acc, int precedence, IHasValue ValueLeft)
        {
            while (true)
            {
                //TokenType tt = default(TokenType);
                //tt = mCurToken.Type;
                int opPrecedence = mEvaluator.GetPrecedence(mCurToken, unary: false);
                if (precedence >= opPrecedence)
                {
                    // if on we have twice the same operator precedence it is more natural to calculate the left operator first
                    // ie 1+2+3-4 will be calculated ((1+2)+3)-4
                    return ValueLeft;
                }
                else
                {
                    if (!mEvaluator.ParseRight(this, mCurToken, opPrecedence, Acc, ref ValueLeft))
                    {
                        return ValueLeft;
                    }
                }
            }
        }

        protected IHasValue ParseLeft(int precedence)
        {
            IHasValue result = null;
            while (mCurToken.Type != TokenType.EndOfFormula)
            {
                // we ignore precedence here, not sure if it is valid
                result = mEvaluator.ParseLeft(this, mCurToken, precedence);
                if (result != null) return result;
            }
            return null;
        }


        [Flags()]
        protected enum CallType
        {
            field = 1,
            method = 2,
            property = 4,
            all = 7
        }

        protected bool EmitCallFunction(ref IHasValue ValueLeft, string funcName, List<IHasValue> parameters, CallType CallType, bool ErrorIfNotFound)
        {
            IHasValue newExpr = null;

            if (ValueLeft == null)
            {
                for (int i = mEvaluator.mEnvironmentFunctionsList.Count - 1; i >= 0; i--)
                {
                    var environmentFunctions = mEvaluator.mEnvironmentFunctionsList[i];
                    if (environmentFunctions is Type)
                    {
                        newExpr = GetMember(null, (Type)environmentFunctions, funcName, parameters, CallType);
                    }
                    else if (environmentFunctions is IHasValue)
                    {
                        var hasValue = environmentFunctions as IHasValue;
                        newExpr = GetMember(hasValue, hasValue.SystemType, funcName, parameters, CallType);
                    }
                    else if (environmentFunctions is IVariableBag)
                    {
                        if (parameters == null || parameters.Count == 0)
                        {
                            newExpr = (environmentFunctions as IVariableBag).GetVariable(funcName);
                        }

                    }
                    else
                    {
                        throw NewParserException("Invalid Environment functions.");
                    }
                    if (newExpr != null) break;
                }
            }
            else if (ValueLeft.SystemType == typeof(StaticFunctionsWrapper))
            {
                newExpr = GetMember(null, (ValueLeft.ObjectValue as StaticFunctionsWrapper).type, funcName, parameters, CallType);
            }
            else
            {
                newExpr = GetMember(ValueLeft, ValueLeft.SystemType, funcName, parameters, CallType);
            }
            if ((newExpr != null))
            {
                ValueLeft = newExpr;
                return true;
            }
            else
            {
                if (ErrorIfNotFound)
                    throw NewParserException("No Variable or public method '" + funcName + "' was not found.");
                return false;
            }
        }

        protected IHasValue GetMember(IHasValue @base, Type baseType, string funcName, List<IHasValue> parameters, CallType CallType)
        {
            MemberInfo mi = null;
            Type resultType;
            if (GetMemberInfo(baseType, isStatic: true, isInstance: @base != null, func: funcName, parameters: parameters, mi: out mi, resultType: out resultType))
            {

                switch (mi.MemberType)
                {
                    case MemberTypes.Field:
                        if ((CallType & CallType.field) == 0)
                            throw NewParserException("Unexpected Field");
                        resultType = (mi as FieldInfo).FieldType;
                        break;
                    case MemberTypes.Method:
                        if ((CallType & CallType.method) == 0)
                            throw NewParserException("Unexpected Method");
                        resultType = (mi as MethodInfo).ReturnType;
                        break;
                    case MemberTypes.Property:
                        if ((CallType & CallType.property) == 0)
                            throw NewParserException("Unexpected Property");
                        resultType = (mi as PropertyInfo).PropertyType;
                        break;
                    default:
                        throw NewUnexpectedToken(mi.MemberType.ToString() + " members are not supported");
                }
                var res = GetNewCallMethodExpr(resultType, @base, mi, parameters);
                return res;
            }
            if (@base is IVariableBag && (parameters == null || parameters.Count == 0))
            {
                IHasValue val = ((IVariableBag)@base).GetVariable(funcName);
                if ((val != null))
                {
                    return val; // new GetVariableExpr(val);
                }
            }
            return null;
        }

        private IHasValue GetNewCallMethodExpr(Type resultType, IHasValue @base, MemberInfo mi, List<IHasValue> parameters)
        {
            var t = typeof(CallMethodExpr<>).MakeGenericType(resultType);
            return (IHasValue)Activator.CreateInstance(t, @base, mi, parameters);
        }

        protected bool GetMemberInfo(Type objType, bool isStatic, bool isInstance, string func, List<IHasValue> parameters, out MemberInfo mi, out Type resultType)
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

            mi = null;
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
                    parameters = new List<IHasValue>();

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
            mi = BestMember;
            if (mi is MethodInfo) resultType = (mi as MethodInfo).ReturnType;
            else if (mi is PropertyInfo) resultType = (mi as PropertyInfo).PropertyType;
            else if (mi is FieldInfo) resultType = (mi as FieldInfo).FieldType;
            else resultType = null;
            return resultType != null;
        }

        protected static int ParamCompatibility(object value, Type type)
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

        protected void ParseDot(ref IHasValue ValueLeft)
        {
            do
            {
                switch (mCurToken.Type)
                {
                    case TokenType.Dot:
                        NextToken();
                        break;
                    case TokenType.OpenParenthesis:
                        break;
                    // fine this is either an array or a default property
                    default:
                        return;
                }
                ParseIdentifier(ref ValueLeft);
            } while (true);
        }

        internal void ParseIdentifier(ref IHasValue ValueLeft)
        {
            // first check functions
            List<IHasValue> parameters = null;
            // parameters... 
            //Dim types As New ArrayList
            string func = mCurToken.ValueString;
            NextToken();
            bool isBrackets = false;
            Type resultType;
            parameters = ParseParameters(ref isBrackets);
            if ((parameters != null))
            {
                List<IHasValue> EmptyParameters = new List<IHasValue>();
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
                        if (GetMemberInfo(t, isStatic: true, isInstance: true, func: null, parameters: parameters, mi: out mi, resultType: out resultType))
                        {
                            ValueLeft = GetNewCallMethodExpr(resultType, ValueLeft, mi, parameters);
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

        internal List<IHasValue> ParseParameters(ref bool brackets)
        {
            List<IHasValue> parameters = null;
            IHasValue Valueleft = null;
            TokenType lClosing = default(TokenType);

            if (mCurToken.Type == TokenType.OpenParenthesis
                || (mCurToken.Type == TokenType.OpenBracket && !mEvaluator.UseParenthesisForArrays))
            {
                switch (mCurToken.Type)
                {
                    case TokenType.OpenBracket:
                        lClosing = TokenType.CloseBracket;
                        brackets = true;
                        break;
                    case TokenType.OpenParenthesis:
                        lClosing = TokenType.CloseParenthesis;
                        break;
                }
                parameters = new List<IHasValue>();
                NextToken();  //eat the parenthesis
                do
                {
                    if (mCurToken.Type == lClosing)
                    {
                        // good we eat the end parenthesis and continue ...
                        NextToken();
                        return parameters;
                    }
                    Valueleft = ParseExpr(null, 0);
                    parameters.Add(Valueleft);

                    if (mCurToken.Type == lClosing)
                    {
                        // good we eat the end parenthesis and continue ...
                        NextToken();
                        return parameters;
                    }
                    else if (mCurToken.Type == TokenType.Comma)
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

    public enum TokenType
    {
        None,
        EndOfFormula,
        OperatorPlus,
        OperatorMinus,
        OperatorMultiply,
        OperatorDivide,
        OperatorModulo,
        OpenParenthesis,
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
        ValueNumber,
        ValueString,
        ValueDate,
        OpenBracket,
        CloseBracket,
        Comma,
        Dot,
        CloseParenthesis,
        ShiftLeft,
        ShiftRight,
        New,
        BackSlash,
        Exponent,
        ImplicitCast,
        ExplicitCast,
        Custom,
    }

}
