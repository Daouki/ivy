namespace Ivy.Frontend
{
    public enum TokenType
    {
        // Literals.
        Identifier,
        Integer,

        // Keywords.
        Let,
        If,
        Else,
        End,
        
        // Operators.
        Asterisk,
        Equals,
        Minus,
        Plus,
        Slash,
        Less,
        Greater,

        Colon,
        Semicolon,

        Print,

        Unknown,
        EndOfFile
    }
}