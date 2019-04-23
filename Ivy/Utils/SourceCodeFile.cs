using System;
using System.Collections.Generic;

namespace Ivy.Utils
{
    public class SourceCodeFile
    {
        public string Path { get; }
        public string SourceCode { get; }
        public int[] NewLines { get; }
        
        public SourceCodeFile(string path, string sourceCode)
        {
            Path = path;
            SourceCode = sourceCode;

            var newLines = new List<int>();
            for (var i = 0; i < sourceCode.Length; i += 1)
            {
                var c = sourceCode[i];
                if (c == '\n')
                {
                    newLines.Add(i);
                }
            }

            NewLines = newLines.ToArray();
        }

        public (int, int) GetLineAndColumn(int index)
        {
            var newLineIndex = Array.FindLastIndex(NewLines, i => i < index);
            return newLineIndex != -1
                   // We add 2 here, because we start from line 1, not 0 and line 1 has the index 0
                   // in the new line array.
                ? (newLineIndex + 2, index - NewLines[newLineIndex])
                : (1, index);
        }
    }
}