using Godot;
using System;

public partial class rotatePosOnCamera : Node3D
{
	Camera3D camera;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		camera = GetViewport().GetCamera3D();
		Vector3 direcao = camera.GlobalPosition - GlobalPosition;
		direcao.Y = 0;
		direcao = -direcao;

		if (GetParent() is Node3D parentNode)
		{
			parentNode.LookAt(parentNode.GlobalPosition + direcao, Vector3.Up);
		}
	}
}
