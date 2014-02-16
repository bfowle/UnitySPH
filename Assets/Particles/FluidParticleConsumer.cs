using UnityEngine;
using System.Collections;
using System;

public class FluidParticleConsumer {

	private float radiusSquared;
	public Vector3 Position;

	public float Radius
	{
		get { return (float)Math.Sqrt (radiusSquared);}
		set
		{
			radiusSquared = value * value;
		}
	}

	public bool Enabled;

	public FluidParticleConsumer()
	{
		Position = Vector3.zero;
		Radius = 1.0f;
		Enabled = true;
	}

	public void Consume(ref ArrayList particles)
	{
		if(this.Enabled)
		{
			for(int i = particles.Count - 1; i >= 0;i--)
			{
				float distSq = (((FluidParticle)particles[i]).Position - this.Position).sqrMagnitude;

				if(distSq < radiusSquared)
				{
					particles.RemoveAt(i);
				}
			}
		}
	}
}
