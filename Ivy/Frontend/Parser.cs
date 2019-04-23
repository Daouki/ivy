using System;
using System.Collections.Generic;
using System.Linq;

namespace Ivy.Frontend
{
    public class Parser
    {
        private static readonly Dictionary<TokenType, int> BinaryOperatorPrecedence =
            new Dictionary<TokenType, int>
            {
                {TokenType.Caret, 80},
                {TokenType.Pipe, 90},
                {TokenType.Ampersand, 100},
                {TokenType.Less, 140},
                {TokenType.Greater, 140},
                {TokenType.LessLess, 150},
                {TokenType.GreaterGreater, 150},
                {TokenType.Minus, 190},
                {TokenType.Plus, 190},
                {TokenType.Asterisk, 200},
                {TokenType.Slash, 200},
            };
        
        private readonly List<Token> _tokens;
        private int _current;
        
        public static List<Statement> Parse(List<Token> tokens)
        {
            return new Parser(tokens).Parse();
        }

        private Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }
        
        private List<Statement> Parse()
        {
            var ast = new List<Statement>();
            try
            {
                while (!IsAtEnd())
                    ast.Add(ParseStatement());
            }
            catch (ParseException e)
            {
                IvyInterpreter.Context.Instance.ReportError(e.Token.Span, e.Message);
            }

            return ast;
        }

        private Statement ParseStatement()
        {
            if (MatchToken(TokenType.Let))
                return ParseLetBinding();
            if (MatchToken(TokenType.If))
                return ParseIfStatement();
            if (MatchToken(TokenType.Until))
                return ParseWhile(true);
            if (MatchToken(TokenType.Print))
                return ParsePrintStatement();
            if (MatchToken(TokenType.While))
                return ParseWhile(false);
                
            var expression = ParseExpression();
            if (MatchToken(TokenType.Equal))
                return ParseAssignment(expression);
            ConsumeToken(TokenType.Semicolon);
            return new Statement.ExpressionStatement(expression);
        }

        private Statement ParseLetBinding()
        {
            var identifier = ConsumeToken(TokenType.Identifier);
            ConsumeToken(TokenType.Equal);
            var initializer = ParseExpression();
            ConsumeToken(TokenType.Semicolon);
            return new Statement.LetBinding(identifier, initializer);
        }

        private Statement ParseIfStatement()
        {
            var condition = ParseExpression();
            ConsumeToken(TokenType.Colon);
            var thenBlock = ParseBlock(TokenType.Else, TokenType.End);
            if (PeekPreviousToken().Type == TokenType.Else)
                return new Statement.If(condition, thenBlock, ParseBlock(TokenType.End));
            return new Statement.If(condition, thenBlock, null);
        }

        private Statement ParseWhile(bool isUntilLoop)
        {
            var condition = ParseExpression();
            ConsumeToken(TokenType.Colon);
            var body = ParseBlock(TokenType.End);
            return new Statement.While(condition, body, isUntilLoop);
        }

        private Statement ParsePrintStatement()
        {
            var expression = ParseExpression();
            ConsumeToken(TokenType.Semicolon);
            return new Statement.Print(expression);
        }

        private Statement ParseAssignment(Expression target)
        {
            var value = ParseExpression();
            ConsumeToken(TokenType.Semicolon);
            return new Statement.Assignment(target, value);
        }

        private List<Statement> ParseBlock(params TokenType[] until)
        {
            var newUntil = new TokenType[until.Length + 1];
            until.CopyTo(newUntil, 0);
            newUntil[newUntil.Length - 1] = TokenType.EndOfFile;

            var beginToken = PeekPreviousToken();
            
            var statements = new List<Statement>();
            while (!MatchToken(newUntil))
                statements.Add(ParseStatement());

            if (PeekCurrentToken().Type == TokenType.EndOfFile)
                ReportError(beginToken, "No `end' to match this `:'.");
                
            return statements;
        }

        private Expression ParseExpression() => ParseBinaryExpression();

        private Expression ParseBinaryExpression()
        {
            var left = ParseUnaryExpression();
            return ParseBinaryExpression(left, 0);
        }

        private Expression ParseBinaryExpression(Expression left, int expressionPrecedence)
        {
            while (true)
            {
                var operatorPrecedence = GetNextTokenPrecedence();
                if (operatorPrecedence < expressionPrecedence)
                    return left;

                var @operator = GetNextToken();
                var right = ParseUnaryExpression();
                var nextOperatorPrecedence = GetNextTokenPrecedence();
                if (operatorPrecedence < nextOperatorPrecedence)
                    right = ParseBinaryExpression(right, operatorPrecedence + 1);

                left = new Expression.Binary(left.Span, left, @operator, right);
            }
        }

        private Expression ParseUnaryExpression()
        {
            return ParsePrimaryExpression();
        }

        private Expression ParsePrimaryExpression()
        {
            var token = GetNextToken();
            switch (token.Type)
            {
                case TokenType.Integer:
                    return new Expression.Literal(token.Span, token.Literal);
                
                case TokenType.Identifier:
                    return new Expression.AtomReference(token.Span, token);
                
                default:
                    throw new ParseException($"Expected primary expression, got {token.Type.ToString()}.", token);
            }
        }

        /// <summary>
        /// Check if the next token type matches any of the given types.
        /// </summary>
        /// <param name="types">Token types to compare with.</param>
        /// <returns>
        /// True if the next token type matches any of the given types; false otherwise.
        /// </returns>
        private bool MatchToken(params TokenType[] types)
        {
            if (types.All(type => type != PeekCurrentToken().Type))
                return false;
            
            GetNextToken();
            return true;
        }

        /// <summary>
        /// Check if the next token type matches any of the given types. Throws an exception
        /// when fails to match the token type.
        /// </summary>
        /// <param name="types">Token types to compare with.</param>
        private Token ConsumeToken(params TokenType[] types)
        {
            if (!MatchToken(types))
                throw new ParseException($"Expected token type: [{string.Join(", ", types)}], got {PeekCurrentToken().Type.ToString()}.",
                    PeekCurrentToken());
            return PeekPreviousToken();
        }

        private bool IsAtEnd() => PeekCurrentToken().Type == TokenType.EndOfFile;

        private Token GetNextToken()
        {
            if (!IsAtEnd())
                _current++;
            return PeekPreviousToken();
        }
        
        private Token PeekCurrentToken() => _tokens[_current];

        private Token PeekPreviousToken() => _current != 0 ? _tokens[_current - 1] : _tokens[0];

        private int GetNextTokenPrecedence() =>
            BinaryOperatorPrecedence.GetValueOrDefault(PeekCurrentToken().Type, -1);

        private void ReportError(Token token, string message)
        {
            IvyInterpreter.Context.Instance.ReportError(token.Span, message);
        }
    }
}