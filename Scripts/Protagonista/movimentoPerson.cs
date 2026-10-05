using Godot;

public partial class movimentoPerson : CharacterBody3D
{
    public const float baseSpeed = 6f;
    public float tempSpeed = 0f;
    public float sceneSpeed = 0f;
    public const float JumpVelocity = 5f;

    [Export] public Node cameraNode;
    [Export] public Node3D cameraPivotNode;

    private cameraBonitaDoFred cameraScript;

    public movementDirection LastMovedDirection { get; private set; } = movementDirection.Front;
    public bool IsMoving { get; private set; }

    [Signal]
    public delegate void MovementDirectionChangedEventHandler(int direction);

    [Signal]
    public delegate void MovementStateChangedEventHandler(bool moving);

    public override void _Ready()
    {
        cameraScript = cameraPivotNode as cameraBonitaDoFred;
    }

    public override void _PhysicsProcess(double delta)
    {
        float Speed = baseSpeed + tempSpeed + sceneSpeed;

        Vector3 velocity = Velocity;

        if (GameState.TempoCongelado)
        {
            if (!IsOnFloor())
                velocity += GetGravity() * (float)delta;

            velocity.X = 0;
            velocity.Z = 0;

            SetMovingState(false);

            Velocity = velocity;
            MoveAndSlide();
            return;
        }

        if (!IsOnFloor())
            velocity += GetGravity() * (float)delta;

        Vector2 inputDir = Input.GetVector(
            "move_left",
            "move_right",
            "move_forward",
            "move_back"
        );

        if (cameraNode == null || cameraScript == null)
        {
            SetMovingState(false);
            Velocity = velocity;
            MoveAndSlide();
            return;
        }

        float yaw = Mathf.DegToRad(cameraScript.GetCameraYaw());

        Vector3 forward = new Vector3(
            Mathf.Sin(yaw),
            0,
            Mathf.Cos(yaw)
        );

        Vector3 right = new Vector3(
            Mathf.Cos(yaw),
            0,
            -Mathf.Sin(yaw)
        );

        Vector3 direction =
            (forward * inputDir.Y + right * inputDir.X).Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

            SetMovingState(true);
            SetMovementDirection(GetMovementDirection(direction));
        }
        else
        {
            velocity.X = Mathf.MoveToward(
                velocity.X,
                0,
                Speed
            );

            velocity.Z = Mathf.MoveToward(
                velocity.Z,
                0,
                Speed
            );

            SetMovingState(false);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private movementDirection GetMovementDirection(Vector3 direction)
    {
        if (Mathf.Abs(direction.Z) > Mathf.Abs(direction.X))
        {
            return direction.Z < 0
                ? movementDirection.Front
                : movementDirection.Back;
        }

        return direction.X > 0
            ? movementDirection.Right
            : movementDirection.Left;
    }

    private void SetMovementDirection(movementDirection direction)
    {
        if (LastMovedDirection == direction)
            return;

        LastMovedDirection = direction;

        EmitSignal(
            SignalName.MovementDirectionChanged,
            (int)direction
        );
    }

    private void SetMovingState(bool moving)
    {
        if (IsMoving == moving)
            return;

        IsMoving = moving;

        EmitSignal(
            SignalName.MovementStateChanged,
            moving
        );
    }
}

public enum movementDirection
{
    Back = 0,
    Right = 1,
    Front = 2,
    Left = 3
}