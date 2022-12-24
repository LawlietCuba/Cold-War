using System.Text.Json;
using Godot;
using System;
using System.IO;


public class CardTemplate : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
    public string Name { get; set; } = "No Info";
    public string Lore { get; set; } = "No Info";
    public string ClassCard { get; set; } = "No Info";      // Comunista; capitalista
    public int Attack { get; set; } = int.MinValue;
    public int Life { get; set; } = int.MinValue;
    public string Effect { get; set; }
    public string Type { get; set; } = "No Info";           // Unidad; evento; politica
    public string Rareness { get; set; } = "No Info";       // Legendaria; comun
    public string PathToPhoto { get; set; }

    public CardTemplate(string Type, string Rareness, string Name, string Lore, int Life, int Attack, string ClassCArd, string Effect, string PathToPhoto)
    {
        this.Type = Type;
        this.Rareness = Rareness;
        this.Name = Name;
        this.Lore = Lore;
        this.Life = Life;
        this.Attack = Attack;
        this.ClassCard = ClassCArd;
        this.Effect = Effect;
        this.PathToPhoto = PathToPhoto;
    }
    public CardTemplate(string Type, string Rareness, string Name, string Lore, int Life, int Attack, string ClassCArd, string Effect)
    {
        this.Type = Type;
        this.Rareness = Rareness;
        this.Name = Name;
        this.Lore = Lore;
        this.Life = Life;
        this.Attack = Attack;
        this.ClassCard = ClassCArd;
        this.Effect = Effect;
        this.PathToPhoto = null;
    }

    public CardTemplate(string JsonPath)
    {
        using (StreamReader r = new StreamReader(JsonPath))
        {
            string json = r.ReadToEnd();
            CardTemplate nCard = JsonSerializer.Deserialize<CardTemplate>(json);

            if (!nCard.Equals(null))
            {
                this.Name = nCard.Name;
                this.Lore = nCard.Lore;
                this.ClassCard = nCard.ClassCard;
                this.Attack = nCard.Attack;
                this.Life = nCard.Life;
                this.Effect = nCard.Effect;
                this.Type = nCard.Type;
                this.Rareness = nCard.Rareness;
                this.PathToPhoto = nCard.PathToPhoto;
            }
        }
    }
    public CardTemplate()
    {

    }
}
