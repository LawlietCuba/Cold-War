namespace Deck_Library;
using CardLibrary;
public class Deck
{
    public List<Card> Cards { get; private set; } = new List<Card>();
    public string? politic { get; private set; }

    public Deck(List<Card> Cards)
    {
        this.Cards = Cards;
        this.politic = Cards[0].ClassCard;
    }

    public Deck(string path)
    {
        var DeckPath = Directory.GetDirectories(path);

        List<Card> Cards = new List<Card>(DeckPath.Length);
        for (int i = 0; i < DeckPath.Length; i++)
        {
            Cards[i] = new Card(DeckPath[i]);
        }

        this.Cards = Cards;
        this.politic = Cards[0].ClassCard;
    }

    public Deck(string[] CardsPaths)
    {

        List<Card> Cards = new List<Card>(CardsPaths.Length);
        for (int i = 0; i < CardsPaths.Count(); i++)
        {
            Cards[i] = new Card(CardsPaths[i]);
        }

        this.Cards = Cards;
        this.politic = Cards[0].ClassCard;
    }

    public void AddCardsToDeck(string[] CardsPaths)
    {
        for (int i = 0; i < CardsPaths.Count(); i++)
        {
            this.Cards.Add(new Card(CardsPaths[i]));
        }
    }
}



