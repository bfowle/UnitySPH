using UnityEngine;
using System.Collections;
using System;

public class PointVolume : BoundingVolume {


	public PointVolume()
	{
		this.Position = Vector3.zero;
	}

	public override void Project (Vector3 axis, out float min, out float max)
	{
		min = max = Vector3.Dot (Position, axis);
	}
}
