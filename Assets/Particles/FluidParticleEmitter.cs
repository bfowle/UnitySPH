using UnityEngine;
using System.Collections;
using System;

public class FluidParticleEmitter {

	private UnityEngine.Random randGen;
	private Vector3 m_direction;
	private double m_time;

	public Vector3 Position;
	public Vector3 Direction
	{
		get { return m_direction;}
		set
		{
			m_direction = value;
			Vector3 dir = new Vector3(m_direction.x,m_direction.y,m_direction.z);
			dir.Normalize();
			m_direction = new Vector3(dir.x,dir.y,dir.z);
		}
	}

	public float Distribution;
	public float VelocityMin;
	public float VelocityMax;
	public double Frequency;
	public float ParticleMass;
	public bool Enabled;

	public FluidParticleEmitter()
	{
		Position = Vector3.zero;
		VelocityMin = 0.0f;
		VelocityMax = this.VelocityMin;
		Direction = Vector3.up;
		Distribution = 1.0f;
		Frequency = 128.0f;
		ParticleMass = 1.0f;
		Enabled = true;
	}

	public void Emit(ref ArrayList particles, double dTime)
	{
		if(this.Enabled)
		{
			m_time += dTime;
			int nParts = (int)(this.Frequency * m_time);

			if ( nParts >0)
			{
				for(int i =0; i < nParts; i++)
				{
					float dist = UnityEngine.Random.value * Distribution - Distribution * 0.5f;
					Vector3 normal = new Vector3(Direction.y, -Direction.x,Direction.z);
					normal = normal * dist;

					Vector3 vel = Direction + normal;
					Vector3 vel3 = new Vector3(vel.x,vel.z, vel.y);
					vel3.Normalize();

					vel = new Vector3(vel3.x,vel3.y,vel3.z);

					float velLen = UnityEngine.Random.value * (this.VelocityMax - this.VelocityMin) + this.VelocityMin;
					vel = vel * velLen;

					Vector3 oldPos= this.Position - vel * (float)m_time;

					FluidParticle f = new FluidParticle();
					f.Position = Position;
					f.PositionOld = oldPos;
					f.Velocity = vel;
					f.Mass = ParticleMass;
					particles.Add(f);
				}
				m_time = 0.0;
			}
		}
	}
}
