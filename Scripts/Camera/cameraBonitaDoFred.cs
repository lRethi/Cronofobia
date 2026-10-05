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

        cameraInicializada = true;

        if (cameraAtiva)
            AtivarCamera();
    }

    public override void _Input(InputEvent @event)
    {
        if (!cameraAtiva || !GameState.cameraInputEnabled || dialogueMode)
            return;

        if (@event is InputEventMouseMotion mouseMotion)
        {
            yawRotation -= mouseMotion.Relative.X * mouseSensitivity;
            UpdateCameraRotation();
        }
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

    public void StartDialogueCamera(DialogueMarker3D npc, float duration)
    {
        if (!IsInstanceValid(npc) || _pcam == null)
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

        float side = relativePosition.X >= 0f ? 1f : -1f;
        float depth = relativePosition.Z >= 0f ? 0.9f : -0.9f;

        dialogueOffset = new Vector3(
            0.65f * side,
            0.1f,
            depth
        );

        dialogueRotation = new Vector3(
            2.5f,
            currentRotation.Y + (5f * side),
            0f
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
}