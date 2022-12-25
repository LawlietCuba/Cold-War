using Godot;
using System;

public class CardSupport : Node2D
{
    public string CardName { get; set; }
    public string Lore { get; set; }
    public string ClassCard { get; set; }
    public int Attack { get; set; }
    public int Life { get; set; }
    public string Effect { get; set; }
    public string Type { get; set; }
    public string Rareness { get; set; }
    public string PathToPhoto { get; set; }
    public bool summoned { get; set; }
    public bool hasParent { get; set;}
    // Called when the node enters the scene tree for the first time.

    // public CardSupport(string name, string lore, string classcard, int attack, int Life, string effect, string type, string rareness, string pathtophoto)
    // {
    //     this.Name = name;
    //     this.Lore = lore;
    //     this.ClassCard = classcard;
    //     this.Attack = attack;
    //     this.Life = Life;
    //     this.Effect = effect;
    //     this.Type = type;
    //     this.Rareness = rareness;
    //     this.PathToPhoto = pathtophoto;
    // }
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
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Effect").Text = this.Effect;
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Lore").Text = this.Lore;
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Attack").Text = this.Attack.ToString();
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/Life").Text = this.Life.ToString();
        GetNode<RichTextLabel>("/root/Main/Game/Board/ShowMargin/BackgroundCard/ClassCard").Text = this.ClassCard;
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
