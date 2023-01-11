using Godot;
using System;
using System.Collections.Generic;

public class CardSupport : Node2D
{
    public string CardName { get; set; }
    public string cardtype { get; set; }
    public string Rareness { get; set; }
    public string Lore { get; set; }
    public int Attack { get; set; }
    public int Health { get; set; }
    public string political_current { get; set; }
    public string PathToPhoto { get; set; }
    public string EffectText{get;set;}
    public List<Token> Effect { get; set; }
    public bool summoned { get; set; }
    public bool hasParent { get; set; }
    public bool hasAttacked { get; set; }
    public bool hasActivatedEffect { get; set; }
    // Called when the node enters the scene tree for the first time.

    public CardSupport(string CardName, string cardtype, string Rareness, string Lore, int Attack, int Health, string politcal_current, string PathToPhoto, string EffectText, List<Token> Effect)
    {
        this.CardName = CardName;
        this.cardtype = cardtype;
        this.Rareness = Rareness;
        this.Lore = Lore;
        this.Attack = Attack;
        this.Health = Health;
        this.political_current = politcal_current;
        this.PathToPhoto = PathToPhoto;
        this.EffectText = EffectText;
        this.Effect = Effect;
    }
    public List<EffectExpression> DoEffect()
    {
        if(this.Effect != null)
        {
            TokenStream effect_stream = new TokenStream(this.Effect);
            EffectParser effect_parser = new EffectParser(effect_stream);
            List<CompilingError> effect_errors = new List<CompilingError>();
            ColdWarProgram effect_program = effect_parser.ParseProgram(effect_errors);

            if (effect_errors.Count > 0)
            {
                foreach (CompilingError error in effect_errors)
                {
                    GD.Print(error.Location.Line + " " + error.Code + " " + error.Argument);
                }
            }
            else
            {
                Context context = new Context();
                Scope scope = new Scope();

                effect_program.CheckSemantic(context, scope, effect_errors);

                if (effect_errors.Count > 0)
                {
                    foreach (CompilingError error in effect_errors)
                    {
                        GD.Print( error.Location.Line + " " + error.Code + " " + error.Argument);
                    }
                }
                else
                {
                    effect_program.Evaluate();

                    return effect_program.Effects;
                }
            } 
        }
        return null;
    }

    public CardSupport()
    {

    }
    public override void _Ready()
    {
        //Hide();
        var CardTexture = new ImageTexture();
        CardTexture.Load(System.IO.Directory.GetCurrentDirectory() + "/Textures/Card.jpg");
        GetNode<Sprite>("CardMargin/BackgroundCard").Position = new Vector2(GetNode<MarginContainer>("CardMargin").RectSize.x / 2, GetNode<MarginContainer>("CardMargin").RectSize.y / 2);
        GetNode<Sprite>("CardMargin/BackgroundCard").Texture = CardTexture;
        GetNode<Sprite>("CardMargin/BackgroundCard").Scale = GetNode<MarginContainer>("CardMargin").RectSize / CardTexture.GetSize();
    }
    public void _on_SelectCard_pressed()
    {
        UpdateCardVisual();
        if (summoned)
        {
            if (!Game.readyforattack)
            {
                Game.SelectedCardName = this.CardName;
                Game.ReadytoSummonCardName = this.CardName;
                Game.cardselected = true;
            }
            else
            {
                Game.AttackedCardName = this.CardName;
                Game.readyforattack = false;
                Game.readyforexecute = true;
            }

            if(Game.ReadyForEffect)
            {
                Game.EffectObjetive = this.CardName;
                Game.ReadyForEffect = false;
                Game.readyforexecuteeffect = true;
            }
        }
        else
        {
            Game.ReadytoSummonCardName = this.CardName;
        }
        GetNode<RichTextLabel>("/root/Main/Game/Board/ActionMessage").Text = "Card Selected";
    }
    public void UpdateCardVisual()
    {
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Name").Text = this.CardName;
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Effect").Text = EffectText;
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Lore").Text = this.Lore;
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Attack").Text = this.Attack.ToString();
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Life").Text = this.Health.ToString();
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/ClassCard").Text = this.political_current;
        GetNode<Sprite>("/root/Main/Game/Board/ShowMargin/BackgroundCard/TypeMargin/TypePhoto").Texture = GetNode<Sprite>("CardMargin/BackgroundCard/TypeMargin/TypePhoto").Texture;
        GetNode<Sprite>("/root/Main/Game/Board/ShowMargin/BackgroundCard/TypeMargin/TypePhoto").Scale = GetNode<MarginContainer>("/root/Main/Game/Board/ShowMargin/BackgroundCard/TypeMargin").RectSize / GetNode<Sprite>("CardMargin/BackgroundCard/TypeMargin/TypePhoto").Texture.GetSize();
        GetNode<Sprite>("/root/Main/Game/Board/ShowMargin/BackgroundCard/RarenessMargin/RarenessPhoto").Texture = GetNode<Sprite>("CardMargin/BackgroundCard/RarenessMargin/RarenessPhoto").Texture;
        GetNode<Sprite>("/root/Main/Game/Board/ShowMargin/BackgroundCard/RarenessMargin/RarenessPhoto").Scale = GetNode<MarginContainer>("/root/Main/Game/Board/ShowMargin/BackgroundCard/RarenessMargin").RectSize / GetNode<Sprite>("CardMargin/BackgroundCard/RarenessMargin/RarenessPhoto").Texture.GetSize();
        GetNode<Sprite>("/root/Main/Game/Board/ShowMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture = GetNode<Sprite>("CardMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture;
        GetNode<Sprite>("/root/Main/Game/Board/ShowMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Scale = GetNode<MarginContainer>("/root/Main/Game/Board/ShowMargin/BackgroundCard/PhotoCardMargin").RectSize / GetNode<Sprite>("CardMargin/BackgroundCard/PhotoCardMargin/PhotoCard").Texture.GetSize();
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
