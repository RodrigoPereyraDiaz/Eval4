using Eval4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eval4ConsoleDemo
{
    class Program
    {
        //static Eval4.Core.Evaluator ev = new CSharpEvaluator();
        static Eval4.Core.Evaluator ev = new CSharpEvaluator();
        public readonly static Int16 X = 12;
        public static Single Y = 34.56F;
        public static Account A = new Account();

        public class Account
        {
            public readonly Int16 T = 6;
        }

        static Program()
        {
            ev.AddEnvironmentFunctions(typeof(Program));
            ev.AddEnvironmentFunctions(typeof(Math));
            ev.SetVariable("pascal", new int[] { 1, 8, 28, 56, 70, 56, 28, 8, 1 });

        }

        static DateTime christmas = new DateTime(2012, 12, 25);

        static void Main(string[] args)
        {
            // Eval("1+2*3.1");
            // TestFormula("false xor false", false);
            // TestFormula("false xor true", true);
            // TestFormula("true xor false", true);
            // TestFormula("true xor true", false);
            // TestFormula("1 | 255", 255);
            // TestFormula("23 % 10", 3);
            // TestFormula("true ?  1:0", 1);
            // TestFormula("3", 3);
            // TestFormula("3 + 2", 5);
            // TestFormula("3 - 2", 1);
            // TestFormula("3 * 2", 6);
            // TestFormula("3 / 2", 3 / 2);
            // TestFormula("1*2*3*4*5*6*7*8*9", 1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9);
            // TestFormula("5-\"A\"", "5-'A' (impossible)");
            //TestFormula("pascal[0]", 1);

            //TestFormula("#2012/12/25#", christmas);
            //TestFormula("#2012/12/25# + 1", christmas.AddDays(1));
            //TestFormula("-1.5*-2.5", -1.5 * -2.5);
            TestFormula("true ?  1:0", 1);

            Console.WriteLine("Completed");
            Console.ReadKey();
        }


        private static void TestFormula(string formula, object expected)
        {
            do
            {
                var actual = Eval(formula);
                if (actual.Equals(expected))
                {
                    Console.WriteLine("[Success] {0}\r\n   = {1}", formula, expected, actual);
                    break;
                }
                else
                {
                    Console.WriteLine("[Failure] {0}\r\n Expected {1} Actual {2}", formula, expected, actual);
                }
            } while (true);
        }

        private static object Eval(string formula)
        {
            var parsed = ev.Parse(formula);
            TextWriter tw = new StringWriter();
            WriteDependencies(tw, "formula", parsed);
            Console.WriteLine(tw.ToString());
            object result = parsed.ObjectValue;
            return result;
        }

        private static void WriteDependencies(TextWriter tw, string name, Eval4.Core.IHasValue expr, string indent = null)
        {
            if (indent == null) indent = string.Empty;

            tw.WriteLine("{0} {1} type {2} ({3})", indent, name, expr.ShortName, expr.SystemType);
            int cpt = 0;
            foreach (var d in expr.Dependencies)
            {
                cpt++;
                WriteDependencies(tw, d.Name, d.Expr, indent + "  |");
            }
            if (cpt == 0)
            {
                tw.WriteLine("{0}  +--> {2}", indent, name, expr.ObjectValue);
            }
            else
            {
                tw.WriteLine("{0}  +--> {2} ({1})", indent, name, expr.ObjectValue);
            }
        }

    }
}
