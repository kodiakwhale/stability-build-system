using UnityEngine;
using StabilityBuild;

public class Wall : Structure {
    
    public override bool CheckSnap (Structure snap, Vector3 cursorPos, int rotations) {
		SetSnap();
		Transform snapB = snap.SetSnap(); //snap transform that becomes a child of the snap structure
		
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
		} else if (snap is Wall) {
			transform.position = snap.transform.position + Vector3.up * (snap is Halfwall ? 2 : 4) * Building.BuildScale;
			transform.rotation = snap.transform.rotation;
		} else {
			return ReturnSnap(false, snap);
		}
		
		return ReturnSnap(true, snap);
    }
	
	//For the wall, we want it to be supported on its sides in order to be placed.
	//If you don't want to have pillars in order to place walls, simply coment out this IsValid override
	public override bool IsValid(LayerMask validityCheckMask) {
		if (base.IsValid(validityCheckMask)) {
			Vector3[] supportCheckSpots = { new Vector3(2, 2, 0), new Vector3(-2, 2, 0)};
			
			bool hasSupport = true;
			for (int i = 0; i < supportCheckSpots.Length; i++) {
				hasSupport = hasSupport && Physics.CheckSphere(transform.TransformPoint(supportCheckSpots[i]), 0.01f, validityCheckMask, QueryTriggerInteraction.Collide);
			}
			return hasSupport;
		}
		return false;
	}
	
}
