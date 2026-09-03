using Godot;

public partial class cenaCompra : Control
{
    [Export] public Label lblPreco;
    [Export] public Button botCompra;
    [Export] public Button botRouba;
    [Export] public Button botFecha;

    public float varPreco { get; private set; }
    public int custo { get; private set; }

    [Signal]
    public delegate void ComprarEventHandler(int custo);

    [Signal]
    public delegate void RoubarEventHandler();

    [Signal]
    public delegate void FecharEventHandler();

    public override void _Ready()
    {
        botCompra.Pressed += OnBotCompra;
        botRouba.Pressed += OnBotRouba;
        botFecha.Pressed += OnBotFecha;
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
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void OnBotCompra()
    {
        EmitSignal(SignalName.Comprar, custo);
        TimeState.Instance.DescongelarTempo();
        Input.MouseMode = Input.MouseModeEnum.Captured;
        QueueFree();
    }

    private void OnBotRouba()
    {
        EmitSignal(SignalName.Roubar);
        TimeState.Instance.DescongelarTempo();
        Input.MouseMode = Input.MouseModeEnum.Captured;
        QueueFree();
    }

    private void OnBotFecha()
    {
        EmitSignal(SignalName.Fechar);
        TimeState.Instance.DescongelarTempo();
        Input.MouseMode = Input.MouseModeEnum.Captured;
        QueueFree();
    }
}