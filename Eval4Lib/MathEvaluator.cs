using System;
using System.Collections.Generic;
using System.Text;
using Eval4.Core;
using System.Linq;

namespace Eval4
{
    
    public class MathEvaluator : Evaluator<MathEvaluator.MathToken>
    {

        public MathEvaluator()
        {
        }

        public enum MathToken
        {
            Transpose,        // '
            ElementWiseAdd,   // .+
            ElementWiseSub,   // .-
            ElementWiseMul,   // .*
            ElementWiseDiv,   // ./
            ElementWisePower  // .^
        }
        
        protected internal override EvaluatorOptions Options
        {
            get
            {
                return EvaluatorOptions.None;
            }
        }

        protected override void DeclareOperators()
        {
            DeclareOperators(typeof(bool));
            DeclareOperators(typeof(double));
                        
            AddBinaryOperation<Matrix, Matrix, Matrix>(TokenType.OperatorPlus, (a, b) => a.ElementWiseAdd(b));
            AddBinaryOperation<double, Matrix, Matrix>(TokenType.OperatorPlus, (a, b) => b.ScalarAdd(a));
            AddBinaryOperation<Matrix, double, Matrix>(TokenType.OperatorPlus, (a, b) => a.ScalarAdd(b));
            AddBinaryOperation<Matrix, Matrix, Matrix>(TokenType.OperatorMinus, (a, b) => a.ElementWiseSubtract(b));
            AddBinaryOperation<double, Matrix, Matrix>(TokenType.OperatorMinus, (a, b) => b.ScalarSubtract(a).ScalarNeg());
            AddBinaryOperation<Matrix, double, Matrix>(TokenType.OperatorMinus, (a, b) => a.ScalarSubtract(b));
            AddBinaryOperation<Matrix, Matrix, Matrix>(TokenType.OperatorMultiply, (a, b) => a.Product(b));
            AddBinaryOperation<Matrix, double, Matrix>(TokenType.OperatorMultiply, (a, b) => a.ScalarMultiply(b));
            AddBinaryOperation<double, Matrix, Matrix>(TokenType.OperatorMultiply, (a, b) => b.ScalarMultiply(a));
            AddBinaryOperation<Matrix, double, Matrix>(TokenType.OperatorMultiply, (a, b) => a.ScalarPower(b));
            AddBinaryOperation<Matrix, double, Matrix>(TokenType.OperatorDivide, (a, b) => a.ScalarDivide(b));
            AddBinaryOperation<Matrix, double, Matrix>(TokenType.OperatorPower, (a, b) => a.ScalarPower(b));
            
            AddBinaryOperation<Matrix, Matrix, Matrix>(MathToken.ElementWiseAdd, (a, b) => a.ElementWiseAdd(b));
            AddBinaryOperation<Matrix, Matrix, Matrix>(MathToken.ElementWiseSub, (a, b) => a.ElementWiseSubtract(b));
            AddBinaryOperation<Matrix, Matrix, Matrix>(MathToken.ElementWiseMul, (a, b) => a.ElementWiseMultiply(b));
            AddBinaryOperation<Matrix, Matrix, Matrix>(MathToken.ElementWiseDiv, (a, b) => a.ElementWiseDivide(b));
            AddBinaryOperation<Matrix, Matrix, Matrix>(MathToken.ElementWisePower, (a, b) => a.ElementWisePower(b));

            DeclareOperators(typeof(string));
            DeclareOperators(typeof(object));
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

                case '\'':
                    NextChar();
                    return new Token(MathToken.Transpose);

                case '.':
                    switch (Char(1))
                    {
                        case '+':
                            NextChar(2);
                            return new Token(MathToken.ElementWiseAdd);
                        case '-':
                            NextChar(2);
                            return new Token(MathToken.ElementWiseSub);
                        case '*':
                            NextChar(2);
                            return new Token(MathToken.ElementWiseMul);
                        case '/':
                            NextChar(2);
                            return new Token(MathToken.ElementWiseDiv);
                        case '^':
                            NextChar(2);
                            return new Token(MathToken.ElementWisePower);
                        default:
                            return base.ParseToken();
                    }
                default:
                    return base.ParseToken();

            }
        }

