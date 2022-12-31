using System;
using System.Collections.Generic;

public class Number : AtomExpression
{
    public bool IsInt
    {
        get
        {
            int a;
            return int.TryParse(GetValue().ToString(), out a);
        }
    }

    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Number;
        }
        set { }
    }

    private object value;

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = value;
    }

    public Number(double value, CodeLocation location) : base(location)
    {
        SetValue(value);
    }
    
    public override bool CheckSemantic(Context context, Scope table, List<CompilingError> errors)
    {
        return true;
    }

    public override void Evaluate()
    {
        
    }

    public override string ToString()
    {
        return String.Format("{0}", GetValue());
    }
}