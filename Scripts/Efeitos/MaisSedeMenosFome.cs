using Godot;
using System;

public partial class MaisSedeMenosFome : EffectBase
{
    public override string Nome => "Mais Sede, Menos Fome";
    public override string Desc => "A IA passa a consumir mais da água do seu corpo para funcionar, mas isso alivia seu estômago. Aumente a sede máxima em 1 e diminua a fome máxima em 1.";
    public override string SpritePath => "res://Assets/Sprites/Placeholder/the_placeholder.png";
    public override void aoEscolher()
    {
        NeedsState.Instance.AlterarMaximoFome(NeedsState.Instance.maximoFome - 1f);
        NeedsState.Instance.AlterarMaximoSede(NeedsState.Instance.maximoSede + 1f);   
    }
}
