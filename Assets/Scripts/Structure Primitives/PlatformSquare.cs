using UnityEngine;
using StabilityBuild;

//Implements Structure functionality
public class PlatformSquare : Structure {
	
	//returns true if there is a snap available, then snaos
	public override bool CheckSnap (Structure snap, Vector3 cursorPos) {
		
		Transform snapA = SetSnap(); //snap transform that becomes a child of the highlight structure
		Transform snapB = snap.SetSnap(); //snap transform that becomes a child of the snap structure
		
		//why snap in Structure?
		//make another script just for the highlight?
		//or a bool to tell if this is a highlight?
		
		if (snap is PlatformSquare) {
			Transform sidesA = snapA.Find("Sides");
			Transform sidesB = snapB.Find("Sides");
			
			int snapIndex = Building.ClosestChildIndex(sidesB, cursorPos);
			transform.position = snap.transform.position;
			transform.rotation = snap.transform.rotation * Quaternion.Euler(0, 180, 0);
			
			Vector3 offset = sidesB.GetChild(snapIndex).position - sidesA.GetChild(snapIndex).position;
			transform.position += offset;
		} else if (snap is PlatformTriangle) {
			Transform sidesA = snapA.Find("Sides");
			Transform sidesB = snapB.Find("Sides");
			
			int snapIndex = Building.ClosestChildIndex(sidesB, cursorPos);
			transform.position = snap.transform.position;
			transform.rotation = snap.transform.rotation * Quaternion.Euler(0, 120 * snapIndex + 180, 0);
			
			Vector3 offset = sidesB.GetChild(snapIndex).position - sidesA.GetChild(0).position;
			transform.position += offset;
		} else if (snap is Wall) {
			//snap PlatformSquare to Wall
			return ReturnSnap(false, snap);
		} else {
			return ReturnSnap(false, snap);
		}
		
		return ReturnSnap(true, snap);
	}

}
