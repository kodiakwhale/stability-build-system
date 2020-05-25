using UnityEngine;
using System.Linq;

public class BuildController : MonoBehaviour {

	//get list of all structures within radious of 3d cursor
	//order by distance to cursor using ClosestPoint
	//test. closest to furthest, if can snap
	//test in Snap every structure to current building

	private LayerMask structureMask = 1 << LayerMask.NameToLayer("Structure");
	private LayerMask validityCheckMask;

	Vector3 cursorPos;
	float checkRadius = 5.0f;

	Structure currentStructure;
	
	//Called when a structure is successfully placed
	void OnInstall () {
		
	}

	//Gets an array of colliders within checkRadius of the 3d cursor position, then sorts it based on distance to the cursor
	Collider[] GetStructures () {
		return Physics.OverlapSphere(cursorPos, checkRadius, structureMask).OrderBy(o => o.ClosestPointOnBounds(cursorPos)).ToArray();
	}

	void SetValidity (bool valid) {
		if (valid) {
			//change material
			//change canPlace to tr
		} else {
			//change material
			//change canPlace to false
		}
	}

	void Snap () {
		Collider[] cols = GetStructures();
		foreach (Collider col in cols) {
			Structure snapTo = col.GetComponent<Structure>();
			if (snapTo == null) {
				Debug.LogWarning("GameObject was detected on Structure layer but has no Structure component");
				continue;
			}
			if (currentStructure.CheckSnap(snapTo)) {
				if (currentStructure.IsValid(validityCheckMask)) {
					//TODO: set valid
					break;
				}
			}
		}
		//TODO: no valid snaps; set invalid
	}
}