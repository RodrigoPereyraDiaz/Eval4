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
        private static string scenario;

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
                        restOfLine = line.Substring(indexOfColon + 1);
                    } 
                    switch (command.Trim().ToLower())
                    {
                        case "scenario":
                        case "language":
                        case "formula":
                        case "expectedresult":
                        case "set":
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write(command);
                            Console.WriteLine(": ");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(restOfLine);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            scenario = restOfLine;
                            break;
                        default:
                            if (line.Length == 0 || command.StartsWith("--"))
                            {
                                Console.WriteLine(line);
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.WriteLine(line);
                                Console.WriteLine("* Invalid command \"{0}\"", command);
                                Console.BackgroundColor = ConsoleColor.Black;
                            }
                            break;

                    }
                }

            }
            Console.WriteLine("Completed");
            Console.ReadKey();
        }

    }
}
