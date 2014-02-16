using UnityEngine;
using System.Collections;
using System;

public class Viscosity : SmoothingKernel {

	public Viscosity()
	{

	}

	public Viscosity(double kernelSize)
	{
		this.kernelSize = kernelSize;
	}

	protected override void CalculateFactor()
	{
		this.factor = (15.0f/ (2.0f * Mathf.PI * kernelSizePow3));
	}

	public override double Calculate(ref Vector3 distance)
	{
		double lengthOfDistanceSQ = distance.sqrMagnitude;
		double epsilon = Mathf.Epsilon;
		
		
		if (lengthOfDistanceSQ > kernelSizePow2) {
			return 0.0d;
		}
		
		if(lengthOfDistanceSQ < epsilon)
		{
			lengthOfDistanceSQ = epsilon;
		}

		double len = Math.Sqrt (lengthOfDistanceSQ);
		double len3 = len * len * len;
		return factor * (((-len3 / (2.0f * kernelSize)) + (lengthOfDistanceSQ / kernelSizePow2) + (kernelSize / 2.0f * len))) - 1.0f;
	}

	public override Vector3 CalculateGradient (ref Vector3 distance)
	{
		throw new NotImplementedException();
	}

	public override double CalculateLaplacian (ref Vector3 distance)
	{
		double lengthOfDistanceSQ = distance.sqrMagnitude;
		double epsilon = Mathf.Epsilon;
		
		
		if (lengthOfDistanceSQ > kernelSizePow2) {
			return 0.0d;
		}
		
		if(lengthOfDistanceSQ < epsilon)
		{
			lengthOfDistanceSQ = epsilon;
		}

		double len = Math.Sqrt (lengthOfDistanceSQ);

		return factor * (6.0f / kernelSizePow3) * (kernelSize - len);
	}
}
