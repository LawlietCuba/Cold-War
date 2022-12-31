using System.Collections.Generic;
using System.Linq;
using System.IO;
using Godot;
using System;
public class Menu : Node2D
{
    // Declare member variables here. Examples:
    bool SelectedDeck;
    PlayerTemplate HumanPlayer;
    string PathToSelectedDeck;
    CardTemplate[] LogicCards;
    List<CardSupport> FinalDeck;
    int CurrentCardIndex;
    List<CardSupport> Deck;
    public static bool somecardselected;
    PackedScene NewNode;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Just a test----------------------------------------------------
        // List<string> results = new List<string>();
        // System.IO.DirectoryInfo rootDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        // RecursiveFileSearch.WalkDirectoryTreeString(rootDir, "Hayek", results);

        // if (results.Count > 0)
        //     GD.Print("Exactly what I was looking for");

        ReadCode();

        if (SelectedDeck)
        {
            Deck = new List<CardSupport>();
            var CardTexture = new ImageTexture();
            CardTexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Card.jpg");
            Deck = MakeDeck(CardTexture, PathToSelectedDeck, new List<CardSupport>());
            FinalDeck = new List<CardSupport>();
            ShowCardsForSelection();
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {

    //  }
    public void _on_PlayGame_pressed()
    {
        GetNode<Node2D>("MainMenu").Hide();
        GetNode<Node2D>("SelectDeck").Show();
    }
    public void _on_ReturnToMainMenu_pressed()
    {
        GetNode<Node2D>("SelectDeck").Hide();
        GetNode<Node2D>("MainMenu").Show();
    }
    public void _on_Communist_pressed()
    {
        PathToSelectedDeck = System.IO.Directory.GetCurrentDirectory() + "/Decks/Communist";
        SelectedDeck = true;
    }
    public void _on_Capitalist_pressed()
    {
        PathToSelectedDeck = System.IO.Directory.GetCurrentDirectory() + "/Decks/Capitalist";
        SelectedDeck = true;
    }
    public void _on_SelectCards_pressed()
    {
        if (SelectedDeck)
        {
            GetNode<Node2D>("SelectDeck").Hide();
            GetNode<Node2D>("SelectCards").Show();
            _Ready();
        }
    }
    public void _on_ReturnToDeckSelector_pressed()
    {
        Deck.Clear();
        Array.Clear(LogicCards, 0, LogicCards.Length);
        FinalDeck.Clear();
        CurrentCardIndex = 0;
        somecardselected = false;
        GetNode<Node2D>("SelectCards").Hide();
        GetNode<Node2D>("SelectDeck").Show();
    }
    public void _on_Ready_pressed()
    {
        if (somecardselected)
        {
            HumanPlayer = new PlayerTemplate(FinalDeck[0].political_current, new Board(FinalDeck));
            Game.HumanPlayer = HumanPlayer;
            GetNode<Node2D>("SelectCards").Hide();
            GetNode<Node2D>("/root/Main/Game").Show();
        }
    }
    public void _on_CreateCards_pressed()
    {
        GetNode<Node2D>("MainMenu").Hide();
        GetNode<Node2D>("Compiler").Show();
    }
    public void _on_ReturnToMainMenuFromCompiler_pressed()
    {
        GetNode<Node2D>("Compiler").Hide();
        GetNode<Node2D>("MainMenu").Show();
    }
    public void _on_Exit_pressed()
    {
        GetTree().Quit();
    }
    public void MakeCard(CardSupport Card)
    {
        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Name").Text = Card.CardName;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Lore").Text = Card.Lore;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/ClassCard").Text = Card.political_current;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Attack").Text = $"{Card.Attack}";

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Life").Text = $"{Card.Health}";

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Effect").Text = "An amazing effect";

        var typetexture = new ImageTexture();
        switch (Card.cardtype)
        {
            case "Unit":
                typetexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Unit.png");
                break;
            case "Event":
                typetexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Event.png");
                break;
            case "Politic":
                typetexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Politic.png");
                break;
        }

        Card.GetNode<Sprite>("CardMargin/BackgroundCard/TypeMargin/TypePhoto").Texture = typetexture;
        Card.GetNode<Sprite>("CardMargin/BackgroundCard/TypeMargin/TypePhoto").Scale = Card.GetNode<MarginContainer>("CardMargin/BackgroundCard/TypeMargin").RectSize / typetexture.GetSize();
        var rarenesstexture = new ImageTexture();
        switch (Card.Rareness)
        {
            case "Legendary":
                rarenesstexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/GoldenShield.png");
                break;
            case "Rare":
                rarenesstexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/SilverShield.png");
                break;
            case "Common":
                rarenesstexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/BronzeShield.png");
                break;
        }
        Card.GetNode<Sprite>("CardMargin/BackgroundCard/RarenessMargin/RarenessPhoto").Texture = rarenesstexture;
        Card.GetNode<Sprite>("CardMargin/BackgroundCard/RarenessMargin/RarenessPhoto").Scale = Card.GetNode<MarginContainer>("CardMargin/BackgroundCard/RarenessMargin").RectSize / rarenesstexture.GetSize();

        var phototexture = new ImageTexture();
        if (Card.PathToPhoto != null)
        {

            phototexture.Load(Card.PathToPhoto);
            Card.GetNode<Sprite>("CardMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture = phototexture;
            Card.GetNode<Sprite>("CardMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Scale = Card.GetNode<MarginContainer>("CardMargin/BackgroundCard/PhotoCardMargin").RectSize / phototexture.GetSize();
        }
        else
        {
            phototexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/foto-perfil-generica.jpg");
            Card.GetNode<Sprite>("CardMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture = phototexture;
            Card.GetNode<Sprite>("CardMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Scale = Card.GetNode<MarginContainer>("CardMargin/BackgroundCard/PhotoCardMargin").RectSize / phototexture.GetSize();
        }
    }
    public List<CardSupport> MakeDeck(ImageTexture CardTexture, string PathToDeck, List<CardSupport> Deck)
    {
        DirectoryInfo di = new DirectoryInfo(PathToDeck);
        FileInfo[] CardstoCreate = di.GetFiles();
        if (CardstoCreate.Length == 0)
        {
            throw new ArgumentException();
        }
        LogicCards = new CardTemplate[CardstoCreate.Length];
        for (int i = 0; i < CardstoCreate.Length; i++)
        {
            LogicCards[i] = new CardTemplate(CardstoCreate[i].FullName);

            NewNode = (PackedScene)GD.Load("res://CardSupport.tscn");
            Deck.Add((CardSupport)NewNode.Instance());
            Deck[i].GetNode<Sprite>("CardMargin/BackgroundCard").Texture = CardTexture;
            Deck[i].GetNode<MarginContainer>("CardMargin").RectSize = GetNode<MarginContainer>("/root/Main/Game/Board/CardOnBoardMargin").RectSize;

            Deck[i].CardName = LogicCards[i].CardName;
            Deck[i].Lore = LogicCards[i].Lore;
            Deck[i].political_current = LogicCards[i].political_current;
            Deck[i].Attack = LogicCards[i].Attack;
            Deck[i].Health = LogicCards[i].Health;
            Deck[i].Effect = LogicCards[i].Effect;
            Deck[i].cardtype = LogicCards[i].cardtype;
            Deck[i].Rareness = LogicCards[i].Rareness;
            Deck[i].PathToPhoto = LogicCards[i].PathToPhoto;

            MakeCard(Deck[i]);
        }
        return Deck;
    }
    public void _on_Previous_pressed()
    {
        if (CurrentCardIndex > 0)
        {
            CurrentCardIndex--;
            ShowCardsForSelection();
        }
    }
    public void _on_Next_pressed()
    {
        if (CurrentCardIndex < Deck.Count - 1)
        {
            CurrentCardIndex++;
            ShowCardsForSelection();
        }
    }
    public void _on_SelectThisCard_pressed()
    {
        somecardselected = true;
        if (!FinalDeck.Contains(Deck[CurrentCardIndex]))
            FinalDeck.Add(Deck[CurrentCardIndex]);
    }
    public void ShowCardsForSelection()
    {
        var CardTexture = new ImageTexture();
        CardTexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Card.jpg");
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard").Position = new Vector2(GetNode<MarginContainer>("SelectCards/CardforSelection").RectSize.x / 2, GetNode<MarginContainer>("SelectCards/CardforSelection").RectSize.y / 2);
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard").Texture = CardTexture;
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard").Scale = GetNode<MarginContainer>("SelectCards/CardforSelection").RectSize / CardTexture.GetSize();


        GetNode<RichTextLabel>("SelectCards/CardforSelection/BackgroundCard/Name").Text = Deck[CurrentCardIndex].CardName;
        GetNode<RichTextLabel>("SelectCards/CardforSelection/BackgroundCard/Effect").Text = "An amazing effect";
        GetNode<RichTextLabel>("SelectCards/CardforSelection/BackgroundCard/Lore").Text = Deck[CurrentCardIndex].Lore;
        GetNode<RichTextLabel>("SelectCards/CardforSelection/BackgroundCard/Attack").Text = Deck[CurrentCardIndex].Attack.ToString();
        GetNode<RichTextLabel>("SelectCards/CardforSelection/BackgroundCard/Life").Text = Deck[CurrentCardIndex].Health.ToString();
        GetNode<RichTextLabel>("SelectCards/CardforSelection/BackgroundCard/ClassCard").Text = Deck[CurrentCardIndex].political_current;
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard/TypeMargin/TypePhoto").Texture = Deck[CurrentCardIndex].GetNode<Sprite>("CardMargin/BackgroundCard/TypeMargin/TypePhoto").Texture;
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard/TypeMargin/TypePhoto").Scale = GetNode<MarginContainer>("SelectCards/CardforSelection/BackgroundCard/TypeMargin").RectSize / Deck[CurrentCardIndex].GetNode<Sprite>("CardMargin/BackgroundCard/TypeMargin/TypePhoto").Texture.GetSize();
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard/RarenessMargin/RarenessPhoto").Texture = Deck[CurrentCardIndex].GetNode<Sprite>("CardMargin/BackgroundCard/RarenessMargin/RarenessPhoto").Texture;
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard/RarenessMargin/RarenessPhoto").Scale = GetNode<MarginContainer>("SelectCards/CardforSelection/BackgroundCard/RarenessMargin").RectSize / Deck[CurrentCardIndex].GetNode<Sprite>("CardMargin/BackgroundCard/RarenessMargin/RarenessPhoto").Texture.GetSize();
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard/PhotoCardMargin/PhotoCard").Texture = Deck[CurrentCardIndex].GetNode<Sprite>("CardMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture;
        GetNode<Sprite>("SelectCards/CardforSelection/BackgroundCard/PhotoCardMargin/PhotoCard").Scale = GetNode<MarginContainer>("SelectCards/CardforSelection/BackgroundCard/PhotoCardMargin").RectSize / Deck[CurrentCardIndex].GetNode<Sprite>("CardMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture.GetSize();
    }

    public void ReadCode()
    {
        GD.Print("Comienza el programa");

        LexicalAnalyzer lex = Compiling.Lexical;

        string text = System.IO.File.ReadAllText(@"code.txt");

        IEnumerable<Token> tokens = lex.GetTokens("code", text, new List<CompilingError>());

        // foreach (Token token in tokens)
        // {
        //     GD.Print(token);
        // }

        TokenStream stream = new TokenStream(tokens);
        Parser parser = new Parser(stream);
        List<CompilingError> errors = new List<CompilingError>();

        ColdWarProgram program = parser.ParseProgram(errors);

        // GD.Print(program);

        if (errors.Count > 0)
        {
            foreach (CompilingError error in errors)
            {
                GD.Print("{0}, {1}, {2}", error.Location.Line, error.Code, error.Argument);
            }
        }
        else
        {
            Context context = new Context();
            Scope scope = new Scope();

            program.CheckSemantic(context, scope, errors);

            if (errors.Count > 0)
            {
                foreach (CompilingError error in errors)
                {
                    GD.Print("{0}, {1}, {2}", error.Location.Line, error.Code, error.Argument);
                }
            }
            else
            {
                program.Evaluate();

                // GD.Print(program);

                foreach(Card ncard in program.Cards.Values)
                {
                    ncard.AddToTheDeckAsCardTemplate();
                }
            }
        }
    }
}
