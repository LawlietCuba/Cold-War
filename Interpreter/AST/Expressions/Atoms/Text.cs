using System;
using System.Collections.Generic;

public class Text : AtomExpression
{
    public override ExpressionType Type
    {
        get
        {
            return ExpressionType.Text;
        }
        set { }
    }

    public object value;

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = value;
    }

    public Text(string value, CodeLocation location) : base(location)
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
        if(object.Equals(GetValue(), null)) return String.Format("");
        else return String.Format("{0}", GetValue());
    }
}