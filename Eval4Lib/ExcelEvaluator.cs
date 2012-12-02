using Eval4.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4
{
    public class Range
    {
        //private Cell c1;
        //private Cell c2;

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
        private IHasValue mParsed;
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
                mParsed = null;
                var firstChar = string.IsNullOrEmpty(mFormula) ? '\0' : mFormula[0];
                if ((firstChar >= '0' && firstChar <= '9') || firstChar == '.' || firstChar == '+' || firstChar == '-' || firstChar == '=')
                {
                    try
                    {
                        mParsed = Ev.Parse((firstChar == '=' ? mFormula.Substring(1) : mFormula));
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
            else if (mParsed != null)
            {
                object val;
                try
                {
                    val = mParsed.ObjectValue;               
                }
                catch (Exception ex)
                {
                    val = ex;
                }
                if (val is double) return ((double)val).ToString("#,##0.00");
                else if (val == this) return "#Circular reference";
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
                if (mParsed != null) return mParsed.ObjectValue;
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
            if (value is Cell) return (value as Cell).ToDouble();
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
                for (int r = table_array.RowMin; r <= table_array.RowMax; r++)
                {
                    var c = ev.GetCell(table_array.ColMin, r);
                    
                    if (c != null)
                    {
                        if (c.ToDouble() > lookup_value)
                        {
                            foundRow = r - 1;
                            break;
                        }
                        else if (c.ToDouble() == lookup_value)
                        {
                            foundRow = r;
                            break;
                        }
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

    public class ExcelEvaluator : Evaluator<ExcelEvaluator.ExcelToken>
    {
        public enum ExcelToken
        {
            None
        }

        public ExcelEvaluator()
        {
            this.AddEnvironmentFunctions(typeof(ExcelFunctions));
        }

        protected internal override EvaluatorOptions Options
        {
            get
            {
                return EvaluatorOptions.None;
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
            DeclareOperators(typeof(double));
            DeclareOperators(typeof(string));
            base.AddImplicitCast<Cell, double>((a) => a.ToDouble(), CastCompatibility.PossibleLoss);

            base.AddBinaryOperation<Cell, Cell, Range>(TokenType.OperatorColon, (c1, c2) => new Range(c1, c2));
            base.AddImplicitCast<Range, double[]>((a) => a.ToArray(), CastCompatibility.PossibleLoss);
            DeclareOperators(typeof(object));
            
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
                    return new Token(TokenType.OperatorPower);

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
            switch (tt)
            {
                case TokenType.Dot:
                case TokenType.OpenParenthesis:
                case TokenType.OpenBracket:
                    return 10;

                case TokenType.OperatorColon:
                    // :
                    return 9;

                case TokenType.OperatorPlus:
                case TokenType.OperatorMinus:
                    // 	Unary	
                    // +  -
                    return (unary ? 8 : 4);

                case TokenType.OperatorModulo:
                    //*  %
                    return 7;

                case TokenType.OperatorPower:
                    // 	^
                    return 6;

                case TokenType.OperatorMultiply:
                case TokenType.OperatorDivide:
                    // 	* /
                    return 5;
                //case TokenType.OperatorPlus:
                //case TokenType.OperatorMinus:
                //    // 	+ -
                //    return 4;

                case TokenType.OperatorConcat:
                    // 	&
                    return 3;


                case TokenType.OperatorLT:
                case TokenType.OperatorLE:
                case TokenType.OperatorGE:
                case TokenType.OperatorGT:
                case TokenType.OperatorEQ:
                case TokenType.OperatorNE:
                    // 	Relational and type testing	
                    //<  >  <=  >=  is  as
                    return 2;

                case TokenType.Comma:
                    // ,  (the documentation put the comma pretty high but I caon't figure out why it should be
                    return 1;

                default:
                    return 0;
            }
        }

        public override string ConvertToString(object value)
        {
            if (value is Cell) return (value as Cell).ToString();
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
