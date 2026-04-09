using Godot;
using System;

public class FirstSceneScript : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private AudioStreamPlayer player;
    // Called when the node enters the scene tree for the first time.
    private Tween tween;
    public override void _Ready()
    {
        player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        //Para el fadeout
        tween = new Tween();
        AddChild(tween);
    }

    public void _on_Button_pressed()
    {
        // Fade to black by reducing alpha to 0 over 1 second
        player.Play();
        tween.InterpolateProperty(
            this, "modulate:a", 1f, 0f, 1.3f,
            Tween.TransitionType.Sine, Tween.EaseType.InOut
        );

        // When finished, change scene
        tween.Connect("tween_all_completed", this, nameof(OnFadeOutFinished));

        tween.Start();
    }

    private void OnFadeOutFinished()
    {
        GetTree().ChangeScene("res://Scenes/PlayScene.tscn");
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
