namespace CardLibrary;
using System.Text.Json;

public class Card
{
    public string Type = "No Info";           // Unidad; evento; politica
    public string Rareness = "No Info";       // Legendaria; comun
    public string Name = "No Info";
    public string Lore = "No Info";
    public int Life = int.MinValue;
    public int Attack = int.MinValue;
    public string ClassCard = "No Info";      // Comunista; capitalista
    public object? Effect;

    public Card(string Type, string Rareness, string Name, string Lore, int Life, int Attack, string ClassCArd, object Effect)
    {
        this.Type = Type;
        this.Rareness = Rareness;
        this.Name = Name;
        this.Lore = Lore;
        this.Life = Life;
        this.Attack = Attack;
        this.ClassCard = ClassCArd;
        this.Effect = Effect;
    }

    public Card(string JsonPath)
    {
        using (StreamReader r = new StreamReader(JsonPath))
        {
            string json = r.ReadToEnd();
            System.Console.WriteLine("Leyendo un json: " + json);
            var nCard = JsonSerializer.Deserialize<Card>(json);

            if (!nCard.Equals(null))
            {
                this.Type = nCard.Type;
                this.Rareness = nCard.Rareness;
                this.Name = nCard.Name;
                this.Lore = nCard.Lore;
                this.Life = nCard.Life;
                this.Attack = nCard.Attack;
                this.ClassCard = nCard.ClassCard;
                this.Effect = nCard.Effect;
            }
        }
    }
}