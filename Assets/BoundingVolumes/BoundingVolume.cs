using UnityEngine;
using System.Collections;

public abstract class BoundingVolume{

	public Vector3 Position;
	public Vector3[] Axis;

	public bool IsFixed;
	public float Margin;
	float epsilon = Mathf.Epsilon;


	protected BoundingVolume()
	{
		IsFixed = false;
		Margin = epsilon;
	}

	public abstract void Project(Vector3 axis, out float min, out float max);

	public bool Intersects(BoundingVolume other)
	{
		Vector3 pentrationNormal;
		float penetrationLength;

		bool r = Intersects (other, out pentrationNormal, out penetrationLength);
		return r;
	}

	public bool Intersects( BoundingVolume other, out Vector3 penetrationNormal, out float penetrationLength)
	{
		penetrationNormal = Vector3.zero;
		penetrationLength = float.MaxValue;

		if(Axis != null)
		{
			foreach ( Vector3 axis in Axis)
			{
				if(!FindLeastPenetrating(axis,other,ref penetrationNormal,ref penetrationLength))
				{
					return false;
				}
			}
		}

		if(other.Axis != null)
		{
			foreach ( Vector3 axis in other.Axis)
			{
				if(!FindLeastPenetrating(axis,other,ref penetrationNormal,ref penetrationLength))
				{
					return false;
				}
			}
		}

		if(Vector3.Dot (other.Position - Position,penetrationNormal) > 0.0f)
		{
			penetrationNormal = penetrationNormal * -1.0f;
		}
		return true;
	}

	private bool FindLeastPenetrating(Vector3 axis, BoundingVolume other,ref Vector3 penetrationNormal,ref float penetrationLength)
	{
		float minThis, maxThis, minOther, maxOther;

		if(TestSeparatingAxis(axis,other,out minThis,out maxThis,out minOther, out maxOther))
		{
			return false;
		}

		float diff = Mathf.Min (maxOther, maxThis) - Mathf.Max (minOther, minThis);
		if (diff < penetrationLength) {
			penetrationLength    = diff;
			penetrationNormal    = axis;
		}
		return true;

	}
	
	private bool TestSeparatingAxis(Vector2 axis, BoundingVolume other, out float minThis, out float maxThis, out float minOther, out float maxOther) {
		Project(axis, out minThis, out maxThis);
		other.Project(axis, out minOther, out maxOther);
		
		// Add safety margin distance
		minThis  -= Margin;
		maxThis  += Margin;
		minOther -= other.Margin;
		maxOther += other.Margin;
		
		if (minThis >= maxOther || minOther >= maxThis) {
			return true;
		}
		return false;
	}
}
