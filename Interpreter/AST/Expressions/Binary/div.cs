using System;
using System.Collections.Generic;
public class Div : BinaryExpression
{
    public override ExpressionType Type {get; set;}

    private object value;

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = value;
    }

    public Div(CodeLocation location) : base(location){}

    public override void Evaluate()
    {
        Right.Evaluate();
        Left.Evaluate();

        SetValue((double)Left.GetValue() / (double)Right.GetValue());
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool right = Right.CheckSemantic(context, scope, errors);
        bool left = Left.CheckSemantic(context, scope, errors);
        if (Right.Type != ExpressionType.Number || Left.Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "We don't do that here... "));
            Type = ExpressionType.ErrorType;
            return false;
        }

        Type = ExpressionType.Number;
        return right && left;
    }

    public override string ToString()
    {
        if (GetValue() == null)
        {
            return String.Format("({0} / {1})", Left, Right);
        }
        return GetValue().ToString();
    }
}