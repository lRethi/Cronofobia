public abstract class EffectBase
{
    public abstract string Nome { get; }
    public abstract string Desc { get; }
    public abstract string SpritePath { get; }

    public virtual bool PodeEscolher() => true;
    public virtual bool PodeAparecerAleatoriamente() => true;

    public virtual void aoEscolher() { }
    public virtual void InicioDoDia() { }
    public virtual void FimDoDia() { }

    public virtual float MultiplicadorVelocidade() => 1f;
    public virtual float BonusVelocidade() => 0f;

    public virtual float MultiplicadorPreco(string itemId) => 1f;
    public virtual bool PodeComprarGratis(string itemId) => false;
    public virtual void CompraConcluida(string itemId) { }

    public virtual float MultiplicadorAlcanceInteracao() => 1f;

    public virtual int BonusSlotsInventario() => 0;

    public virtual bool PermiteDormirSemLugar() => false;
    public virtual bool PermiteTransformarAguaComida() => false;
    public virtual bool PermiteCozinharFeijoada() => false;

    public virtual bool DescartarFeijaoAoPegar(string itemId) => false;

    public virtual void AoEscolherOpcaoDialogo() { }

    public virtual float ModificarFomeAoUsar(string itemId, float valor)
    {
        return valor;
    }

    public virtual float ModificarSedeAoUsar(string itemId, float valor)
    {
        return valor;
    }
}