using System;
using System.Collections.Generic;
using System.Text;

namespace Eval4
{
    public class SyntaxError : Exception
    {
        public readonly string formula;
        public readonly int pos;

        public SyntaxError(string str, string formula, int pos)
            : base(str)
        {
            this.formula = formula;
            this.pos = pos;
        }
    }
}
