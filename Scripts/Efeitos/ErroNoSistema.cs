using Godot;

public partial class ErroNoSistema : EffectBase
{
    public override string Nome => "Erro no Sistema";

    public override string Desc =>
        "Receba entre 3 e 7 manutenções aleatórias, e deixe de receber novas manutenções nos próximos dias. Algo estranho está acontecendo...";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override bool PodeAparecerAleatoriamente()
    {
        return false;
    }

    public override void aoEscolher()
    {
        int diasFaltando =
            TimeState.Instance.maximoDias -
            TimeState.Instance.diaAtual;

        int quantidade =
            Mathf.Clamp(
                diasFaltando,
                3,
                7
            );

        EffectManager.Instance.AplicarEfeitosAleatorios(
            quantidade
        );

        GameState.Instance.AdicionarWeirdRouteValue(5);

        EffectManager.Instance.BloquearNovasManutencoes();
    }
}