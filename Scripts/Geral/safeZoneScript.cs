using Godot;
using System;

public partial class safeZoneScript : Area3D
{
	[Export] public CharacterBody3D personagem;
	void OnBodyEntered(Node3D body)
	{
		if (body == personagem)
		{
			TimeState.Instance.LugarSeguroParaDormir(true);
		}
	}

	void OnBodyExited(Node3D body)
	{
		if (body == personagem)
		{
			TimeState.Instance.LugarSeguroParaDormir(false);
		}
	}
}
