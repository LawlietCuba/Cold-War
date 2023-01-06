using System;
using System.Collections.Generic;
public class BoolComparer : BinaryExpression
{
    public string op_comparer;
    private object value;

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = value;
    }

    public override ExpressionType Type{get;set;}

    public BoolComparer(CodeLocation location) : base(location)
    {

    }

    public override void Evaluate()
    {
        Right.Evaluate();
        Left.Evaluate();

        if(Right.Type == ExpressionType.Number && Left.Type == ExpressionType.Number)
        {
            switch(op_comparer.ToString())
            {
                case TokenValues.BooleanGreather:
                    SetValue((double)Left.GetValue() - (double)Right.GetValue() > 0);
                    break;
                case TokenValues.BooleanGreatherEqual:
                    SetValue((double)Left.GetValue() - (double)Right.GetValue() >= 0);
                    break;
                case TokenValues.BooleanSmaller:
                    SetValue((double)Left.GetValue() - (double)Right.GetValue() < 0);
                    break;
                case TokenValues.BooleanSmallerEqual:
                    SetValue((double)Left.GetValue() - (double)Right.GetValue() <= 0);
                    break;
                case TokenValues.BooleanEqual:
                    SetValue((double)Left.GetValue() - (double)Right.GetValue() == 0);
                    break;
                default:
                    throw new ArgumentException("Da fack with this Numerica Bool Comparer");
            }
        }
        else
        {
            if(op_comparer.ToString() == TokenValues.BooleanEqual)
            {
                if(Left.GetValue().ToString() == Right.GetValue().ToString())
                {
                    SetValue(true);
                }
                else
                {
                    SetValue(false);
                }
            }
            else
            {
                throw new ArgumentException("We don't do that here");
            }
        }
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool right = Right.CheckSemantic(context, scope, errors);
        bool left = Left.CheckSemantic(context, scope, errors);

        if ((Right.Type == ExpressionType.Number && Left.Type == ExpressionType.Number) || (Right.Type == ExpressionType.Text && Left.Type == ExpressionType.Text))
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "We don't do that here... "));
            Type = ExpressionType.ErrorType;
            return false;
        }

        Type = ExpressionType.Boolean;
        return right && left;
    }

    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}

