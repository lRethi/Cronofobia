using Godot;

public partial class cenaCompra : Control
{
    [Export] public Label lblPreco;
    [Export] public Button botCompra;
    [Export] public Button botRouba;
    [Export] public Button botFecha;

    public float varPreco { get; private set; }
    public int custo { get; private set; }

    public static cenaCompra InstanciaAberta { get; private set; }

    public static bool HaInstanciaAberta =>
        IsInstanceValid(InstanciaAberta) &&
        !InstanciaAberta.IsQueuedForDeletion();

    private bool encerrando;

    [Signal]
    public delegate void ComprarEventHandler(int custo);

    [Signal]
    public delegate void RoubarEventHandler();

    [Signal]
    public delegate void FecharEventHandler();

    public override void _Ready()
    {
        if (HaInstanciaAberta && InstanciaAberta != this)
        {
            QueueFree();
            return;
        }

        InstanciaAberta = this;

        botCompra.Pressed += OnBotCompra;
        botRouba.Pressed += OnBotRouba;
        botFecha.Pressed += OnBotFecha;
    }

    public override void _ExitTree()
    {
        if (InstanciaAberta == this)
            InstanciaAberta = null;
    }

    public void SetupScene(float precoRecebido)
    {
        int centavos = (int)(GD.Randi() % 4) + 96;

        varPreco = precoRecebido + centavos / 100f;
        custo = Mathf.CeilToInt(varPreco);

        lblPreco.Text = $"R${varPreco:0.00}";

        botCompra.Disabled =
            NeedsState.Instance.varDinheiro < custo;

        TimeState.Instance.CongelarTempo();

        GameState.Instance.SetCameraInputEnabled(false);
        GameState.Instance.SetCameraMouseCaptured(false);
    }

    private void OnBotCompra()
    {
        if (encerrando)
            return;

        EmitSignal(SignalName.Comprar, custo);

        FinalizarCompra();
    }

    private void OnBotRouba()
    {
        if (encerrando)
            return;

        EmitSignal(SignalName.Roubar);

        FinalizarCompra();
    }

    private void OnBotFecha()
    {
        if (encerrando)
            return;

        EmitSignal(SignalName.Fechar);

        FinalizarCompra();
    }

    private void FinalizarCompra()
    {
        if (encerrando)
            return;

        encerrando = true;

        TimeState.Instance.DescongelarTempo();

        GameState.Instance.SetCameraInputEnabled(true);
        GameState.Instance.SetCameraMouseCaptured(true);

        QueueFree();
    }
}