        public override Token CheckKeyword(string keyword)
        {
            {
                switch (keyword)
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

        protected override IHasValue ParseLeft(Token token, ref IHasValue result)
        {
            switch (token.Type)
            {
                case TokenType.OpenBracket:
                    return ParseMatrix();

                default:
                    return base.ParseLeft(token, ref result);
            }
        }

        protected override void ParseRight(Token tk, int opPrecedence, IHasValue Acc, ref IHasValue valueLeft)
        {
            var tt = tk.Type;
            switch (tt)
            {
                case TokenType.Custom:
                    switch (tk.CustomType)
                    {
                        case MathToken.Transpose:
                            NextToken();
                            if (EmitDelegateExpr(ref valueLeft, new Func<Matrix, Matrix>(a => a.Transpose()),"'")) return;
                            break;
                        default:
                            base.ParseRight(tk, opPrecedence, Acc, ref valueLeft);
                            break;
                    }
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

                case TokenType.Custom:
                    switch (token.CustomType)
                    {
                        case MathToken.Transpose:
                            return 14; // not really sure
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }

        internal IHasValue ParseMatrix()
        {
            List<IEnumerable<double>> result = new List<IEnumerable<double>>();
            var curRow = new List<double>();
            result.Add(curRow);
            NextToken();  //eat the parenthesis
            do
            {
                if (mCurToken.Type == TokenType.CloseBracket)
                {
                    // good we eat the end parenthesis and continue ...
                    NextToken();
                    break;
                }
                var value = ParseExpr(null, 0);
                curRow.Add((double)value.ObjectValue);

                switch (mCurToken.Type)
                {
                    case TokenType.CloseBracket:
                        // good we eat the end parenthesis and continue ...
                        continue;
                    case TokenType.Comma:
                        NextToken();
                        break;
                    case TokenType.SemiColon:
                        NextToken();
                        curRow = new List<double>();
                        result.Add(curRow);
                        break;
                    default:
                        return NewUnexpectedToken("Character ']' not found");
                }
            } while (true);


            var matrix = new Matrix(result);

            return new ConstantExpr<Matrix>(matrix);
        }
    }

    // Immutable matrix
    public class Matrix : IEquatable<Matrix>
    {
        private int _rowCount;
        private int _columnCount;
        private double[][] _data;
        private readonly static Random _rnd = new Random();

        public Matrix(int rowCount, int columnCount)
        {
            if (rowCount < 1 || columnCount < 1)
            {
                throw new Exception("Invalid matrix size(" + rowCount + "," + columnCount + ")");
            }
            _rowCount = rowCount;
            _columnCount = columnCount;
            _data = new double[rowCount][];
            for (var r = 0; r < rowCount; r++)
            {
                _data[r] = new double[columnCount];
            }            
        }

        public Matrix(IEnumerable<IEnumerable<double>> result)
        {
            _rowCount = result.Count();
            _columnCount = result.Max(l => l.Count());
            int row = 0;
            _data = new double[_rowCount][];
            foreach (var l in result)
            {
                int col = 0;
                var newCol = new double[_columnCount];
                foreach (var v in l)
                {
                    newCol[col++] = v;
                }
                _data[row++] = newCol;
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("[");
            for (var r = 0; r < _rowCount; r++)
            {
                if (r > 0) result.Append(";");
                for (var c = 0; c < _columnCount; c++)
                {
                    if (c > 0) result.Append(","); // "\n"
                    double value = _data[r][c];
                    result.Append(double.IsNaN(value) ? "NaN" : value.ToString()); // "#,##0.00"
                }
            }
            result.Append("]");
            //if (this._rowCount > 3 || this._columnCount > 3) result.Append(" Size:" + this.size().ToString());
            return result.ToString();
        }

        public static Matrix filledMatrix(int rowCount, int columnCount, double value)
        {
            var m1 = new Matrix(rowCount, columnCount);
            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < columnCount; c++)
                {
                    m1._data[r][c] = value;
                }
            }
            return m1;
        }

        public static Matrix zeros(int rowCount, int columnCount)
        {
            return filledMatrix(rowCount, columnCount, 0);
        }

        public static Matrix ones(int rowCount, int columnCount)
        {
            return filledMatrix(rowCount, columnCount, 1);
        }

        public static Matrix vector(double[] array)
        {
            var columnCount = array.Length;
            var result = new Matrix(1, columnCount);
            // perhaps we could use splice or something similar
            for (var c = 0; c < columnCount; c++)
            {
                result._data[0][c] = array[c];
            }
            return result;
        }

        public static Matrix rand(int rowCount, int columnCount)
        {
            var result = new Matrix(rowCount, columnCount);
            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < columnCount; c++)
                {
                    result._data[r][c] = _rnd.NextDouble();
                }
            }
            return result;
        }

        public static Matrix rowVector(double[] array)
        {
            var len = array.Length;
            var result = new Matrix(len, 1);
            for (var r = 0; r < len; r++)
            {
                result._data[r][0] = array[r];
            }
            return result;
        }

        public int RowCount
        {
            get { return _rowCount; }
        }

        public int ColumnCount
        {
            get { return _columnCount; }
        }

        public double Value(int row, int column)
        {
            return _data[row][column];
        }

        public Matrix ElementWiseOp(Matrix m2, Func<double, double, double> func)
        {
            var rowCount = RowCount;
            var columnCount = ColumnCount;
            if (m2.RowCount != rowCount || m2.ColumnCount != columnCount) throw new Exception("ElementWise operations requires same size matrices.");
            var m3 = new Matrix(rowCount, columnCount);
            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < columnCount; c++)
                {
                    m3._data[r][c] = func(Value(r, c), m2.Value(r, c));
                }
            }
            return m3;
        }

        public Matrix ElementWiseAdd(Matrix m2)
        {
            return ElementWiseOp(m2, (v1, v2) => v1 + v2);
        }

        public Matrix ElementWiseSubtract(Matrix m2)
        {
            return ElementWiseOp(m2, (v1, v2) => v1 - v2);
        }

        public Matrix ElementWiseMultiply(Matrix m2)
        {
            return ElementWiseOp(m2, (v1, v2) => v1 * v2);
        }

        public Matrix ElementWiseDivide(Matrix m2)
        {
            return ElementWiseOp(m2, (v1, v2) => v1 / v2);
        }

        public Matrix ElementWisePower(Matrix m2)
        {
            return ElementWiseOp(m2, (v1, v2) => Math.Pow(v1, v2));
        }

        public Matrix ScalarOp(Func<double, double> func)
        {
            var rowCount = RowCount;
            var columnCount = ColumnCount;
            var m2 = new Matrix(rowCount, columnCount);
            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < columnCount; c++)
                {
                    m2._data[r][c] = func(Value(r, c));
                }
            }
            return m2;
        }

        public Matrix ScalarAdd(double n)
        {
            return ScalarOp(v1 => v1 + n);
        }

        public Matrix ScalarSubtract(double n)
        {
            return ScalarOp(v1 => v1 - n);
        }

        public Matrix ScalarMultiply(double n)
        {
            return ScalarOp(v1 => v1 * n);
        }

        public Matrix ScalarDivide(double n)
        {
            return ScalarOp(v1 => v1 / n);
        }

        public Matrix ScalarPower(double n)
        {
            return ScalarOp(v1 => Math.Pow(v1, n));
        }

        public Matrix ScalarNeg()
        {
            return ScalarOp(v1 => -v1);
        }

        public Matrix ScalarInverse()
        {
            return ScalarOp(v1 => 1 / v1);
        }

        public Matrix Product(Matrix m2)
        {
            var rowCount = RowCount;
            var middle = m2.RowCount;
            var columnCount = m2.ColumnCount;
            if (ColumnCount != m2.RowCount) throw new Exception("Matrix product function requires compatible matrices (matrice 1 is " + Size() + ", matrice 2 is " + m2.Size());

            var m3 = new Matrix(rowCount, columnCount);
            for (var r = 0; r < rowCount; r++)
            {
                for (var c = 0; c < columnCount; c++)
                {
                    var v = 0.0;
                    for (var m = 0; m < middle; m++)
                    {
                        v += Value(r, m) * m2.Value(m, c);
                    }
                    m3._data[r][c] = v;
                }
            }
            return m3;
        }

        public Matrix Size()
        {
            var result = new Matrix(1, 2);
            result._data[0][0] = _rowCount;
            result._data[0][1] = _columnCount;
            return result;
        }

        public Matrix Transpose()
        {
            var result = new Matrix(/*rowCount:*/_columnCount, /*columnCount:*/_rowCount);
            for (var r = 0; r < _rowCount; r++)
            {
                for (var c = 0; c < _columnCount; c++)
                {
                    result._data[/*r is */ c][/* c is */ r] = _data[r][c];
                }
            }
            return result;
        }

        public Matrix Sum()
        {
            var result = new Matrix(1, _columnCount);
            for (var c = 0; c < _columnCount; c++)
            {
                var sum = 0.0;
                for (var r = 0; r < _rowCount; r++)
                {
                    sum += _data[r][c];
                }
                result._data[0][c] = sum;
            }
            return result;
        }

        public double TotalSum()
        {
            double result;
            if (_rowCount > 1)
            {
                result = Sum().TotalSum();
            }
            else
            {
                result = 0;
                for (var c = 0; c < _columnCount; c++)
                {
                    result += _data[0][c];
                }
            }
            return result;
        }

        public Matrix Std()
        {
            var result = Variance();
            for (var c = 0; c < _columnCount; c++)
            {
                // here we violate the imutability but it seems safe
                result._data[0][c] = Math.Sqrt(result._data[0][c]);
            }
            return result;
        }

        public Matrix Variance()
        {
            // Matlab and Octave are not calculating the textbook variance 
            // but unbiaised variance where we divide by n - 1
            // we'll do the same
            var result = new Matrix(1, _columnCount);
            var mean = Mean();
            for (var c = 0; c < _columnCount; c++)
            {
                var sqDiffSum = 0.0;
                for (var r = 0; r < _rowCount; r++)
                {
                    var diff = _data[r][c] - mean._data[0][c];
                    sqDiffSum += diff * diff;
                }
                // this is UNBIASED variance  -------------------vvvv
                result._data[0][c] = sqDiffSum / (_rowCount - 1);
            }
            return result;
        }

        public Matrix Mean()
        {
            var result = Sum();
            result = result.ScalarDivide(_rowCount);
            return result;
        }

        public Matrix TransformByColumnCount(int newColumnCount)
        {
            var size = _rowCount * _columnCount;
            var newrowCount = size / newColumnCount;
            if (newrowCount * newColumnCount != size) 
                throw new Exception("Current size " + Size() + " cannot be transformed into " + newColumnCount + " columnCount.");
            
            var result = new Matrix(newrowCount, newColumnCount);
            int r2 = 0, c2 = 0;
            
            for (var r = 0; r < result._rowCount; r++)
            {
                for (var c = 0; c < result._columnCount; c++)
                {
                    result._data[r][c] = _data[r2][c2];
                    c2++;
                    if (c2 == _columnCount)
                    {
                        c2 = 0;
                        r2++;
                    }
                }
            }
            return result;
        }

        public Matrix Portion(int rowFrom, int columnFrom, int rowCount = -1, int columnCount = -1)
        {
            if (rowCount == -1) rowCount = _rowCount - rowFrom;
            if (columnCount == -1) columnCount = _columnCount - columnCount;

            var r2 = rowFrom;
            var result = new Matrix(rowCount, columnCount);
            for (var r = 0; r < rowCount; r++)
            {
                var c2 = columnFrom;
                for (var c = 0; c < columnCount; c++)
                {
                    result._data[r][c] = _data[r2][c2];
                    c2++;
                }
                r2++;
            }
            return result;
        }

        public Matrix AppendRight(Matrix m2)
        {
            if (m2._rowCount != _rowCount) throw new Exception("Cannot append a " + m2.RowCount.ToString() + " high matrix to a " + _rowCount.ToString() + " high matrix.");

            var result = new Matrix(_rowCount, _columnCount + m2._columnCount);
            for (var r = 0; r < _rowCount; r++)
            {
                int c;
                for (c = 0; c < _columnCount; c++)
                {
                    result._data[r][c] = _data[r][c];
                }
                for (var c2 = 0; c2 < m2._columnCount; c2++)
                {
                    result._data[r][c] = m2._data[r][c2];
                    c++;
                }
            }
            return result;
        }

        public Matrix AppendBottom(Matrix m2)
        {
            if (m2._columnCount != _columnCount) throw new Exception("Cannot append a " + m2.ColumnCount.ToString() + " high matrix to a " + _columnCount.ToString() + " high matrix.");

            var result = new Matrix(_rowCount + m2._rowCount, _columnCount);
            int r;
            for (r = 0; r < _rowCount; r++)
            {
                for (var c = 0; c < _columnCount; c++)
                {
                    result._data[r][c] = _data[r][c];
                }
            }
            // ToDo: variable 'r2' is never modified in 'for' loop.
            for (int r2 = 0; r2 < m2._rowCount; r++)
            {
                for (var c = 0; c < m2._columnCount; c++)
                {
                    result._data[r][c] = m2._data[r2][c];
                    r++;
                }
            }
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix && (obj as Matrix).Equals(this);
        }

        public bool Equals(Matrix other)
        {
            if (other._rowCount != RowCount || other._columnCount != ColumnCount) 
                return false;

            for (int r = 0; r < _rowCount; r++)
            {
                for (int c = 0; c < _columnCount; c++)
                {
                    if (_data[r][c] != other._data[r][c]) 
                        return false;
                }
            }
            return true;
        }
    }
}