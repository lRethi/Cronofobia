using Godot;
using System;

public partial class inventoryExtraSlotsVisual : TextureRect
{

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(EffectManager.Instance == null)
		{
			return;
		}

		if (EffectManager.Instance.TemEfeito<MochilaModerna>() == true)
		{
			this.Visible = true;
		}
		else
		{
			this.Visible = false;
		}
	}
}
