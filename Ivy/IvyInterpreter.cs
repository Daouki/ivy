using System;
using System.IO;
using Ivy.Backend;
using Ivy.Frontend;
using Ivy.Runtime;

namespace Ivy
{
    internal static class IvyInterpreter
    {
        public class Context
        {
            public static Context Instance { get; } = new Context();

            public int ErrorsReported { get; private set; }

            public void ReportError(string filePath, int line, int column, string message)
            {
                Console.WriteLine($"{filePath}:{line}:{column}: error: {message}");
                ErrorsReported++;
            }
            
            static Context()
            {
            }

            private Context()
            {
            }
        }
    
        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                Console.WriteLine("Usage: ivy [script.ivy]");
                Environment.Exit(1);
            }
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

        private static void RunCode(string sourceCode)
        {
            var tokens = Lexer.ScanTokens(sourceCode);
            if (Context.Instance.ErrorsReported > 0)
                return;

            var ast = Parser.Parse(tokens);
            if (Context.Instance.ErrorsReported > 0)
                return;

            // TODO: Scanning on demand.
            tokens.Clear();

            var byteCode = Compiler.Compile(ast);
            if (Context.Instance.ErrorsReported > 0)
                return;

#if DEBUG
            Disassembler.Disassemble(byteCode.ToArray());
            Console.WriteLine("----------------------------------------");
#endif

            VirtualMachine.Execute(byteCode);
        }
    }
}