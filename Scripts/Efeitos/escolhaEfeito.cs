using Godot;
using System;
using System.Collections.Generic;

public partial class escolhaEfeito : CanvasLayer
{
	[Export] public Button botao1;
	[Export] public Button botao2;
	[Export] public Button botao3;
	public List<EffectBase> efeitos = new List<EffectBase>();

    public override void _Ready()
    {
		GD.Print($"Criado: {GetPath()}");
        botao1.Pressed += OnBotao1Pressed;
		botao2.Pressed += OnBotao2Pressed;
		botao3.Pressed += OnBotao3Pressed;
    }

	public void Abrir(List<EffectBase> opcoes)
	{
		efeitos = opcoes;

		botao1.GetNode<RichTextLabel>("Nome").Text = efeitos[0].Nome;
		botao1.GetNode<RichTextLabel>("Desc").Text = efeitos[0].Desc;
		botao1.GetNode<TextureRect>("Sprite").Texture = GD.Load<Texture2D>(efeitos[0].SpritePath);

		botao2.GetNode<RichTextLabel>("Nome").Text = efeitos[1].Nome;
		botao2.GetNode<RichTextLabel>("Desc").Text = efeitos[1].Desc;
		botao2.GetNode<TextureRect>("Sprite").Texture = GD.Load<Texture2D>(efeitos[1].SpritePath);

		botao3.GetNode<RichTextLabel>("Nome").Text = efeitos[2].Nome;
		botao3.GetNode<RichTextLabel>("Desc").Text = efeitos[2].Desc;
		botao3.GetNode<TextureRect>("Sprite").Texture = GD.Load<Texture2D>(efeitos[2].SpritePath);
	}
	private void OnBotao1Pressed()
	{
		EscolherEfeito(0);
	}

	private void OnBotao2Pressed()
	{
		EscolherEfeito(1);
	}

	private void OnBotao3Pressed()
	{
		EscolherEfeito(2);
	}
	private void EscolherEfeito(int indice)
	{
		EffectManager.Instance.AdicionarEfeito(efeitos[indice]);
		TimeState.Instance.proximoDia();
		QueueFree();
	}
}
