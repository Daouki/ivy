namespace Ivy.Utils
{
    public class SourceCodeSpan
    {
        public SourceCodeFile File { get; }
        public int Begin { get; }
        public int Length { get; }
        public int Line { get; }
        public int Column { get; }
        
        public SourceCodeSpan(SourceCodeFile file, int begin, int length)
        {
            File = file;
            Begin = begin;
            Length = length;
            (Line, Column) = File.GetLineAndColumn(begin);
        }
    }
}