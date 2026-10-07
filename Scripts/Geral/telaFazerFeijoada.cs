using Godot;
using System;

public partial class telaFazerFeijoada : Panel
{

	[Export] public Button btnFazerFeijoada { get; set; }
	// Called every frame. 'delta' is the elapsed time since the previous frame.

	public override void _Ready()
	{
		btnFazerFeijoada.Pressed += () => FazerFeijoada();
	}

	public override void _Process(double delta)
	{
		if(EffectManager.Instance == null)
		{
			return;
		}

		if (EffectManager.Instance.TemEfeito<NeurochipDeCozinha>() == true)
		{
			if(InventoryState.Instance.ContarAgua() >= 2 && InventoryState.Instance.ContarItemPorId(EffectManager.IdLataFeijao) >= 2)
			{
				this.Visible = true;
				btnFazerFeijoada.Disabled = false;
			}
			else
			{
				this.Visible = false;
				btnFazerFeijoada.Disabled = true;
			}
		}
		else
		{
			this.Visible = false;
			btnFazerFeijoada.Disabled = true;
		}
	}

	private void FazerFeijoada()
	{
		InventoryState.Instance.CozinharFeijoada();
	}
}
