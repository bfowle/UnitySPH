using UnityEngine;
using System.Collections;
using System;

public class OBB : BoundingVolume {

	public Vector3 Extents;

	public OBB()
	{
		Position = Vector3.zero;
		Extents = new Vector3 (1.0f, 1.0f, 1.0f);
		Axis = new Vector3[]{
			new Vector3(1,0,0),
			new Vector3(0,1,0),
			new Vector3(0,0,1)
		};
	}

	public override void Project (Vector3 axis, out float min, out float max)
	{
		float pos = Vector3.Dot (this.Position, axis);
		float radius = Math.Abs (Vector3.Dot (axis, this.Axis [0])) * this.Extents.x + Math.Abs (Vector3.Dot (axis, this.Axis [1])) * this.Extents.y + Math.Abs (Vector3.Dot (axis, this.Axis [2])) * this.Extents.z;
		min = pos - radius;
		max = pos + radius;
	}

	public void Rotate(double angle)
	{
		Axis[0] = RotateXAxis(angle,Axis[0]);
		Axis[1] = RotateYAxis(angle,Axis[1]);
		Axis[2] = RotateZAxis(angle,Axis[1]);
	}

	private static Vector3 RotateXAxis(double angle,Vector3 axis)
	{
		return new Vector3(axis.x,
		                   axis.y * (float)Math.Cos (angle) - axis.z * (float)Math.Sin (angle),
		                   axis.y * (float)Math.Sin (angle) + axis.z * (float)Math.Cos (angle)
		                   );
	}

	private static Vector3 RotateYAxis(double angle, Vector3 axis)
	{
		return new Vector3(axis.z * (float)Math.Sin (angle) + axis.x * (float)Math.Cos (angle),
		                   axis.y,
		                   axis.z * (float)Math.Cos (angle) - axis.x * (float)Math.Sin (angle)
		                   );
	}
	private static Vector3 RotateZAxis(double angle, Vector3 axis)
	{
		return new Vector3(axis.x * (float)Math.Cos (angle) - axis.y * (float)Math.Sin (angle),
		                   axis.x * (float)Math.Sin (angle) + axis.y * (float)Math.Cos (angle),
		                   axis.z
		                   );
	}
}
