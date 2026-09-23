using Godot;

public partial class DayNightController : Node3D
{
    private DirectionalLight3D objSol;
    private WorldEnvironment objAmbiente;
    private Environment ambiente;
    private ShaderMaterial skyMaterial;

    [Export] public Color corManha = new Color(1.0f, 0.82f, 0.68f);
    [Export] public Color corDia = new Color(1.0f, 0.96f, 0.86f);
    [Export] public Color corTarde = new Color(1.0f, 0.62f, 0.38f);
    [Export] public Color corCrepusculo = new Color(0.92f, 0.35f, 0.22f);
    [Export] public Color corNoite = new Color(0.16f, 0.19f, 0.34f);

    [Export] public Color corAmbienteManha = new Color(0.52f, 0.50f, 0.46f);
    [Export] public Color corAmbienteDia = new Color(0.58f, 0.57f, 0.53f);
    [Export] public Color corAmbienteTarde = new Color(0.55f, 0.38f, 0.30f);
    [Export] public Color corAmbienteCrepusculo = new Color(0.30f, 0.16f, 0.18f);
    [Export] public Color corAmbienteNoite = new Color(0.075f, 0.085f, 0.15f);

    [Export] public float energiaAmbienteManha = 0.48f;
    [Export] public float energiaAmbienteDia = 0.60f;
    [Export] public float energiaAmbienteTarde = 0.46f;
    [Export] public float energiaAmbienteCrepusculo = 0.30f;
    [Export] public float energiaAmbienteNoite = 0.16f;

    [Export] public float intensidadeSol = 0.85f;
    [Export] public float intervaloShader = 0.05f;
    [Export] public float intervaloAmbiente = 0.10f;

    private float tempoShader;
    private float tempoAmbiente;

    private float ultimoMorningFactor = -1f;
    private float ultimoDayFactor = -1f;
    private float ultimoEveningFactor = -1f;
    private float ultimoTwilightFactor = -1f;
    private float ultimoNightFactor = -1f;
    private float ultimoArtificialFactor = -1f;
    private float ultimaEnergiaSol = -1f;

    public override void _Ready()
    {
        objSol = GetNode<DirectionalLight3D>("DirectionalLight3D_Sun");
        objAmbiente = GetNode<WorldEnvironment>("../WorldEnvironment");

        ambiente = objAmbiente.Environment;
        skyMaterial = ambiente.Sky?.SkyMaterial as ShaderMaterial;

        objSol.LightEnergy = intensidadeSol;
        objSol.LightColor = corManha;
    }

    public override void _Process(double delta)
    {
        float deltaFloat = (float)delta;

        tempoShader -= deltaFloat;
        tempoAmbiente -= deltaFloat;

        if (tempoShader <= 0f)
        {
            tempoShader = intervaloShader;
            AtualizarShader();
        }

        if (tempoAmbiente <= 0f)
        {
            tempoAmbiente = intervaloAmbiente;
            AtualizarAmbiente();
        }
    }

    private void CalcularFatores(
        out float morningFactor,
        out float dayFactor,
        out float eveningFactor,
        out float twilightFactor,
        out float nightFactor,
        out float artificialFactor
    )
    {
        float tempo = Mathf.Clamp(
            TimeState.Instance.tempoNormalizado,
            0f,
            1f
        );

        morningFactor = 1f - Mathf.SmoothStep(
            0.00f,
            0.20f,
            tempo
        );

        dayFactor = 1f - Mathf.SmoothStep(
            0.38f,
            0.70f,
            tempo
        );

        eveningFactor =
            Mathf.SmoothStep(
                0.45f,
                0.68f,
                tempo
            ) *
            (
                1f -
                Mathf.SmoothStep(
                    0.70f,
                    0.84f,
                    tempo
                )
            );

        twilightFactor =
            Mathf.SmoothStep(
                0.66f,
                0.79f,
                tempo
            ) *
            (
                1f -
                Mathf.SmoothStep(
                    0.80f,
                    0.93f,
                    tempo
                )
            );

        nightFactor = Mathf.SmoothStep(
            0.76f,
            0.96f,
            tempo
        );

        artificialFactor = Mathf.SmoothStep(
            0.03f,
            0.86f,
            nightFactor
        );

        artificialFactor = Mathf.Max(
            artificialFactor,
            eveningFactor * 0.32f
        );

        artificialFactor = Mathf.Max(
            artificialFactor,
            twilightFactor * 0.72f
        );

        morningFactor = Mathf.Clamp(
            morningFactor,
            0f,
            1f
        );

        dayFactor = Mathf.Clamp(
            dayFactor,
            0f,
            1f
        );

        eveningFactor = Mathf.Clamp(
            eveningFactor,
            0f,
            1f
        );

        twilightFactor = Mathf.Clamp(
            twilightFactor,
            0f,
            1f
        );

        nightFactor = Mathf.Clamp(
            nightFactor,
            0f,
            1f
        );
    }

