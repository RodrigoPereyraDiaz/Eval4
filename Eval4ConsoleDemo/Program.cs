using Eval4;
using Eval4.Core;
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
        private static string scenario;
        private static string mFormula;
        private static Evaluator ev;

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            using (var sr = new StreamReader("specs.txt"))
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
                            WriteLine(ConsoleColor.Cyan, "Language: " + restOfLine);
                            switch (restOfLine.Trim().ToLower())
                            {
                                case "vb":
                                    ev = new VbEvaluator();
                                    break;
                                case "excel":
                                    ev = new ExcelEvaluator();
                                    break;
                                case "cs":
                                    ev = new CSharpEvaluator();
                                    break;
                                case "javascript":
                                    ev = new JavascriptEvaluator();
                                    break;
                                case "math":
                                    ev = new MathEvaluator();
                                    break;
                                default:
                                    WriteLine(ConsoleColor.Red, "Unknown language " + restOfLine);
                                    break;
                            }
                            break;
                        case "formula":
                            Write(ConsoleColor.Gray, restOfLine);
                            mFormula = restOfLine;
                            break;
                        case "expectedresult":
                            Write(ConsoleColor.DarkGray, " = ");
                            String resultString = null;
                            Exception ex0 = null;
                            try
                            {
                                var result = ev.Eval(mFormula);
                                resultString = result.ToString();
                            }
                            catch (Exception ex)
                            {
                                resultString = ex.GetType().Name;
                            }
                            if (resultString == restOfLine)
                            {
                                WriteLine(ConsoleColor.Green, restOfLine);
                            }
                            else
                            {
                                WriteLine(ConsoleColor.Red, resultString);
                                WriteLine(ConsoleColor.Red, "Expected " + restOfLine);
                            }
                            if (ex0 != null)
                            {
                                WriteLine(ConsoleColor.Red, ex0.Message);
                            }

                            break;
                        case "set":
                            Write(ConsoleColor.Gray, command + ":");
                            WriteLine(ConsoleColor.White, restOfLine);
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
