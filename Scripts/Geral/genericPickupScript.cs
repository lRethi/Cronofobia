using System.Security.Cryptography;
using Godot;

public partial class genericPickupScript : Area3D
{
    [Export] public TipoRecursoEnum tipoRecurso;
    [Export] public float ValorRecurso = 1f;
    [Export] public float varPreco = 0f;
    [Export] public bool varCompravel = false;

	public enum TipoRecursoEnum
	{
		Fome,
		Sede,
		Dinheiro
	}

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not movimentoPerson)
            return;

        if(!varCompravel) pegarItem();

        QueueFree();
    }

    private void mostrarCenaCompra()
    {
        
    }
    public void comprarItem(float precoCompra)
    {
        NeedsState.Instance.SetDinheiro(NeedsState.Instance.varDinheiro - precoCompra);
        pegarItem();
    }

    public void roubarItem()
    {
        pegarItem();
    }

    private void pegarItem()
    {
        switch (tipoRecurso)
        {
            case TipoRecursoEnum.Fome:
                NeedsState.Instance.SetFome(NeedsState.Instance.varFome + ValorRecurso);
                break;

            case TipoRecursoEnum.Sede:
                NeedsState.Instance.SetSede(NeedsState.Instance.varSede + ValorRecurso);
                break;

            case TipoRecursoEnum.Dinheiro:
                NeedsState.Instance.SetDinheiro(NeedsState.Instance.varDinheiro + ValorRecurso);
                break;
        }
    }
}