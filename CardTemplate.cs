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
    public string EffectText{get;set;}
    public List<Token> Effect { get; set; }

    public CardTemplate(string CardName, string cardtype, string Rareness, string Lore, int Health, int Attack, string political_current, string PathToPhoto, string EffectText, List<Token> Effect)
    {
        this.CardName = CardName;
        this.cardtype = cardtype;
        this.Rareness = Rareness;
        this.Lore = Lore;
        this.Health = Health;
        this.Attack = Attack;
        this.political_current = political_current;
        this.PathToPhoto = PathToPhoto;
        this.EffectText = EffectText;
        this.Effect = Effect;
    }
    public CardTemplate(string cardtype, string Rareness, string CardName, string Lore, int Health, int Attack, string political_current, string EffectText, List<Token> Effect)
    {
        this.cardtype = cardtype;
        this.Rareness = Rareness;
        this.CardName = CardName;
        this.Lore = Lore;
        this.Health = Health;
        this.Attack = Attack;
        this.political_current = political_current;
        this.PathToPhoto = null;
        this.EffectText = EffectText;
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
                this.EffectText = nCard.EffectText;
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

    
    //parche
    public CardTemplate()
    {

    }
}