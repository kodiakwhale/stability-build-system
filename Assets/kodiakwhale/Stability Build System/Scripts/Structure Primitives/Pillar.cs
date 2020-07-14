using UnityEngine;
using StabilityBuild;

public class Pillar : Structure {
	
	//Store last pillar's renderer to reduce GetComponent calls
	Structure prevPillar = null;
	Renderer prevRend = null;
    
    public override bool CheckSnap (Structure snap, Vector3 cursorPos, int rotations) {
		SetSnap();
		Transform snapB = snap.SetSnap();
		Transform snapTransform = snap.transform;
		
		//Rotate the pillar 15 degrees every time the player presses the rotate key
		transform.rotation = snapTransform.rotation * Quaternion.Euler(0, rotations * 15, 0);
		
		if (snap is PlatformSquare || snap is PlatformTriangle) {
			//Snap pillar to platform: use Corners snap offset closest to the cursor
			Transform cornersB = snapB.Find("Corners");
			
			int snapIndex = Building.ClosestChildIndex(cornersB, cursorPos);
			transform.position = cornersB.GetChild(snapIndex).position;
		} else if (snap is Pillar) {
			//Snap pillar to pillar: offset by the height of the mesh, then subtract the floor height (0.5m)
			if (prevPillar != snap) {
				prevPillar = snap;
				prevRend = snap.GetComponent<Renderer>();
			}
			transform.position = snapTransform.position + (Vector3.up * prevRend.bounds.extents.y * 2) - (Vector3.up * 0.5f);
			transform.rotation = snapTransform.rotation; //Align pillar rotations to make it easier to stack pillars
		} else {
			return ReturnSnap(false, snap);
		}
		
		return ReturnSnap(true, snap);
    }
	
}
