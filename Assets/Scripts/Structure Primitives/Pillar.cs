﻿using UnityEngine;
using StabilityBuild;

public class Pillar : Structure {
    
    public override bool CheckSnap (Structure snap, Vector3 cursorPos, int rotations) {
		SetSnap();
		Transform snapB = snap.SetSnap(); //snap transform that becomes a child of the snap structure
		
		//why snap in Structure?
		//make another script just for the highlight?
		//or a bool to tell if this is a highlight?
		
		if (snap is PlatformSquare) {
			Transform cornersB = snapB.Find("Corners");
			
			int snapIndex = Building.ClosestChildIndex(cornersB, cursorPos);
			transform.position = cornersB.GetChild(snapIndex).position;
			transform.rotation = Quaternion.Euler(0, rotations * 15, 0);
		} else if (snap is PlatformTriangle) {
			Transform cornersB = snapB.Find("Corners");
			
			int snapIndex = Building.ClosestChildIndex(cornersB, cursorPos);
			transform.position = cornersB.GetChild(snapIndex).position;
			transform.rotation = Quaternion.Euler(0, rotations * 15, 0);
		} else if (snap is Pillar) {
			transform.position = snap.transform.position + snap.transform.up * snap.GetComponent<Renderer>().bounds.extents.y * 2;
			transform.rotation = snap.transform.rotation;
		} else {
			return ReturnSnap(false, snap);
		}
		
		return ReturnSnap(true, snap);
    }
	
	public override bool IsValid(LayerMask validityCheckMask) {
		if (Physics.CheckSphere(GetComponent<Renderer>().bounds.center, 0.01f, validityCheckMask)) {
			return false;
		}
		return true;
	}
	
}
