using Godot;
using PhantomCamera;
using DialogueManagerRuntime;

public partial class cameraBonitaDoFred : Node3D
{
    private Node3D _pcamNode;
    private PhantomCamera3D _pcam;
    private Tween cameraTween;

    [Signal]
    public delegate void CameraChangedEventHandler(int yaw, int pitch);

    [Export]
    private Node3D player;

    [Export]
    private float mouseSensitivity = 0.15f;

    [Export]
    private float controllerSensitivity = 180f;

    private readonly float basePitch = 0f;
    private float yawRotation = 0f;
    private int lastYawState;

    private Vector3 currentRotation;
    private Vector3 currentOffset;

    private bool dialogueMode = false;
    private bool cameraInicializada = false;
    private bool cameraAtiva = false;

    private Vector3 dialogueRotation;
    private Vector3 dialogueOffset;
    private Vector3 savedRotation;
    private Vector3 savedOffset;

    [Export] private uint dialogueCollisionMask = 6;
    [Export] private float dialogueCollisionMargin = 0.15f;

    public float GetCameraYaw()
    {
        return yawRotation;
    }

    public int GetYawState()
    {
        return Mathf.PosMod(
            Mathf.RoundToInt(yawRotation / 90f),
            4
        );
    }

    public override async void _Ready()
    {
        AddToGroup("camera_principal");

        _pcamNode = GetNode<Node3D>("%PhantomCamera3D");
        _pcam = _pcamNode.AsPhantomCamera3D();

        if (player == null)
            player = GetNode<Node3D>("%charGeraldoSalvador");

        _pcam.Priority = 0;

        await ToSignal(
            GetTree(),
            SceneTree.SignalName.ProcessFrame
        );

        if (_pcam == null)
            return;

        currentRotation = _pcam.GetThirdPersonRotationDegrees();
        currentOffset = _pcam.FollowOffset;

        yawRotation = currentRotation.Y;
        lastYawState = GetYawState();

        GameState.Instance.SetCameraInputEnabled(true);
        GameState.Instance.SetCameraMouseCaptured(true);

        _pcamNode.Set("collision_mask", (int)dialogueCollisionMask);
        _pcamNode.Set("margin", dialogueCollisionMargin);

        cameraInicializada = true;

        if (cameraAtiva)
            AtivarCamera();
    }

    public override void _Input(InputEvent @event)
    {
        if (!cameraAtiva ||
            !GameState.cameraInputEnabled ||
            dialogueMode)
        {
            return;
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            yawRotation -=
                mouseMotion.Relative.X *
                mouseSensitivity;

            UpdateCameraRotation();
        }
    }

    public override void _Process(double delta)
    {
        if (!cameraAtiva ||
            !cameraInicializada ||
            !GameState.cameraInputEnabled ||
            dialogueMode ||
            _pcam == null)
        {
            return;
        }

        float cameraInput = Input.GetAxis(
            "cam_left",
            "cam_right"
        );

        if (Mathf.Abs(cameraInput) < 0.01f)
            return;

        yawRotation -=
            cameraInput *
            controllerSensitivity *
            (float)delta;

        UpdateCameraRotation();
    }

    public void AtivarCamera()
    {
        cameraAtiva = true;

        if (!cameraInicializada || _pcam == null)
            return;

        ProcessMode = Node.ProcessModeEnum.Pausable;
        SetProcessInput(true);

        _pcam.Priority = 100;

        GD.Print(
            "ATIVANDO PCAM | ",
            _pcamNode.GetPath(),
            " | Priority: ",
            _pcam.Priority,
            " | IsActive: ",
            _pcam.IsActive
        );

        GameState.Instance.SetCameraInputEnabled(true);
        GameState.Instance.SetCameraMouseCaptured(true);
    }

    public void DesativarCamera()
    {
        cameraAtiva = false;

        SetProcessInput(false);

        if (_pcam == null)
            return;

        _pcam.Priority = 0;

        GD.Print(
            "DESATIVANDO PCAM | ",
            _pcamNode.GetPath(),
            " | Priority: ",
            _pcam.Priority,
            " | IsActive: ",
            _pcam.IsActive
        );
    }

    public void StartDialogueCamera(
    DialogueMarker3D npc,
    float duration
    )
    {
        if (!IsInstanceValid(npc) || _pcam == null || player == null)
            return;

        if (!dialogueMode)
        {
            savedRotation = currentRotation;
            savedOffset = currentOffset;
            dialogueMode = true;

            GameState.Instance.SetCameraInputEnabled(false);
        }

        cameraTween?.Kill();

        Vector3 relativePosition =
            player.GlobalTransform.Basis.Inverse() *
            (npc.GlobalPosition - player.GlobalPosition);

        float side =
            relativePosition.X >= 0f
                ? 1f
                : -1f;

        float depth =
            relativePosition.Z >= 0f
                ? 0.9f
                : -0.9f;

        Vector3 desiredLocalOffset = new Vector3(
            0.65f * side,
            0.1f,
            depth
        );

        Vector3 desiredWorldOffset =
            player.GlobalTransform.Basis * desiredLocalOffset;

        Vector3 desiredRotation = new Vector3(
            2.5f,
            savedRotation.Y + (5f * side),
            0f
        );

        GetSafeDialoguePose(
            desiredWorldOffset,
            desiredRotation,
            out dialogueOffset,
            out dialogueRotation
        );

        Vector3 startRotation = currentRotation;
        Vector3 startOffset = currentOffset;

        cameraTween = CreateTween();
        cameraTween.SetParallel(true);

        cameraTween.TweenMethod(
            Callable.From<Vector3>(rotation =>
            {
                currentRotation = rotation;
                _pcam.SetThirdPersonRotationDegrees(rotation);
            }),
            startRotation,
            dialogueRotation,
            duration
        );

        cameraTween.TweenMethod(
            Callable.From<Vector3>(offset =>
            {
                currentOffset = offset;
                _pcam.FollowOffset = offset;
            }),
            startOffset,
            dialogueOffset,
            duration
        );

        cameraTween.SetEase(Tween.EaseType.InOut);
        cameraTween.SetTrans(Tween.TransitionType.Cubic);

        cameraTween.Finished += () =>
        {
            currentRotation = dialogueRotation;
            currentOffset = dialogueOffset;
        };
    }

