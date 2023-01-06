using System.Linq;
using Godot;
using System;
using System.IO;
using System.Collections.Generic;

public class Game : Node2D
{
    PackedScene NewNode;
    List<PlayerTemplate> Players;
    bool deckpressed;
    bool Instanced;
    bool readyforsummon;
    bool startgame;
    CardTemplate[] Cards;
    string PathToEnemyDeck;
    bool communistside;
    bool[] GamePhases = new bool[3];
    bool PassTurnPressed;
    bool endround = true;
    bool gameready;
    Dictionary<string, PlayerTemplate> RoundWinner = new Dictionary<string, PlayerTemplate>();
    int Round = 1;
    List<Position2D> HumanPlayerField = new List<Position2D>();
    List<Position2D> EnemyPlayerField = new List<Position2D>();
    public static PlayerTemplate HumanPlayer;
    public static PlayerTemplate EnemyPlayer;
    public static string AttackedCardName;
    public static bool readyforattack;
    public static bool ReadyForEffect;
    public static string SelectedCardName;
    public static bool readyforexecute;
    public static bool readyforexecuteeffect;
    public static bool cardselected;
    public static string ReadytoSummonCardName;
    public static string EffectObjetive;
    public static string Effect;
    public static int TempAmount;
    public static bool AutomaticEffect;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var CardTexture = new ImageTexture();
        CardTexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Card.jpg");
        GetNode<RichTextLabel>("Board/GameWinner").Hide();
        if (!startgame)
        {
            HumanPlayerField.Add(GetNode<Position2D>("Board/Position2D9"));
            HumanPlayerField.Add(GetNode<Position2D>("Board/Position2D10"));
            HumanPlayerField.Add(GetNode<Position2D>("Board/Position2D11"));
            HumanPlayerField.Add(GetNode<Position2D>("Board/Position2D12"));
            HumanPlayerField.Add(GetNode<Position2D>("Board/Position2D13"));
            HumanPlayerField.Add(GetNode<Position2D>("Board/Position2D14"));
            HumanPlayerField.Add(GetNode<Position2D>("Board/Position2D15"));
            HumanPlayerField.Add(GetNode<Position2D>("Board/Position2D16"));

            EnemyPlayerField.Add(GetNode<Position2D>("Board/Position2D"));
            EnemyPlayerField.Add(GetNode<Position2D>("Board/Position2D2"));
            EnemyPlayerField.Add(GetNode<Position2D>("Board/Position2D3"));
            EnemyPlayerField.Add(GetNode<Position2D>("Board/Position2D4"));
            EnemyPlayerField.Add(GetNode<Position2D>("Board/Position2D5"));
            EnemyPlayerField.Add(GetNode<Position2D>("Board/Position2D6"));
            EnemyPlayerField.Add(GetNode<Position2D>("Board/Position2D7"));
            EnemyPlayerField.Add(GetNode<Position2D>("Board/Position2D8"));

            GetNode<Button>("Board/EffectButton").Disabled = true;
            GetNode<Button>("Board/AttackButton").Disabled = true;
            GetNode<Button>("Board/Summon").Disabled = true;
            GetNode<Button>("Board/EndPhase").Disabled = true;
            GetNode<Button>("Board/PassTurn").Disabled = true;
            GetNode<Button>("Board/NextRound").Disabled = true;

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
        if (deckpressed && !Instanced)
        {
            Instanced = true;
            List<CardSupport> Deck = new List<CardSupport>();
            Deck = MakeDeck(CardTexture, PathToEnemyDeck, new List<CardSupport>());
            EnemyPlayer = new PlayerTemplate(Deck[0].political_current, new Board(Deck));
            ShuffleCards(HumanPlayer.PlayerBoard.Deck);
            ShuffleCards(EnemyPlayer.PlayerBoard.Deck);
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta = 1/10)
    {
        if (deckpressed)
        {
            endround = false;
            GamePhases[0] = true;
            GetNode<Button>("Board/PassTurn").Disabled = false;
            GetNode<RichTextLabel>("Board/ActionMessage").Text = "Summon Phase";
            var CardTexture = new ImageTexture();
            CardTexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Card.jpg");
            DrawCards(HumanPlayer, GetNode<Position2D>("Board/Position2D17"), GetNode<Position2D>("Board/Position2D18"), 5);
            DrawCards(EnemyPlayer, GetNode<Position2D>("Board/Position2D19"), GetNode<Position2D>("Board/Position2D20"), 5);
            deckpressed = false;
            gameready = true;
        }
        if (readyforexecute)
        {
            Attack(GetNode<CardSupport>("Board/" + SelectedCardName), GetNode<CardSupport>("Board/" + AttackedCardName));
            readyforexecute = false;
        }
        if(readyforexecuteeffect)
        {
            switch(Effect)
            {
                case TokenValues.DestroyCard:
                    DestroyCard(GetNode<CardSupport>("Board/" + EffectObjetive));
                    break;
                case TokenValues.DrawCards:
                    break;
                case TokenValues.DecreaseHealth:
                    if(CanDoTheEffect()) DecreaseHealth(GetNode<CardSupport>("Board/" + EffectObjetive));
                    else GetNode<RichTextLabel>("Board/ActionMessage").Text = "This effect can't effect Event and Politic Types";
                    break;
                case TokenValues.DecreaseAttack:
                    if(CanDoTheEffect())DecreaseAttack(GetNode<CardSupport>("Board/" + EffectObjetive));
                    else GetNode<RichTextLabel>("Board/ActionMessage").Text = "This effect can't effect Event and Politic Types";
                    break;
                case TokenValues.IncreaseHealth:
                    if(CanDoTheEffect()) IncreaseHealth(GetNode<CardSupport>("Board/" + EffectObjetive));
                    else GetNode<RichTextLabel>("Board/ActionMessage").Text = "This effect can't effect Event and Politic Types";
                    break;
                case TokenValues.IncreaseAttack:
                    if(CanDoTheEffect())IncreaseAttack(GetNode<CardSupport>("Board/" + EffectObjetive));
                    else GetNode<RichTextLabel>("Board/ActionMessage").Text = "This effect can't effect Event and Politic Types";
                    break;
                default:
                    break;
            }
            readyforexecuteeffect = false;
            AutomaticEffect = false;
        }
        if (!endround)
        {
            if (GamePhases[0])
            {

                GetNode<Button>("Board/EffectButton").Disabled = true;
                GetNode<Button>("Board/AttackButton").Disabled = true;
                GetNode<Button>("Board/Summon").Disabled = false;
                GetNode<Button>("Board/EndPhase").Disabled = false;
                GetNode<Button>("Board/NextRound").Disabled = true;

            }
            else if (GamePhases[1])
            {
                GetNode<Button>("Board/EffectButton").Disabled = false;
                GetNode<Button>("Board/AttackButton").Disabled = false;
                GetNode<Button>("Board/Summon").Disabled = true;
                GetNode<Button>("Board/EndPhase").Disabled = false;
                GetNode<Button>("Board/NextRound").Disabled = true;
            }
            else if (GamePhases[2])
            {
                GetNode<Button>("Board/EffectButton").Disabled = true;
                GetNode<Button>("Board/AttackButton").Disabled = true;
                GetNode<Button>("Board/Summon").Disabled = false;
                GetNode<Button>("Board/EndPhase").Disabled = false;
                GetNode<Button>("Board/NextRound").Disabled = true;
            }
            if (gameready)
            {
                if ((HumanPlayer.PlayerBoard.HandCards.Count == 0 && HumanPlayer.PlayerBoard.CardsOnBoard.Count == 0) ||
                (EnemyPlayer.PlayerBoard.HandCards.Count == 0 && EnemyPlayer.PlayerBoard.CardsOnBoard.Count == 0))
                {
                    EndRound();
                }
                if (EnemyPlayer.name == "Communist")
                {
                    if (communistside)
                    {
                        PlayVirtualPlayer();
                    }
                }
                else
                {
                    if (!communistside)
                    {
                        PlayVirtualPlayer();
                    }
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

    private bool CanDoTheEffect()
    {
        if(GetNode<CardSupport>("Board/"+EffectObjetive).cardtype == TokenValues.Event || GetNode<CardSupport>("Board/"+EffectObjetive).cardtype == TokenValues.Politic)
            return false;
        return true;
    }

    private bool CanDoTheEffect(string CardName)
    {
        if(GetNode<CardSupport>("Board/"+CardName).cardtype == TokenValues.Event || GetNode<CardSupport>("Board/"+CardName).cardtype == TokenValues.Politic)
            return false;
        return true;
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
        if (endround)
        {
            deckpressed = true;
        }
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
        if (Player.PlayerBoard.HandCards.Contains(GetNode<CardSupport>("Board/" + ReadytoSummonCardName)))
        {
            GetNode<CardSupport>("Board/" + ReadytoSummonCardName).summoned = true;
            GetNode<CardSupport>("Board/" + ReadytoSummonCardName).GetNode<MarginContainer>("CardMargin").RectPosition = square.Position;
            Player.PlayerBoard.CardsOnBoard.Add(GetNode<CardSupport>("Board/" + ReadytoSummonCardName), square);
            Player.PlayerBoard.HandCards.Remove(GetNode<CardSupport>("Board/" + ReadytoSummonCardName));
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

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/ClassCard").Text = Card.political_current;

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Attack").Text = $"{Card.Attack}";

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Life").Text = $"{Card.Health}";

        Card.GetNode<RichTextLabel>("CardMargin/BackgroundCard/Effect").Text = Card.EffectText;

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
            Deck[i].political_current = Cards[i].political_current;
            Deck[i].Attack = Cards[i].Attack;
            Deck[i].Health = Cards[i].Health;
            Deck[i].Effect = Cards[i].Effect;
            Deck[i].cardtype = Cards[i].cardtype;
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
        PassTurnPressed = false;
        if (cardselected)
        {
            if ((communistside && GetNode<CardSupport>("Board/" + SelectedCardName).political_current == "Communist") || (!communistside && GetNode<CardSupport>("Board/" + SelectedCardName).political_current == "Capitalist"))
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
    }

    public void Attack(CardSupport AttackingCard, CardSupport AttackedCard)
    {
        if (AttackingCard.political_current != AttackedCard.political_current)
        {
            int attack = AttackingCard.Attack;
            int Health = AttackedCard.Health;
            Health -= attack;
            AttackedCard.Health = Health;
            AttackedCard.UpdateCardVisual();
            GetNode<RichTextLabel>("Board/ActionMessage").Text = AttackedCard.CardName + " has received " + attack + " damage";
            AttackingCard.hasAttacked = true;
            if (Health <= 0)
            {
                DestroyCard(AttackedCard);
            }
        }
    }
    public void DestroyCard(CardSupport Card)
    {
        Card.GetNode<MarginContainer>("CardMargin").RectPosition = GetNode<Position2D>("Board/Position2D18").Position;
        if (HumanPlayer.name == Card.political_current)
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
        GetNode<RichTextLabel>("Board/ActionMessage").Text = Card.CardName + " Destroyed";
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
                        UpdateCardStatus();
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

    public void UpdateCardStatus()
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
            if(Card.hasActivatedEffect)
                Card.hasActivatedEffect = false;
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
            UpdateCardStatus();
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
            HumanPlayerPoints += Card.Health;
        }
        foreach (CardSupport Card in EnemyPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            EnemyPlayerPoints += Card.Health;
        }
        if (HumanPlayerPoints > EnemyPlayerPoints)
        {
            DeclareRoundWinner(HumanPlayer);
        }
        else if (HumanPlayerPoints < EnemyPlayerPoints)
        {
            DeclareRoundWinner(EnemyPlayer);
        }
        else if (PassTurnPressed)
        {
            GetNode<RichTextLabel>("Board/ActionMessage").Text = "Tie";
            PassTurnPressed = false;
        }
        else
        {
            if (HumanPlayer.PlayerBoard.HandCards.Count > 0)
            {
                DeclareRoundWinner(HumanPlayer);
            }
            else
            {
                DeclareRoundWinner(EnemyPlayer);
            }
        }
        endround = true;
        GetNode<Button>("Board/NextRound").Disabled = false;
    }
    public void _on_NextRound_pressed()
    {
        GamePhases[0] = false;
        GamePhases[1] = false;
        GamePhases[2] = false;
        for (int i = HumanPlayer.PlayerBoard.CardsOnBoard.Keys.Count - 1; i >= 0; i--)
        {
            DestroyCard(HumanPlayer.PlayerBoard.CardsOnBoard.Keys.ElementAt(i));
        }
        for (int i = EnemyPlayer.PlayerBoard.CardsOnBoard.Keys.Count - 1; i >= 0; i--)
        {
            DestroyCard(EnemyPlayer.PlayerBoard.CardsOnBoard.Keys.ElementAt(i));
        }
    }
    public void DeclareWinner(PlayerTemplate Player)
    {
        _on_NextRound_pressed();
        GetNode<RichTextLabel>("Board/GameWinner").Text = Player.name + " Wins the Game";
        GetNode<RichTextLabel>("Board/GameWinner").Show();
    }
    public void DeclareRoundWinner(PlayerTemplate Player)
    {
        if (Round != 3)
        {
            GetNode<RichTextLabel>("Board/ActionMessage").Text = Player.name + " Wins this Round";
            if (Round == 1) RoundWinner.Add("First", Player);
            else
            {
                RoundWinner.Add("Second", Player);
                if (RoundWinner["First"].name == Player.name)
                {
                    DeclareWinner(Player);
                }
            }
            Round++;
        }
        else
        {
            DeclareWinner(Player);
        }
    }
    public void ShuffleCards(List<CardSupport> Cards)
    {
        Random rnd = new Random();
        for (int i = Cards.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            CardSupport temp = Cards[j];
            Cards[j] = Cards[i];
            Cards[i] = temp;
        }
    }
    public void PlayVirtualPlayer()
    {
        if (GamePhases[0])
        {
            if (EnemyPlayer.PlayerBoard.HandCards.Count > 0 && EnemyPlayer.PlayerBoard.CardsOnBoard.Count < 8)
            {
                int amounttosummon = 8 - EnemyPlayer.PlayerBoard.CardsOnBoard.Count;
                if (amounttosummon > EnemyPlayer.PlayerBoard.HandCards.Count)
                {
                    amounttosummon = EnemyPlayer.PlayerBoard.HandCards.Count;
                }
                OrderByAttack(EnemyPlayer.PlayerBoard.HandCards);
                List<Position2D> PossiblePositions = new List<Position2D>();
                SetPossiblePositions(EnemyPlayer, EnemyPlayerField, PossiblePositions);
                for (int i = amounttosummon - 1; i >= 0; i--)
                {
                    ReadytoSummonCardName = EnemyPlayer.PlayerBoard.HandCards[i].CardName;
                    MakeSummon(PossiblePositions[i]);
                }
            }
            _on_EndPhase_pressed();
        }
        if (GamePhases[1])
        {
            if (HumanPlayer.PlayerBoard.CardsOnBoard.Count > 0 && EnemyPlayer.PlayerBoard.CardsOnBoard.Count > 0)
            {
                List<CardSupport> HumanCardsOnBoard = new List<CardSupport>();
                foreach (CardSupport Card in HumanPlayer.PlayerBoard.CardsOnBoard.Keys)
                {
                    HumanCardsOnBoard.Add(Card);
                }
                OrderByLife(HumanCardsOnBoard);
                List<CardSupport> EnemyCardsOnBoard = new List<CardSupport>();
                foreach (CardSupport Card in EnemyPlayer.PlayerBoard.CardsOnBoard.Keys)
                {
                    EnemyCardsOnBoard.Add(Card);
                }
                OrderByAttack(EnemyCardsOnBoard);
                int i = 0;
                do
                {
                    SelectedCardName = EnemyCardsOnBoard[i].CardName;
                    _on_AttackButton_pressed();
                    AttackedCardName = HumanCardsOnBoard[0].CardName;
                    Attack(GetNode<CardSupport>("Board/" + SelectedCardName), GetNode<CardSupport>("Board/" + AttackedCardName));
                    if (HumanPlayer.PlayerBoard.Graveyard.Contains(GetNode<CardSupport>("Board/" + AttackedCardName)))
                    {
                        HumanCardsOnBoard.Remove(GetNode<CardSupport>("Board/" + AttackedCardName));
                    }
                    i++;
                }
                while (HumanPlayer.PlayerBoard.CardsOnBoard.Count > 0 && i < EnemyCardsOnBoard.Count);
            }
            _on_EndPhase_pressed();
        }
        if (GamePhases[2])
        {
            _on_EndPhase_pressed();
        }
    }
    public void _on_EffectButton_pressed()
    {
        PassTurnPressed = false;

        if(cardselected)
        {

            if ((communistside && GetNode<CardSupport>("Board/" + SelectedCardName).political_current == "Communist") || (!communistside && GetNode<CardSupport>("Board/" + SelectedCardName).political_current == "Capitalist"))
            {
                if (!GetNode<CardSupport>("Board/" + SelectedCardName).hasActivatedEffect)
                {
                    cardselected = false;
                    GetNode<RichTextLabel>("Board/ActionMessage").Text = "Ready for activate effect";

                    List<EffectExpression> Effects = GetNode<CardSupport>("Board/" + SelectedCardName).DoEffect();
                    if(Effects != null)
                    {
                        foreach(EffectExpression eff in Effects)
                        {
                            switch(eff.GetValue().ToString())
                            {
                                case TokenValues.DrawCards:
                                    TempAmount = Convert.ToInt32( eff.Amount.GetValue());
                                    DrawCards(HumanPlayer, GetNode<Position2D>("Board/Position2D17"), GetNode<Position2D>("Board/Position2D18"), TempAmount);
                                    break;
                                case TokenValues.DestroyCard:
                                    Effect = eff.GetValue().ToString();
                                    checkEffectConditional(eff);
                                    if(AutomaticEffect) readyforexecuteeffect = true;
                                    else ReadyForEffect = true;
                                    break;
                                case TokenValues.DecreaseAttack:
                                case TokenValues.DecreaseHealth:
                                case TokenValues.IncreaseAttack:
                                case TokenValues.IncreaseHealth:
                                    Effect = eff.GetValue().ToString();
                                    TempAmount = Convert.ToInt32(eff.Amount.GetValue());
                                    checkEffectConditional(eff);
                                    if(AutomaticEffect) readyforexecuteeffect = true;
                                    else ReadyForEffect = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if(GetNode<CardSupport>("Board/" + SelectedCardName).cardtype == "Event" || GetNode<CardSupport>("Board/" + SelectedCardName).cardtype == "Politic")
                        {
                            DestroyCard(GetNode<CardSupport>("Board/"+SelectedCardName));
                        }
                    } 

                    GetNode<CardSupport>("Board/" + SelectedCardName).hasActivatedEffect = true;
                }
                else
                {
                    GetNode<RichTextLabel>("Board/ActionMessage").Text = SelectedCardName + " has already activated effect";
                }
            }

            
        }
    }

    public void OrderByAttack(List<CardSupport> Cards)
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            for (int j = 0; j < Cards.Count; j++)
            {
                if (Cards[i].Attack > Cards[j].Attack)
                {
                    CardSupport temp = Cards[i];
                    Cards[i] = Cards[j];
                    Cards[j] = temp;
                }
            }
        }
    }
    public void OrderByLife(List<CardSupport> Cards)
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            for (int j = 0; j < Cards.Count; j++)
            {
                if (Cards[i].Health > Cards[j].Health)
                {
                    CardSupport temp = Cards[i];
                    Cards[i] = Cards[j];
                    Cards[j] = temp;
                }
            }
        }
    }
    public void SetPossiblePositions(PlayerTemplate Player, List<Position2D> PlayerField, List<Position2D> PossiblePositions)
    {
        for (int i = 0; i < 8; i++)
        {
            if (!Player.PlayerBoard.CardsOnBoard.ContainsValue(PlayerField.ElementAt(i)))
            {
                PossiblePositions.Add(PlayerField.ElementAt(i));
            }
        }
    }

    public void DecreaseHealth(CardSupport Card)
    {
        Card.Health-=TempAmount;
        GetNode<RichTextLabel>("Board/ActionMessage").Text = Card.CardName + " has lost " + TempAmount + "of points of health";
    }

    public void DecreaseAttack(CardSupport Card)
    {
        Card.Attack-=TempAmount;
        GetNode<RichTextLabel>("Board/ActionMessage").Text = Card.CardName + " has lost " + TempAmount + "of points of attack";
    }
    public void IncreaseHealth(CardSupport Card)
    {

        Card.Health+=TempAmount;
        GetNode<RichTextLabel>("Board/ActionMessage").Text = Card.CardName + " has gain " + TempAmount + "of points of health";
    }
    public void IncreaseAttack(CardSupport Card)
    {
        Card.Attack+=TempAmount;
        GetNode<RichTextLabel>("Board/ActionMessage").Text = Card.CardName + " has gain " + TempAmount + "of points of attack";
    }

    public void checkEffectConditional(EffectExpression effexp)
    {   
        if(effexp.EffectConditional != null)
        {
            AutomaticEffect = true;
            if(EffectObjetive == null)
            {
                var randomCard = SearchRandomCardOnBoard();
                EffectObjetive = randomCard.CardName;
            }

            switch(effexp.EffectConditional)
            {
                case TokenValues.minHealth:
                    minHealth();
                    break;
                case TokenValues.minAttack:
                    minAttack();
                    break;
                case TokenValues.maxHealth:
                    maxHealth();
                    break;
                case TokenValues.maxAttack:
                    maxAttack();
                    break;
                default:
                    return;
            }
        }
    }

    public void minHealth()
    {
        foreach(CardSupport cardSupport in EnemyPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            if(cardSupport.Health<=GetNode<CardSupport>("Board/"+EffectObjetive).Health && CanDoTheEffect(cardSupport.CardName))
            {
                EffectObjetive = cardSupport.CardName;
            }
        }

        foreach(CardSupport cardSupport in HumanPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            if(cardSupport.Health<=GetNode<CardSupport>("Board/"+EffectObjetive).Health && CanDoTheEffect(cardSupport.CardName))
            {
                EffectObjetive = cardSupport.CardName;
            }
        }
    }
    public void minAttack()
    {
        foreach(CardSupport cardSupport in EnemyPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            if(cardSupport.Attack<=GetNode<CardSupport>("Board/"+EffectObjetive).Attack && CanDoTheEffect(cardSupport.CardName))
            {
                EffectObjetive = cardSupport.CardName;
            }
        }

        foreach(CardSupport cardSupport in HumanPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            if(cardSupport.Attack<=GetNode<CardSupport>("Board/"+EffectObjetive).Attack && CanDoTheEffect(cardSupport.CardName))
            {
                EffectObjetive = cardSupport.CardName;
            }
        }
    }
    public void maxHealth()
    {
        foreach(CardSupport cardSupport in EnemyPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            if(cardSupport.Health>=GetNode<CardSupport>("Board/"+EffectObjetive).Health && CanDoTheEffect(cardSupport.CardName))
            {
                EffectObjetive = cardSupport.CardName;
            }
        }

        foreach(CardSupport cardSupport in HumanPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            if(cardSupport.Health>=GetNode<CardSupport>("Board/"+EffectObjetive).Health && CanDoTheEffect(cardSupport.CardName))
            {
                EffectObjetive = cardSupport.CardName;
            }
        }
    }
    public void maxAttack()
    {
        foreach(CardSupport cardSupport in EnemyPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            if(cardSupport.Attack>=GetNode<CardSupport>("Board/"+EffectObjetive).Attack && CanDoTheEffect(cardSupport.CardName))
            {
                EffectObjetive = cardSupport.CardName;
            }
        }

        foreach(CardSupport cardSupport in HumanPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            if(cardSupport.Attack>=GetNode<CardSupport>("Board/"+EffectObjetive).Attack && CanDoTheEffect(cardSupport.CardName))
            {
                EffectObjetive = cardSupport.CardName;
            }
        }
    }

    public CardSupport SearchRandomCardOnBoard()
    {
        foreach(CardSupport cardSupport in EnemyPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            return cardSupport;
        }
        foreach(CardSupport cardSupport in HumanPlayer.PlayerBoard.CardsOnBoard.Keys)
        {
            return cardSupport;
        }

        return null;
    }
    
    // public override void _Input(InputEvent @event)
    // {
    //     if(@event is InputEventMouseButton eventMouseButton)
    //     GD.Print(eventMouseButton.Position);
    //     GD.Print("Viewport Resolution is: ", GetViewportRect().Size);   
    //     GD.Print("Mouse Position", GetViewport().GetMousePosition());
    // }
}