using System.Collections.Generic;
using System.Linq;

namespace Ivy
{
    public class Parser
    {
        private static readonly Dictionary<TokenType, int> BinaryOperatorPrecedence =
            new Dictionary<TokenType, int>
            {
                {TokenType.Minus, 30},
                {TokenType.Plus, 30},
                {TokenType.Asterisk, 40},
                {TokenType.Slash, 40},
            };
        
        private readonly List<Token> _tokens;
        private int _current;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }
        
        public List<Statement> Parse()
        {
            var ast = new List<Statement>();
            try
            {
                while (!IsAtEnd())
                    ast.Add(ParseStatement());
            }
            catch (ParseException e)
            {
                var token = e.Token;
                IvyInterpreter.Context.Instance.ReportError(token.FilePath, token.Line,
                    token.Column, e.Message);
            }

            return ast;
        }

        private Statement ParseStatement()
        {
            if (MatchToken(TokenType.Let))
                return ParseLetBinding();
            return new Statement.ExpressionStatement(ParseExpression());
        }

        private Statement ParseLetBinding()
        {
            var identifier = ConsumeToken(TokenType.Identifier);
            ConsumeToken(TokenType.Equals);
            var initializer = ParseExpression();
            ConsumeToken(TokenType.Semicolon);
            return new Statement.LetBinding(identifier, initializer);
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

                left = new Expression.Binary(left, @operator, right);
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
                    return new Expression.Literal(token.Literal);
                
                default:
                    throw new ParseException("Expected primary expression", token);
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
                throw new ParseException("TODO: ConsumeToken: token type not matched");
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
    }
}