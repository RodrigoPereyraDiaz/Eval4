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
            for (int row = 0; row < NBROWS; row++)
            {
                for (int col = 0; col < NBCOLUMN; col++)
                {
                    var cellName = Cell.GetCellName(curCell.X, curCell.Y);
                    mCells[col, row] = new Cell(ev, col, row);
                }
            }
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
                e.Value = new Variable<Cell>(c);
                e.Handled = true;
            }
        }

        int colWidth = 100;
        int firstColWidth = 50;
        int rowHeight = 24;

        int NBCOLUMN = 20;
        int NBROWS = 50;
        private Point curCell;


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
                e.Graphics.DrawRectangle(new Pen(textBox1.Focused ? Color.Blue : Color.DarkGray, 2), r);

            }
        }

        // of text centered on the page.
        static StringFormat Centered = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        static StringFormat MiddleLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        static StringFormat MiddleRight = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        private string mOriginalText;

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
                var col = Cell.GetColName(x);
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

                        text = c.ToString();
                        if (c.ValueObject != null) stringFormat = MiddleRight;
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
            if (x > 0 && y > 0 && x <= NBCOLUMN && y <= NBROWS)
            {
                var previousCell = curCell;
                curCell = new Point(x, y);
                InvalidateCells(previousCell);
                InvalidateCells(curCell);
                Cell c = mCells[curCell.X - 1, curCell.Y - 1];

                textBox1.Text = c.Formula;
                textBox1.SelectAll();
                textBox1.Focus();

                ScrollCellIntoView(x,y);
            }
        }

        private void ScrollCellIntoView(int x, int y)
        {
            if (x == 1) x = 0;
            if (y == 1) y = 0;
            var cr = this.GetRect(x, y);
            var dr = panel1.DisplayRectangle;
            Point scroll = new Point(-dr.Left, -dr.Top);
            var clr = panel1.ClientSize;
            if (cr.Left < scroll.X) scroll.X = cr.Left;
            if (cr.Top < scroll.Y) scroll.Y = cr.Top;
            if (cr.Right > scroll.X + clr.Width) scroll.X = cr.Right - clr.Width;
            if (cr.Bottom > scroll.Y + clr.Height) scroll.Y = cr.Bottom - clr.Height;
            panel1.AutoScrollPosition = scroll;
        }

        private void InvalidateCells(Point cell)
        {
            InvalidateCell(cell.X, cell.Y);
            InvalidateCell(0, cell.Y);
            InvalidateCell(cell.X, 0);
        }

        private void InvalidateCell(int x, int y)
        {
            Trace.WriteLine("invalidate " + Cell.GetCellName(x, y));
            var r = GetRect(x, y);
            r.Inflate(4, 4);
            panel2.Invalidate(r);
        }




        private void ExcelSheet_Load(object sender, EventArgs e)
        {
            switch (this.Name)
            {
                case "excelSheet1":
                    mCells[0, 0].Formula = "Net"; mCells[1, 0].Formula = "12.00";
                    mCells[0, 1].Formula = "VAT"; mCells[1, 1].Formula = "20.5";
                    mCells[0, 2].Formula = "Gross"; mCells[1, 2].Formula = "=B1*(1.0+B2/100.0)";


                    break;
                case "excelSheet2":
                    break;
                case "excelSheet3":
                    break;
            }
            SetFocusedCell(1, 1);
        }


        //private void panel2_Enter(object sender, EventArgs e)
        //{
        //    inPanel2 = true;
        //    InvalidateCell(curCell.X, curCell.Y);
        //}

        //private void panel2_Leave(object sender, EventArgs e)
        //{
        //    inPanel2 = false;
        //    InvalidateCell(curCell.X, curCell.Y);
        //}

        //private void panel2_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    switch (e.KeyChar)
        //    {
        //        case '\t':
        //        case '\r':
        //            e.Handled = true;
        //            break;
        //        default:
        //            textBox1.Focus();
        //            textBox1.Text = e.KeyChar.ToString();
        //            textBox1.SelectionStart = 1;
        //            break;
        //    }
        //}

        //private void panel2_KeyDown(object sender, KeyEventArgs e)
        //{
        //    var keyCode = e.KeyCode;
        //    if (e.KeyCode == Keys.Tab) keyCode = (e.Modifiers == Keys.Shift ? Keys.Left : Keys.Right);
        //    else if (e.KeyCode == Keys.Return) keyCode = (e.Modifiers == Keys.Shift ? Keys.Up : Keys.Down);


        //    switch (keyCode)
        //    {
        //        case Keys.Left:
        //            SetFocusedCell(curCell.X - 1, curCell.Y);
        //            e.Handled = true;
        //            break;
        //        case Keys.Right:
        //            SetFocusedCell(curCell.X + 1, curCell.Y);
        //            e.Handled = true;
        //            break;
        //        case Keys.Up:
        //            SetFocusedCell(curCell.X, curCell.Y - 1);
        //            e.Handled = true;
        //            break;
        //        case Keys.Down:
        //            SetFocusedCell(curCell.X, curCell.Y + 1);
        //            e.Handled = true;
        //            break;
        //        case Keys.F2:
        //            textBox1.Focus();
        //            textBox1.SelectionStart = textBox1.Text.Length;
        //            e.Handled = true;
        //            break;
        //        case Keys.Delete:
        //            textBox1.Text = string.Empty;
        //            e.Handled = true;
        //            break;

        //    }
        //}

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            bool shift=((e.Modifiers & Keys.Shift) != 0);

            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    SetFocusedCell(curCell.X, curCell.Y - 1);
                    break;
                case Keys.Down:
                    e.Handled = true;
                    SetFocusedCell(curCell.X, curCell.Y + 1);
                    break;
                case Keys.Left:
                    if ((e.Modifiers != Keys.Control)
                        && (textBox1.SelectionStart == 0))
                    {
                        SetFocusedCell(curCell.X - 1, curCell.Y);
                        e.Handled = true;
                    }
                    else if (textBox1.SelectionLength > 0)
                    {
                        textBox1.SelectionLength = 0;
                    }
                    break;
                case Keys.Right:
                    if ((e.Modifiers != Keys.Control)
                        && (textBox1.SelectionStart + textBox1.SelectionLength == textBox1.TextLength))
                    {
                        SetFocusedCell(curCell.X + 1, curCell.Y);
                        e.Handled = true;
                    }
                    break;
                case Keys.Escape:
                    textBox1.Text = mOriginalText;
                    textBox1.SelectionLength = textBox1.TextLength;
                    e.Handled = true;
                    break;
                case Keys.F2:
                    textBox1.SelectionStart = textBox1.TextLength;
                    textBox1.SelectionLength = 0;
                    break;
                case Keys.Tab:
                    if ((curCell.X > 0 || !shift) 
                        && (curCell.X < NBCOLUMN || shift))
                    {
                        e.Handled = true;
                        SetFocusedCell(curCell.X + (shift ? -1 : 1), curCell.Y);
                    }
                    break;
                case Keys.Return:
                    if ((curCell.Y > 0 || !shift) 
                        && (curCell.Y < NBROWS || shift))
                    {
                        e.Handled = true;
                        SetFocusedCell(curCell.X, curCell.Y + (shift ? -1 : 1));
                    }
                    break;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            switch (e.KeyChar)
            {
                case '\r':
                case '\t':
                    e.Handled = true;
                    break;
                default:
                    Trace.WriteLine("textBox1_KeyPress(" + e.KeyChar + ")");
                    break;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var c = mCells[curCell.X - 1, curCell.Y - 1];
            c.Formula = textBox1.Text;
            InvalidateCell(curCell.X, curCell.Y);
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            InvalidateCell(curCell.X, curCell.Y);
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            InvalidateCell(curCell.X, curCell.Y);
        }
    }
}
