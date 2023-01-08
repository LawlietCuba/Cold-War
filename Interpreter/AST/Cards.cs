using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;

public class Card : ASTNode
{
    public string Id{get;set;}  // name
    public Expression cardtype{get;set;}
    public Expression Rareness{get;set;}
    public Expression Lore{get;set;}
    public Expression Health{get;set;}
    public Expression Attack{get;set;}
    public Expression political_current{get;set;}  
    public Expression PathToPhoto{get;set;}
    public Expression EffectText{get;set;}
    public List<Token> Effect{get;set;}

    public Card(string Id, CodeLocation location) : base(location) {
        this.Id = Id;
    }

    public override bool CheckSemantic(Context context, Scope scope, List<CompilingError> errors)
    {

        bool checkCardType = cardtype.CheckSemantic(context, scope, errors);
        if(cardtype.ToString() != Card_Type.Unit.ToString() && cardtype.ToString() != Card_Type.Event.ToString() && cardtype.ToString() != Card_Type.Politic.ToString()) {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "The CardType only accept Unit, Event or Politic"));
        }

        bool checkRareness = Rareness.CheckSemantic(context, scope, errors);
        if(Rareness.ToString() != RarenessType.Legendary.ToString() && Rareness.ToString() != RarenessType.Common.ToString() && Rareness.ToString() != RarenessType.Rare.ToString()) {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "The Rareness only accept Legendary, Rare or Common"));
        }

        bool checkLore = Lore.CheckSemantic(context, scope, errors);
        if(Lore.Type != ExpressionType.Text) {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "The Lore must be a string"));
        }

        bool checkHealth = Health.CheckSemantic(context, scope, errors);
        if(Health.Type != ExpressionType.Number) {
             errors.Add(new CompilingError(Location, ErrorCode.Invalid, "The Life must be numerical"));
        }

        bool checkAttack = Attack.CheckSemantic(context, scope, errors);
        if(Attack.Type != ExpressionType.Number) {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "The Attack must be numerical"));
        }

        bool checkpolitical_currents = political_current.CheckSemantic(context, scope, errors);
        if(!context.political_currents.Contains(political_current.ToString())) {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, String.Format("{0} politcal_current does not exists", political_current)));
            checkpolitical_currents = false;
        }

        bool checkPathToPhoto = PathToPhoto.CheckSemantic(context, scope, errors);
        if(PathToPhoto.Type != ExpressionType.Text) {
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "The Path must be a string"));
        }

        bool checkEffectText = true;
        if(EffectText != null)
        {
            checkEffectText = EffectText.CheckSemantic(context,scope,errors);
            if(EffectText.Type != ExpressionType.Text)
            {
                errors.Add(new CompilingError(Location, ErrorCode.Invalid, "The effect text must be a string"));
            }
        }
        
        return checkCardType && checkRareness && checkLore && checkHealth && checkAttack && checkpolitical_currents && checkPathToPhoto && checkEffectText;
    }

    public void Evaluate()
    {
        Attack.Evaluate();
        Health.Evaluate();
        PathToPhoto.Evaluate();
    }

    public override string ToString()
    {
        return String.Format("Card {0} \n\t Card Type: {1} \n\t Rareness: {2} \n\t Lore: {3} \n\t Life: {4} \n\t Attack: {5} \n\t political_current: {6} \n\t PathToPhoto: {7} \n\t Effect: {8} " , Id, cardtype, Rareness, Lore, Health, Attack, political_current, PathToPhoto, Effect);
    }

    public void AddToTheDeck() {
        string ConvertToJson = JsonSerializer.Serialize(this);

        // string path = Path.Join("Decks", political_current.ToString());
        string path = "Decks" + "/" +political_current.ToString();

        string FileName = CreateJsonDirection(path, Id);

        if(System.IO.File.Exists(FileName) == false) {
            System.IO.File.WriteAllText(FileName, ConvertToJson);
        }
        else {
            System.IO.File.Delete(FileName);
            System.IO.File.WriteAllText(FileName, ConvertToJson);
        }
    }

    public void AddToTheDeckAsCardTemplate() {
        string nName = "";
        for(int i=0; i<Id.Length; i++)
        {
            if(Char.IsUpper(Id[i]) && i!=0) nName+=" ";
            nName += Id[i];
        }
        string ncardtype = this.cardtype.GetValue().ToString();
        string nRareness = this.Rareness.GetValue().ToString();
        string nLore = this.Lore.GetValue().ToString();
        int nHealth = int.Parse(string.Format("{0}", this.Health.GetValue()));
        int nAttack = int.Parse(string.Format("{0}", this.Attack.GetValue()));
        string npolitcal_current = this.political_current.ToString();

        // Busquemos si existe una foto para la carta con el nombre dado
        string nPathToPhoto = null;

        if(PathToPhoto.GetValue() != null)
        {
            List<string> results = new List<string>();
            System.IO.DirectoryInfo rootDir = new DirectoryInfo("Images");
            RecursiveFileSearch.WalkDirectoryTreeString(rootDir, PathToPhoto.ToString(), results);
            if(results.Count > 0)
            {
                Random rand = new Random();
                nPathToPhoto = results[0];
            }
        }

        string nEffectText = this.EffectText.GetValue().ToString();
        
        CardTemplate card_template = new CardTemplate(nName, ncardtype, nRareness, nLore, nHealth, nAttack, npolitcal_current, nPathToPhoto, nEffectText, this.Effect);
    
        card_template.CreateJson();
    }

    string CreateJsonDirection(string path, string name) {
        return path + "/" + name + ".json";
    }

    public static CardTemplate SearchCardTemplate(string target)
    {   
        List<string> results = new List<string>();
        System.IO.DirectoryInfo rootDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        RecursiveFileSearch.WalkDirectoryTreeString(rootDir, target, results);

        if(results.Count != 0)
        {   
            return new CardTemplate(results[0]);
        }

        return null;
    }

    // public void DoEffect()
    // {
    //     if(this.Effect != null)
    //     {
    //         TokenStream effect_stream = new TokenStream(this.Effect);
    //         Parser effect_parser = new Parser(effect_stream);
    //         List<CompilingError> effect_errors = new List<CompilingError>();
    //         ColdWarProgram effect_program = effect_parser.ParseProgram(effect_errors);

    //         if (effect_errors.Count > 0)
    //         {
    //             foreach (CompilingError error in effect_errors)
    //             {
    //                 Console.WriteLine("{0}, {1}, {2}", error.Location.Line, error.Code, error.Argument);
    //             }
    //         }
    //         else
    //         {
    //             Context context = new Context();
    //             Scope scope = new Scope();

    //             effect_program.CheckSemantic(context, scope, effect_errors);

    //             if (effect_errors.Count > 0)
    //             {
    //                 foreach (CompilingError error in effect_errors)
    //                 {
    //                     Console.WriteLine("{0}, {1}, {2}", error.Location.Line, error.Code, error.Argument);
    //                 }
    //             }
    //             else
    //             {
    //                 effect_program.Evaluate();

    //             }
    //         } 
    //     }
    // }

    public Card(CodeLocation location) : base(location)
    {

    }
}

public enum Card_Type
{
    Unit,
    Politic,
    Event
}
public enum RarenessType
{
    Legendary,
    Rare,
    Common
}