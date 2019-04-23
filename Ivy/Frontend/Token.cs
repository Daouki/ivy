using Ivy.Utils;

namespace Ivy.Frontend
{
    // I'm really waiting for C#8's record types...
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly object Literal;
        public readonly SourceCodeSpan Span;

        public Token(TokenType type, string lexeme, object literal, SourceCodeSpan span)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Span = span;
        }
    }
}