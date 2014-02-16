using UnityEngine;
using System.Collections;
using System;

public class ParticleSystem{

	private bool wasMaxReached;
	public ArrayList Particles;
	public ArrayList Emitters;

	public bool HasEmitters
	{
		get{ return Emitters != null && Emitters.Count > 0;}
	}

	public ArrayList Consumers;

	public bool HasConsumers
	{
		get{ return Consumers != null && Consumers.Count > 0;}
	}

	public int MaxLife;
	public int MaxParticles;
	public bool DoRebirth;

	public ParticleSystem()
	{
		Emitters = new ArrayList ();
		Consumers = new ArrayList ();
		MaxLife = 1024;
		MaxParticles = 500;
		DoRebirth = true;
		Reset ();
	}
	public void Reset()
	{
		Particles = new ArrayList (MaxParticles);
		wasMaxReached = false;
	}

	public void Update(double dTime)
	{
		if(this.HasConsumers)
		{
			for(int i = 0; i < Consumers.Count;i++)
			{
				FluidParticleConsumer consumer = (FluidParticleConsumer)Consumers[i];
				consumer.Consume (ref Particles);
			}
		}

		if (wasMaxReached && !DoRebirth) 
		{

		} else if (this.Particles.Count < this.MaxParticles) 
		{
			for(int i = 0; i < Emitters.Count;i++)
			{
				FluidParticleEmitter emitter = (FluidParticleEmitter) Emitters[i];
				emitter.Emit(ref Particles,dTime);
			}
		} 
		else 
		{
			wasMaxReached = true;
		}
	}

	public static ArrayList Create(int nParticles, float cellSpace, Rect domain, float particleMass)
	{
		ArrayList particles = new ArrayList (nParticles);
		float x0 = domain.x + cellSpace;
		float x = x0;
		float y = domain.y;
		float z = domain.width;
		float z0 = domain.width + cellSpace;
		float z0m = -domain.width + cellSpace;
		for(int i =0; i < nParticles;i++)
		{
			if(x == x0)
			{
				y += cellSpace;
			}

			if(z == z0 || z == z0m)
			{
				x += cellSpace;
			}
			
			Vector3 pos = new Vector3(x,y,z);
			FluidParticle p = new FluidParticle();
			p.Position = pos;
			p.PositionOld = pos;
			p.Mass = particleMass;
			particles.Add (p);
			x = x + cellSpace < domain.width ? x + cellSpace : x0;
		}
		return particles;
	}
}
