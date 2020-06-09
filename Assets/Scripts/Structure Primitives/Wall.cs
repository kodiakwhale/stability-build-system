using UnityEngine;
using StabilityBuild;

public class Wall : Structure {
    
    public override bool CheckSnap (Structure snap, Vector3 cursorPos, int rotations) {
		SetSnap();
		Transform snapB = snap.SetSnap(); //snap transform that becomes a child of the snap structure
		
		//why snap in Structure?
		//make another script just for the highlight?
		//or a bool to tell if this is a highlight?
		
		if (snap is PlatformSquare) {
			Transform sidesB = snapB.Find("Sides");
			
			int snapIndex = Building.ClosestChildIndex(sidesB, cursorPos);
			transform.position = sidesB.GetChild(snapIndex).position;
			transform.rotation = snap.transform.rotation * Quaternion.Euler(0, snapIndex * 90 + 180, 0);
		} else if (snap is PlatformTriangle) {
			Transform sidesB = snapB.Find("Sides");
			
			int snapIndex = Building.ClosestChildIndex(sidesB, cursorPos);
			transform.position = sidesB.GetChild(snapIndex).position;
			transform.rotation = snap.transform.rotation * Quaternion.Euler(0, 120 * snapIndex + 180, 0);
		} else {
			return ReturnSnap(false, snap);
		}
		
		return ReturnSnap(true, snap);
    }
	
	public override bool IsValid(LayerMask validityCheckMask) {
		if (Physics.CheckSphere(GetComponent<Renderer>().bounds.center, 0.01f, validityCheckMask, QueryTriggerInteraction.Collide)) {
			return false;
		}
		return true;
	}
	
}
