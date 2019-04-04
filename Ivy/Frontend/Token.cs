namespace Ivy.Frontend
{
    // I'm really waiting for C#8's record types...
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly object Literal;
        public readonly string FilePath;
        public readonly int Line;
        public readonly int Column;
        public readonly int Position;
        public readonly int Length;

        public Token(TokenType type, string lexeme, object literal, string filePath, int line,
            int column, int position, int length)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            FilePath = filePath;
            Line = line;
            Column = column;
            Position = position;
            Length = length;
        }
    }
}