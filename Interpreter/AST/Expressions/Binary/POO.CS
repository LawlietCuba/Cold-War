using System;
using System.Collections.Generic;
public class POObject : BinaryExpression
{
     public override ExpressionType Type{get;set;}

    private object value;

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = value;
    }

    public POObject(CodeLocation location) : base(location){}

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool checkId = false;
        if(context.cards.Contains(Left.GetValue().ToString())) checkId = true;
        else
        {
            errors.Add(new CompilingError(Location, ErrorCode.None, "Doesn't exist in the current context"));
            return false;
        }

        bool checkKeyword = false;
        if(CardAttributes.Contains(Right.GetValue().ToString())) checkKeyword = true;
        else
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Doesn't exist that card Attribute"));
            return false;
        }

        return checkId && checkKeyword;
    }
    public override void Evaluate()
    {
        this.SetValue(10.01);
        this.Type = ExpressionType.Number;
    }

    public void Evaluate(bool OnGame)
    {
        if(OnGame)
        {
            this.SetValue(10.0);
            this.Type = ExpressionType.Number;
        }
        else
        {
            if(Left != null && Left.GetValue() != null)
            {
                CardTemplate scard = Card.SearchCardTemplate(Left.GetValue().ToString());
                if(scard != null)
                {
                    switch(Right.GetValue().ToString())
                    {
                        case TokenValues.Health:
                            this.SetValue(scard.Health);
                            this.Type = ExpressionType.Number;
                            break;
                        case TokenValues.Attack:
                            this.SetValue(scard.Attack);
                            this.Type = ExpressionType.Number;
                            break;
                        case TokenValues.Rareness:
                            this.SetValue(scard.Rareness);
                            this.Type = ExpressionType.Text;
                            break;
                        case TokenValues.political_current:
                            this.SetValue(scard.political_current);
                            this.Type = ExpressionType.Text;
                            break;
                        case TokenValues.CardType:
                            this.SetValue(ExpressionType.Text);
                            this.Type = ExpressionType.Text;
                            break;
                        default:
                            this.SetValue(-1);
                            this.Type = ExpressionType.Number;
                            break;
                    }
                }
                else
                {
                    this.SetValue(-1);
                }
            }
        }
    }

    public override string ToString()
    {
        return String.Format(Left.GetValue().ToString() + " and " + Right.GetValue().ToString());
    }

    private List<string> CardAttributes = new List<string>(){TokenValues.CardType, TokenValues.Rareness, TokenValues.Attack, TokenValues.Health, TokenValues.political_current};
}