    public void EndDialogueCamera(float duration)
    {
        if (_pcam == null)
            return;

        cameraTween?.Kill();

        Vector3 startRotation = currentRotation;
        Vector3 startOffset = currentOffset;

        cameraTween = CreateTween();
        cameraTween.SetParallel(true);

        cameraTween.TweenMethod(
            Callable.From<Vector3>(rotation =>
            {
                currentRotation = rotation;
                _pcam.SetThirdPersonRotationDegrees(rotation);
            }),
            startRotation,
            savedRotation,
            duration
        );

        cameraTween.TweenMethod(
            Callable.From<Vector3>(offset =>
            {
                currentOffset = offset;
                _pcam.FollowOffset = offset;
            }),
            startOffset,
            savedOffset,
            duration
        );

        cameraTween.SetEase(Tween.EaseType.InOut);
        cameraTween.SetTrans(Tween.TransitionType.Cubic);

        cameraTween.Finished += () =>
        {
            currentRotation = savedRotation;
            currentOffset = savedOffset;

            yawRotation = savedRotation.Y;

            dialogueMode = false;

            GameState.Instance.SetCameraInputEnabled(true);
            GameState.Instance.SetCameraMouseCaptured(true);
        };
    }

    private void UpdateCameraRotation()
    {
        currentRotation = new Vector3(
            basePitch,
            yawRotation,
            0f
        );

        if (_pcam == null)
            return;

        _pcam.SetThirdPersonRotationDegrees(currentRotation);

        int newYawState = GetYawState();

        if (newYawState != lastYawState)
        {
            lastYawState = newYawState;

            EmitSignal(
                SignalName.CameraChanged,
                newYawState,
                0
            );
        }
    }

    private void GetSafeDialoguePose(
    Vector3 desiredWorldOffset,
    Vector3 desiredRotation,
    out Vector3 safeOffset,
    out Vector3 safeRotation
    )
    {
        Vector3 initialOffset = currentOffset;
        Vector3 initialRotation = currentRotation;

        float springLength = _pcamNode.Get("spring_length").AsSingle();

        for (int attempt = 0; attempt < 7; attempt++)
        {
            float factor = Mathf.Pow(0.5f, attempt);

            Vector3 candidateOffset =
                initialOffset.Lerp(desiredWorldOffset, factor);

            Vector3 candidateRotation =
                initialRotation.Lerp(desiredRotation, factor);

            if (IsDialogueTransitionSafe(
                initialOffset,
                initialRotation,
                candidateOffset,
                candidateRotation,
                springLength))
            {
                safeOffset = candidateOffset;
                safeRotation = candidateRotation;
                return;
            }
        }

        safeOffset = initialOffset;
        safeRotation = initialRotation;
    }

    private bool IsDialogueTransitionSafe(
        Vector3 initialOffset,
        Vector3 initialRotation,
        Vector3 candidateOffset,
        Vector3 candidateRotation,
        float springLength
    )
    {
        Vector3 playerPosition = player.GlobalPosition;

        Vector3 previousAnchor =
            playerPosition + initialOffset;

        for (int sample = 1; sample <= 5; sample++)
        {
            float t = sample / 5f;

            Vector3 offset =
                initialOffset.Lerp(candidateOffset, t);

            Vector3 rotation =
                initialRotation.Lerp(candidateRotation, t);

            Vector3 anchor =
                playerPosition + offset;

            if (!IsRayClear(previousAnchor, anchor))
                return false;

            Basis cameraBasis = Basis.FromEuler(
                rotation * Mathf.DegToRad(1f)
            );

            Vector3 cameraPosition =
                anchor + cameraBasis.Z * springLength;

            if (!IsRayClear(anchor, cameraPosition))
                return false;

            previousAnchor = anchor;
        }

        return true;
    }

    private bool IsRayClear(Vector3 from, Vector3 to)
    {
        if (from.DistanceSquaredTo(to) < 0.0001f)
            return true;

        PhysicsRayQueryParameters3D query =
            PhysicsRayQueryParameters3D.Create(from, to);

        query.CollisionMask = dialogueCollisionMask;
        query.CollideWithAreas = false;
        query.CollideWithBodies = true;

        if (player is CollisionObject3D playerCollider)
        {
            query.Exclude = new Godot.Collections.Array<Rid>
            {
                playerCollider.GetRid()
            };
        }

        var result =
            GetWorld3D().DirectSpaceState.IntersectRay(query);

        return result.Count == 0;
    }
}