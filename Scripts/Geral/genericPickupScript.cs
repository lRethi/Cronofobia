using Godot;

public partial class genericPickupScript : Area3D
{
    [Export]
    public TipoPickup tipoPickup;

    [Export]
    public ItemDefinition item;

    [Export]
    public float valorRecurso = 1f;

    [Export]
    public float preco = 0f;

    [Export]
    public bool compravel = false;

    [Export]
    public PackedScene cenaCompra;

    [Export]
    public bool canSetFlag = false;

    [Export]
    public bool canSetFlagOnSteal = true;

    [Export]
    public string flagToSetOnPickup;

    [Export]
    public string flagToSetOnSteal;

    [Export]
    public float alcanceInteracao = 1.25f;

    [Export]
    public float alcanceIma = 3f;

    [Export]
    public float velocidadeIma = 8f;

    private CharacterBody3D personagem;
    private bool podeInteragir = true;

    public enum TipoPickup
    {
        Item,
        Dinheiro
    }

    public override void _Ready()
    {
        if (personagem != null)
            return;

        Node area = GetParent();

        while (area != null)
        {
            CharacterBody3D encontrado =
                area.FindChild(
                    "charGeraldoSalvador",
                    true,
                    false
                ) as CharacterBody3D;

            if (encontrado != null)
            {
                personagem = encontrado;
                break;
            }

            area = area.GetParent();
        }
    }

    public override void _Process(double delta)
    {
        if (personagem == null)
            return;

        if (EffectManager.Instance != null &&
            EffectManager.Instance.TemEfeito<Ima>() &&
            !compravel)
        {
            ProcessarIma(
                (float)delta
            );

            return;
        }

        if (!Input.IsActionPressed("interact"))
            podeInteragir = true;

        float alcance =
            EffectManager.Instance != null
                ? EffectManager.Instance.GetInteractionRange(
                    alcanceInteracao
                )
                : alcanceInteracao;

        float distancia =
            (
                personagem.GlobalPosition -
                GlobalPosition
            ).Length();

        if (distancia > alcance)
            return;

        if (!Input.IsActionJustPressed("interact"))
            return;

        if (!podeInteragir)
            return;

        podeInteragir = false;

        if (compravel)
        {
            MostrarCenaCompra();
            return;
        }

        if (PegarItem(true))
            QueueFree();
    }

    private void ProcessarIma(float delta)
    {
        float distancia =
            (
                personagem.GlobalPosition -
                GlobalPosition
            ).Length();

        if (distancia > alcanceIma)
            return;

        GlobalPosition =
            GlobalPosition.MoveToward(
                personagem.GlobalPosition,
                velocidadeIma * delta
            );

        if (distancia > 0.2f)
            return;

        if (PegarItem(true))
            QueueFree();
    }

    private void MostrarCenaCompra()
    {
        if (cenaCompra == null)
            return;

        var cena =
            cenaCompra.Instantiate<cenaCompra>();

        GetTree().CurrentScene.AddChild(cena);

        float precoFinal =
            EffectManager.Instance != null
                ? EffectManager.Instance.AplicarPreco(
                    item?.Id,
                    preco
                )
                : preco;

        cena.SetupScene(precoFinal);

        cena.Comprar += ComprarItem;
        cena.Roubar += RoubarItem;
        cena.Fechar += FecharCompra;

        TimeState.Instance.CongelarTempo();
    }

    private void ComprarItem(int precoCompra)
    {
        if (NeedsState.Instance.varDinheiro <
            precoCompra)
        {
            return;
        }

        if (!PegarItem(false))
            return;

        NeedsState.Instance.SetDinheiro(
            NeedsState.Instance.varDinheiro -
            precoCompra
        );

        EffectManager.Instance?.CompraConcluida(
            item?.Id
        );

        QueueFree();
    }

    private void RoubarItem()
    {
        if (!PegarItem(true))
            return;

        if (canSetFlagOnSteal &&
            !string.IsNullOrEmpty(flagToSetOnSteal))
        {
            GameState.Instance.SetFlag(
                flagToSetOnSteal,
                true
            );
        }

        QueueFree();
    }

    private void FecharCompra()
    {
    }

    private bool PegarItem(bool veioDoChao)
    {
        switch (tipoPickup)
        {
            case TipoPickup.Item:

                if (item == null)
                    return false;

                if (veioDoChao &&
                    EffectManager.Instance != null &&
                    EffectManager.Instance.DeveDescartarFeijaoAoPegar(
                        item.Id
                    ))
                {
                    return true;
                }

                if (InventoryState.Instance == null)
                    return false;

                bool resultado =
                    InventoryState.Instance.AdicionarItem(
                        item
                    );

                if (resultado &&
                    canSetFlag &&
                    !string.IsNullOrEmpty(
                        flagToSetOnPickup))
                {
                    GameState.Instance.SetFlag(
                        flagToSetOnPickup,
                        true
                    );
                }

                return resultado;

            case TipoPickup.Dinheiro:

                NeedsState.Instance.SetDinheiro(
                    NeedsState.Instance.varDinheiro +
                    valorRecurso
                );

                return true;
        }

        return false;
    }
}