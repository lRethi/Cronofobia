using Godot;
using System;

public partial class cameraBonitaDoFred : Node3D
{
	[Export]
	Node cameraNode;

	Quaternion targetRotation;
	float speed = 5.0f;
	float currentPitch = -15f;
	float currentYaw = 0f;
	float basePitch = -15f;
	float pitchOffset = 0f;
	int pitchState = 0;
	int yawState = 0;

	public override async void _Ready()
	{
		await ToSignal(GetTree(), "process_frame");

		targetRotation = (Quaternion)cameraNode.Call("get_third_person_quaternion");
	}

	public override void _Process(double delta)
	{
		Quaternion current = (Quaternion)cameraNode.Call("get_third_person_quaternion");

		Quaternion result = current.Slerp(targetRotation, (float)delta * speed);

		cameraNode.Call("set_third_person_quaternion", result);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("cam_back"))
		{
			pitchState++;
			pitchState = Mathf.Clamp(pitchState, -1, 1);
			currentPitch = basePitch + pitchState * 90f;
			SetTarget(new Vector3(currentPitch, currentYaw, 0));
		}
		else if (@event.IsActionPressed("cam_left"))
		{
			yawState--;
			currentYaw = yawState * 90f;
			SetTarget(new Vector3(currentPitch, currentYaw, 0));
		}
		else if (@event.IsActionPressed("cam_forward"))
		{
			pitchState--;
			pitchState = Mathf.Clamp(pitchState, -1, 1);
			currentPitch = basePitch + pitchState * 90f;
			SetTarget(new Vector3(currentPitch, currentYaw, 0));
		}
		else if (@event.IsActionPressed("cam_right"))
		{
			yawState++;
			currentYaw = yawState * 90f;
			SetTarget(new Vector3(currentPitch, currentYaw, 0));
		}
	}

	void SetTarget(Vector3 degrees)
	{
		Vector3 rad = new Vector3(
			Mathf.DegToRad(degrees.X),
			Mathf.DegToRad(degrees.Y),
			Mathf.DegToRad(degrees.Z)
		);

		targetRotation = new Quaternion(Basis.FromEuler(rad));
	}
}