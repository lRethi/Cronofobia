using System;
using Godot;
public partial class oObservador : Marker3D
{
	[Export] public Node3D Personagem;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 posPlayer = Personagem.GlobalPosition;
		Vector3 rot = Rotation;
		Vector3 dirCamera = posPlayer - this.GlobalPosition;
		dirCamera.Y = 0;

		float angRota = Mathf.Atan2(dirCamera.X, dirCamera.Z);
		angRota = -angRota;
		rot.Y = Mathf.LerpAngle(rot.Y, angRota, 0.1f);
		Rotation = rot;
	}
}
