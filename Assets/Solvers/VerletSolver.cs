using UnityEngine;
using System.Collections;

public class VerletSolver : Solver {

	public VerletSolver()
	{

	}

	public override void Solve (ref Vector3 position, ref Vector3 positionOld, ref Vector3 velocity, Vector3 acceleration, float timeStep)
	{
		Vector3 t;

		Vector3 oldPos = position;

		acceleration = acceleration * (timeStep * timeStep);
		t = position - positionOld;
		t = t * (1.0f - Dampening);
		t = t + acceleration;
		position = position + t;
		positionOld = oldPos;
		t = position - positionOld;
		velocity = t / timeStep;
	}
}
