using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class InventoryState : Node
{
    public static InventoryState Instance { get; private set; }

    [Export]
    public string ItemOferecidoId { get; set; } = "";

    private int quantidadeSlots = 4;
    private List<ItemDefinition> itensInventario = new List<ItemDefinition>();
    private ItemDefinition itemOferecido;

    public int QuantidadeSlots => quantidadeSlots;
    public IReadOnlyList<ItemDefinition> ItensInventario => itensInventario;
    public int QuantidadeItens => itensInventario.Count;

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

        ItemDefinition item = itensInventario[index];

        if (!item.PodeSerUsado)
            return false;

        NeedsState.Instance.SetFome(
            NeedsState.Instance.varFome + item.FomeAoUsar
        );

        NeedsState.Instance.SetSede(
            NeedsState.Instance.varSede + item.SedeAoUsar
        );

        return RemoverItem(index);
    }

    public bool AdicionarItem(ItemDefinition item)
    {
        if (item == null || EstaCheio())
            return false;

        itensInventario.Add(item);
        EmitSignal(SignalName.InventarioAlterado);

        return true;
    }

    public bool RemoverItem(int index)
    {
        if (index < 0 || index >= itensInventario.Count)
            return false;

        itensInventario.RemoveAt(index);
        EmitSignal(SignalName.InventarioAlterado);

        return true;
    }

    public bool PossuiItem(ItemDefinition item)
    {
        return item != null && itensInventario.Contains(item);
    }

    public int EncontrarItem(ItemDefinition item)
    {
        return itensInventario.IndexOf(item);
    }

    public bool EstaCheio()
    {
        return itensInventario.Count >= quantidadeSlots;
    }

    public void Limpar()
    {
        itensInventario.Clear();
        itemOferecido = null;
        ItemOferecidoId = "";

        EmitSignal(SignalName.InventarioAlterado);
    }

    public async Task OferecerItem()
    {
        itemOferecido = null;
        ItemOferecidoId = "";

        EmitSignal(SignalName.OfertaSolicitada);

        await ToSignal(this, SignalName.OfertaFinalizada);
    }

    public bool PodeOferecerItem(ItemDefinition item)
    {
        return item != null && item.PodeSerDescartado;
    }

    public bool ConfirmarOferta(ItemDefinition item)
    {
        if (!PodeOferecerItem(item))
            return false;

        if (!PossuiItem(item))
            return false;

        itemOferecido = item;
        ItemOferecidoId = item.Id;

        EmitSignal(SignalName.OfertaFinalizada);

        return true;
    }

    public void CancelarOferta()
    {
        itemOferecido = null;
        ItemOferecidoId = "";

        EmitSignal(SignalName.OfertaFinalizada);
    }

    public bool ConsumirItemOferecido()
    {
        if (itemOferecido == null)
            return false;

        int index = EncontrarItem(itemOferecido);

        if (index < 0)
        {
            itemOferecido = null;
            ItemOferecidoId = "";
            return false;
        }

        bool removido = RemoverItem(index);

        if (removido)
        {
            itemOferecido = null;
            ItemOferecidoId = "";
        }

        return removido;
    }
}