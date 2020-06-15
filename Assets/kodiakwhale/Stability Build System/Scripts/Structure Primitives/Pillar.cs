using UnityEngine;
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
		} else if (snap is PlatformTriangle) {
			Transform cornersB = snapB.Find("Corners");
			
			int snapIndex = Building.ClosestChildIndex(cornersB, cursorPos);
			transform.position = cornersB.GetChild(snapIndex).position;
		} else if (snap is Pillar) {
			transform.position = snap.transform.position + snap.transform.up * snap.GetComponent<Renderer>().bounds.extents.y * 2 - Vector3.up * 0.5f;
		} else {
			return ReturnSnap(false, snap);
		}
		
		transform.rotation = snap.transform.rotation * Quaternion.Euler(0, rotations * 15, 0);
		
		return ReturnSnap(true, snap);
    }
	
}
