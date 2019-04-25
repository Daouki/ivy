using System;
using System.Collections.Generic;
using System.IO;

namespace Ivy.Utils
{
    public class SourceCodeFile
    {
        public string Path { get; }
        public string SourceCode { get; }

        private readonly int[] _newLines;
        
        public SourceCodeFile(string path)
        {
            var sourceCode = File.ReadAllText(path);
            Path = path;
            SourceCode = sourceCode;

            var newLines = new List<int>();
            for (var i = 0; i < SourceCode.Length; i += 1)
            {
                var c = SourceCode[i];
                if (c == '\n')
                {
                    newLines.Add(i);
                }
            }

            _newLines = newLines.ToArray();
        }
        
        public (int, int) GetLineAndColumn(int index)
        {
            var newLineIndex = Array.FindLastIndex(_newLines, i => i < index);
            return newLineIndex != -1
                   // We add 2 here, because we start from line 1, not 0 and line 1 has the index 0
                   // in the new line array.
                ? (newLineIndex + 2, index - _newLines[newLineIndex])
                : (1, index);
        }
    }
}