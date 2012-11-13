using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eval5Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // I would like to make the evaluators definition as simple as possible
            // to allow extensibility
            // to avoid BNF rules syntax as there are already many tools to do that

            var ev = new MyEvaluator();
            var val = ev.Eval<int>("1+2*3");

            
        }

        public class MyEvaluator : Evaluator
        {
            public MyEvaluator()
            {
            }

        }
    }
}
