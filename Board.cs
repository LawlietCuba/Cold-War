using Godot;
using System.Collections.Generic;
using System;

public class Board : Node
{
    // Declare member variables here. Examples:
    public List<CardSupport> Deck { get; set; }
    public List<CardSupport> HandCards { get; set; }
    public List<CardSupport> Graveyard { get; set; }
    public Dictionary<CardSupport, Position2D> CardsOnBoard { get; set; }
    public Board(List<CardSupport> Deck)
    {
        this.Deck = new List<CardSupport>();
        this.HandCards = new List<CardSupport>();
        this.Graveyard = new List<CardSupport>();
        this.CardsOnBoard = new Dictionary<CardSupport, Position2D>();
        this.Deck = Deck;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
