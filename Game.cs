using Godot;
using System;
using System.IO;
using System.Collections.Generic;

public class Game : Node2D
{
    PackedScene NewNode;
    List<PlayerTemplate> Players;
    bool deckpressed;
    //bool notInstanced;
    bool readyforsummon;
    bool startgame;
    CardTemplate[] Cards;
    string PathToEnemyDeck;
    bool communistside;
    bool[] GamePhases = new bool[3];
    bool PassTurnPressed;
    bool endround;
    bool gameready;
    public static PlayerTemplate HumanPlayer;
    public static PlayerTemplate EnemyPlayer;
    public static string AttackedCardName;
    public static bool readyforattack;
    public static string SelectedCardName;
    public static bool readyforexecute;
    public static bool cardselected;
    public static string ReadytoSummonCardName;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var CardTexture = new ImageTexture();
        CardTexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Card.jpg");
        if (!startgame)
        {
            GetNode<Button>("Board/EffectButton").Disabled = true;
            GetNode<Button>("Board/AttackButton").Disabled = true;
            GetNode<Button>("Board/Summon").Disabled = true;
            GetNode<Button>("Board/EndPhase").Disabled = true;
            GetNode<Button>("Board/PassTurn").Disabled = true;

            GetNode<TextureButton>("Board/EnemyField").Show();
            GetNode<TextureButton>("Board/HumanPlayerField").Hide();

            GetNode<Sprite>("Board/ShowMargin/BackgroundCard").Position = new Vector2(GetNode<MarginContainer>("Board/ShowMargin").RectSize.x / 2, GetNode<MarginContainer>("Board/ShowMargin").RectSize.y / 2);
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard").Texture = CardTexture;
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard").Scale = GetNode<MarginContainer>("Board/ShowMargin").RectSize / CardTexture.GetSize();

            var PhotoCardTexture = new ImageTexture();
            PhotoCardTexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/foto-perfil-generica.jpg");
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture = PhotoCardTexture;
            GetNode<Sprite>("Board/ShowMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Scale = GetNode<MarginContainer>("Board/ShowMargin/BackgroundCard/PhotoCardMargin").RectSize / PhotoCardTexture.GetSize();

            startgame = true;
        }
        // else
        // {
        //     startgame = false;
        //     if (!notInstanced)
        //     {
        //         notInstanced = true;
        //     }
        // }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (deckpressed)
        {
            GamePhases[0] = true;
            GetNode<Button>("Board/PassTurn").Disabled = false;
            GetNode<RichTextLabel>("Board/ActionMessage").Text = "Summon Phase";
            var CardTexture = new ImageTexture();
            CardTexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Card.jpg");
            DrawCards(HumanPlayer, GetNode<Position2D>("Board/Position2D17"), GetNode<Position2D>("Board/Position2D18"), 8);
            List<CardSupport> Deck = new List<CardSupport>();
            Deck = MakeDeck(CardTexture, PathToEnemyDeck, new List<CardSupport>());
            EnemyPlayer = new PlayerTemplate(Deck[0].ClassCard, new Board(Deck));
            DrawCards(EnemyPlayer, GetNode<Position2D>("Board/Position2D19"), GetNode<Position2D>("Board/Position2D20"), 8);
            deckpressed = false;
            gameready = true;
        }
        if (readyforexecute)
        {
            Attack(GetNode<CardSupport>("Board/" + SelectedCardName), GetNode<CardSupport>("Board/" + AttackedCardName));
            readyforexecute = false;
        }
        if (!endround)
        {
            if (GamePhases[0])
            {

                //GetNode<Button>("Board/EffectButton").Disabled = true;
                GetNode<Button>("Board/AttackButton").Disabled = true;
                GetNode<Button>("Board/Summon").Disabled = false;
                GetNode<Button>("Board/EndPhase").Disabled = false;
            }
            else if (GamePhases[1])
            {
                //GetNode<Button>("Board/EffectButton").Disabled = false;
                GetNode<Button>("Board/AttackButton").Disabled = false;
                GetNode<Button>("Board/Summon").Disabled = true;
                GetNode<Button>("Board/EndPhase").Disabled = false;
            }
            else if (GamePhases[2])
            {
                //GetNode<Button>("Board/EffectButton").Disabled = false;
                GetNode<Button>("Board/AttackButton").Disabled = true;
                GetNode<Button>("Board/Summon").Disabled = false;
                GetNode<Button>("Board/EndPhase").Disabled = false;
            }
            if (gameready)
            {
                if ((HumanPlayer.PlayerBoard.HandCards.Count == 0 && HumanPlayer.PlayerBoard.CardsOnBoard.Count == 0 && EnemyPlayer.PlayerBoard.HandCards.Count == 0) ||
                (EnemyPlayer.PlayerBoard.HandCards.Count == 0 && EnemyPlayer.PlayerBoard.CardsOnBoard.Count == 0 && HumanPlayer.PlayerBoard.HandCards.Count == 0))
                {
                    EndRound();
                }
            }
        }
        else
        {
            GetNode<Button>("Board/EffectButton").Disabled = true;
            GetNode<Button>("Board/AttackButton").Disabled = true;
            GetNode<Button>("Board/Summon").Disabled = true;
            GetNode<Button>("Board/EndPhase").Disabled = true;
            GetNode<Button>("Board/PassTurn").Disabled = true;
        }
    }



