using System;
using System.Collections.Generic;

namespace Ivy.Frontend
{
    public class Lexer
    {
        private static readonly Dictionary<string, TokenType> _keywords =
            new Dictionary<string, TokenType>
            {
                {"else", TokenType.Else},
                {"end", TokenType.End},
                {"if", TokenType.If},
                {"let", TokenType.Let},
                {"until", TokenType.Until},
                {"print", TokenType.Print},
                {"while", TokenType.While},
            };

        private static readonly Dictionary<string, TokenType> _twoCharsOperators =
            new Dictionary<string, TokenType>
            {
                {"<<", TokenType.LessLess},
                {">>", TokenType.GreaterGreater},
            };
        
        private static readonly Dictionary<string, TokenType> _singleCharOperators =
            new Dictionary<string, TokenType>
            {
                {"*", TokenType.Asterisk},
                {":", TokenType.Colon},
                {"=", TokenType.Equal},
                {"-", TokenType.Minus},
                {";", TokenType.Semicolon},
                {"/", TokenType.Slash},
                {"+", TokenType.Plus},
                {"<", TokenType.Less},
                {">", TokenType.Greater},
            };

        private readonly string _sourceCode;
        private readonly List<Token> _tokens = new List<Token>();
               
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;
        private int _lastNewLine = 0;

        private Lexer(string sourceCode)
        {
            _sourceCode = sourceCode;
        }

        public static List<Token> ScanTokens(string sourceCode)
        {
            var lexer = new Lexer(sourceCode);
            
            while (!lexer.IsAtEnd())
            {
                lexer._start = lexer._current;
                lexer.ScanToken();
            }

            lexer._start = lexer._current;
            lexer.PushToken(TokenType.EndOfFile);
            return lexer._tokens;
        }

        private void ScanToken()
        {
            var c = GetNextCharacter();
            if (char.IsWhiteSpace(c))
            {
                if (c == '\n')
                {
                    _lastNewLine = _current;
                    _line++;
                }
            }
            else if (char.IsSymbol(c) || char.IsPunctuation(c))
                ScanOperator();
            else if (char.IsDigit(c))
                ScanNumber();
            else if (char.IsLetterOrDigit(c))
                ScanIdentifierOrKeyword();
            else
                throw new Exception("Unknown character");
        }

        private void ScanOperator()
        {
            try
            {
                var twoCharOperator = _sourceCode.Substring(_start, 2);
                if (_twoCharsOperators.TryGetValue(twoCharOperator, out var twoCharsOperator))
                {
                    PushToken(twoCharsOperator);
                }
            }
            catch (IndexOutOfRangeException)
            {
            }

            var c = PeekCurrentCharacter();
            PushToken(_singleCharOperators.TryGetValue(c.ToString(), out var operatorTokenType)
                ? operatorTokenType
                : TokenType.Unknown);
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

            _tokens.Add(new Token(type, lexeme, literal, "", _line, _current - _lastNewLine, _start,
                length));
        }

        private bool IsAtEnd() => _current >= _sourceCode.Length;

        private char GetNextCharacter() => _sourceCode[_current++];

        private char PeekCurrentCharacter() => _current == 0 ? '\0' : _sourceCode[_current - 1];
        
        private char PeekNextCharacter() => IsAtEnd() ? '\0' : _sourceCode[_current];
    }
}