using Godot;

public partial class irParaJogo : Button
{
    [Export] public string caminhoCena;
    [Export] public TextureRect[] imagensLoading;
    [Export] public Control loadingScreen;

    public override void _Ready()
    {
        Pressed += Trocar;
    }

    private async void Trocar()
    {
        SceneTree arvore = GetTree();

        Disabled = true;
        loadingScreen.Visible = true;

        foreach (TextureRect imagem in imagensLoading)
            imagem.Visible = false;

        if (imagensLoading.Length > 0)
        {
            int indice = GD.RandRange(0, imagensLoading.Length - 1);
            imagensLoading[indice].Visible = true;
        }

        await ToSignal(
            arvore,
            SceneTree.SignalName.ProcessFrame
        );

        Error erro = ResourceLoader.LoadThreadedRequest(caminhoCena);

        if (erro != Error.Ok)
        {
            GD.PrintErr($"Erro ao iniciar carregamento da cena: {erro}");
            Disabled = false;
            loadingScreen.Visible = false;
            return;
        }

        while (true)
        {
            ResourceLoader.ThreadLoadStatus status =
                ResourceLoader.LoadThreadedGetStatus(caminhoCena);

            if (status == ResourceLoader.ThreadLoadStatus.Loaded)
                break;

            if (status == ResourceLoader.ThreadLoadStatus.Failed ||
                status == ResourceLoader.ThreadLoadStatus.InvalidResource)
            {
                GD.PrintErr("Falha ao carregar a cena.");
                Disabled = false;
                loadingScreen.Visible = false;
                return;
            }

            await ToSignal(
                arvore,
                SceneTree.SignalName.ProcessFrame
            );
        }

        PackedScene cena =
            ResourceLoader.LoadThreadedGet(caminhoCena) as PackedScene;

        if (cena == null)
        {
            GD.PrintErr("O recurso carregado não é uma PackedScene.");
            Disabled = false;
            loadingScreen.Visible = false;
            return;
        }

        LoadingScreen telaLoading = loadingScreen as LoadingScreen;

        loadingScreen.GetParent().RemoveChild(loadingScreen);
        arvore.Root.AddChild(loadingScreen);

        Error troca = arvore.ChangeSceneToPacked(cena);

        if (troca != Error.Ok)
        {
            telaLoading.QueueFree();
            GD.PrintErr($"Erro ao trocar de cena: {troca}");
            return;
        }

        telaLoading.IniciarFinalizacao();
    }
}