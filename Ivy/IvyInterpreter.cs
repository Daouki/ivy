using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

using Ivy.Backend;
using Ivy.Frontend;
using Ivy.Runtime;
using Ivy.Utils;

using Parser = Ivy.Frontend.Parser;

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
            var exitCode = CommandLine.Parser.Default.ParseArguments
                <AnalyzeOptions, CompileOptions, ExecuteOptions, RunOptions, DisassembleOptions
                >(args)
                .MapResult(
                    (AnalyzeOptions options) => AnalyzeFile(options.InputFile),
                    (CompileOptions options) => CompileFile(options.InputFile, options.OutputFile),
                    (ExecuteOptions options) => ExecuteFile(options.InputFile),
                    (RunOptions options) => RunFile(options.InputFile),
                    (DisassembleOptions options) => DisassembleFile(options.InputFile),
                    errs => 1
                );
            
            Environment.Exit(exitCode);
        }

        private static int AnalyzeFile(string filePath)
        {
            try
            {
                var file = new SourceCodeFile(filePath);
                return AnalyzeCode(file) != null ? 1 : 0;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return 0;
        }

        private static List<Statement> AnalyzeCode(SourceCodeFile file)
        {
            var tokens = Lexer.ScanTokens(file);
            if (Context.Instance.ErrorsReported > 0)
                return null;

            var ast = Parser.Parse(tokens);
            return Context.Instance.ErrorsReported == 0 ? ast : null;
        }

        private static int CompileFile(string inputFilePath, string outputFilePath)
        {
            try
            {
                var file = new SourceCodeFile(inputFilePath);
                var ast = AnalyzeCode(file);
                if (ast == null)
                    return 1;

                var byteCode = Compiler.Compile(ast);
                if (Context.Instance.ErrorsReported > 0)
                    return 1;

                File.WriteAllBytes(inputFilePath ?? "out.ibc", byteCode.ToArray());
                return 0;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return 1;
        }

        private static int ExecuteFile(string filePath)
        {
            try
            {
                var byteCode = File.ReadAllBytes(filePath);
                VirtualMachine.Execute(byteCode.ToList());
                return Context.Instance.ErrorsReported > 0 ? 1 : 0;
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }
        }
        
        private static int RunFile(string filePath)
        {
            try
            {
                var sourceFile = new SourceCodeFile(filePath);
                var reportedErrors = RunCode(sourceFile);
                return reportedErrors ? 1 : 0;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return 0;
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
        
        private static int DisassembleFile(string filePath)
        {
            try
            {
                var byteCode = File.ReadAllBytes(filePath);
                Disassembler.Disassemble(byteCode);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }
    }
}