using UnityEngine;
using System.Collections;
using System;

public class Spiky : SmoothingKernel {

	public Spiky()
	{

	}

	public Spiky(double kernelSize)
	{
		this.kernelSize = kernelSize;
	}

	protected override void CalculateFactor()
	{
		double kernelR = Math.Pow(kernelSize,6.0d);
		this.factor = (15.0f/ (Math.PI * kernelR));
	}
	
	public override double Calculate(ref Vector3 distance)
	{
		// Mathf Sqrt(Vector3.Dot(v,v));
		double lengthOfDistanceSQ = distance.sqrMagnitude;
		double epsilon = Mathf.Epsilon;
		
		
		if (lengthOfDistanceSQ > kernelSizePow2) 
		{
			return 0.0f;
		}
		
		if(lengthOfDistanceSQ < epsilon)
		{
			lengthOfDistanceSQ = epsilon;
		}
		
		double f = kernelSize - Math.Sqrt (lengthOfDistanceSQ);
		return factor * f * f * f;
	}
	
	public override Vector3 CalculateGradient (ref Vector3 distance)
	{
		// Mathf Sqrt(Vector3.Dot(v,v));
		double lengthOfDistanceSQ = distance.sqrMagnitude;
		double epsilon = Mathf.Epsilon;
		
		
		if (lengthOfDistanceSQ > kernelSizePow2) {
			return new Vector3 (0.0f, 0.0f, 0.0f);
		}
		
		if(lengthOfDistanceSQ < epsilon)
		{
			lengthOfDistanceSQ = epsilon;
		}
		
		double len = Math.Sqrt (lengthOfDistanceSQ);
		double f = -this.factor * 3.0f * (this.kernelSize - len) * (this.kernelSize - len) / len;
		return new Vector3 (distance.x * (float)f, distance.y * (float)f, distance.z * (float)f);
	}
	
	public override double CalculateLaplacian(ref Vector3 distance)
	{
		throw new NotImplementedException();
	}
}
