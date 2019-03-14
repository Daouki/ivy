namespace Ivy
{
    public class Expression
    {
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
        }

        public class Literal : Expression
        {
            public readonly object Value;

            public Literal(object value)
            {
                Value = value;
            }
        }
    }
}