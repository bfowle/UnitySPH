using UnityEngine;
using System.Collections;
using System;

public class SPHSimulation{

	public IndexGrid m_grid;
	public float CellSpace;
	public Rect Domain;
	public SmoothingKernel SKGeneral;
	public SmoothingKernel SKPressure;
	public SmoothingKernel SKViscosity;
	public float Viscosity;

	public SPHSimulation()
	{
		CellSpace = 8;
		Domain = new Rect (0, 0, 256, 256);
	}

	public SPHSimulation(float cellSpace, Rect domain)
	{
		CellSpace = cellSpace;
		Domain = domain;
		Viscosity = 0.2f;
		m_grid = new IndexGrid (CellSpace, Domain);
		SKGeneral = new Poly6 (CellSpace);
		SKPressure = new Spiky (CellSpace);
		SKViscosity = new Viscosity (CellSpace);
	}

	public void Calculate(ref ArrayList particles, Vector3 globalForce,float dTime)
	{
		m_grid.Refresh (ref particles);
		CalculatePressureAndDensities (ref particles, ref m_grid);
		CalculateForces (ref particles, ref m_grid, globalForce);
		UpdateParticles (ref particles, dTime);
		CheckParticleDistance (ref particles, ref m_grid);
	}

	private void CalculatePressureAndDensities(ref ArrayList particles, ref IndexGrid grid)
	{
		Vector3 dist;

		foreach(FluidParticle particle in particles)
		{
			particle.Density = 0.0f;
			foreach(int nIdx in grid.GetNeighbourIndex(particle))
			{
				if(particle != (FluidParticle)particles[nIdx])
				{
					dist = particle.Position - ((FluidParticle) particles[nIdx]).Position;
					particle.Density += particle.Mass * (float)this.SKGeneral.Calculate(ref dist);
				}
			}
			particle.UpdatePressure();
		}
	}

	private void CalculateForces(ref ArrayList particles, ref IndexGrid grid, Vector3 globalForce)
	{
		Vector3 f, dist;
		float scalar; 

		for(int i =0; i < particles.Count;i++)
		{
			FluidParticle p = (FluidParticle) particles[i];
			p.Force += globalForce;

			foreach(int nIdx in grid.GetNeighbourIndex(p))
			{
				if(nIdx < i)
				{
					FluidParticle pn = (FluidParticle)particles[nIdx];

					if(pn.Density > Mathf.Epsilon && p != pn)
					{
						dist = p.Position - pn.Position;

						scalar = pn.Mass * (p.Pressure + pn.Pressure) / (2.0f * pn.Density);
						f = SKPressure.CalculateGradient(ref dist);
						f = f * scalar;
						p.Force -= f;
						pn.Force += f;

						scalar   = pn.Mass * (float)this.SKViscosity.CalculateLaplacian(ref dist) * Viscosity * 1 / pn.Density;
						f = pn.Velocity - p.Velocity;
						f = f * scalar;
						p.Force += f;
						pn.Force -= f;
					}
				}
			}
		}
	}

	private void UpdateParticles(ref ArrayList particles, float dTime)
	{
		float r = Domain.xMax;
		float l = Domain.x;
		float t = Domain.yMax;
		float b = Domain.y;

		for(int i = 0; i < particles.Count;i++)
		{
			FluidParticle particle = (FluidParticle) particles[i];

			if(particle.Position.x < l)
			{
				particle.Position.x = l + Mathf.Epsilon;
			}
			else if (particle.Position.x > r) {
				particle.Position.x = r - Mathf.Epsilon;
			}


			if (particle.Position.y < b) {
				particle.Position.y = b + Mathf.Epsilon;
			}
			else if (particle.Position.y > t) {
				particle.Position.y = t - Mathf.Epsilon;
			}

			if(particle.Position.z < -1)
			{
				particle.Position.z = particle.Position.z  + 0.0005f;
			}else if (particle.Position.z > 1.5) {
				particle.Position.z = particle.Position.z - 0.0005f;
			}

			// Update velocity + position using forces
			particle.Update(dTime);
			// Reset force
			particle.Force = Vector2.zero;
		}
	}

	private void CheckParticleDistance(ref ArrayList particles, ref IndexGrid grid)
	{
		float minDist = 0.5f * CellSpace;
		float minDistSq = minDist * minDist;

		Vector3 dist;

		for(int i = 0; i < particles.Count;i++)
		{
			FluidParticle p = (FluidParticle) particles[i];

			foreach (int nIdx in grid.GetNeighbourIndex(p))
			{
				FluidParticle pn = (FluidParticle) particles[nIdx];

				if( p != pn)
				{
					dist = pn.Position - p.Position;
					float distLenSq = dist.sqrMagnitude;

					if(distLenSq < minDistSq)
					{
						if(distLenSq > Mathf.Epsilon)
						{
							float distLen = (float)Math.Sqrt((double)distLenSq);
							dist = dist * 0.5f * (distLen - minDist)/distLen;
							pn.Position = pn.Position - dist;
							pn.PositionOld = pn.PositionOld - dist;
							p.Position = p.Position + dist;
							p.PositionOld = p.PositionOld + dist;
						}
						else
						{
							float diff = 0.5f * minDist;
							pn.Position.x -= diff;
							pn.Position.y -= diff;
							p.Position.x  += diff;
							p.PositionOld.y += diff;
						}
					}
				}                                       
			}
		}
	}
}
