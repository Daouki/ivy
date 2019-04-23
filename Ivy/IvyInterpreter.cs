using System;
using System.IO;
using Ivy.Backend;
using Ivy.Frontend;
using Ivy.Runtime;
using Ivy.Utils;

namespace Ivy
{
    internal static class IvyInterpreter
    {
        public class Context
        {
            public static Context Instance { get; } = new Context();

            public int ErrorsReported { get; private set; }

            public void ReportError(SourceCodeSpan span, string message)
            {
                Console.WriteLine($"{span.File.Path}:{span.Line}:{span.Column}: error: {message}");
                ErrorsReported++;
            }
            
            static Context()
            {
            }

            private Context()
            {
            }
        }

        public static void ReportError(SourceCodeSpan span, string message)
        {
            Context.Instance.ReportError(span, message);
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
                var reportedErrors = RunCode(new SourceCodeFile(filePath, sourceCode));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        private static bool RunCode(SourceCodeFile file)
        {
            var tokens = Lexer.ScanTokens(file);
            if (Context.Instance.ErrorsReported > 0)
                return true;

            var ast = Parser.Parse(tokens);
            if (Context.Instance.ErrorsReported > 0)
                return true;

            // TODO: Scanning on demand.
            tokens.Clear();

            var byteCode = Compiler.Compile(ast);
            if (Context.Instance.ErrorsReported > 0)
                return true;

#if DEBUG
            Disassembler.Disassemble(byteCode.ToArray());
            Console.WriteLine("----------------------------------------");
#endif

            VirtualMachine.Execute(byteCode);
            return false;
        }
    }
}