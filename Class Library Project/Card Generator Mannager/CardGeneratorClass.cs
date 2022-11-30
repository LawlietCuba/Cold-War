namespace CardManagement;

using System.Text.Json;
using Selector;
using CardLibrary;

public class GameMaster
{
    public string path { get; set; }

    public GameMaster(string path)
    {
        this.path = path;
    }

    public virtual void PrintMenu()
    {
        Console.Clear();
        System.Console.WriteLine("Welcome to the Game Master Menu. What would you like to do?");
        System.Console.WriteLine("[D] Go To Decks");
        System.Console.WriteLine("[E] Exit");
    }
}

public class DeckManager : GameMaster
{
    public string[] directories;
    public SelectionReferee DeckList { get; private set; }

    public DeckManager(string path) : base(path)
    {
        this.directories = Directory.GetDirectories(this.path);
        this.DeckList = new SelectionReferee(directories);

        for (int i = 0; i < DeckList.ListOfSelection.Count(); i++)
        {
            DeckList.ListOfSelection[i] = Current(DeckList.ListOfSelection[i]);
        }
    }

    public override void PrintMenu()
    {
        Console.Clear();
        System.Console.WriteLine("Deck Manangment Menu");
        System.Console.WriteLine("[A] Manage cards in a deck");
        System.Console.WriteLine("[N] New deck");
        System.Console.WriteLine("[D] Delete deck");
        System.Console.WriteLine("[F] Finish");
    }

    private string Current(string direction)
    {
        int last = direction.Count() - 1;   // index of the last slash
        while (last >= 0 && direction[last] != '/')
        {
            last--;
        }

        return direction.Substring(last + 1, direction.Count() - last - 1);
    }

    public void AddDeck()
    {
        System.Console.Clear();
        System.Console.WriteLine("Name of the new deck");
        string DeckName = Console.ReadLine();
        string NewDeckPath = this.path + "/" + DeckName;
        if (NewDeckPath != null)
        {
            if (!Directory.Exists(NewDeckPath))
            {
                Directory.CreateDirectory(NewDeckPath);
            }
        }
    }

