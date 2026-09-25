using Godot;

[GlobalClass]
public partial class QuestBancoPerfilIncompativel : QuestResource
{
    protected override void InicializarFlags()
    {
        Flags = new bool[1];
    }

    public override void Atualizar(GameState gameState)
    {
        if (!Ativa || Concluida)
            return;

        Flags[0] =
            gameState.GetFlag("perfil_incompativel");

        if (!Flags[0])
            return;

        Finalizar();
    }
}