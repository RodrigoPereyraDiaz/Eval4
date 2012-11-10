using Eval4;
using Eval4.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eval4.ConsoleDemo
{
    class Program
    {
        private static string scenario;
        private static string mFormula;
        private static Eval4.Excel.ExcelSheet sheet;
        private static IEvaluator ev;

        static void Main(string[] args)
        {
            //SetLanguage("math");
            //TestFormula("[1,2,3;4,5,6]", "[1,2,3;4,5,6]");
            SetLanguage("javascript");
            TestFormula("1+1", "2.000");
            RunTemplate("specs.txt");
        }


        static void RunTemplate(string templateFile)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            using (var sr = new StreamReader(templateFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var indexOfColon = line.IndexOf(':');
                    string command, restOfLine;
                    if (indexOfColon < 0)
                    {
                        command = line;
                        restOfLine = string.Empty;
                    }
                    else
                    {
                        command = line.Substring(0, indexOfColon);
                        restOfLine = line.Substring(indexOfColon + 1).Trim();
                    }
                    string cmd1, cmd2;
                    cmd1 = command.Trim();
                    var indexOfSpace = cmd1.IndexOf(' ');
                    if (indexOfSpace >= 0)
                    {
                        cmd2 = cmd1.Substring(indexOfSpace + 1).Trim();
                        cmd1 = cmd1.Substring(0, indexOfSpace);
                    }
                    else cmd2 = string.Empty;
                    switch (cmd1.ToLower())
                    {
                        case "scenario":
                            WriteLine(ConsoleColor.Yellow, restOfLine);
                            scenario = restOfLine;
                            break;
                        case "language":
                            SetLanguage(restOfLine);
                            break;
                        case "formula":
                            mFormula = restOfLine;
                            break;
                        case "cell":
                            SetCell(cmd2, restOfLine);
                            break;
                        case "expectedresult":
                            TestFormula(mFormula, restOfLine);
                            break;
                        case "set":
                            SetVariable(cmd2, restOfLine);
                            break;
                        default:
                            if (line.Length == 0 || command.StartsWith("--"))
                            {
                                WriteLine(ConsoleColor.DarkGray, line);
                            }
                            else
                            {
                                WriteLine(ConsoleColor.Red, string.Format("* Invalid command \"{0}\"", command));
                            }
                            break;

                    }
                }

            }
            Console.WriteLine("Completed");
            Console.ReadKey();
        }

        private static void SetCell(string cellName, string formula)
        {
            
        }

        private static void SetLanguage(string language)
        {
            WriteLine(ConsoleColor.Cyan, "Language: " + language);
            switch (language.Trim().ToLower())
            {
                case "vb":
                    ev = new VB.VbEvaluator();
                    break;
                case "excel":
                    ev = new Eval4.Excel.ExcelEvaluator();
                    break;
                case "cs":
                    ev = new CSharp.CSharpEvaluator();
                    break;
                case "javascript":
                    ev = new Javascript.JavascriptEvaluator();
                    break;
                case "math":
                    ev = new Math.MathEvaluator();
                    break;
                default:
                    WriteLine(ConsoleColor.Red, "Unknown language " + language);
                    break;
            }
            ev.SetVariable("arr", new int[] { 2, 4, 6 });
        }

        private static void TestFormula(string formula, string expectedResult)
        {
            Write(ConsoleColor.Gray, formula);
            Write(ConsoleColor.DarkGray, " = ");
            String resultString = null;
            Exception ex0 = null;
            try
            {
                var result = ev.Eval(formula);
                resultString = ev.ConvertToString(result);
            }
            catch (Exception ex)
            {
                resultString = ex.GetType().Name;
            }
            if (resultString == expectedResult)
            {
                WriteLine(ConsoleColor.Green, expectedResult);
            }
            else
            {
                WriteLine(ConsoleColor.Red, resultString);
                WriteLine(ConsoleColor.Red, "Expected " + expectedResult);
            }
            if (ex0 != null)
            {
                WriteLine(ConsoleColor.Red, ex0.Message);
            }

        }

        private static void SetVariable(string variableName, string formula)
        {
            Write(ConsoleColor.Gray, "Set " + variableName + ":");
            try
            {
                var parsed = ev.Parse(formula);
                ev.SetVariable(variableName, parsed);
                WriteLine(ConsoleColor.Green, formula);
            }
            catch (Exception ex)
            {
                WriteLine(ConsoleColor.Red, formula);
                WriteLine(ConsoleColor.Red, ex.GetType().Name);
            }

        }

        private static void Write(ConsoleColor consoleColor, string restOfLine)
        {
            Console.ForegroundColor = consoleColor;
            Console.Write(restOfLine);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void WriteLine(ConsoleColor consoleColor, string restOfLine)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(restOfLine);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

    }
}
