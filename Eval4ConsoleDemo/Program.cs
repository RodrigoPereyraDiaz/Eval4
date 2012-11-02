﻿using Eval4;
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
        static VbEvaluator ev = new VbEvaluator();
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
        }

        static void Main(string[] args)
        {
            //Eval("1+2*3");

            Eval("A.T*X+Y"); //("Sin(X)*5.1");
            Console.ReadKey(intercept: true);
        }

        private static void Eval(string formula)
        {
            var parsed = ev.Parse(formula);
            TextWriter tw = new StringWriter();
            WriteDependencies(tw, "formula", parsed);
            Console.WriteLine(tw.ToString());
            Console.WriteLine(parsed.ObjectValue);
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
