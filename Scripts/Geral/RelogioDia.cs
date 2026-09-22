using Godot;
using System;

public partial class RelogioDia : Control
{
    [Export] public Texture2D imagemMorning;
    [Export] public Texture2D imagemAfternoon;
    [Export] public Texture2D imagemEvening;
    [Export] public Texture2D imagemNight;

    [Export] public float duracaoTransicao = 1.5f;

    private TextureRect imagemAtual;
    private TextureRect imagemProxima;

    private DayState estadoAtual;
    private Tween tweenTransicao;

    public override void _Ready()
    {
        imagemAtual = GetNode<TextureRect>("ImagemAtual");
        imagemProxima = GetNode<TextureRect>("ImagemProxima");

        estadoAtual = TimeState.Instance.CurrentDayState;

        imagemAtual.Texture = ObterImagem(estadoAtual);
        imagemAtual.Modulate = new Color(1f, 1f, 1f, 1f);

        imagemProxima.Modulate = new Color(1f, 1f, 1f, 0f);

        TimeState.Instance.DayStateChanged += TrocarEstado;
    }

    public override void _ExitTree()
    {
        if (TimeState.Instance != null)
            TimeState.Instance.DayStateChanged -= TrocarEstado;

        tweenTransicao?.Kill();
    }

    private void TrocarEstado(DayState novoEstado)
    {
        if (novoEstado == estadoAtual)
            return;

        estadoAtual = novoEstado;

        Texture2D novaImagem = ObterImagem(novoEstado);

        if (novaImagem == null)
            return;

        tweenTransicao?.Kill();

        imagemProxima.Texture = novaImagem;
        imagemProxima.Modulate = new Color(1f, 1f, 1f, 0f);

        tweenTransicao = CreateTween();
        tweenTransicao.SetParallel(true);
        tweenTransicao.SetTrans(Tween.TransitionType.Sine);
        tweenTransicao.SetEase(Tween.EaseType.InOut);

        tweenTransicao.TweenProperty(
            imagemAtual,
            "modulate:a",
            0f,
            duracaoTransicao
        );

        tweenTransicao.TweenProperty(
            imagemProxima,
            "modulate:a",
            1f,
            duracaoTransicao
        );

        tweenTransicao.SetParallel(false);

        tweenTransicao.TweenCallback(
            Callable.From(FinalizarTransicao)
        );
    }

    private void FinalizarTransicao()
    {
        imagemAtual.Texture = imagemProxima.Texture;

        imagemAtual.Modulate = new Color(1f, 1f, 1f, 1f);
        imagemProxima.Modulate = new Color(1f, 1f, 1f, 0f);
    }

    private Texture2D ObterImagem(DayState estado)
    {
        return estado switch
        {
            DayState.Morning => imagemMorning,
            DayState.Afternoon => imagemAfternoon,
            DayState.Evening => imagemEvening,
            DayState.Night => imagemNight,
            _ => imagemMorning
        };
    }
}