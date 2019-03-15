using System;
using System.IO;

namespace Ivy
{
    internal static class IvyInterpreter
    {
        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: ivy [script.ivy]");
                Environment.Exit(1);
            }
            else if (args.Length == 1)
                RunFile(args[0]);
            else
                RunREPL();
        }

        private static void RunFile(string filePath)
        {
            try
            {
                var sourceCode = File.ReadAllText(filePath);
                RunCode(sourceCode);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        private static void RunREPL()
        {
            while (true)
            {
                Console.Write("> ");
                RunCode(Console.ReadLine());
            }
        }

        private static void RunCode(string sourceCode)
        {
            var lexer = new Lexer(sourceCode);
            var tokens = lexer.ScanTokens();
            
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            
            // We don't need tokens anymore.
            // TODO: Scanning on demand.
            tokens.Clear();
            
            var compiler = new Compiler(ast);
            var byteCode = compiler.Compile();
            
            var virtualMachine = new VirtualMachine(byteCode);
            virtualMachine.Execute();
        }
    }
}