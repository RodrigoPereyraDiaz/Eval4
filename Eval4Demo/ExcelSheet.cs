using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eval4.Core;
using System.Diagnostics;

namespace Eval4.DemoCSharp
{
    public partial class ExcelSheet : UserControl
    {
        ExcelEvaluator ev;

        Cell[,] mCells;

        public ExcelSheet()
        {

            InitializeComponent();
            panel2.Size = new Size(firstColWidth + NBCOLUMN * colWidth, rowHeight + NBROWS * rowHeight);
            ev = new ExcelEvaluator();
            ev.FindVariable += ev_FindVariable;
            mCells = new Cell[NBCOLUMN, NBROWS];
            for (int row = 0; row < NBCOLUMN; row++)
            {
                for (int col = 0; col < NBCOLUMN; col++)
                {
                    var cellName = GetCellName(curCell.X, curCell.Y);
                    mCells[col, row] = new Cell(ev, col, row);
                }
            }
            SetFocusedCell(1, 1);
        }

        void ev_FindVariable(object sender, FindVariableEventArgs e)
        {
            var n = e.Name;
            int i = 0;
            int row = 0, col = 0;

            while (i < n.Length)
            {
                char c = n[i];
                if (c >= 'A' && c <= 'Z') col = col * 26 + (c - 'A');
                else if (c >= 'a' && c <= 'z') col = col * 26 + (c - 'a');
                else break;
                i++;
            }
            while (i < n.Length)
            {
                char c = n[i];
                if (c >= '0' && c <= '9') row = row * 10 + (c - '0');
                else break;
                i++;
            }
            row--; //rows start a 1            
            if (i == n.Length && col >= 0 && row >= 0 && col < NBCOLUMN && row < NBROWS)
            {
                Cell c = mCells[col, row];
                e.Value = new Variable<object>(new object());
                e.Handled = true;
            }
        }

        int colWidth = 100;
        int firstColWidth = 50;
        int rowHeight = 24;

        int NBCOLUMN = 20;
        int NBROWS = 50;
        private Point curCell;

        public class Cell
        {
            private Evaluator mEv;
            private string mFormula;
            private string mName;
            private IHasValue mValue;
            public Exception Exception;

            public Cell(Evaluator ev, int col, int row)
            {
                mName = GetCellName(col + 1, row + 1);
                this.mEv = ev;
                ev.SetVariable(mName, this);
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
                        if (firstChar == '=') mFormula = mFormula.Substring(1);
                        try
                        {
                            mValue = mEv.Parse(mFormula);
                        }
                        catch (Exception ex)
                        {
                            Exception = ex;
                        }
                    }
                }
            }


            public string DisplayText
            {
                get
                {
                    if (Exception != null) return Exception.Message;
                    else if (mValue != null) return mValue.ObjectValue.ToString();
                    else
                    {
                        if (mFormula != null && mFormula.StartsWith("'")) return mFormula.Substring(1);
                        else return mFormula;
                    }
                }
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            var r = e.ClipRectangle;
            Point pfrom = cellAtPos(r.Left, r.Top);
            Point pto = cellAtPos(r.Right, r.Bottom);
            for (int x = pfrom.X; x <= pto.X; x++)
            {
                for (int y = pfrom.Y; y <= pto.Y; y++)
                {
                    DrawCell(e.Graphics, x, y);
                }
            }

            r = GetRect(curCell.X, curCell.Y);
            if (r.IntersectsWith(e.ClipRectangle))
            {
                e.Graphics.DrawRectangle(new Pen(Color.Blue, 2), r);
            }
        }

        // of text centered on the page.
        static StringFormat Centered = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        static StringFormat MiddleLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        static StringFormat MiddleRight = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

