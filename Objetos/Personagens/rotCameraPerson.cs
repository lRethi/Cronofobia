using Godot;
using System;

public partial class rotCameraPerson : Sprite3D
{
	ShaderMaterial mat;
	Camera3D camera;

	[Export] CharacterBody3D personagem;

	public override void _Ready()
	{
		// pega a câmera da viewport
		camera = GetViewport().GetCamera3D();

		// duplica material se existir
		if (MaterialOverlay != null)
		{
			MaterialOverlay = MaterialOverlay.Duplicate() as Material;
			mat = MaterialOverlay as ShaderMaterial;
		}
	}

	public override void _Process(double delta)
	{
		// checagem de segurança
		if (personagem == null || camera == null || mat == null)
			return;

		Vector3 distanciaPersonNPC = personagem.GlobalPosition - GlobalPosition;

		if (distanciaPersonNPC.Length() <= 0.6f)
		{
			// olha pro personagem
			Vector3 direcaoPerson = personagem.GlobalPosition - GlobalPosition;
			OlharParaAlvo(direcaoPerson);

			if (distanciaPersonNPC.Length() <= 0.3f)
			{
				mat.SetShaderParameter("enable_outline", true);
			}
			else
			{
				mat.SetShaderParameter("enable_outline", false);
			}
		}
		else
		{
			// olha pra câmera
			Vector3 direcaoCamera = camera.GlobalPosition - GlobalPosition;
			OlharParaAlvo(direcaoCamera);
		}
	}

	public void OlharParaAlvo(Vector3 alvoDir)
	{
		alvoDir.Y = 0;
		alvoDir = -alvoDir;

		if (GetParent() is Node3D parentNode)
		{
			parentNode.LookAt(parentNode.GlobalPosition + alvoDir, Vector3.Up);
		}
	}
}