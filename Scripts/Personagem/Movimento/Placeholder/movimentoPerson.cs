using Godot;
using System;

public partial class movimentoPerson : CharacterBody3D
{
	public const float Speed = 3f;
	public const float JumpVelocity = 5f;

	[Export] public Node cameraNode;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		if (GameState.EmDialogo)
		{
			// continua aplicando gravidade
			if (!IsOnFloor())
				velocity += GetGravity() * (float)delta;
			// trava mov horizontal
			velocity.X = 0;
			velocity.Z = 0;

			Velocity = velocity;
			MoveAndSlide();
			return;
		}

		if (!IsOnFloor())
			velocity += GetGravity() * (float)delta;

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

		if (cameraNode == null)
		{
			Velocity = velocity;
			MoveAndSlide();
			return;
		}

		// pega a base da câmera
		var camTransform = (Transform3D)cameraNode.Get("global_transform");
		Basis camBasis = camTransform.Basis;

		// transforma as direções baseaando nela, pra sempre ficar consistente com a posição da câmera
		Vector3 forward = camBasis.Z;
		Vector3 right = camBasis.X;

		forward.Y = 0;
		right.Y = 0;

		forward = forward.Normalized();
		right = right.Normalized();

		// dir final
		Vector3 direction = (forward * inputDir.Y + right * inputDir.X).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
