using System;

public abstract class EffectBase
{
    public abstract string Nome {get;}
    public abstract string Desc {get;}
    public abstract string SpritePath {get;}
    public virtual void aoEscolher() { }
    public virtual void InicioDoDia() { }
    public virtual void FimDoDia() { }
}
