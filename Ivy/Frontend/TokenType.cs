namespace Ivy.Frontend
{
    public enum TokenType
    {
        // Literals.
        Identifier,
        Integer,

        // Keywords.
        Else,
        End,
        If,
        Let,
        Until,
        Print,
        While,
        
        // Operators.
        LeftParenthesis,
        RightParenthesis,
        Ampersand,
        Asterisk,
        Caret,
        Equal,
        Minus,
        Pipe,
        Plus,
        Slash,
        Less,
        Greater,
        
        LessLess,
        GreaterGreater,

        Colon,
        Semicolon,

        Unknown,
        EndOfFile
    }
}