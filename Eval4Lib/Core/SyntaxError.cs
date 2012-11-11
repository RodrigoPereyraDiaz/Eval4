using System;
using System.Collections.Generic;
using System.Text;

namespace Eval4.Core
{
    public class SyntaxError 
    {
        public readonly string message;
        public readonly string formula;
        public readonly int pos;
        
        public SyntaxError(string message, string formula, int pos)
        {
            this.message = message;
            this.formula = formula;
            this.pos = pos;
        }
    }
}
