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
			transform.position = snap.transform.position;
			transform.rotation = snap.transform.rotation * Quaternion.Euler(0, 180, 0);
			
			int snapIndex = Building.ClosestChildIndex(sidesB, cursorPos);
			Vector3 offset = sidesB.GetChild(snapIndex).position - sidesA.GetChild(snapIndex).position;
			transform.position += offset;
			//print(sidesA.GetChild(snapIndex).position + ", " + sidesB.GetChild(snapIndex).position);
		} else if (snap is PlatformTriangle) {
			//snap PlatformSquare to PlatformTriangle
		} else if (snap is Wall) {
			//snap PlatformSquare to Wall
		} else {
			ReturnSnap();
			snap.ReturnSnap();
			return false;
		}
		
		ReturnSnap();
		snap.ReturnSnap();
		return true;
	}

}
