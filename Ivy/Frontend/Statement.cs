using System.Collections.Generic;

namespace Ivy.Frontend
{
    public abstract class Statement
    {
        public interface IVisitor<out T>
        {
            T VisitLetBinding(LetBinding statement);
            T VisitIf(If statement);
            T VisitPrint(Print statement);
            T VisitAssignment(Assignment statement);
            T VisitExpresionStatement(ExpressionStatement statement);
        }

        public class LetBinding : Statement
        {
            public Token Identifier;
            public Expression Initializer;

            public LetBinding(Token identifier, Expression initializer)
            {
                Identifier = identifier;
                Initializer = initializer;
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitLetBinding(this);
        }

        public class If : Statement
        {
            public Expression Condition;
            public List<Statement> ThenBlock;
            public List<Statement> ElseBlock;

            public If(Expression condition, List<Statement> thenBlock, List<Statement> elseBlock)
            {
                Condition = condition;
                ThenBlock = thenBlock;
                ElseBlock = elseBlock;
            }
            
            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitIf(this);
        }

        public class Print : Statement
        {
            public Expression Expression;

            public Print(Expression expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitPrint(this);
        }

        public class Assignment : Statement
        {
            public Expression Target;
            public Expression Value;

            public Assignment(Expression target, Expression value)
            {
                Target = target;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitAssignment(this);
        }

        public class ExpressionStatement : Statement
        {
            public Expression Expression;

            public ExpressionStatement(Expression expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitExpresionStatement(this);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}