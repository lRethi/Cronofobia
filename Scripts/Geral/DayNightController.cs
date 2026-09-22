using Godot;

public partial class DayNightController : Node3D
{
    private DirectionalLight3D objSol;
    private WorldEnvironment objAmbiente;
    private Environment ambiente;
    private ShaderMaterial skyMaterial;

    [Export] public Color corDia = new Color(0.78f, 0.76f, 0.70f);
    [Export] public Color corNoite = new Color(0.08f, 0.10f, 0.22f);
    [Export] public Color corCrepusculo = new Color(0.85f, 0.45f, 0.55f);

    [Export] public Color corAmbienteDia = new Color(0.48f, 0.48f, 0.46f);
    [Export] public Color corAmbienteNoite = new Color(0.07f, 0.08f, 0.14f);
    [Export] public Color corAmbienteCrepusculo = new Color(0.52f, 0.28f, 0.24f);

    [Export] public float energiaAmbienteDia = 0.55f;
    [Export] public float energiaAmbienteNoite = 0.20f;
    [Export] public float energiaAmbienteCrepusculo = 0.40f;

    [Export] public float intensidadeSol = 0.75f;

    [Export] public float intervaloShader = 0.05f;
    [Export] public float intervaloAmbiente = 0.10f;

    private float tempoShader;
    private float tempoAmbiente;

    private float ultimoDayFactor = -1f;
    private float ultimoNightFactor = -1f;
    private float ultimoTwilightFactor = -1f;
    private float ultimoArtificialFactor = -1f;
    private float ultimaEnergiaSol = -1f;

    public override void _Ready()
    {
        objSol = GetNode<DirectionalLight3D>("DirectionalLight3D_Sun");

        objAmbiente = GetNode<WorldEnvironment>("../WorldEnvironment");
        ambiente = objAmbiente.Environment;
        skyMaterial = ambiente.Sky?.SkyMaterial as ShaderMaterial;

        objSol.LightEnergy = intensidadeSol;
        objSol.LightColor = corDia;
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
        out float dayFactor,
        out float nightFactor,
        out float twilightFactor,
        out float artificialFactor
    )
    {
        float tempo = TimeState.Instance.tempoNormalizado;

        float anguloSolar = tempo * 360f - 90f;
        float altitudeSolar = Mathf.Sin(Mathf.DegToRad(anguloSolar));

        float diaNormalizado = Mathf.Clamp(
            (altitudeSolar + 0.25f) / 1.25f,
            0f,
            1f
        );

        dayFactor = Mathf.SmoothStep(
            0f,
            1f,
            diaNormalizado
        );

        float noiteNormalizada = Mathf.Clamp(
            (0.10f - altitudeSolar) / 0.55f,
            0f,
            1f
        );

        nightFactor = Mathf.SmoothStep(
            0f,
            1f,
            noiteNormalizada
        );

        float proximidadeCrepusculo =
            1.0f -
            Mathf.Clamp(
                Mathf.Abs(altitudeSolar) / 0.42f,
                0f,
                1f
            );

        twilightFactor = Mathf.SmoothStep(
            0f,
            1f,
            proximidadeCrepusculo
        );

        twilightFactor *= Mathf.Clamp(
            1f - dayFactor * 0.55f,
            0f,
            1f
        );

        twilightFactor *= Mathf.Clamp(
            1f - nightFactor * 0.55f,
            0f,
            1f
        );

        artificialFactor = Mathf.SmoothStep(
            0.10f,
            0.72f,
            nightFactor
        );

        artificialFactor = Mathf.Max(
            artificialFactor,
            twilightFactor * 0.32f
        );
    }

    private void AtualizarShader()
    {
        if (skyMaterial == null)
            return;

        CalcularFatores(
            out float dayFactor,
            out float nightFactor,
            out float twilightFactor,
            out float artificialFactor
        );

        if (!Mathf.IsEqualApprox(dayFactor, ultimoDayFactor))
        {
            skyMaterial.SetShaderParameter(
                "day_factor",
                dayFactor
            );

            ultimoDayFactor = dayFactor;
        }

        if (!Mathf.IsEqualApprox(nightFactor, ultimoNightFactor))
        {
            skyMaterial.SetShaderParameter(
                "night_factor",
                nightFactor
            );

            ultimoNightFactor = nightFactor;
        }

        if (!Mathf.IsEqualApprox(twilightFactor, ultimoTwilightFactor))
        {
            skyMaterial.SetShaderParameter(
                "twilight_factor",
                twilightFactor
            );

            ultimoTwilightFactor = twilightFactor;
        }

        if (!Mathf.IsEqualApprox(artificialFactor, ultimoArtificialFactor))
        {
            skyMaterial.SetShaderParameter(
                "artificial_light_factor",
                artificialFactor
            );

            ultimoArtificialFactor = artificialFactor;
        }
    }

    private void AtualizarAmbiente()
    {
        CalcularFatores(
            out float dayFactor,
            out float nightFactor,
            out float twilightFactor,
            out float artificialFactor
        );

        Color corAmbiente = corAmbienteNoite.Lerp(
            corAmbienteDia,
            dayFactor
        );

        corAmbiente = corAmbiente.Lerp(
            corAmbienteCrepusculo,
            twilightFactor
        );

        float energiaAmbiente = Mathf.Lerp(
            energiaAmbienteNoite,
            energiaAmbienteDia,
            dayFactor
        );

        energiaAmbiente = Mathf.Lerp(
            energiaAmbiente,
            energiaAmbienteCrepusculo,
            twilightFactor
        );

        float energiaSol = Mathf.Lerp(
            intensidadeSol * 0.55f,
            intensidadeSol,
            dayFactor
        );

        energiaSol = Mathf.Lerp(
            energiaSol,
            intensidadeSol * 0.20f,
            twilightFactor
        );

        energiaSol *= Mathf.Lerp(
            1.0f,
            0.05f,
            nightFactor
        );

        if (!Mathf.IsEqualApprox(
            energiaSol,
            ultimaEnergiaSol
        ))
        {
            objSol.LightEnergy = energiaSol;
            ultimaEnergiaSol = energiaSol;
        }

        ambiente.AmbientLightColor = corAmbiente;
        ambiente.AmbientLightEnergy = energiaAmbiente;
    }
}