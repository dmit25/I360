using System;
using System.IO;
using System.Linq;
using System.Text;
using News360.Equation.Canonization;
using News360.Equation.Parsing;
using News360.Equation.Rendering;

namespace News360.Equation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                RunInteractiveMode();
            }
            else
            {
                try
                {
                    var path = args[0];
                    var parser = new Parser();
                    var canonizer = new Canonizer();
                    var renderer = new Renderer();
                    File
                        .WriteAllLines(
                            path + ".out",
                            File
                                .ReadLines(path)
                                .Select(l => renderer.Render(canonizer.Canonize(parser.Parse(l.ToString()))))
                        );
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }

        private static void RunInteractiveMode()
        {
            Console.TreatControlCAsInput = true;
            var parser = new Parser();
            var canonizer = new Canonizer();
            var renderer = new Renderer();
            Console.WriteLine("Press CTRL+C to quit");
            Console.WriteLine("Enter your expression:");
            var input = new StringBuilder();
            do
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control)
                {
                    return;
                }
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.Write(key.KeyChar);
                }
                else if (key.KeyChar == '\n'
                    || key.KeyChar == '\r')
                {
                    try
                    {
                        Console.Clear();
                        Console.WriteLine($"Expression: {input}");
                        Console.WriteLine($"Result: {renderer.Render(canonizer.Canonize(parser.Parse(input.ToString())))}");
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                    finally
                    {
                        input.Clear();
                        Console.WriteLine("Press CTRL+C to quit");
                        Console.WriteLine("Enter your expression:");
                    }
                }
                else if (key.KeyChar >= '0' && key.KeyChar <= '9'
                    || key.KeyChar >= 'a' && key.KeyChar <= 'z'
                    || key.KeyChar >= 'A' && key.KeyChar <= 'Z'
                    || key.KeyChar == '.'
                    || key.KeyChar == '^'
                    || key.KeyChar == '='
                    || key.KeyChar == ')'
                    || key.KeyChar == '('
                    || key.KeyChar == '-'
                    || key.KeyChar == '+'
                    || key.KeyChar == ' ')
                {
                    input.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            } while (true);
        }
    }
}
