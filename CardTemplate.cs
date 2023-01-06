using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using System;
using System.IO;
using System.Collections.Generic;

// [JsonConverter(typeof(CardTemplateConverter))]
public class CardTemplate
{
    public string CardName { get; set; } 
    public string cardtype { get; set; }           // Unidad; evento; politica
    public string Rareness { get; set; }        // Legendaria; comun
    public string Lore { get; set; } 
    public int Health { get; set; }
    public int Attack { get; set; }
    public string political_current { get; set; }     // Comunista; capitalista
    public string PathToPhoto { get; set; }
    public List<Token> Effect { get; set; }

    public CardTemplate(string CardName, string cardtype, string Rareness, string Lore, int Health, int Attack, string political_current, string PathToPhoto, List<Token> Effect)
    {
        this.CardName = CardName;
        this.cardtype = cardtype;
        this.Rareness = Rareness;
        this.Lore = Lore;
        this.Health = Health;
        this.Attack = Attack;
        this.political_current = political_current;
        this.Effect = Effect;
        this.PathToPhoto = PathToPhoto;
    }
    public CardTemplate(string cardtype, string Rareness, string CardName, string Lore, int Health, int Attack, string political_current, List<Token> Effect)
    {
        this.cardtype = cardtype;
        this.Rareness = Rareness;
        this.CardName = CardName;
        this.Lore = Lore;
        this.Health = Health;
        this.Attack = Attack;
        this.political_current = political_current;
        this.PathToPhoto = null;
        this.Effect = Effect;
    }

    public CardTemplate(string JsonPath)
    {
        using (StreamReader r = new StreamReader(JsonPath))
        {
            string json = r.ReadToEnd();
            CardTemplate nCard = JsonSerializer.Deserialize<CardTemplate>(json);

            if (!nCard.Equals(null))
            {
                this.CardName = nCard.CardName;
                this.cardtype = nCard.cardtype;
                this.Lore = nCard.Lore;
                this.Rareness = nCard.Rareness;
                this.Health = nCard.Health;
                this.Attack = nCard.Attack;
                this.Effect = nCard.Effect;
                this.political_current = nCard.political_current;
                this.PathToPhoto = nCard.PathToPhoto;
            }
        }
    }

    public void CreateJson()
    {
        
        //---------------------------------------------
        string ConvertToJson = JsonSerializer.Serialize(this);

        // string path = Path.Join("Decks", political_current.ToString());
        string path = "Decks" + "/" + political_current.ToString();

        string FileName = CreateJsonDirection(path, this.CardName);
        GD.Print(FileName);

        if(System.IO.File.Exists(FileName) == false) {
            System.IO.File.WriteAllText(FileName, ConvertToJson);
        }
        else {
            System.IO.File.Delete(FileName);
            System.IO.File.WriteAllText(FileName, ConvertToJson);
        }
    }
    private string CreateJsonDirection(string path, string CardName) {
        // return Path.Join(path, CardName + ".json");
        return path + "/" + CardName + ".json";
    }

    public override string ToString()
    {
        return String.Format("CardName: {0} \n\t CardType: {1} \n\t Rareness: {2} \n\t Lore: {3} \n\t Health: {4}, Attack: {5} \n\t PolitcalCurrent: {6} \n\t PathToPhoto: {7} \n\t Effect: {8}", CardName, cardtype, Rareness, Lore, Health, Attack, political_current, PathToPhoto, Effect);
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

    //                 Console.WriteLine(effect_program);
    //             }
    //         } 
    //     }
    // }

    //parche
    public CardTemplate()
    {

    }
}