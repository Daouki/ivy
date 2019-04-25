using System.Collections.Generic;
using CommandLine;

namespace Ivy.Utils
{
    [Verb("analyze")]
    public class AnalyzeOptions
    {
        [Value(0)]
        public string InputFile { get; set; }
    }
    
    [Verb("compile")]
    public class CompileOptions
    {
        [Value(0)]
        public string InputFile { get; set; }
        
        [Option('o', Required = true)]
        public string OutputFile { get; set; }
    }
    
    [Verb("execute")]
    public class ExecuteOptions
    {
        [Value(0)]
        public string InputFile { get; set; }
    }
    
    [Verb("run")]
    public class RunOptions
    {
        [Value(0)]
        public string InputFile { get; set; }
    }
    
    [Verb("disasm")]
    public class DisassembleOptions
    {
        [Value(0)]
        public string InputFile { get; set; }
    }
}