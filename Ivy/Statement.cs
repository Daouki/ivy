namespace Ivy
{
    public abstract class Statement
    {
        public interface IVisitor<out T>
        {
            T VisitExpresionStatement(ExpressionStatement statement);
        }

        public class ExpressionStatement : Statement
        {
            public Expression Expression;

            public ExpressionStatement(Expression expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitExpresionStatement(this);
            }
        }

        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}