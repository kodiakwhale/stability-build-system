﻿using UnityEngine;
using StabilityBuild;

public class PlatformFoundationTriangle : PlatformTriangle {
	
	protected Renderer rend;
	
	void Awake () {
		rend = GetComponent<Renderer>();
	}
	
	//Uses the same snapping as a regular PlatformTriangle, so we can inherit from that class and use its functionality
	public override bool IsValid (LayerMask validityCheckMask) {
		//Add to the platform validity check to make sure the foundation is touching terrain in its center
		if (base.IsValid(validityCheckMask)) {
			return Physics.CheckSphere(rend.bounds.center, validityCheckRadius, Building.terrainMask, QueryTriggerInteraction.Collide);
		}
        
        return false;
    }
}
