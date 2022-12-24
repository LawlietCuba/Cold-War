using System.Collections.Generic;
using System.Linq;
using System.IO;
using Godot;
using System;
public class Menu : Node2D
{
    // Declare member variables here. Examples:
    public bool SelectedDeck { get; set; }
    public PlayerTemplate HumanPlayer { get; set; }
    public string PathToSelectedDeck { get; set; }
    public CardTemplate[] Cards { get; set; }
    public List<CardSupport> FinalDeck { get; set; }
    public int CurrentCardIndex { get; set; }
    public List<CardSupport> Deck { get; set; }
    public static bool somecardselected { get; set; }
    public PackedScene Temp;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (SelectedDeck)
        {
            Deck = new List<CardSupport>();
            var CardTexture = new ImageTexture();
            CardTexture.Load(@"res://Textures//Card.jpg");
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
        PathToSelectedDeck = "/home/daniel/Documents/Programacion/Proyecto Battle Cards/Subido a GitHub/Cold-War-Develop/Decks/Communist";
        SelectedDeck = true;
    }
    public void _on_Capitalist_pressed()
    {
        PathToSelectedDeck = "/home/daniel/Documents/Programacion/Proyecto Battle Cards/Subido a GitHub/Cold-War-Develop/Decks/Capitalist";
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
        Array.Clear(Cards, 0, Cards.Length);
        FinalDeck.Clear();
        CurrentCardIndex = 0;
        somecardselected=false;
        GetNode<Node2D>("SelectCards").Hide();
        GetNode<Node2D>("SelectDeck").Show();
    }
    public void _on_Ready_pressed()
    {
        if (somecardselected)
        {
            HumanPlayer = new PlayerTemplate(FinalDeck[0].ClassCard, new Board(FinalDeck));
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
        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Name").Text = Card.Name;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Lore").Text = Card.Lore;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/ClassCard").Text = Card.ClassCard;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Attack").Text = $"{Card.Attack}";

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Life").Text = $"{Card.Life}";

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Effect").Text = Card.Effect;

        var typetexture = new ImageTexture();
        switch (Card.Type)
        {
            case "Unit":
                typetexture.Load(@"res://Textures//Unit.png");
                break;
            case "Event":
                typetexture.Load(@"res://Textures//Event.png");
                break;
            case "Politic":
                typetexture.Load(@"res://Textures//Politic.png");
                break;
        }

        Card.GetNode<Sprite>("CardMargin/BackgroundCard/TypeMargin/TypePhoto").Texture = typetexture;
        Card.GetNode<Sprite>("CardMargin/BackgroundCard/TypeMargin/TypePhoto").Scale = Card.GetNode<MarginContainer>("CardMargin/BackgroundCard/TypeMargin").RectSize / typetexture.GetSize();
        var rarenesstexture = new ImageTexture();
        switch (Card.Rareness)
        {
            case "Legendary":
                rarenesstexture.Load(@"res://Textures//GoldenShield.png");
                break;
            case "Rare":
                rarenesstexture.Load(@"res://Textures//SilverShield.png");
                break;
            case "Common":
                rarenesstexture.Load(@"res://Textures//BronzeShield.png");
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
            phototexture.Load(@"res://Textures//foto-perfil-generica.jpg");
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
        Cards = new CardTemplate[CardstoCreate.Length];
        for (int i = 0; i < CardstoCreate.Length; i++)
        {
            Cards[i] = new CardTemplate(CardstoCreate[i].FullName);

            Temp = (PackedScene)GD.Load("res://CardSupport.tscn");
            Deck.Add((CardSupport)Temp.Instance());
            Deck[i].GetNode<Sprite>("CardMargin/BackgroundCard").Texture = CardTexture;
            Deck[i].GetNode<MarginContainer>("CardMargin").RectSize = GetNode<MarginContainer>("SelectCards/CardforSelection").RectSize;

            Deck[i].Name = Cards[i].Name;
            Deck[i].Lore = Cards[i].Lore;
            Deck[i].ClassCard = Cards[i].ClassCard;
            Deck[i].Attack = Cards[i].Attack;
            Deck[i].Life = Cards[i].Life;
            Deck[i].Effect = Cards[i].Effect;
            Deck[i].Type = Cards[i].Type;
            Deck[i].Rareness = Cards[i].Rareness;
            Deck[i].PathToPhoto = Cards[i].PathToPhoto;

            MakeCard(Deck[i]);

            Deck[i].GetNode<MarginContainer>("CardMargin").RectPosition = new Vector2(GetNode<Position2D>("SelectCards/Position2D").Position.x, GetNode<Position2D>("SelectCards/Position2D").Position.y);

            GetNode<Node2D>("SelectCards").AddChild(Deck[i]);
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
        for (int i = 0; i < Deck.Count; i++)
        {
            if (i != CurrentCardIndex)
                Deck[i].GetNode<Sprite>("CardMargin/BackgroundCard").Hide();
            else
                Deck[i].GetNode<Sprite>("CardMargin/BackgroundCard").Show();
        }
    }
}
