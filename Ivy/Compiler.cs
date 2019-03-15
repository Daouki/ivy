using System;
using System.Collections.Generic;

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
    public class Compiler : Expression.IVisitor<Void>
    {
        private readonly Expression _ast;
        private readonly List<byte> _byteCode = new List<byte>();
        
        public Compiler(Expression ast)
        {
            _ast = ast;
        }

        public List<byte> Compile()
        {
            VisitExpression(_ast);
            return _byteCode;
        }

        private void VisitExpression(Expression expression) =>
            expression.Accept<Void>((Expression.IVisitor<Void>) this);
        
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