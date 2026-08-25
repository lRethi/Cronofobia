using Godot;

public partial class DirectionalSprite : StaticBody3D
{
    [Export]
    private DirectionalSpriteResource directionalTextures;

    private cameraBonitaDoFred cameraScript;

    [Export]
    private Sprite3D sprite;

    [Export]
    public SpriteDirection FacingDirection = SpriteDirection.Front;

    private StandardMaterial3D material;
    private ShaderMaterial overlayMaterial;

    public override void _Ready()
	{
		SetupMaterial();
		SetupOverlay();

		cameraScript = GetNode<cameraBonitaDoFred>("../CameraPivot");
		cameraScript.CameraChanged += atualizarSprite;

		ApplyMaterialSettings();
		ApplyOverlaySettings();

		atualizarSprite(
			cameraScript.GetYawState(),
			cameraScript.GetPitchState()
		);
	}

    private void SetupOverlay()
    {
        overlayMaterial = sprite.MaterialOverlay as ShaderMaterial;

        if (overlayMaterial == null)
        {
            overlayMaterial = new ShaderMaterial();

            Shader shader = GD.Load<Shader>(
                "res://Scripts/Shaders/objShine.gdshader"
            );

            overlayMaterial.Shader = shader;
            sprite.MaterialOverlay = overlayMaterial;
        }
    }

	public void SetGlowEnabled(bool enabled)
	{
		if (overlayMaterial == null)
			return;

		overlayMaterial.SetShaderParameter(
			"glow_enabled",
			enabled
		);
	}

    private void SetupMaterial()
    {
        material = sprite.MaterialOverride as StandardMaterial3D;

        if (material == null)
        {
            material = new StandardMaterial3D();
            sprite.MaterialOverride = material;
        }

        material.Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor;
        material.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
        material.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.BillboardMode = BaseMaterial3D.BillboardModeEnum.FixedY;
    }

        private void ApplyOverlaySettings()
    {
        overlayMaterial.SetShaderParameter(
            "glow_enabled",
            directionalTextures.GlowEnabled
        );

        overlayMaterial.SetShaderParameter(
            "glow_base_color",
            directionalTextures.GlowBaseColor
        );

        overlayMaterial.SetShaderParameter(
            "glow_tolerance",
            directionalTextures.GlowTolerance
        );

        overlayMaterial.SetShaderParameter(
            "glow_strength",
            directionalTextures.GlowStrength
        );
    }

    private DirectionalSpriteData GetSpriteData(SpriteDirection direction)
    {
        return direction switch
        {
            SpriteDirection.Back => directionalTextures.Back,
            SpriteDirection.Right => directionalTextures.Right,
            SpriteDirection.Front => directionalTextures.Front,
            SpriteDirection.Left => directionalTextures.Left,
            _ => null
        };
    }

    public void atualizarSprite(int cameraYaw, int cameraPitch)
    {
        int horizontal = (cameraYaw - (int)FacingDirection + 4) % 4;
        SpriteDirection direction = (SpriteDirection)horizontal;

        DirectionalSpriteData data = GetSpriteData(direction);

        if (data == null)
            return;

        material.AlbedoTexture = data.Texture;
        material.NormalTexture = data.NormalMap;

        overlayMaterial.SetShaderParameter(
            "albedo_texture",
            data.Texture
        );
    }

    public override void _ExitTree()
    {
        if (cameraScript != null)
        {
            cameraScript.CameraChanged -= atualizarSprite;
        }
    }

    private void ApplyMaterialSettings()
    {
        material.BacklightEnabled = true;
        material.Backlight = Color.FromHtml("#2f2f2f");

        sprite.CastShadow = directionalTextures.CastShadow
            ? GeometryInstance3D.ShadowCastingSetting.On
            : GeometryInstance3D.ShadowCastingSetting.Off;
    }
    public void SetSpriteSet(DirectionalSpriteResource resource)
	{
		if (resource == null || resource == directionalTextures)
			return;

		directionalTextures = resource;

		if (material == null)
			SetupMaterial();

		if (overlayMaterial == null)
			SetupOverlay();

		ApplyMaterialSettings();
		ApplyOverlaySettings();

		if (cameraScript != null)
		{
			atualizarSprite(
				cameraScript.GetYawState(),
				cameraScript.GetPitchState()
			);
		}
	}
}
public enum SpriteDirection
{
    Back = 0,
    Right = 1,
    Front = 2,
    Left = 3 
}

public enum SpritePitch
{
    Up = 0,
    Middle = 1,
    Down = 2
}

