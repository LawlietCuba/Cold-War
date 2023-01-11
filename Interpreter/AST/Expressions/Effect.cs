using System.Collections.Generic;

public class EffectExpression : Expression
{
    private object value; // amount of cards
    public Expression Amount{get;set;}
    public override ExpressionType Type {  
        get;set;
    }
    public string EffectConditional{get;set;}
    public Card CardToHandle{get;set;}
    public EffectExpression(CodeLocation location) : base(location)
    {

    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool checkValue = false;
        if(value is string)
        {
            checkValue = true;
        }
        else
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "The effect isn't correct"));
            Type = ExpressionType.ErrorType;
            return false;
        }

        bool checkAmount = CheckSemantic(context, scope, errors);
        if(Amount.Type != ExpressionType.Number)
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "We don't do that here... "));
            Type = ExpressionType.ErrorType;
            return false;
        }

        bool checkCardToHandle = CheckSemantic(context, scope, errors);
        if(CardToHandle != null)
        {
            if(context.cards.Contains(CardToHandle.Id))
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid, "That card doesn't exist in the actual context"));
                Type = ExpressionType.ErrorType;
                return false;
            }
        }

        return checkValue && checkAmount && checkCardToHandle;

    }

    public override void Evaluate()
    {
        Amount.Evaluate();
    }
    public override void SetValue(object value)
    {
        this.value = value;
    }

    public override object GetValue()
    {
        return value;
    }

    public override string ToString()
    {
        return string.Format(value.ToString() + " " + Amount);
    }

    protected List<string> ListOfEffects = new List<string>(){};

}