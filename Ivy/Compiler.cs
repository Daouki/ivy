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

        public Void VisitIf(Statement.If statement)
        {
            int elseOffset;
            
            VisitExpression(statement.Condition);
            _byteCode.Add((byte) Instruction.JmpIfFalse);
            var ifFalseJumpOffsetIndex = _byteCode.Count;
            _byteCode.AddRange(BitConverter.GetBytes(0ul));

            var thenOffset = _byteCode.Count;
            VisitBlock(statement.ThenBlock);

            if (statement.ElseBlock != null)
            {
                elseOffset = _byteCode.Count - thenOffset;
                VisitBlock(statement.ElseBlock);
            }
            else
            {
                elseOffset = _byteCode.Count - thenOffset;
            }
            
            // TODO: There must be a better way.
            _byteCode.RemoveRange(ifFalseJumpOffsetIndex, 8);
            _byteCode.InsertRange(ifFalseJumpOffsetIndex, BitConverter.GetBytes((long) elseOffset));
            return null;
        }

        private void VisitBlock(IEnumerable<Statement> block)
        {
            foreach (var statement in block)
                VisitStatement(statement);
        }

        public Void VisitPrint(Statement.Print statement)
        {
            VisitExpression(statement.Expression);
            _byteCode.Add((byte) Instruction.PrintI64);
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
                
                case TokenType.Less:
                    _byteCode.Add((byte) Instruction.CmpLessI64);
                    break;
                
                case TokenType.Greater:
                    _byteCode.Add((byte) Instruction.CmpGreaterI64);
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

        public Void VisitLiteral(Expression.Literal expression)
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

        public Void VisitAtomReference(Expression.AtomReference expression)
        {
            var identifier = expression.Identifier;
            var localIndex = _locals[0].FindIndex((local) => local == identifier.Lexeme); 
            if (localIndex > -1)
            {
                _byteCode.Add((byte) Instruction.LoadI64);
                _byteCode.AddRange(BitConverter.GetBytes((ulong) localIndex));
            }
            else
            {
                IvyInterpreter.Context.Instance.ReportError(identifier.FilePath, identifier.Line,
                    identifier.Column, "Identifier was not defined in the current scope");
            }
            return null;
        }
    }
}
