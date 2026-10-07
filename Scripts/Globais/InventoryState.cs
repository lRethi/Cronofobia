using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class InventoryState : Node
{
    public static InventoryState Instance { get; private set; }

    [Export]
    public string ItemOferecidoId { get; set; } = "";

    private const int quantidadeSlotsBase = 4;

    private List<ItemDefinition> itensInventario =
        new List<ItemDefinition>();

    private ItemDefinition itemOferecido;

    public int QuantidadeSlots =>
        quantidadeSlotsBase +
        (EffectManager.Instance?.GetBonusSlotsInventario() ?? 0);

    public IReadOnlyList<ItemDefinition> ItensInventario =>
        itensInventario;

    public int QuantidadeItens =>
        itensInventario.Count;

    [Signal]
    public delegate void InventarioAlteradoEventHandler();

    [Signal]
    public delegate void OfertaSolicitadaEventHandler();

    [Signal]
    public delegate void OfertaFinalizadaEventHandler();

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public bool UsarItem(int index)
    {
        if (index < 0 || index >= itensInventario.Count)
            return false;

        ItemDefinition item =
            itensInventario[index];

        if (item == null || !item.PodeSerUsado)
            return false;

        float fome = item.FomeAoUsar;
        float sede = item.SedeAoUsar;

        if (EffectManager.Instance != null)
        {
            fome = EffectManager.Instance.ModificarFomeAoUsar(
                item.Id,
                fome
            );

            sede = EffectManager.Instance.ModificarSedeAoUsar(
                item.Id,
                sede
            );
        }

        NeedsState.Instance.SetFome(
            NeedsState.Instance.varFome + fome
        );

        NeedsState.Instance.SetSede(
            NeedsState.Instance.varSede + sede
        );

        return RemoverItem(index);
    }

    public bool AdicionarItem(ItemDefinition item)
    {
        if (item == null)
            return false;

        if (EstaCheio())
            return false;

        itensInventario.Add(item);

        EmitSignal(
            SignalName.InventarioAlterado
        );

        return true;
    }

    public bool RemoverItem(int index)
    {
        if (index < 0 || index >= itensInventario.Count)
            return false;

        itensInventario.RemoveAt(index);

        EmitSignal(
            SignalName.InventarioAlterado
        );

        return true;
    }

    public bool PossuiItem(ItemDefinition item)
    {
        return item != null &&
               itensInventario.Contains(item);
    }

    public int EncontrarItem(ItemDefinition item)
    {
        return itensInventario.IndexOf(item);
    }

    public bool EstaCheio()
    {
        return itensInventario.Count >= QuantidadeSlots;
    }

    public void Limpar()
    {
        itensInventario.Clear();

        itemOferecido = null;
        ItemOferecidoId = "";

        EmitSignal(
            SignalName.InventarioAlterado
        );
    }

    public async Task OferecerItem()
    {
        itemOferecido = null;
        ItemOferecidoId = "";

        EmitSignal(
            SignalName.OfertaSolicitada
        );

        await ToSignal(
            this,
            SignalName.OfertaFinalizada
        );
    }

    public bool PodeOferecerItem(ItemDefinition item)
    {
        return item != null &&
               item.PodeSerDescartado;
    }

    public bool ConfirmarOferta(ItemDefinition item)
    {
        if (!PodeOferecerItem(item))
            return false;

        if (!PossuiItem(item))
            return false;

        itemOferecido = item;
        ItemOferecidoId = item.Id;

        EmitSignal(
            SignalName.OfertaFinalizada
        );

        return true;
    }

    public void CancelarOferta()
    {
        itemOferecido = null;
        ItemOferecidoId = "";

        EmitSignal(
            SignalName.OfertaFinalizada
        );
    }

    public bool ConsumirItemOferecido()
    {
        if (itemOferecido == null)
            return false;

        int index =
            EncontrarItem(itemOferecido);

        if (index < 0)
            return false;

        bool removido =
            RemoverItem(index);

        if (removido)
        {
            itemOferecido = null;
            ItemOferecidoId = "";
        }

        return removido;
    }

    public int ContarItemPorId(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return 0;

        int quantidade = 0;

        foreach (ItemDefinition item in itensInventario)
        {
            if (item != null && item.Id == itemId)
                quantidade++;
        }

        return quantidade;
    }

    public int ContarAgua()
    {
        return
            ContarItemPorId(EffectManager.IdGalaoAgua) +
            ContarItemPorId(EffectManager.IdGarrafaAgua);
    }

    public bool RemoverQuantidadePorId(
        string itemId,
        int quantidade
    )
    {
        if (string.IsNullOrEmpty(itemId) ||
            quantidade <= 0)
        {
            return false;
        }

        if (ContarItemPorId(itemId) < quantidade)
            return false;

        int removidos = 0;

        for (int i = itensInventario.Count - 1;
             i >= 0;
             i--)
        {
            ItemDefinition item =
                itensInventario[i];

            if (item == null ||
                item.Id != itemId)
            {
                continue;
            }

            itensInventario.RemoveAt(i);
            removidos++;

            if (removidos == quantidade)
                break;
        }

        EmitSignal(
            SignalName.InventarioAlterado
        );

        return true;
    }

    public bool RemoverQuantidadeAgua(int quantidade)
    {
        if (quantidade <= 0)
            return false;

        if (ContarAgua() < quantidade)
            return false;

        int removidos = 0;

        for (int i = itensInventario.Count - 1;
             i >= 0 && removidos < quantidade;
             i--)
        {
            ItemDefinition item =
                itensInventario[i];

            if (item == null)
                continue;

            if (item.Id != EffectManager.IdGalaoAgua &&
                item.Id != EffectManager.IdGarrafaAgua)
            {
                continue;
            }

            itensInventario.RemoveAt(i);
            removidos++;
        }

        EmitSignal(
            SignalName.InventarioAlterado
        );

        return removidos == quantidade;
    }

    public bool TransformarItem(
        string idOrigem,
        string idDestino
    )
    {
        if (EffectManager.Instance == null)
            return false;

        if (!EffectManager.Instance.PermiteTransformarAguaComida())
            return false;

        if (ContarItemPorId(idOrigem) <= 0)
            return false;

        ItemDefinition itemDestino =
            EffectManager.Instance.ObterItemPorId(
                idDestino
            );

        if (itemDestino == null)
            return false;

        if (!RemoverQuantidadePorId(
                idOrigem,
                1))
        {
            return false;
        }

        return AdicionarItem(itemDestino);
    }

    public bool CozinharFeijoada()
    {
        if (EffectManager.Instance == null)
            return false;

        if (!EffectManager.Instance.PermiteCozinharFeijoada())
            return false;

        if (ContarItemPorId(
                EffectManager.IdLataFeijao) < 2)
        {
            return false;
        }

        if (ContarAgua() < 2)
            return false;

        ItemDefinition feijoada =
            EffectManager.Instance.ObterItemPorId(
                EffectManager.IdFeijoada
            );

        if (feijoada == null)
            return false;

        if (!RemoverQuantidadePorId(
                EffectManager.IdLataFeijao,
                2))
        {
            return false;
        }

        if (!RemoverQuantidadeAgua(2))
            return false;

        return AdicionarItem(feijoada);
    }
}