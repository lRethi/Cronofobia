using Godot;

public partial class TesteHominho : CharacterBody3D
{
    [Export] public float Speed = 0.6f;
    [Export] public float Gravity = 2.0f;

    [Export] public Node3D Camera;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 v = Velocity;

        if (!IsOnFloor())
            v.Y -= Gravity * (float)delta;

        Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

        Vector3 forward = Camera.Transform.Basis.Z;
        Vector3 right = Camera.Transform.Basis.X;

        forward.Y = 0;
        right.Y = 0;

        forward = forward.Normalized();
        right = right.Normalized();

        Vector3 direction = (right * input.X + forward * input.Y);

        v.X = direction.X * Speed;
        v.Z = direction.Z * Speed;

        Velocity = v;
        MoveAndSlide();
    }
}