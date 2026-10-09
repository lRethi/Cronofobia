using Godot;
using System;

public partial class IndicadorPodeDormir : TextureRect
{
	public override void _Process(double delta)
	{
		if (TimeState.lugarParaDormir)
		{
			this.Visible = true;
		}
		else
		{
			this.Visible = false;
		}
	}
}
