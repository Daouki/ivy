using Ivy.Utils;

namespace Ivy.Frontend
{
    public abstract class Expression
    {
        public interface IVisitor<out T>
        {
            T VisitBinaryExpression(Binary expression);
            T VisitUnaryExpression(Unary expression);
            T VisitLiteral(Literal expression);
            T VisitAtomReference(AtomReference expression);
        }
        
        public class Binary : Expression
        {
            public readonly Expression Left;
            public readonly Token Operator;
            public readonly Expression Right;

            public Binary(SourceCodeSpan span, Expression left, Token @operator, Expression right)
                : base(span)
            {
                Left = left;
                Operator = @operator;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryExpression(this);
        }

        public class Unary : Expression
        {
            public readonly Token Operator;
            public readonly Expression Right;

            public Unary(SourceCodeSpan span, Token @operator, Expression right)
                : base(span)
            {
                Operator = @operator;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpression(this);
        }

        public class Literal : Expression
        {
            public readonly object Value;

            public Literal(SourceCodeSpan span, object value)
                : base(span)
            {
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitLiteral(this);
        }

        public class AtomReference : Expression
        {
            public Token Identifier;

            public AtomReference(SourceCodeSpan span, Token identifier)
                : base(span)
            {
                Identifier = identifier;
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitAtomReference(this);
        }

        public SourceCodeSpan Span { get; }

        public Expression(SourceCodeSpan span)
        {
            Span = span;
        }
        
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}