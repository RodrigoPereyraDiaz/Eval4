using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4.Core
{
    public class Token
    {
        public Token()
        {
        }

        public TokenType Type { get; set; }
        public String ValueString { get; set; }
        public Object ValueObject { get; set; }

        public override string ToString()
        {
            return Type.ToString() + " " + ValueString;
        }
    }

    public class Token<T> : Token
    {
        public Token()
        {
        }

        public T CustomType { get; set; }

    }
}
