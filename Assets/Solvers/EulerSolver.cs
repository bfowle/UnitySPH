using UnityEngine;
using System.Collections;

public class EulerSolver : Solver {

	public EulerSolver()
	{

	}

	public override void Solve(ref Vector3 position, ref Vector3 positionOld, ref Vector3 velocity, Vector3 acceleration, float timeStep) 
	{
		Vector3 t;

		positionOld = position;
		t = velocity * timeStep;
		t = t * (1.0f - Dampening);
		position = position + t;
		t = acceleration * timeStep;
		velocity = velocity + t;
	}

}
