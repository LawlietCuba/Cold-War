
using System;
using System.Collections.Generic;
using Godot;

public class ColdWarProgram : ASTNode
{
    public List<CompilingError> Errors {get; set;}
    public Dictionary<string, PoliticalCurrent> political_currents {get; set;}
    public Dictionary<string, Card> Cards {get; set;}
    public List<Expression> PrintingList {get; set;}

    public ColdWarProgram(CodeLocation location) : base (location)
    {
        Errors = new List<CompilingError>();
        political_currents = new Dictionary<string, PoliticalCurrent>();
        Cards = new Dictionary<string, Card>();
        PrintingList = new List<Expression>();
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
            // This prints the if's
            GD.Print(exp);
        }
    }

    public override string ToString()
    {
        GD.Print("Presente en el ToString de ColdWarProgram");
        string s = "";
        foreach (PoliticalCurrent PoliticalCurrent in political_currents.Values)
        {
            s = s + "\n" + PoliticalCurrent.ToString();
        }
        GD.Print(Cards.Count);
        foreach (Card card in Cards.Values)
        {
            GD.Print("Inside cards");
            GD.Print(card);
            s += "\n" + card.ToString();
        }
        GD.Print(s);
        return s;
    }
}