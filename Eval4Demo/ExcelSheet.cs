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
        public ExcelSheet()
        {
            mFomulas = new Dictionary<string, Cell>();

            InitializeComponent();
            panel2.Size = new Size(firstColWidth + NBCOLUMN * colWidth, rowHeight + NBROWS * rowHeight);
        }
        Dictionary<string, Cell> mFomulas;
        int colWidth = 100;
        int firstColWidth = 50;
        int rowHeight = 24;

        int NBCOLUMN = 256;
        int NBROWS = 10000;
        private Point curCell = new Point(-1, -1);

        public class Cell
        {
            public string formula;
            public IHasValue parsed;
            public string value;
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
                    Cell c;
                    if (mFomulas.TryGetValue(cell, out c))
                    {
                        text = c.formula;
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
            var previousCell= curCell;
            curCell = cellAtPos(e.X, e.Y);
            InvalidateCells(previousCell);
            InvalidateCells(curCell);
            textBox1.Focus();
        }

        private void InvalidateCells(Point focusedCell)
        {
            InvalidateCell(focusedCell.X, focusedCell.Y);
            InvalidateCell(0, focusedCell.Y);
            InvalidateCell(focusedCell.X, 0);
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
    }
}