    public void PrintCardsinRange(List<CardSupport> CardsToPrint, Position2D Left, Position2D Right, int amount)
    {
        double length = Right.Position.x - Left.Position.x;
        double CradWidth = length / amount;
        for (int i = 0; i < amount; i++)
        {
            CardsToPrint[i].GetNode<MarginContainer>("CardMargin").RectPosition = new Vector2((float)(Left.Position.x + i * CradWidth), Left.Position.y);
            if (!CardsToPrint[i].hasParent)
            {
                GetNode<Sprite>("Board").AddChild(CardsToPrint[i], true);
                GetNode<Node2D>("Board/CardSupport").Name = CardsToPrint[i].CardName;
                CardsToPrint[i].hasParent = true;
            }
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
        GetNode<RichTextLabel>("Board/ActionMessage").Text = "";
        _Ready();
    }
    public void _on_Capitalist_pressed()
    {
        communistside = false;
        PathToEnemyDeck = System.IO.Directory.GetCurrentDirectory() + "/Decks/Communist";
    }
    public void _on_Communist_pressed()
    {
        communistside = true;
        PathToEnemyDeck = System.IO.Directory.GetCurrentDirectory() + "/Decks/Capitalist";
    }
    public void _on_Summon_pressed()
    {
        PassTurnPressed = false;
        if (ReadytoSummonCardName != null)
        {
            readyforsummon = true;
            GetNode<RichTextLabel>("Board/ActionMessage").Text = "Ready for Summon";
        }
    }
    public void MakeSummon(Position2D square)
    {
        if (!communistside)
        {
            if (HumanPlayer.name == "Capitalist")
                MakeSummonAfterCheckingConditionals(HumanPlayer, square);
            else
            {
                MakeSummonAfterCheckingConditionals(EnemyPlayer, square);
            }
        }
        else
        {
            if (HumanPlayer.name == "Communist")
            {
                MakeSummonAfterCheckingConditionals(HumanPlayer, square);
            }
            else
            {
                MakeSummonAfterCheckingConditionals(EnemyPlayer, square);
            }
        }
        readyforsummon = false;
    }
    public void MakeSummonAfterCheckingConditionals(PlayerTemplate Player, Position2D square)
    {
        for (int i = 0; i < Player.PlayerBoard.HandCards.Count; i++)
        {
            if (Player.PlayerBoard.HandCards[i].Name == ReadytoSummonCardName)
            {
                Player.PlayerBoard.HandCards[i].summoned = true;
                Player.PlayerBoard.HandCards[i].GetNode<MarginContainer>("CardMargin").RectPosition = square.Position;
                Player.PlayerBoard.CardsOnBoard.Add(Player.PlayerBoard.HandCards[i], square);
                Player.PlayerBoard.HandCards.RemoveAt(i);
                break;
            }
        }
    }
    public void _on_Button_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D"));
        }
    }
    public void _on_Button2_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D2"));
        }
    }
    public void _on_Button3_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D3"));
        }
    }
    public void _on_Button4_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D4"));
        }
    }
    public void _on_Button5_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D5"));
        }
    }
    public void _on_Button6_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D6"));
        }
    }
    public void _on_Button7_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D7"));
        }
    }
    public void _on_Button8_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D8"));
        }
    }
    public void _on_Button9_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D9"));
        }
    }
    public void _on_Button10_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D10"));
        }
    }
    public void _on_Button11_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D11"));
        }
    }
    public void _on_Button12_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D12"));
        }
    }
    public void _on_Button13_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D13"));
        }
    }
    public void _on_Button14_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D14"));
        }
    }
    public void _on_Button15_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D15"));
        }
    }
    public void _on_Button16_pressed()
    {
        if (readyforsummon)
        {
            MakeSummon(GetNode<Position2D>("Board/Position2D16"));
        }
    }
    public void MakeCard(CardSupport Card)
    {
        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Name").Text = Card.CardName;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Lore").Text = Card.Lore;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/ClassCard").Text = Card.ClassCard;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Attack").Text = $"{Card.Attack}";

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Life").Text = $"{Card.Life}";

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Effect").Text = Card.Effect;

        var typetexture = new ImageTexture();
        switch (Card.Type)
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
        Cards = new CardTemplate[CardstoCreate.Length];
        for (int i = 0; i < CardstoCreate.Length; i++)
        {
            Cards[i] = new CardTemplate(CardstoCreate[i].FullName);

            NewNode = (PackedScene)GD.Load("res://CardSupport.tscn");
            Deck.Add((CardSupport)NewNode.Instance());
            Deck[i].GetNode<Sprite>("CardMargin/BackgroundCard").Texture = CardTexture;
            Deck[i].GetNode<MarginContainer>("CardMargin").RectSize = GetNode<MarginContainer>("Board/CardOnBoardMargin").RectSize;

            Deck[i].CardName = Cards[i].CardName;
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
    public void ChangeSide()
    {
        if (GetNode<TextureButton>("Board/EnemyField").Visible)
        {
            GetNode<TextureButton>("Board/EnemyField").Hide();
            GetNode<TextureButton>("Board/HumanPlayerField").Show();
        }
        else
        {
            GetNode<TextureButton>("Board/EnemyField").Show();
            GetNode<TextureButton>("Board/HumanPlayerField").Hide();
        }
        if (communistside)
        {
            communistside = false;
            GetNode<RichTextLabel>("Board/ActionMessage").Text = "Capitalist Side";

        }
        else
        {
            communistside = true;
            GetNode<RichTextLabel>("Board/ActionMessage").Text = "Communist Side";
        }
    }
    public void _on_AttackButton_pressed()
    {
        PassTurnPressed = true;
        if (cardselected)
        {
            if (!GetNode<CardSupport>("Board/" + SelectedCardName).hasAttacked)
            {
                readyforattack = true;
                cardselected = false;
                GetNode<RichTextLabel>("Board/ActionMessage").Text = "Ready for Attack";
            }
            else
            {
                GetNode<RichTextLabel>("Board/ActionMessage").Text = SelectedCardName + " has already attacked";
            }
        }
    }
    public void Attack(CardSupport AttackingCard, CardSupport AttackedCard)
    {
        if (AttackingCard.ClassCard != AttackedCard.ClassCard)
        {
            int attack = AttackingCard.Attack;
            int life = AttackedCard.Life;
            life -= attack;
            AttackedCard.Life = life;
            AttackedCard.UpdateCardVisual();
            GetNode<RichTextLabel>("Board/ActionMessage").Text = AttackedCard.CardName + " has received " + attack + " damage";
            AttackingCard.hasAttacked = true;
            if (life <= 0)
            {
                DestroyCard(AttackedCard);
            }
        }
    }
    public void DestroyCard(CardSupport Card)
    {
        Card.GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D18").Position;
        if (HumanPlayer.name == GetNode<CardSupport>("Board/" + AttackedCardName).ClassCard)
        {
            HumanPlayer.PlayerBoard.Graveyard.Add(Card);
            HumanPlayer.PlayerBoard.CardsOnBoard.Remove(Card);
            Card.summoned = false;
        }
        else
        {
            EnemyPlayer.PlayerBoard.Graveyard.Add(Card);
            EnemyPlayer.PlayerBoard.CardsOnBoard.Remove(Card);
            Card.summoned = false;
        }
        GetNode<RichTextLabel>("Board/ActionMessage").Text = AttackedCardName + " Destroyed";
    }
    public void DrawCards(PlayerTemplate Player, Position2D Left, Position2D Right, int amount)
    {
        if (Player.PlayerBoard.Deck.Count > 0)
        {
            if (Player.PlayerBoard.Deck.Count < amount)
            {
                amount = Player.PlayerBoard.Deck.Count;
            }
            for (var i = amount - 1; i >= 0; i--)
            {
                Player.PlayerBoard.HandCards.Add(Player.PlayerBoard.Deck[i]);
                Player.PlayerBoard.Deck.RemoveAt(i);
            }
            PrintCardsinRange(Player.PlayerBoard.HandCards, Left, Right, Player.PlayerBoard.HandCards.Count);
        }
        else
        {
            GetNode<RichTextLabel>("Board/ActionMessage").Text = "No more cards to draw";
        }
    }
    public void _on_EndPhase_pressed()
    {
        for (int i = 0; i < GamePhases.Length; i++)
        {
            if (GamePhases[i])
            {
                GamePhases[i] = false;
                if (i != GamePhases.Length - 1)
                {
                    GamePhases[i + 1] = true;
                    if (i == 0)
                    {
                        GetNode<RichTextLabel>("Board/ActionMessage").Text = "Battle Phase";
                    }
                    else if (i == 1)
                    {
                        UpdateCardAttackingStatus();
                        GetNode<RichTextLabel>("Board/ActionMessage").Text = "Second Summon Phase";
                    }
                    break;
                }
                else
                {
                    GamePhases[0] = true;
                    GetNode<RichTextLabel>("Board/ActionMessage").Text = "Summon Phase";
                    ChangeSide();
                }
            }
        }
    }
    public void UpdateCardAttackingStatus()
    {
        PlayerTemplate Player;
        if (communistside)
        {
            if (HumanPlayer.name == "Communist")
            {
                Player = HumanPlayer;
            }
            else
            {
                Player = EnemyPlayer;
            }
        }
        else
        {
            if (HumanPlayer.name == "Capitalist")
            {
                Player = HumanPlayer;
            }
            else
            {
                Player = EnemyPlayer;
            }
        }
        foreach (CardSupport Card in Player.PlayerBoard.CardsOnBoard.Keys)
        {
            if (Card.hasAttacked)
                Card.hasAttacked = false;
        }
    }
    public void _on_PassTurn_pressed()
    {
        if (PassTurnPressed)
        {
            EndRound();
        }
        else
        {

            PassTurnPressed = true;
            UpdateCardAttackingStatus();
            for (int i = 0; i < GamePhases.Length; i++)
            {
                if (GamePhases[i])
                {
                    GamePhases[i] = false;
                    GamePhases[0] = true;
                }
            }
            ChangeSide();
        }
    }
    public void EndRound()
    {
        int HumanPlayerPoints = 0;
        int EnemyPlayerPoints = 0;
        foreach (CardSupport Card in HumanPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            HumanPlayerPoints += Card.Life;
        }
        foreach (CardSupport Card in EnemyPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            EnemyPlayerPoints += Card.Life;
        }
        if (HumanPlayerPoints > EnemyPlayerPoints)
        {
            GetNode<RichTextLabel>("Board/ActionMessage").Text = HumanPlayer.name + " Wins this Round";
        }
        else if (HumanPlayerPoints < EnemyPlayerPoints)
        {
            GetNode<RichTextLabel>("Board/ActionMessage").Text = EnemyPlayer.name + " Wins this Round";
        }
        else
        {
            GetNode<RichTextLabel>("Board/ActionMessage").Text = "Draw";
        }
        endround = true;
    }
    // public override void _Input(InputEvent @event)
    // {
    //     if(@event is InputEventMouseButton eventMouseButton)
    //     GD.Print(eventMouseButton.Position);
    //     GD.Print("Viewport Resolution is: ", GetViewportRect().Size);   
    //     GD.Print("Mouse Position", GetViewport().GetMousePosition());
    // }
}
