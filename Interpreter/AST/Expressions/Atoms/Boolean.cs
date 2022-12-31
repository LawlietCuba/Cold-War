using System.Collections.Generic;

public class Boolean : AtomExpression
{
    private object value;

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = value;
    }

    public override ExpressionType Type { 
        get
        {
            return ExpressionType.Boolean;
        }
        set{}
    }

    public Boolean(bool value, CodeLocation location) : base(location)
    {
        this.SetValue(value);
    }

    public override string ToString()
    {
        return GetValue().ToString();
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        return true;
    }

    public override void Evaluate()
    {
        
    }
}