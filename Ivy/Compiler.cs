using System;
using System.Collections.Generic;
using System.Linq;

namespace Ivy
{
    // We can't use primitive type void as polymorphic function return type, so we need to use
    // a dummy Void type instead.
    public class Void
    {
    }

    /// <summary>
    /// Generates byte code from an AST.
    /// </summary>
    public class Compiler : Statement.IVisitor<Void>, Expression.IVisitor<Void>
    {
        private readonly List<Statement> _ast;
        private readonly List<byte> _byteCode = new List<byte>();

        private readonly List<List<string>> _locals = new List<List<string>>(32);
        
        public Compiler(List<Statement> ast)
        {
            _ast = ast;
        }

        public List<byte> Compile()
        {
            _locals.Add(new List<string>(512));
            
            foreach (var statement in _ast)
                VisitStatement(statement);
            return _byteCode;
        }

        private void VisitStatement(Statement statement) =>
            statement.Accept(this);

        public Void VisitLetBinding(Statement.LetBinding statement)
        {
            VisitExpression(statement.Initializer);
            StoreLocal(statement.Identifier);
            return null;
        }

        private void StoreLocal(Token identifier)
        {
            _byteCode.Add((byte) Instruction.StoreI64);
            _byteCode.AddRange(BitConverter.GetBytes((ulong) _locals.Count - 1));
            _locals[0].Add(identifier.Lexeme);
        }

        public Void VisitExpresionStatement(Statement.ExpressionStatement statement)
        {
            VisitExpression(statement.Expression);
            return null;
        }
        
        private void VisitExpression(Expression expression) =>
            expression.Accept<Void>(this);

        public Void VisitBinaryExpression(Expression.Binary expression)
        {
            VisitExpression(expression.Right);
            VisitExpression(expression.Left);
            switch (expression.Operator.Type)
            {
                case TokenType.Asterisk:
                    _byteCode.Add((byte) Instruction.MulI64);
                    break;
                
                case TokenType.Minus:
                    _byteCode.Add((byte) Instruction.SubI64);
                    break;
                
                case TokenType.Plus:
                    _byteCode.Add((byte) Instruction.AddI64);
                    break;
                
                case TokenType.Slash:
                    _byteCode.Add((byte) Instruction.DivI64);
                    break;
                
                default:
                    throw new Exception();
            }
            return null;
        }

        public Void VisitUnaryExpression(Expression.Unary expression)
        {
            return null;
        }

        public Void VisitLiteralExpression(Expression.Literal expression)
        {
            switch (expression.Value)
            {
                case long i:
                    _byteCode.Add((byte) Instruction.Push64);
                    _byteCode.AddRange(BitConverter.GetBytes(i));
                    break;
                
                default:
                    throw new Exception();
            }

            return null;
        }
    }
}