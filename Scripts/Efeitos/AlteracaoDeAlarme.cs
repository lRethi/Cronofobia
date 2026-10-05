public partial class AlteracaoDeAlarme : EffectBase
{
    public override string Nome => "Alteração de Alarme";

    public override string Desc =>
        "Tenha 2 horas a mais para encontrar lugar para dormir, mas acorde 2 horas mais tarde.";

    public override string SpritePath =>
        "res://Assets/Sprites/Placeholder/the_placeholder.png";

    public override void aoEscolher()
    {
        TimeState.Instance.alterarFimNoite(
            TimeState.Instance.minutoFimNoite + 120f
        );

        TimeState.Instance.alterarInicioDia(
            TimeState.Instance.minutoInicioDia + 120f
        );
    }
}