using Godot;
using System;

public partial class dayAndNightCycle : DirectionalLight3D
{
	public override void _Process(double delta)
    {
        float t = TimeState.Instance.tempoNormalizado;

        float angle = t * 360f;

        RotationDegrees = new Vector3(angle - 90f, 0f, 0f);
    }
}
