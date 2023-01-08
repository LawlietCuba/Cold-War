public abstract class Expression : ASTNode
{
    public abstract void Evaluate();

    public abstract ExpressionType Type { get; set; }

    public abstract object GetValue();
    public abstract void SetValue(object value);

    public Expression(CodeLocation location) : base (location) { }
}

public enum ExpressionType
{
    Anytype,
    Text,
    Number,
    Boolean,
    ErrorType
}