public abstract class BinaryExpression : Expression
{
    public Expression Right { get; set; }
    public Expression Left { get; set; }
    
    public BinaryExpression(CodeLocation location) : base(location){}
}