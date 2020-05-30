using UnityEngine;
using System.Linq;

public class BuildController : MonoBehaviour {

	//get list of all structures within radious of 3d cursor
	//order by distance to cursor using ClosestPoint
	//test. closest to furthest, if can snap
	//test in Snap every structure to current building
	
	private string structureLayer = "Structure";
	private LayerMask structureMask;
	private LayerMask validityCheckMask;
	
	public GameObject platform;
	
	Vector3 cursorPos;
	[SerializeField]
	float checkRadius = 5.0f;
	[SerializeField]
	float checkDist = 5.0f;

	//GameObject highlightPrefab;
	[SerializeField]
	GameObject highlight;
	Structure currentStructure;
	GameObject structurePrefab;
	
	MeshFilter highlightMesh;
	Renderer highlightRenderer;
	Structure highlightStructure;
	
	[SerializeField]
	Camera cam;
	
	[SerializeField]
	private Material validMaterial;
	[SerializeField]
	private Material invalidMaterial;
	
	private bool canPlace = false;
	
	void Awake () {
		validityCheckMask = 1 << LayerMask.NameToLayer(structureLayer);
		structureMask = 1 << LayerMask.NameToLayer(structureLayer);
		highlightMesh = highlight.GetComponent<MeshFilter>();
		highlightRenderer = highlight.GetComponent<Renderer>();
	}
	
	void Update () {
		cursorPos = GetCursorPos();
		Snap();
		
		if (Input.GetButtonDown("Fire1")) {
			Install();
		}
	}

	//Gets an array of colliders within checkRadius of the 3d cursor position, then sorts it based on distance to the cursor using magic LINQ
	Collider[] GetStructures () {
		return Physics.OverlapSphere(cursorPos, checkRadius, structureMask).OrderBy(o => (o.ClosestPointOnBounds(cursorPos) - cursorPos).sqrMagnitude).ToArray();
	}
	
	//Adds a structure component to the highlight (for snapping), and changes the mesh accordingly
	//Because the highlight is not on the structure layer, it is never accidentally detected as a structure by scripts (via raycasting/overlapsphere)
	public void ChangeStructure (GameObject structure) {
		Structure structureComponent = structure.GetComponent<Structure>();
		structurePrefab = structure;
		
		//Make sure a valid structure prefab was passed in
		if (structure.layer != LayerMask.NameToLayer(structureLayer) || structureComponent == null) {
			highlightMesh.mesh = null;
			return;
		}
		
		//Ger rid of the previous structure component, if it exists
		if (highlightStructure != null) {
			Destroy(highlightStructure);
		}
		
		currentStructure = highlight.AddComponent(structureComponent.GetType()) as Structure;
		highlightMesh.mesh = structure.GetComponent<MeshFilter>().sharedMesh;
		//Snap();
	}
	
	void Install () {
		if (canPlace && structurePrefab != null) {
			GameObject newStructure = Instantiate(structurePrefab, highlight.transform.position, highlight.transform.rotation, null);
			newStructure.layer = LayerMask.NameToLayer("Default");
			newStructure.GetComponent<Structure>().OnInstall();
			newStructure.layer = LayerMask.NameToLayer("Structure");
		}
	}
    
	//Toggles whether we can place a structure
    void SetValidity (bool valid) {
		if (valid) {
			highlightRenderer.material = validMaterial;
			canPlace = true;
		} else {
			highlightRenderer.material = invalidMaterial;
			canPlace = false;
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
			if (currentStructure.CheckSnap(snapTo, cursorPos)) {
				if (currentStructure.IsValid(validityCheckMask)) {
					//TODO: set valid
					SetValidity(true);
					return;
				}
			}
		}
		//TODO: check if doesn't need to be snapped, then test validity against free-place mask
		highlight.transform.position = cursorPos;
		highlight.transform.localEulerAngles = Vector3.zero;
		SetValidity(false);
	}
	
	//gets the 3D point that the cursor is mousing over
	Vector3 GetCursorPos () {
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, checkDist)) {
			return hit.point;
		}
		return ray.GetPoint(checkDist);
	}
}