    public void DeleteDeck()
    {
        System.Console.Clear();
        System.Console.WriteLine("Name of the deck to delete");
        DeckList.PrintSelectionList();

        int DeckIndex = int.Parse(Console.ReadLine());

        if (DeckIndex < 1 && DeckIndex > directories.Length)
        {
            System.Console.WriteLine("Wrong number. That deck doesn't exist. Try again");
            var WaitForIt = Console.ReadKey();
            return;
        }
        else
        {
            int index = DeckIndex - 1;
            var DeckFiles = Directory.GetFiles(directories[index]);
            int count = DeckFiles.Count();

            if (count == 0)
            {
                Directory.Delete(directories[index]);
            }
            else
            {
                System.Console.Clear();
                System.Console.WriteLine("Do you wish to delete all the cards in " + Current(directories[index]) + "?");
                System.Console.WriteLine("[Y] Yes");
                System.Console.WriteLine("[N] No");
                var UserInput = Console.ReadKey();

                switch (UserInput.Key)
                {
                    case ConsoleKey.Y:
                        DeleteAllInTheDeck(DeckFiles);
                        Directory.Delete(directories[index]);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void DeleteAllInTheDeck(string[] DeckFiles)
    {
        foreach (var DeleteFile in DeckFiles)
        {
            File.Delete(DeleteFile);
        }
    }
}

public class CardManager
{
    public string path { get; private set; }
    private string ClassCard;

    public string[]? TheDeck { get; private set; }

    public List<Card>? TheCards { get; private set; }

    public CardManager(string path)
    {
        System.Console.WriteLine(path);
        this.path = path;
        this.TheDeck = Directory.GetFiles(path);

        this.TheCards = new List<Card>();

        for (int i = 0; i < TheDeck.Length; i++)
        {
            TheCards.Add(new Card(TheDeck[i]));
        }

        this.ClassCard = Current(path);
    }

    public void PrintMenu()
    {
        System.Console.Clear();
        System.Console.WriteLine($"What would you like to do with the deck {Current(path)}?");
        System.Console.WriteLine("[C] Create Cards");
        System.Console.WriteLine("[S] See the cards");
        System.Console.WriteLine("[D] Delete a Card");
        System.Console.WriteLine("[A] Delete All Cards");
    }

    public void GenerateCard()
    {
        System.Console.Clear();

        //--------------------------------------------------------
        //--------------------------------------------------------
        // Change the CardGenerator

        System.Console.WriteLine("Type of the card");
        string[] s = new string[] { "Unit", "Event", "Politic" };
        var Ref = new SelectionReferee(s);
        string? type = s[Ref.SelectOne()];
        System.Console.Clear();

        System.Console.WriteLine("Rareness of the card");
        s = new string[] { "Legendary", "Rare", "Common" };
        Ref = new SelectionReferee(s);
        string? rareness = s[Ref.SelectOne()];
        System.Console.Clear();

        System.Console.WriteLine("Name of the card");
        string? name = Console.ReadLine();
        System.Console.Clear();

        System.Console.WriteLine("A brief description");
        string? lore = Console.ReadLine();
        Console.Clear();

        System.Console.WriteLine("Life");
        System.Console.WriteLine("An integer between " + 0 + " and " + int.MaxValue);
        int life = int.Parse(Console.ReadLine());
        Console.Clear();

        System.Console.WriteLine("Attack");
        System.Console.WriteLine("An integer between " + 0 + " and " + int.MaxValue);
        int attack = int.Parse(Console.ReadLine());
        Console.Clear();

        System.Console.WriteLine("Effect");
        string? something = Console.ReadLine();
        object? effect = null;

        var NewCard = new Card(type, rareness, name, lore, life, attack, this.ClassCard, effect);

        TheCards.Add(NewCard);
        AddToTheDeck(NewCard);
        System.Console.WriteLine("Time to add to the deck");
    }

    // private void AddToTheDeck(Card NewCard)
    // {
    //     var ConvertToJson = JsonSerializer.Serialize(NewCard);
    //     System.Console.WriteLine("Adding: " + ConvertToJson);

    //     string FileName = CreateJsonDirection(this.path, NewCard.Name);

    //     if (File.Exists(FileName) == false)
    //     {
    //         File.WriteAllText(FileName, ConvertToJson);
    //     }
    //     else
    //     {
    //         File.Delete(FileName);
    //         File.WriteAllText(FileName, ConvertToJson);
    //     }
    // }

     private void AddToTheDeck(Card NewCard)
    {
        var ConvertToJson = JsonSerializer.Serialize(NewCard);
        System.Console.WriteLine("Adding: " + ConvertToJson);

        string FileName = CreateJsonDirection(this.path, NewCard.Name);

        if (File.Exists(FileName) == false)
        {
            File.WriteAllText(FileName, ConvertToJson);
        }
        else
        {
            File.Delete(FileName);
            File.WriteAllText(FileName, ConvertToJson);
        }
    }

    string CreateJsonDirection(string path, string name)
    {
        return path + "/" + name + ".json";
    }

    private string Current(string direction)
    {
        int last = direction.Count() - 1;   // index of the last slash
        while (last >= 0 && direction[last] != '/')
        {
            last--;
        }

        return direction.Substring(last + 1, direction.Count() - last - 1);
    }

    public void PrintCards()
    {
        System.Console.Clear();
        for (int i = 0; i < TheCards.Count(); i++)
        {
            System.Console.WriteLine(this.path);
            System.Console.WriteLine("Number: " + i);
            System.Console.WriteLine("Type : " + TheCards[i].Type);
            System.Console.WriteLine("Rareness: " + TheCards[i].Rareness);
            System.Console.WriteLine("Name : " + TheCards[i].Name);
            System.Console.WriteLine("Lore : " + TheCards[i].Lore);
            System.Console.WriteLine("Class Card: " + TheCards[i].ClassCard);
            System.Console.WriteLine("Life : " + TheCards[i].Life);
            System.Console.WriteLine("Attack : " + TheCards[i].Attack);
            System.Console.WriteLine("Effect : " + TheCards[i].Effect);
        }

        if (TheCards.Count() == 0) System.Console.WriteLine("There isn't any card in this deck");
    }

    public void DeleteACard()
    {
        PrintCards();
        System.Console.WriteLine("What is the number of card to delete?");
        int number = int.Parse(Console.ReadLine());
        string JsonToDelete = CreateJsonDirection(this.path, TheCards[number].Name);

        File.Delete(JsonToDelete);
    }

    public void DeleteAllCards()
    {
        var AllCards = Directory.GetFiles(this.path);

        foreach (string JsonToDelete in AllCards)
        {
            File.Delete(JsonToDelete);
        }
    }
}