using Godot;
using System;
using System.Security.Cryptography;

public partial class cenaCompra : Panel
{
	[Export] public Label lblPreco;
	[Export] public Button botCompra;
	[Export] public Button botRouba;
	[Export] public Button botFecha;
	private float randomAffix = 0f;
	public float varPreco {get; private set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (botCompra.Pressed
	}

	

	private void generateRandomAffix()
	{
		randomAffix = GD.Randi() % 96 + 5;
	}

	public void setupScene(float precoRecebido)
	{
		varPreco = precoRecebido;
		generateRandomAffix();
		lblPreco.Text = $"R${varPreco}.{randomAffix}";
		if(varPreco+1 > NeedsState.Instance.varDinheiro)
		{
			botCompra.Disabled = true;
		}
	}
}
