namespace Player_Library;
using Deck_Library;
using Selector;
using CardLibrary;
public class Player
{
    public string name { get; private set; }
    public Deck PlayerDeck { get; private set; }
    public Player(string name, Deck PlayerDeck)
    {
        this.name = name;
        string pathDeck = PickDeck();
        this.PlayerDeck = PickCards(pathDeck);
    }

    public string PickDeck()
    {
        Console.WriteLine("Pick a deck:");
        string path = Path.Join("..", "..", "Decks");
        var DeckListToSelect = Directory.GetDirectories(path);

        var DeckSelector = new SelectionReferee(DeckListToSelect, true);
        DeckSelector.PrintSelectionList();
        int selected = DeckSelector.SelectOne();
        return DeckListToSelect[selected];
    }

    public Deck PickCards(string pathDeck)
    {
        Console.WriteLine($"You have chosen the {Path.GetFileName(pathDeck)} deck.");
        Console.WriteLine("Pick your number of cards. It must be between 30 and 40 cards.");

        // The player choose the DeckSize
        int DeckSize = 0;

        while (true)
        {
            DeckSize = int.Parse(Console.ReadLine());
            if (DeckSize < 30 || DeckSize > 40)
                Console.WriteLine("The number must be between 30 and 40. Choose again");
            else break;
        }
        System.Console.Clear();

        int AmountOfLegendaryCards = DeckSize * 20 / 100;   // 20 per cent of the cards will be legendary
        int AmountofRareCards = DeckSize * 30 / 100;        // 30 per cent of the cards will be rare
        int AmountOfCommonCards = DeckSize - AmountOfLegendaryCards - AmountofRareCards;  // the rest will be common

        // Se eligen las cartas atendiendo al criterio

        Deck FinalPlayerDeck = new Deck(ChoseCardsByRareness(pathDeck, "Legendary", AmountOfLegendaryCards));
        FinalPlayerDeck.AddCardsToDeck(ChoseCardsByRareness(pathDeck, "Rare", AmountofRareCards));
        FinalPlayerDeck.AddCardsToDeck(ChoseCardsByRareness(pathDeck, "Common", AmountOfCommonCards));

        return FinalPlayerDeck;
    }

    private string[] MakeTheListToPassTheSelectorFromAPath(string path)
    {
        var ListPaths = Directory.GetDirectories(path);

        string[] ListToPass = new string[ListPaths.Length];
        for (int i = 0; i < ListToPass.Length; i++)
        {
            ListToPass[i] = Path.GetFileName(ListPaths[i]);
        }

        return ListToPass;
    }

    private string[] MakeTheListOfCardsWithRarenessCriteria(string path, string criteria)
    {
        var CardsList = Directory.GetDirectories(path);
        var CardsWithSelectedCriteria = new string[CardsList.Length];

        for (int i = 0; i < CardsList.Length; i++)
        {
            if (PlayerDeck.Cards[i].Rareness == criteria)
            {
                CardsWithSelectedCriteria[i] = Path.GetFileName(CardsList[i]);
            }
        }

        return CardsWithSelectedCriteria;
    }

    private string[] ChoseCardsByRareness(string pathDeck, string criteria, int AmountOfCards)
    {
        var ListOfCards = MakeTheListOfCardsWithRarenessCriteria(pathDeck, criteria);

        var SelectorCards = new SelectionReferee(ListOfCards, true);

        Console.WriteLine($"Pick your {0} cards. You can pick {1} cards.", criteria, AmountOfCards);
        SelectorCards.PrintSelectionList();

        var SelectedCards = new string[AmountOfCards];

        for (int i = 0; i < AmountOfCards; i++)
        {
            SelectedCards[i] = ListOfCards[SelectorCards.SelectOne()];
            Console.Clear();
        }

        return SelectedCards;
    }
}

