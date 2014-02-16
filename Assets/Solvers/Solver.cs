using UnityEngine;
using System.Collections;
using System;

public abstract class Solver {

	public float Dampening;

	public Solver()
	{
		Dampening = 1.0f;
	}

	public abstract void Solve(ref Vector3 position, ref Vector3 positionOld, ref Vector3 velocity, Vector3 acceleration, float timeStep);
	
	public virtual void Solve(ref Vector3 position, ref Vector3 positionOld, ref Vector3 velocity, Vector3 force, float mass, float timeStep) 
	{
		Solve(ref position, ref positionOld, ref velocity, force / mass, timeStep);
	}
}
