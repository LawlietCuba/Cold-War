using Godot;
using System;
using System.IO;
using System.Collections.Generic;

public class Game : Node2D
{
    public PackedScene x;
    public List<PlayerTemplate> Players { get; set; }
    public bool deckpressed;
    public bool notInstanced;
    public bool readyforsummon;
    public bool startgame;
    public CardTemplate[] Cards;
    public string PathToDeckPlayer1 { get; set; }
    public string PathToDeckPlayer2 { get; set; }
    public int CardOnHandIndex { get; set; }
    public bool communistside;
    public static PlayerTemplate HumanPlayer;
    public static string attackedcard;
    public static bool readyforattack;
    public static string selectedcard;
    public static bool readyforexecute;
    public static bool SelectedCard;
    public static string readytoSummon;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var CardTexture = new ImageTexture();
        CardTexture.Load(@"res://Textures//Card.jpg");
        if (!deckpressed)
        {
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard").Position = new Vector2(GetNode<MarginContainer>("Board/ShowMargin").RectSize.x / 2, GetNode<MarginContainer>("Board/ShowMargin").RectSize.y / 2);
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard").Texture = CardTexture;
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard").Scale = GetNode<MarginContainer>("Board/ShowMargin").RectSize / CardTexture.GetSize();

            var PhotoCardTexture = new ImageTexture();
            PhotoCardTexture.Load(@"res://Textures//foto-perfil-generica.jpg");
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture = PhotoCardTexture;
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Scale = GetNode<MarginContainer>("Board/ShowMargin/BackgroundCard/PhotoCardMargin").RectSize / PhotoCardTexture.GetSize();
        }
        if (startgame)
        {
            startgame = false;
            if (!notInstanced)
            {
                string[] Paths = new string[2];
                Paths[0] = PathToDeckPlayer1;
                Paths[1] = PathToDeckPlayer2;
                Position2D[] Left = new Position2D[2];
                Left[0] = GetNode<Position2D>("Board/Position2D17");
                Left[1] = GetNode<Position2D>("Board/Position2D19");
                Position2D[] Right = new Position2D[2];
                Right[0] = GetNode<Position2D>("Board/Position2D18");
                Right[1] = GetNode<Position2D>("Board/Position2D20");
                Players = new List<PlayerTemplate>();
                for (int i = 0; i < 2; i++)
                {
                    List<CardSupport> Deck = new List<CardSupport>();
                    Deck = MakeDeck(CardTexture, Paths[i], new List<CardSupport>());
                    Players.Add(new PlayerTemplate(Deck[0].ClassCard, new Board(Deck)));
                    // if (deckpressed)
                    // {
                    int amount;
                    if (Deck.Count >= 8) amount = 8;
                    else amount = Deck.Count;
                    int a = Players[i].PlayerBoard.Deck.Count;
                    PrintCardsinRange(Players[i], Left[i], Right[i], amount);
                    deckpressed = false;
                    //}
                }
            }
            notInstanced = true;
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (readyforexecute)
        {
            Attack();
            readyforexecute = false;
        }
    }
    public void PrintCardsinRange(PlayerTemplate Player, Position2D Left, Position2D Right, int amount)
    {
        double length = Right.Position.x - Left.Position.x;
        double CradWidth = length / amount;
        for (int i = amount - 1; i >= 0; i--)
        {
            Player.PlayerBoard.HandCards.Add(Player.PlayerBoard.Deck[i]);
            Player.PlayerBoard.Deck[i].GetNode<MarginContainer>("CardMargin").RectPosition = new Vector2((float)(Left.Position.x + i * CradWidth), Left.Position.y);
            GetNode<Sprite>("Board").AddChild(Player.PlayerBoard.Deck[i]);
            GetNode<Node2D>("Board/CardSupport").Name = Player.PlayerBoard.Deck[i].Name;
            Player.PlayerBoard.Deck.RemoveAt(i);
        }
    }
    public void _on_Ready_pressed()
    {
        if (Menu.somecardselected)
        {
            startgame = true;
            _Ready();
        }
    }
    public void _on_Deck_pressed()
    {
        deckpressed = true;
        _Ready();
    }
    public void _on_Summon_pressed()
    {
        if (readytoSummon != null)
        {
            if (!communistside)
            {
                readyforsummon = true;
                GetNode<RichTextLabel>("Board/ActionMessage").Text = "Ready for Summon";
            }
            else
            {
                readyforsummon = true;
                GetNode<RichTextLabel>("Board/ActionMessage").Text = "Ready for Summon";
            }
        }
    }
    public void _on_Capitalist_pressed()
    {
        communistside = false;
        PathToDeckPlayer1 = "/home/daniel/Documents/Programacion/Proyecto Battle Cards/Subido a GitHub/Cold-War-Develop/Decks/Capitalist";
        PathToDeckPlayer2 = "/home/daniel/Documents/Programacion/Proyecto Battle Cards/Subido a GitHub/Cold-War-Develop/Decks/Communist";
    }
    public void _on_Communist_pressed()
    {
        communistside = true;
        PathToDeckPlayer1 = "/home/daniel/Documents/Programacion/Proyecto Battle Cards/Subido a GitHub/Cold-War-Develop/Decks/Capitalist";
        PathToDeckPlayer2 = "/home/daniel/Documents/Programacion/Proyecto Battle Cards/Subido a GitHub/Cold-War-Develop/Decks/Communist";
    }
    public void _on_Button_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button2_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D2").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D2").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button3_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D3").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D3").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button4_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D4").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D4").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button5_pressed()
    {
        if (!communistside)
            for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
            {
                if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                {
                    Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D5").Position;
                    break;
                }
            }
        else
            for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
            {
                if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                {
                    Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D5").Position;
                    break;
                }
            }
        readyforsummon = false;
    }
    public void _on_Button6_pressed()
    {
        if (!communistside)
            for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
            {
                if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                {
                    Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D6").Position;
                    break;
                }
            }
        else
            for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
            {
                if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                {
                    Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D6").Position;
                    break;
                }
            }
        readyforsummon = false;
    }
    public void _on_Button7_pressed()
    {
        if (!communistside)
            for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
            {
                if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                {
                    Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D7").Position;
                    break;
                }
            }
        else
            for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
            {
                if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                {
                    Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D7").Position;
                    break;
                }
            }
        readyforsummon = false;
    }
    public void _on_Button8_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D8").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D8").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button9_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D9").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D9").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button10_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D10").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D10").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button11_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D11").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D11").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button12_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D12").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D12").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button13_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D13").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D13").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button14_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D14").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D14").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button15_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D15").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D15").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
    }
    public void _on_Button16_pressed()
    {
        if (readyforsummon)
        {
            if (!communistside)
                for (int i = 0; i < Players[0].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[0].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[0].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D16").Position;
                        break;
                    }
                }
            else
                for (int i = 0; i < Players[1].PlayerBoard.HandCards.Count; i++)
                {
                    if (Players[1].PlayerBoard.HandCards[i].Name == readytoSummon)
                    {
                        Players[1].PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D16").Position;
                        break;
                    }
                }
            readyforsummon = false;
        }
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

            x = (PackedScene)GD.Load("res://CardSupport.tscn");
            Deck.Add((CardSupport)x.Instance());
            Deck[i].GetNode<Sprite>("CardMargin/BackgroundCard").Texture = CardTexture;
            Deck[i].GetNode<MarginContainer>("CardMargin").RectSize = GetNode<MarginContainer>("Board/CardOnBoardMargin").RectSize;

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
        }
        return Deck;
    }
    public void _on_ChangeSide_pressed()
    {
        if (communistside)
        {
            communistside = false;
        }
        else
        {
            communistside = true;
        }
    }
    public void _on_AttackButton_pressed()
    {
        if (SelectedCard)
        {
            readyforattack = true;
            SelectedCard = false;
        }
    }
    public void Attack()
    {
        int attack = GetNode<CardSupport>("Board/" + selectedcard).Attack;
        int life = GetNode<CardSupport>("Board/" + attackedcard).Life;
        life -= attack;
        GetNode<CardSupport>("Board/" + attackedcard).Life = life;
    }
    // public override void _Input(InputEvent @event)
    // {
    //     if(@event is InputEventMouseButton eventMouseButton)
    //     GD.Print(eventMouseButton.Position);
    //     GD.Print("Viewport Resolution is: ", GetViewportRect().Size);   
    //     GD.Print("Mouse Position", GetViewport().GetMousePosition());
    // }
}
