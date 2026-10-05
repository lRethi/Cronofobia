using Godot;

public partial class DirectionalSprite : Node3D
{
    [Export]
    private DirectionalSpriteResource directionalTextures;

    [Export]
    private DirectionalSpriteResource parado;

    [Export]
    private DirectionalSpriteAnimation andando;

    [Export]
    private movimentoPerson player;

    [Export]
    public cameraBonitaDoFred cameraScript;

    [Export]
    private Sprite3D sprite;

    [Export]
    public SpriteDirection FacingDirection = SpriteDirection.Front;

    [Export]
    private float animationSpeed = 0.15f;

    private StandardMaterial3D material;
    private ShaderMaterial overlayMaterial;

    private int currentAnimationFrame;
    private double animationTimer;

    private bool usesMovementAnimation;

   public override async void _Ready()
    {
        SetupMaterial();
        SetupOverlay();
        ApplyMaterialSettings();

        usesMovementAnimation =
            parado != null &&
            andando != null &&
            andando.Frames != null &&
            andando.Frames.Length > 0;

        if (!usesMovementAnimation && directionalTextures != null)
            ApplyResourceSettings(directionalTextures);

        if (player != null)
        {
            player.MovementDirectionChanged += OnMovementDirectionChanged;
            player.MovementStateChanged += OnMovementStateChanged;
        }

        await ToSignal(
            GetTree(),
            SceneTree.SignalName.ProcessFrame
        );

        if (cameraScript != null)
            cameraScript.CameraChanged += OnCameraChanged;

        if (cameraScript == null)
            return;

        if (usesMovementAnimation)
            AtualizarSprite();
        else
            atualizarSprite(cameraScript.GetYawState());
    }

    private void EncontrarCamera()
    {
        foreach (Node node in GetTree().GetNodesInGroup("camera_principal"))
        {
            if (node is cameraBonitaDoFred camera)
            {
                cameraScript = camera;
                return;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (!usesMovementAnimation)
            return;

        if (player == null || !player.IsMoving)
            return;

        if (andando == null ||
            andando.Frames == null ||
            andando.Frames.Length == 0)
            return;

        animationTimer += delta;

        if (animationTimer < animationSpeed)
            return;

        animationTimer = 0.0;

        currentAnimationFrame++;

        if (currentAnimationFrame >= andando.Frames.Length)
            currentAnimationFrame = 0;

        AtualizarSprite();
    }

    private void OnCameraChanged(int yaw, int pitch)
    {
        if (usesMovementAnimation)
            AtualizarSprite();
        else
            atualizarSprite(yaw);
    }

    private void OnMovementDirectionChanged(int direction)
    {
        if (!usesMovementAnimation)
            return;

        AtualizarSprite();
    }

    private void OnMovementStateChanged(bool moving)
    {
        if (!usesMovementAnimation)
            return;

        currentAnimationFrame = 0;
        animationTimer = 0.0;

        AtualizarSprite();
    }

    private void AtualizarSprite()
    {
        if (cameraScript == null || player == null)
            return;

        DirectionalSpriteResource resource;

        if (player.IsMoving &&
            andando != null &&
            andando.Frames != null &&
            andando.Frames.Length > 0)
        {
            resource = andando.Frames[currentAnimationFrame];
        }
        else
        {
            resource = parado;
        }

        if (resource == null)
            return;

        ApplyResourceSettings(resource);

        int cameraYaw = cameraScript.GetYawState();
        int movementYaw = (int)player.LastMovedDirection;

        int horizontal = Mathf.PosMod(
            cameraYaw - movementYaw,
            4
        );

        SpriteDirection direction =
            (SpriteDirection)horizontal;

        DirectionalSpriteData data =
            GetSpriteData(resource, direction);

        if (data == null)
            return;

        material.AlbedoTexture = data.Texture;
        material.NormalTexture = data.NormalMap;

        overlayMaterial.SetShaderParameter(
            "albedo_texture",
            data.Texture
        );
    }

    private DirectionalSpriteData GetSpriteData(
        DirectionalSpriteResource resource,
        SpriteDirection direction
    )
    {
        return direction switch
        {
            SpriteDirection.Back => resource.Back,
            SpriteDirection.Right => resource.Right,
            SpriteDirection.Front => resource.Front,
            SpriteDirection.Left => resource.Left,
            _ => null
        };
    }

    private void ApplyResourceSettings(
        DirectionalSpriteResource resource
    )
    {
        if (resource == null)
            return;

        if (overlayMaterial != null)
        {
            overlayMaterial.SetShaderParameter(
                "glow_enabled",
                resource.GlowEnabled
            );

            overlayMaterial.SetShaderParameter(
                "glow_base_color",
                resource.GlowBaseColor
            );

            overlayMaterial.SetShaderParameter(
                "glow_tolerance",
                resource.GlowTolerance
            );

            overlayMaterial.SetShaderParameter(
                "glow_strength",
                resource.GlowStrength
            );
        }

        if (sprite != null)
        {
            sprite.CastShadow = resource.CastShadow
                ? GeometryInstance3D.ShadowCastingSetting.On
                : GeometryInstance3D.ShadowCastingSetting.Off;
        }
    }

    private void SetupOverlay()
    {
        if (sprite == null)
            return;

        overlayMaterial =
            sprite.MaterialOverlay as ShaderMaterial;

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
        if (sprite == null)
            return;

        material =
            sprite.MaterialOverride as StandardMaterial3D;

        if (material == null)
        {
            material = new StandardMaterial3D();
            sprite.MaterialOverride = material;
        }

        material.Transparency =
            BaseMaterial3D.TransparencyEnum.AlphaScissor;

        material.TextureFilter =
            BaseMaterial3D.TextureFilterEnum.Nearest;

        material.CullMode =
            BaseMaterial3D.CullModeEnum.Disabled;

        material.ShadingMode =
            BaseMaterial3D.ShadingModeEnum.PerPixel;

        material.BillboardMode =
            BaseMaterial3D.BillboardModeEnum.FixedY;
    }

    private void ApplyMaterialSettings()
    {
        if (material == null || sprite == null)
            return;

        material.BacklightEnabled = true;
        material.Backlight = Color.FromHtml("#2f2f2f");

        if (directionalTextures != null)
        {
            sprite.CastShadow =
                directionalTextures.CastShadow
                    ? GeometryInstance3D.ShadowCastingSetting.On
                    : GeometryInstance3D.ShadowCastingSetting.Off;
        }
    }

    public void SetSpriteSet(
        DirectionalSpriteResource resource
    )
    {
        if (resource == null ||
            resource == directionalTextures)
            return;

        directionalTextures = resource;

        if (material == null)
            SetupMaterial();

        if (overlayMaterial == null)
            SetupOverlay();

        ApplyMaterialSettings();
        ApplyResourceSettings(resource);

        if (cameraScript != null &&
            !usesMovementAnimation)
        {
            atualizarSprite(
                cameraScript.GetYawState()
            );
        }
    }

    public void atualizarSprite(int cameraYaw)
    {
        if (directionalTextures == null ||
            material == null ||
            overlayMaterial == null)
            return;

        int horizontal = Mathf.PosMod(
            cameraYaw - (int)FacingDirection,
            4
        );

        SpriteDirection direction =
            (SpriteDirection)horizontal;

        DirectionalSpriteData data =
            GetSpriteData(
                directionalTextures,
                direction
            );

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
            cameraScript.CameraChanged -= OnCameraChanged;

        if (player != null)
        {
            player.MovementDirectionChanged -=
                OnMovementDirectionChanged;

            player.MovementStateChanged -=
                OnMovementStateChanged;
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