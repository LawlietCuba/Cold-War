using Godot;
using System.Collections.Generic;
using System;

public class PlayerTemplate : Node
{
    // Declare member variables here. Examples:
    public string name { get; private set; }
    public Board PlayerBoard { get; set; }

    public PlayerTemplate(string name, Board PlayerBoard)
    {
        this.name = name;
        this.PlayerBoard = PlayerBoard;
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
