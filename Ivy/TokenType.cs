namespace Ivy
{
    public enum TokenType
    {
        // Literals.
        Identifier,
        Integer,

        // Keywords.
        Let,
        
        // Operators.
        Asterisk,
        Equals,
        Minus,
        Plus,
        Slash,

        Print,

        Semicolon,

        Unknown,
        EndOfFile
    }
}