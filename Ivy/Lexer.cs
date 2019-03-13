using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace Ivy
{
    public class Lexer
    {
        private static readonly Dictionary<string, TokenType> _keywords =
            new Dictionary<string, TokenType>
            {
                {"let", TokenType.Let}
            };

        private readonly string _sourceCode;
        private readonly List<Token> _tokens = new List<Token>();
               
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public Lexer(string sourceCode)
        {
            _sourceCode = sourceCode;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _start = _current;
            PushToken(TokenType.EndOfFile);
            return _tokens;
        }

        private void ScanToken()
        {
            var c = GetNextCharacter();
            if (char.IsWhiteSpace(c))
            {
                if (c == '\n')
                    _line++;
            }
            else if (char.IsSymbol(c) || char.IsPunctuation(c))
                ScanOperator();
            else if (char.IsDigit(c))
                ScanNumber();
            else if (char.IsLetterOrDigit(c))
                ScanIdentifierOrKeyword();
            else
            {
                // TODO: Error.
            }
        }

        private void ScanOperator()
        {
            var c = PeekCurrentCharacter();
            switch (c)
            {
                case '*':
                    PushToken(TokenType.Asterisk);
                    break;
                
                case '=':
                    PushToken(TokenType.Equals);
                    break;
                
                case '-':
                    PushToken(TokenType.Minus);
                    break;
                
                case '+':
                    PushToken(TokenType.Plus);
                    break;
                
                case '/':
                    PushToken(TokenType.Slash);
                    break;
                
                default:
                    PushToken(TokenType.Unknown);
                    break;
            }
        }
        
        private void ScanNumber()
        {
            while (char.IsDigit(PeekNextCharacter()))
                GetNextCharacter();
            PushToken(TokenType.Integer);
        }

        private void ScanIdentifierOrKeyword()
        {
            while (char.IsLetterOrDigit(PeekNextCharacter()))
                GetNextCharacter();

            var identifier = _sourceCode.Substring(_start, _current - _start);
            PushToken(_keywords.TryGetValue(identifier, out var keywordType)
                ? keywordType
                : TokenType.Identifier);
        }
        
        private void PushToken(TokenType type)
        {
            var length = _current - _start;
            var lexeme = _sourceCode.Substring(_start, length);
            object literal = null;
            
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (type)
            {
                case TokenType.Integer:
                    literal = long.Parse(lexeme);
                    break;
            }
            
            _tokens.Add(new Token(type, lexeme, literal, "", _line, 0, _start, length));
        }

        private bool IsAtEnd() => _current >= _sourceCode.Length;

        private char GetNextCharacter()
        {
            _current++;
            return IsAtEnd() ? '\0' : _sourceCode[_current - 1];
        }

        private char PeekCurrentCharacter() => _current == 0 ? '\0' : _sourceCode[_current - 1];
        
        private char PeekNextCharacter() => IsAtEnd() ? '\0' : _sourceCode[_current];
    }
}