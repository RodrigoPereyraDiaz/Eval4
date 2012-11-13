using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval5Demo
{
    class Evaluator
    {
        internal Func<T> Parse<T>(string source)
        {
            T result = default(T);
            return (() => result);
        }

        public T Eval<T>(string source)
        {
            var result= Parse<T>(source);
            return result();
        }

    }
}