    private void AtualizarShader()
    {
        if (skyMaterial == null)
            return;

        CalcularFatores(
            out float morningFactor,
            out float dayFactor,
            out float eveningFactor,
            out float twilightFactor,
            out float nightFactor,
            out float artificialFactor
        );

        DefinirParametro(
            "morning_factor",
            morningFactor,
            ref ultimoMorningFactor
        );

        DefinirParametro(
            "day_factor",
            dayFactor,
            ref ultimoDayFactor
        );

        DefinirParametro(
            "evening_factor",
            eveningFactor,
            ref ultimoEveningFactor
        );

        DefinirParametro(
            "twilight_factor",
            twilightFactor,
            ref ultimoTwilightFactor
        );

        DefinirParametro(
            "night_factor",
            nightFactor,
            ref ultimoNightFactor
        );

        DefinirParametro(
            "artificial_light_factor",
            artificialFactor,
            ref ultimoArtificialFactor
        );

        Vector3 sunDirection =
            -objSol.GlobalTransform.Basis.Z.Normalized();

        skyMaterial.SetShaderParameter(
            "sun_direction",
            sunDirection
        );
    }

    private void DefinirParametro(
        string nome,
        float valor,
        ref float ultimoValor
    )
    {
        if (Mathf.IsEqualApprox(valor, ultimoValor))
            return;

        skyMaterial.SetShaderParameter(
            nome,
            valor
        );

        ultimoValor = valor;
    }

    private void AtualizarAmbiente()
    {
        CalcularFatores(
            out float morningFactor,
            out float dayFactor,
            out float eveningFactor,
            out float twilightFactor,
            out float nightFactor,
            out float artificialFactor
        );

        Color corAmbiente = corAmbienteNoite.Lerp(
            corAmbienteManha,
            1f - nightFactor
        );

        corAmbiente = corAmbiente.Lerp(
            corAmbienteDia,
            dayFactor
        );

        corAmbiente = corAmbiente.Lerp(
            corAmbienteTarde,
            eveningFactor
        );

        corAmbiente = corAmbiente.Lerp(
            corAmbienteCrepusculo,
            twilightFactor
        );

        float energiaAmbiente = Mathf.Lerp(
            energiaAmbienteNoite,
            energiaAmbienteManha,
            1f - nightFactor
        );

        energiaAmbiente = Mathf.Lerp(
            energiaAmbiente,
            energiaAmbienteDia,
            dayFactor
        );

        energiaAmbiente = Mathf.Lerp(
            energiaAmbiente,
            energiaAmbienteTarde,
            eveningFactor
        );

        energiaAmbiente = Mathf.Lerp(
            energiaAmbiente,
            energiaAmbienteCrepusculo,
            twilightFactor
        );

        float energiaSol = Mathf.Lerp(
            intensidadeSol * 0.48f,
            intensidadeSol,
            dayFactor
        );

        energiaSol = Mathf.Lerp(
            energiaSol,
            intensidadeSol * 0.58f,
            eveningFactor
        );

        energiaSol = Mathf.Lerp(
            energiaSol,
            intensidadeSol * 0.24f,
            twilightFactor
        );

        energiaSol = Mathf.Lerp(
            energiaSol,
            intensidadeSol * 0.02f,
            nightFactor
        );

        Color corSol = corAmbienteManha;

        corSol = corSol.Lerp(
            corDia,
            dayFactor
        );

        corSol = corSol.Lerp(
            corTarde,
            eveningFactor
        );

        corSol = corSol.Lerp(
            corCrepusculo,
            twilightFactor
        );

        corSol = corSol.Lerp(
            corNoite,
            nightFactor
        );

        float tempo = Mathf.Clamp(
            TimeState.Instance.tempoNormalizado,
            0f,
            1f
        );

        float anguloSol = Mathf.Lerp(
            -12f,
            252f,
            tempo
        );

        float elevacao = Mathf.Sin(
            Mathf.DegToRad(anguloSol)
        );

        float alturaSol = Mathf.Clamp(
            elevacao * 0.5f + 0.5f,
            0f,
            1f
        );

        float rotacaoX = Mathf.Lerp(
            -18f,
            -72f,
            alturaSol
        );

        float rotacaoY = Mathf.Lerp(
            -35f,
            145f,
            tempo
        );

        objSol.RotationDegrees = new Vector3(
            rotacaoX,
            rotacaoY,
            0f
        );

        if (!Mathf.IsEqualApprox(
            energiaSol,
            ultimaEnergiaSol
        ))
        {
            objSol.LightEnergy = energiaSol;
            ultimaEnergiaSol = energiaSol;
        }

        objSol.LightColor = corSol;

        ambiente.AmbientLightColor =
            corAmbiente;

        ambiente.AmbientLightEnergy =
            energiaAmbiente;
    }
}