namespace Ivy
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

        Print,

        Semicolon,

        Unknown,
        EndOfFile
    }
}