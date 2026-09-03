using Godot;

public partial class DayNightController : Node3D
{
    private DirectionalLight3D objSol;
    private DirectionalLight3D objLua;
    private WorldEnvironment objAmbiente;
    private PhysicalSkyMaterial skyMaterial;

    [Export] public Color corNoite = new Color(0.08f, 0.10f, 0.22f);
    [Export] public Color corDia = new Color(0.65f, 0.62f, 0.48f);
	[Export] public Color corCrepusculo = new Color(0.85f, 0.45f, 0.55f);
    [Export] public Color corNascerDoSol = new Color(0.95f, 0.72f, 0.55f);
    [Export] public Color corPorDoSol = new Color(1.0f, 0.65f, 0.45f);

	[Export] public Color corFogHorizonteDia = new Color(0.8f, 0.7f, 0.5f);
	[Export] public Color corFogHorizonteNoite = new Color(0.1f, 0.1f, 0.2f);
	[Export] public Color corFogHorizonteCrepusculo = new Color(0.8f, 0.4f, 0.2f);

	[Export] public float densidadeFogHorizonteDia = 0.07f;
	[Export] public float densidadeFogHorizonteNoite = 0.12f;
	[Export] public float densidadeFogHorizonteCrepusculo = 0.10f;

    [Export] public float intensidadeMaximaSol = 1.5f;
    [Export] public float intensidadeMaximaLua = 0.5f;

    public override void _Ready()
    {
        objSol = GetNode<DirectionalLight3D>("DirectionalLight3D_Sun");
        objLua = GetNode<DirectionalLight3D>("DirectionalLight3D_Moon");
        objAmbiente = GetNode<WorldEnvironment>("../WorldEnvironment");

        skyMaterial = (PhysicalSkyMaterial)objAmbiente.Environment.Sky.SkyMaterial;
    }

    public override void _Process(double delta)
    {
        float tempo = TimeState.Instance.tempoNormalizado;

        float anguloSolar = tempo * 360f - 90f;
        float altitudeSolar = Mathf.Sin(Mathf.DegToRad(anguloSolar));

        float fatorSol = Mathf.SmoothStep(-0.15f, 0.15f, altitudeSolar);
        float fatorLua = Mathf.SmoothStep(0f, 1f, -altitudeSolar);
		float fatorCrepusculo = Mathf.Clamp(1f - Mathf.Abs(altitudeSolar) * 5f, 0f, 1f);
		float intensidadeCrepusculo = Mathf.Pow(fatorCrepusculo, 2f);

        float fatorHorizonte = Mathf.Pow(1f - Mathf.Abs(altitudeSolar), 2f);

        atualizarSol(anguloSolar, fatorSol, intensidadeCrepusculo);
        atualizarLua(anguloSolar, fatorLua, intensidadeCrepusculo);
        atualizarAmbiente(fatorSol, fatorLua, fatorHorizonte, altitudeSolar, intensidadeCrepusculo);
    }

    private void atualizarSol(float anguloSolar, float fatorSol, float fatorCrepusculo)
	{
		objSol.RotationDegrees = new Vector3(anguloSolar + 180f, 0f, 0f);

		objSol.LightEnergy = Mathf.Lerp(0.15f, intensidadeMaximaSol, fatorSol);

		Color cor = corDia.Lerp(corNascerDoSol, fatorCrepusculo * 0.6f);

		objSol.LightColor = cor;
	}

    private void atualizarLua(float anguloSolar, float fatorLua, float fatorCrepusculo)
    {
        objLua.RotationDegrees = new Vector3(anguloSolar, 0f, 0f);

        objLua.LightEnergy = Mathf.Lerp(0.15f, intensidadeMaximaLua, fatorLua);

		Color cor = corPorDoSol.Lerp(corNoite, fatorLua);

        objLua.LightColor = cor;
    }

    private void atualizarAmbiente(float fatorSol, float fatorLua, float fatorHorizonte, float altitudeSolar, float fatorCrepusculo)
    {
        Environment ambiente = objAmbiente.Environment;

        Color corBase = corNoite.Lerp(corDia, fatorSol);

		corBase = corBase.Lerp(corCrepusculo, fatorCrepusculo * 0.7f);

        ambiente.AmbientLightColor = corBase;

        float energiaAmbiente = 0.15f + fatorSol * 0.85f + fatorLua * 0.15f;
		
		ambiente.AmbientLightEnergy = energiaAmbiente;

        float brilhoCeu = Mathf.Clamp(fatorSol * 0.9f + fatorHorizonte * 0.25f, 0.15f, 1f);

		skyMaterial.EnergyMultiplier = brilhoCeu;

		Color corFog = corFogHorizonteNoite.Lerp(corFogHorizonteDia, fatorSol);
		corFog = corFog.Lerp(corFogHorizonteCrepusculo, fatorCrepusculo);
		ambiente.FogLightColor = corFog;

		float densidadeFog = Mathf.Lerp(densidadeFogHorizonteNoite, densidadeFogHorizonteDia, fatorSol);
		densidadeFog = Mathf.Lerp(densidadeFog, densidadeFogHorizonteCrepusculo, fatorCrepusculo);
		ambiente.FogDensity = densidadeFog;
    }
}