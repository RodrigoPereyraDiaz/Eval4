using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Eval4.DemoCSharp
{
    public class CellsViewer : UserControl
    {
        public CellsViewer()
        {
            // Set the value of the double-buffering style bits to true.
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
            | ControlStyles.UserPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.Selectable, true);

            //UpdateStyles();
            this.TabStop = true;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            var key = (Keys)((int)keyData & 0xFF); // remove control / alt
            switch (key)
            {
                case Keys.Enter: // same as Keys.Return
                case Keys.Tab:
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

    }
}
