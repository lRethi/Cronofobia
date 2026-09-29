using Godot;
using System;

public partial class cenaTitulo : Control
{
    [Export] public Texture2D[] imagensBackground;
    [Export] public TextureRect background;
    [Export] public TextureRect backgroundTransicao;

    private int imagemAtual = 0;
    private bool executando = true;

    public override void _Ready()
    {
        if (imagensBackground.Length == 0)
            return;

        background.Texture = imagensBackground[0];
        backgroundTransicao.Modulate = new Color(1, 1, 1, 0);

        CicloBackground();
    }

    private async void CicloBackground()
    {
        while (executando)
        {
            await ToSignal(
                GetTree().CreateTimer(3.75),
                SceneTreeTimer.SignalName.Timeout
            );

            if (!executando)
                break;

            int proximaImagem = (imagemAtual + 1) % imagensBackground.Length;

            backgroundTransicao.Texture = imagensBackground[proximaImagem];
            backgroundTransicao.Modulate = new Color(1, 1, 1, 0);

            Tween tween = CreateTween();

            tween.TweenProperty(
                backgroundTransicao,
                "modulate:a",
                1.0f,
                2.0f
            );

            await ToSignal(
                tween,
                Tween.SignalName.Finished
            );

            background.Texture = imagensBackground[proximaImagem];
            backgroundTransicao.Modulate = new Color(1, 1, 1, 0);

            imagemAtual = proximaImagem;
        }
    }

    public override void _ExitTree()
    {
        executando = false;
    }
}