using UnityEngine;
using System.Collections;
using System;

public class FluidParticle{

	public int Life;
	public float Mass;
	public Vector3 Position;
	public Vector3 PositionOld;
	public Vector3 Velocity;
	public Vector3 Force;
	public float Density;
	public float Pressure;
	public Rect SimDomain;
	public float cellSize;
	public Solver Solver;
	public BoundingVolume boundingVolume;

	public FluidParticle()
	{
		Mass = 1.0f;
		Position = Vector3.zero;
		PositionOld = this.Position;
		Velocity = Vector3.zero;
		Force = Vector3.zero;
		Density = 100f;
		Solver = new VerletSolver ();
		Solver.Dampening = 0.001f;
		boundingVolume = new PointVolume ();
		boundingVolume.Position = this.Position;
		SimDomain = new Rect(.1f, .1f, 6.1f, 6.1f);
		cellSize = (SimDomain.width + SimDomain.height) / 32;
		boundingVolume.Margin = cellSize * 0.25f;

		UpdatePressure ();
	}

	public void UpdatePressure()
	{
		Pressure = 1.205f * (Density - 100.0f);
	}

	public void Update(float dTime)
	{
		Life++;
		Solver.Solve (ref Position, ref PositionOld, ref Velocity, Force, Mass, dTime);
		boundingVolume.Position = Position;
	}

	public override int GetHashCode()
	{
		return (int)(Position.x * 73856093) ^ (int)(Position.y * 19349663 );
	}

	public override bool Equals(object obj) {
		if (obj != null) {
			return obj.GetHashCode().Equals(this.GetHashCode());
		}
		return base.Equals(obj);
	}
}
