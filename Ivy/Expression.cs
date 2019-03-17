namespace Ivy
{
    public abstract class Expression
    {
        public interface IVisitor<out T>
        {
            T VisitBinaryExpression(Binary expression);
            T VisitUnaryExpression(Unary expression);
            T VisitLiteralExpression(Literal expression);
        }
        
        public class Binary : Expression
        {
            public readonly Expression Left;
            public readonly Token Operator;
            public readonly Expression Right;

            public Binary(Expression left, Token @operator, Expression right)
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

            public Unary(Token @operator, Expression right)
            {
                Operator = @operator;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpression(this);
        }

        public class Literal : Expression
        {
            public readonly object Value;

            public Literal(object value)
            {
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor) =>
                visitor.VisitLiteralExpression(this);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}