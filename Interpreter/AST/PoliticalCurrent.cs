using System;
using System.Collections.Generic;
using System.IO;

public class PoliticalCurrent : ASTNode
{
    public string Id{get;set;}

    public PoliticalCurrent(string Id, CodeLocation location) : base(location) {
        this.Id = Id;
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {
        return true;
    }

    public bool CollectPoliticalCurrents(Context context, Scope scope, List<CompilingError> errors)
    {
        if (context.political_currents.Contains(Id))
        {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Element already defined"));
            return false;
        }
        else
        {
            context.political_currents.Add(Id);
        }
        return true;
    }
    
    public override string ToString()
    {
        return String.Format("PoliticalCurrent {0}", Id);
    }

    public void CreateDeck(){
        // string path = Path.Join("Decks", Id);
        string path = "Decks" + "/" + Id;
        if(!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}