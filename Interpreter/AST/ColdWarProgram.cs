
using System;
using System.Collections.Generic;
using Godot;

public class ColdWarProgram : ASTNode
{
    public List<CompilingError> Errors {get; set;}
    public Dictionary<string, PoliticalCurrent> political_currents {get; set;}
    public Dictionary<string, Card> Cards {get; set;}
    public List<Expression> PrintingList {get; set;}
    public List<EffectExpression> Effects{get;set;}

    public ColdWarProgram(CodeLocation location) : base (location)
    {
        Errors = new List<CompilingError>();
        political_currents = new Dictionary<string, PoliticalCurrent>();
        Cards = new Dictionary<string, Card>();
        PrintingList = new List<Expression>();
        Effects = new List<EffectExpression>();
    }
    
    /* To check a program semantic we sould first collect all the existing PoliticalCurrents and store them in the context.
    Then, we check semantics of PoliticalCurrents and cards */
    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        bool checkPoliticalCurrents = true;
        foreach (PoliticalCurrent political_current in political_currents.Values)
        {
            checkPoliticalCurrents = checkPoliticalCurrents && political_current.CollectPoliticalCurrents(context, scope.CreateChild(), errors);
        }
        foreach (PoliticalCurrent political_current in political_currents.Values)
        {
            checkPoliticalCurrents = checkPoliticalCurrents && political_current.CheckSemantic(context, scope.CreateChild(), errors);
        }

        bool checkCards = true;
        foreach (Card card in Cards.Values)
        {
            checkCards = checkCards && card.CheckSemantic(context, scope, errors);
        }

        return checkPoliticalCurrents && checkCards;
    }

    public void Evaluate()
    {
        foreach (Card card in Cards.Values)
        {
            card.Evaluate();
        }

        foreach (Expression exp in PrintingList)
        {
            GD.Print(exp);
        }
    }

    public override string ToString()
    {
        string s = "";
        foreach (PoliticalCurrent PoliticalCurrent in political_currents.Values)
        {
            s = s + "\n" + PoliticalCurrent.ToString();
        }
        foreach (Card card in Cards.Values)
        {
            s += "\n" + card.ToString();
        }
        return s;
    }
}