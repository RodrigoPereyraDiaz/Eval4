using Eval4.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Excel
{
    public enum ExcelToken
    {
        None
    }

    public class Range
    {
        private Cell c1;
        private Cell c2;

        int colMin, colMax;
        int rowMin, rowMax;

        public int ColMin { get { return colMin; } }
        public int ColMax { get { return colMax; } }
        public int RowMin { get { return rowMin; } }
        public int RowMax { get { return rowMax; } }

        public Range(Cell c1, Cell c2)
        {
            GetMinMax(c1.Col, c2.Col, out colMin, out colMax);
            GetMinMax(c1.Row, c2.Row, out rowMin, out rowMax);
            Ev = c1.Ev;
        }

        private void GetMinMax(int v1, int v2, out int min, out int max)
        {
            if (v1 < v2) { min = v1; max = v2; }
            else { min = v2; max = v1; }
        }

        internal double[] ToArray()
        {
            var result = new List<double>();
            for (int r = rowMin; r <= rowMax; r++)
            {
                for (int c = colMin; c <= colMax; c++)
                {
                    var cell = Ev.GetCell(c, r);
                    if (cell != null) result.Add(cell.ToDouble());
                }
            }
            return result.ToArray();
        }

        public ExcelEvaluator Ev { get; private set; }
    }

    public class Cell
    {
        public ExcelEvaluator Ev { get; private set; }
        private string mFormula;
        private string mName;
        private IHasValue mValue;
        public Exception Exception;
        public int Row { get; private set; }
        public int Col { get; private set; }

        public Cell(ExcelEvaluator ev, int col, int row, string formula = null)
        {
            this.Row = row;
            this.Col = col;
            this.Ev = ev;
            mName = Cell.GetCellName(col, row);
            ev.SetVariable(mName, this);
            if (formula != null) this.Formula = formula;
        }

        public static string GetCellName(int col, int row)
        {
            return GetColName(col) + (row + 1);
        }

        public static bool GetCellPos(string name, out int x, out int y)
        {
            int i = 0;
            int row = 0, col = 0;

            while (i < name.Length)
            {
                char c = name[i];
                if (c >= 'A' && c <= 'Z') col = col * 26 + (c - 'A');
                else if (c >= 'a' && c <= 'z') col = col * 26 + (c - 'a');
                else break;
                i++;
            }
            while (i < name.Length)
            {
                char c = name[i];
                if (c >= '0' && c <= '9') row = row * 10 + (c - '0');
                else break;
                i++;
            }
            row--; //rows start a 1            
            if (i == name.Length && col >= 0 && row >= 0)
            {
                x = col;
                y = row;
                return true;
            }
            else
            {
                x = 0;
                y = 0;
                return false;
            }
        }

        public static string GetColName(int x)
        {
            string result = string.Empty;
            if (x < 26)
            {
                result = ((char)(65 + x)).ToString();
            }
            else if (x <= 26 * 26)
            {
                var x1 = (x / 26);
                var x2 = (x % 26);

                result = ((char)(65 + x1)).ToString() + ((char)(65 + x2)).ToString();
            }
            return result;
        }

        public string Formula
        {
            get
            {
                return mFormula;
            }
            set
            {
                mFormula = value;
                Exception = null;
                mValue = null;
                var firstChar = string.IsNullOrEmpty(mFormula) ? '\0' : mFormula[0];
                if ((firstChar >= '0' && firstChar <= '9') || firstChar == '.' || firstChar == '+' || firstChar == '-' || firstChar == '=')
                {
                    try
                    {
                        mValue = Ev.Parse((firstChar == '=' ? mFormula.Substring(1) : mFormula));
                    }
                    catch (Exception ex)
                    {
                        Exception = ex;
                    }
                }
            }
        }

        public override string ToString()
        {
            if (Exception != null) return Exception.Message;
            else if (mValue != null)
            {
                object val = mValue.ObjectValue;
                if (val is double) return ((double)val).ToString("#,##0.00");
                else return val.ToString();
            }
            else
            {
                if (mFormula != null && mFormula.StartsWith("'")) return mFormula.Substring(1);
                else return mFormula;
            }
        }

        public object ValueObject
        {
            get
            {
                if (mValue != null) return mValue.ObjectValue;
                else return mFormula;
            }
        }

        static DateTime EPOCH = new DateTime(1900, 1, 1);

        internal double ToDouble()
        {
            var value = ValueObject;
            if (value is double) return (double)value;
            if (value is bool) return ((bool)value) ? 1 : 0;
            if (value is DateTime) return ((DateTime)value).Subtract(EPOCH).TotalDays;
            return double.NaN;
        }
    }


    public static class ExcelFunctions
    {
        public static double Sum(params double[] items)
        {
            return items.Where(a => !double.IsNaN(a)).Sum();
        }

        public static double Average(params double[] items)
        {
            return items.Where(a => !double.IsNaN(a)).Average();
        }

        public static Cell VLookup(double lookup_value, Range table_array, int column_index, bool range_lookup)
        {
            var ev = table_array.Ev;
            int foundRow = -1;
            if (range_lookup)
            {
                for (int r = table_array.RowMin; r < table_array.RowMax; r++)
                {
                    var c = ev.GetCell(table_array.ColMin, r);
                    if (c != null && c.ToDouble() == lookup_value)
                    {
                        foundRow = r;
                        break;
                    }
                }
            }
            else
            {
                int min = table_array.RowMin;
                int max = table_array.RowMax;
                while (min < max)
                {
                    int mid = (min + max) / 2;
                    var c = ev.GetCell(table_array.ColMin, mid);
                    var val = c.ToDouble();
                    if (lookup_value < val)
                    {
                        max = mid - 1;
                    }
                    else if (lookup_value > val)
                    {
                        min = mid + 1;
                    }
                    else
                    {
                        foundRow = mid;
                        break;
                    }
                }
                if (foundRow < 0) foundRow = min;
            }
            if (foundRow >= 0)
            {
                var c = ev.GetCell(table_array.ColMin + column_index - 1, foundRow);
                return c;
            }
            return null;
        }
    }
    public class ExcelEvaluator : Evaluator<ExcelToken>
    {

        public ExcelEvaluator()
        {
            this.AddEnvironmentFunctions(typeof(ExcelFunctions));
        }

        protected internal override EvaluatorOptions Options
        {
            get
            {
                return EvaluatorOptions.BooleanLogic
                    //| EvaluatorOptions.CaseSensitive
                    | EvaluatorOptions.DoubleValues
                    //| EvaluatorOptions.IntegerValues
                    | EvaluatorOptions.ObjectValues
                    | EvaluatorOptions.StringValues;
            }
        }

        protected override object LastChanceFindVariable(string funcName)
        {
            int x, y;
            if (Cell.GetCellPos(funcName, out x, out y))
            {
                return new Cell(this, x, y);
            }
            return null;
        }

        protected override void DeclareOperators()
        {
            base.DeclareOperators();
            base.AddImplicitCast<Cell, double>((a) => a.ToDouble(), CastCompatibility.PossibleLoss);

            base.AddBinaryOperation<Cell, Cell, Range>(TokenType.OperatorColon, (c1, c2) => new Range(c1, c2));
            base.AddImplicitCast<Range, double[]>((a) => a.ToArray(), CastCompatibility.PossibleLoss);
        }

        public override bool UseParenthesisForArrays
        {
            get { return false; }
        }

        public override IHasValue ParseUnaryExpression(Token token, int precedence)
        {
            switch (token.Type)
            {
                default:
                    return base.ParseUnaryExpression(token, precedence);
            }

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
                    return new Token(TokenType.OperatorIf);

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
                switch (keyword.ToLower().ToString())
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
                    IHasValue thenExpr = ParseExpr(null, 0);
                    if (!Expect(TokenType.OperatorColon, "Missing : in ? expression test ? valueIfTrue : valueIfFalse.", ref valueLeft))
                        return;
                    IHasValue elseExpr = ParseExpr(null, 0);
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
                    return 16;

                case TokenType.OperatorColon:
                    return 13;

                case TokenType.OperatorPlus:
                case TokenType.OperatorMinus:
                    return (unary ? 15 : 12);

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

        public override string ConvertToString(object value)
        {
            if (value is Cell)
            {
                return ConvertToString(((Cell)value).ValueObject);
            }
            else return base.ConvertToString(value);
        }
        public void SetCell(string cellName, string formula)
        {
            int x, y;
            if (Cell.GetCellPos(cellName, out x, out y))
            {
                var cell = new Cell(this, x, y, formula);
                SetVariable<Cell>(cellName, cell);
            }
            else throw new Exception(string.Format("Invalid Cell \"{0}\"", cellName));
        }

        internal Cell GetCell(int x, int y)
        {
            string cellName = Cell.GetCellName(x, y);
            IHasValue<Cell> var = base.GetVariable<Cell>(cellName);
            if (var == null) return null;
            return var.Value;
        }
    }
}
