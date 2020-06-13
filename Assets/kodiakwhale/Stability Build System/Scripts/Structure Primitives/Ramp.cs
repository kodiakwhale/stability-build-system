using UnityEngine;
using StabilityBuild;

//Implements Structure functionality
public class Ramp : Structure {
	
	//returns true if there is a snap available, then snaos
	public override bool CheckSnap (Structure snap, Vector3 cursorPos, int rotations) {
		
		Transform snapA = SetSnap(); //snap transform that becomes a child of the highlight structure
		Transform snapB = snap.SetSnap(); //snap transform that becomes a child of the snap structure
		
		if (snap is PlatformSquare) {
			Transform sidesA = snapA.Find("Sides");
			Transform sidesB = snapB.Find("Sides");
			
			int snapIndex = Building.ClosestChildIndex(sidesB, cursorPos);
			transform.position = snap.transform.position;
			transform.rotation = snap.transform.rotation * Quaternion.Euler(0, snapIndex * 90, 0);
		} else {
			return ReturnSnap(false, snap);
		}
		
		return ReturnSnap(true, snap);
	}

}
