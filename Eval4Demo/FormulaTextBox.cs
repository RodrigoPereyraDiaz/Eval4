using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Eval4.Demo
{
    class FormulaTextBox : TextBox
    {
        protected override bool IsInputKey(Keys keyData)
        {
            var key = (Keys)((int)keyData & 0xFF); // remove control / alt
            switch (key)
            {
                case Keys.Enter: // same as Keys.Return
                case Keys.Tab:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }
    }
}
