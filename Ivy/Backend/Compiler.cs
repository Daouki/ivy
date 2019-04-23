using System;
using System.Collections.Generic;
using System.ComponentModel;
using Ivy.Frontend;

namespace Ivy.Backend
{
    using ByteCodeChunk = List<byte>;

    internal static class ByteCodeChunkExtensions
    {
        public static void AddInstruction(this ByteCodeChunk chunk, Instruction instruction)
        {
            chunk.Add((byte) instruction);
        }

        public static void AddInstruction(this ByteCodeChunk chunk, Instruction instruction,
            long value)
        {
            chunk.Add((byte) instruction);
            chunk.AddRange(BitConverter.GetBytes(value));
        }

        public static void AddInstruction(this ByteCodeChunk chunk, Instruction instruction,
            ulong value)
        {
            chunk.Add((byte) instruction);
            chunk.AddRange(BitConverter.GetBytes(value));
        }
    }
    
    /// <summary>
    /// Generates byte code from an AST.
    /// </summary>
    public class Compiler : Statement.IVisitor<ByteCodeChunk>, Expression.IVisitor<ByteCodeChunk>
    {
        private readonly List<Statement> _ast;

        private readonly List<List<string>> _locals = new List<List<string>>(32);

        private Compiler(List<Statement> ast)
        {
            _ast = ast;
            _locals.Add(new List<string>(512));
        }

        public static List<byte> Compile(List<Statement> ast)
        {
            var compiler = new Compiler(ast);
            var byteCode = new ByteCodeChunk();
            foreach (var statement in compiler._ast)
                byteCode.AddRange(compiler.VisitStatement(statement));
            return byteCode;
        }

        private ByteCodeChunk VisitStatement(Statement statement) =>
            statement.Accept(this);

        public ByteCodeChunk VisitLetBinding(Statement.LetBinding statement)
        {
            var chunk = new ByteCodeChunk();
            chunk.AddRange(VisitExpression(statement.Initializer));
            chunk.AddRange(StoreLocal(statement.Identifier));
            return chunk;
        }

        public ByteCodeChunk VisitIf(Statement.If statement)
        {
            var chunk = new ByteCodeChunk();
            chunk.AddRange(VisitExpression(statement.Condition));
            var thenBlock = VisitBlock(statement.ThenBlock);
            chunk.AddInstruction(Instruction.JmpIfZero, thenBlock.Count);
            chunk.AddRange(thenBlock);
            
            if (statement.ElseBlock == null)
                return chunk;
            
            var elseBlock = VisitBlock(statement.ElseBlock);
            chunk.AddInstruction(Instruction.JmpShort, elseBlock.Count);
            chunk.AddRange(elseBlock);
            return chunk;
        }

        public ByteCodeChunk VisitWhile(Statement.While statement)
        {
            var chunk = new ByteCodeChunk();
            chunk.AddRange(VisitExpression(statement.Condition));
            var body = VisitBlock(statement.Body);
            chunk.AddInstruction(
                statement.IsUntilLoop ? Instruction.JmpIfTrue : Instruction.JmpIfFalse,
                body.Count + 9);    // 9 is the size of the final JmpShort instruction.
            chunk.AddRange(body);
            chunk.AddInstruction(Instruction.JmpShort, -(chunk.Count + 9));
            return chunk;
        }

        private ByteCodeChunk VisitBlock(IEnumerable<Statement> block)
        {
            var chunk = new ByteCodeChunk();
            foreach (var statement in block)
                chunk.AddRange(VisitStatement(statement));
            return chunk;
        }

        public ByteCodeChunk VisitPrint(Statement.Print statement)
        {
            var chunk = new ByteCodeChunk();
            chunk.AddRange(VisitExpression(statement.Expression));
            chunk.AddInstruction(Instruction.PrintI64);
            return chunk;
        }

        public ByteCodeChunk VisitAssignment(Statement.Assignment statement)
        {
            var chunk = new ByteCodeChunk();
            if (statement.Target is Expression.AtomReference atom)
            {
                var localIndex = _locals[0].IndexOf(atom.Identifier.Lexeme);
                if (localIndex >= 0)
                {
                    chunk.AddRange(VisitExpression(statement.Value));
                    chunk.AddInstruction(Instruction.Store, (ulong) localIndex);
                }
                else
                {
                    IvyInterpreter.ReportError(statement.Target.Span,
                        "The assignment target wasn't declared in this scope.");
                }
            }
            else
            {
                IvyInterpreter.ReportError(statement.Target.Span,
                    "The expression isn't a valid assignment target.");
            }
            return chunk;
        }

        private ByteCodeChunk StoreLocal(Token identifier)
        {
            var chunk = new ByteCodeChunk(9);
            chunk.AddInstruction(Instruction.Store, (ulong) _locals[0].Count);
            _locals[0].Add(identifier.Lexeme);
            return chunk;
        }

        public ByteCodeChunk VisitExpresionStatement(Statement.ExpressionStatement statement)
        {
            var chunk = new ByteCodeChunk();
            chunk.AddRange(VisitExpression(statement.Expression));
            chunk.AddInstruction(Instruction.Pop64);
            return chunk;
        }

        private ByteCodeChunk VisitExpression(Expression expression) =>
            expression.Accept(this);

        public ByteCodeChunk VisitBinaryExpression(Expression.Binary expression)
        {
            var chunk = new ByteCodeChunk();
            chunk.AddRange(VisitExpression(expression.Right));
            chunk.AddRange(VisitExpression(expression.Left));
            switch (expression.Operator.Type)
            {
                case TokenType.Asterisk:
                    chunk.AddInstruction(Instruction.MulI);
                    break;
                
                case TokenType.Minus:
                    chunk.AddInstruction(Instruction.SubI);
                    break;
                
                case TokenType.Plus:
                    chunk.AddInstruction(Instruction.AddI);
                    break;
                
                case TokenType.Slash:
                    chunk.AddInstruction(Instruction.DivI);
                    break;
                
                case TokenType.Less:
                    chunk.AddInstruction(Instruction.CmpLessI);
                    break;
                
                case TokenType.Greater:
                    chunk.AddInstruction(Instruction.CmpGreaterI);
                    break;
                
                default:
                    throw new Exception();
            }
            
            return chunk;
        }

        public ByteCodeChunk VisitUnaryExpression(Expression.Unary expression)
        {
            return null;
        }

        public ByteCodeChunk VisitLiteral(Expression.Literal expression)
        {
            var chunk = new ByteCodeChunk(9);
            switch (expression.Value)
            {
                case long i:
                    chunk.AddInstruction(Instruction.Push64, i);
                    break;
                
                default:
                    throw new Exception();
            }

            return chunk;
        }

        public ByteCodeChunk VisitAtomReference(Expression.AtomReference expression)
        {
            var chunk = new ByteCodeChunk(9);
            var identifier = expression.Identifier;
            var localIndex = _locals[0].FindIndex((local) => local == identifier.Lexeme); 
            if (localIndex > -1)
            {
                chunk.AddInstruction(Instruction.Load, (ulong) localIndex);
            }
            else
            {
                IvyInterpreter.Context.Instance.ReportError(identifier.Span,
                    "Identifier was not defined in the current scope");
            }
            return chunk;
        }
    }
}
