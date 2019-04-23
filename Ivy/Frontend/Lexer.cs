using System;
using System.Collections.Generic;
using Ivy.Utils;

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
                {"&", TokenType.Ampersand},
                {"*", TokenType.Asterisk},
                {"^", TokenType.Caret},
                {":", TokenType.Colon},
                {"=", TokenType.Equal},
                {"-", TokenType.Minus},
                {";", TokenType.Semicolon},
                {"/", TokenType.Slash},
                {"|", TokenType.Pipe},
                {"+", TokenType.Plus},
                {"<", TokenType.Less},
                {">", TokenType.Greater},
            };

        private readonly SourceCodeFile _file;
        private readonly List<Token> _tokens = new List<Token>();

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;
        private int _lastNewLine = 0;

        private Lexer(SourceCodeFile file)
        {
            _file = file;
        }

        public static List<Token> ScanTokens(SourceCodeFile file)
        {
            var lexer = new Lexer(file);
            
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
            if (_start + 2 < _file.SourceCode.Length)
            {
                var twoCharOperator = _file.SourceCode.Substring(_start, 2);
                if (_twoCharsOperators.TryGetValue(twoCharOperator, out var twoCharsOperator))
                {
                    PushToken(twoCharsOperator);
                }
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

            var identifier = _file.SourceCode.Substring(_start, _current - _start);
            PushToken(_keywords.TryGetValue(identifier, out var keywordType)
                ? keywordType
                : TokenType.Identifier);
        }

        private void PushToken(TokenType type)
        {
            var length = _current - _start;
            var tokenSpan = new SourceCodeSpan(_file, _start, length);
            var lexeme = _file.SourceCode.Substring(_start, length);
            object literal = null;
            
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (type)
            {
                case TokenType.Integer:
                    literal = long.Parse(lexeme);
                    break;

                case TokenType.Unknown:
                    IvyInterpreter.Context.Instance.ReportError(tokenSpan,
                        $"Invalid token `{lexeme}'.");
                    break;
            }

            _tokens.Add(new Token(type, lexeme, literal, tokenSpan));
        }

        private bool IsAtEnd() => _current >= _file.SourceCode.Length;

        private char GetNextCharacter() => _file.SourceCode[_current++];

        private char PeekCurrentCharacter() => _current == 0 ? '\0' : _file.SourceCode[_current - 1];
        
        private char PeekNextCharacter() => IsAtEnd() ? '\0' : _file.SourceCode[_current];
    }
}