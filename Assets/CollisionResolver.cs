using UnityEngine;
using System.Collections;
using System;

public class CollisionResolver{

	public ArrayList BoundingVolumes;
	public float Bounciness;
	public float Friction;

	public CollisionResolver()
	{
		BoundingVolumes = new ArrayList ();
		Bounciness = 1.0f;
		Friction = 0.0f;
	}

	public bool Solve()
	{
		bool hasCollided = false;
		Vector3 penetration = new Vector3(0,0,0);
		float penLen = 0.0f;;

		foreach(BoundingVolume bv1 in BoundingVolumes)
		{
			foreach(BoundingVolume bv2 in BoundingVolumes)
			{
				if(bv1 != bv2)
				{
					hasCollided = true;
					penetration *= penLen;

					if(bv2.IsFixed)
					{
						bv1.Position += penetration;
					}
					else
					{
						bv2.Position -= penetration;
					}
				}
			}
		}
		return hasCollided;
	}

	public bool Solve(ref ArrayList particles)
	{
		bool hasCollided = false;
		Vector3 penetration, penNormal, v, vn, vt;
		float penLen, dp;

		foreach (BoundingVolume bv in BoundingVolumes) 
		{
			foreach (FluidParticle particle in particles) 
			{
				if(bv.Intersects(particle.boundingVolume, out penNormal, out penLen))
				{
					hasCollided = true;
					penetration = penNormal * penLen;

					if(particle.boundingVolume.IsFixed)
					{
						bv.Position += penetration;
					}
					else
					{
						particle.boundingVolume.Position -= penetration;
						v = particle.Position - particle.PositionOld;

						Vector3 tangent = new Vector3(penNormal.y,-penNormal.x,penNormal.z);
						dp = Vector3.Dot (v,penNormal);
						vn = penNormal * (dp * -Bounciness);
						dp = Vector3.Dot (v,tangent);
						vt = tangent * (dp * (1.0f -Friction));
						v = vn + vt;
						particle.Position -= penetration;
						particle.PositionOld = particle.Position - v;
					}
				}
			}
		}
		return hasCollided;
	}

	public BoundingVolume FindIntersect(BoundingVolume boundingVolume)
	{
		for(int i=0; i < BoundingVolumes.Count;i++)
		{
			BoundingVolume bv = (BoundingVolume)BoundingVolumes[i];
			if(bv.Intersects(boundingVolume))
			{
				return bv;
			}
		}
		return null;
	}
}
