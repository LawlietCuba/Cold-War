using System;
using System.Collections.Generic;

public class Add : BinaryExpression
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

    public Add(CodeLocation location) : base(location){}

    public override void Evaluate()
    {
        Right.Evaluate();
        Left.Evaluate();

        SetValue((double)Right.GetValue() + (double)Left.GetValue());
    }

     /* We check semantics for the Right and Left Expressions. Both must be numerical types
     because our little game can only operate over numbers. A good idea could be extend this for
     add (concat) text types. There you have a homework.
     Sub, Mul and Div are the same thing. */
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
            return String.Format("({0} + {1})", Left, Right);
        }
        return GetValue().ToString();
    }
}