        private void DrawCell(Graphics g, int x, int y)
        {
            Rectangle r = GetRect(x, y);
            string text = string.Empty;
            var backColor = Brushes.White;
            var borderColor = Pens.DarkGray;
            StringFormat stringFormat = MiddleLeft;

            if (x == 0)
            {
                backColor = (y == curCell.Y ? Brushes.Silver : Brushes.DarkGray);
                stringFormat = Centered;
                if (y > 0) text = y.ToString();
            }
            else
            {
                var col = GetColName(x);
                var cell = col + y.ToString();
                if (y == 0)
                {
                    text = col;
                    stringFormat = Centered;
                    backColor = (x == curCell.X ? Brushes.Silver : Brushes.DarkGray);
                }
                else
                {
                    if (x > 0 && x <= NBCOLUMN && y > 0 && y < NBROWS)
                    {
                        var c = mCells[x - 1, y - 1];
                        text = c.DisplayText;
                    }
                }
            }
            g.FillRectangle(backColor, r);
            g.DrawRectangle(borderColor, r);
            r.Inflate(-5, -5);
            g.DrawString(text, this.Font, Brushes.Black, r, stringFormat);

        }

        private Rectangle GetRect(int x, int y)
        {
            Rectangle r = Rectangle.Empty;
            r.Y = y * rowHeight;
            r.Height = rowHeight;
            if (x == 0)
            {
                r.X = 0;
                r.Width = firstColWidth;
            }
            else
            {
                r.X = firstColWidth + (x - 1) * colWidth;
                r.Width = colWidth;
            }
            return r;
        }

        private Point cellAtPos(int x, int y)
        {
            Point result = Point.Empty;
            result.X = (x < firstColWidth ? 0 : 1 + (x - firstColWidth) / colWidth);
            result.Y = (y / rowHeight);
            return result;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            var newCell = cellAtPos(e.X, e.Y);
            SetFocusedCell(newCell.X, newCell.Y);
        }

        private void SetFocusedCell(int x, int y)
        {
            var previousCell = curCell;
            curCell = new Point(x, y);
            InvalidateCells(previousCell);
            InvalidateCells(curCell);
            Cell c = mCells[curCell.X - 1, curCell.Y - 1];
            textBox1.Text = c.Formula;
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void InvalidateCells(Point cell)
        {
            InvalidateCell(cell.X, cell.Y);
            InvalidateCell(0, cell.Y);
            InvalidateCell(cell.X, 0);
        }

        private void InvalidateCell(int x, int y)
        {
            var cell = GetCellName(x, y);
            Trace.WriteLine("invalidate " + cell);
            var r = GetRect(x, y);
            r.Inflate(4, 4);
            panel2.Invalidate(r);
        }

        private static string GetCellName(int x, int y)
        {
            var col = GetColName(x);
            var cell = col + y.ToString();
            return cell;
        }

        private static string GetColName(int x)
        {
            string result = string.Empty;
            if (x <= 26)
            {
                result = ((char)(64 + x)).ToString();
            }
            else if (x <= 26 * 26)
            {
                var x1 = ((x - 1) / 26);
                var x2 = 1 + ((x - 1) % 26);

                result = ((char)(64 + x1)).ToString() + ((char)(64 + x2)).ToString();
            }
            return result;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var c = mCells[curCell.X - 1, curCell.Y - 1];
            c.Formula = textBox1.Text;
            InvalidateCell(curCell.X, curCell.Y);
        }

        private void ExcelSheet_Load(object sender, EventArgs e)
        {
            switch (this.Name)
            {
                case "excelSheet1":
                    mCells[0, 0].Formula = "Net"; mCells[1, 0].Formula = "12.00";
                    mCells[0, 1].Formula = "VAT"; mCells[1, 1].Formula = "20.5";
                    mCells[0, 2].Formula = "Gross"; mCells[1, 2].Formula = "=B1*(1+B2/100)";


                    break;
                case "excelSheet2":
                    break;
                case "excelSheet3":
                    break;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    break;
                case Keys.Return:
                    e.Handled = true;
                    break;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
