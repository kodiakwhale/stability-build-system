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
	
	[SerializeField]
	GameObject highlight;
	Structure currentStructure;
	GameObject structurePrefab;
	
	MeshFilter highlightMesh;
	Renderer highlightRenderer;
	Structure highlightStructure;
	
	[SerializeField]
	private KeyCode removeKey = KeyCode.X;
	[SerializeField]
	private KeyCode rotateKey = KeyCode.R;
	int rotations;
	
	[SerializeField]
	Camera cam;
	
	[SerializeField]
	private Material validMaterial;
	[SerializeField]
	private Material invalidMaterial;
	
	private bool canPlace = false;
	
	void Awake () {
		//Use of LayerMasks to improve performance and code simplification
		validityCheckMask = 1 << LayerMask.NameToLayer(structureLayer);
		structureMask = 1 << LayerMask.NameToLayer(structureLayer);
		
		//When we change the structure we're placing, we need to change the highlight mesh and materials
		highlightMesh = highlight.GetComponent<MeshFilter>();
		highlightRenderer = highlight.GetComponent<Renderer>();
	}
	
	void Update () {
		cursorPos = GetCursorPos();
		Snap();
		
		if (Input.GetKeyDown(rotateKey)) {
			rotations++;
		}
		
		
		if (Input.GetButtonDown("Fire1")) {
			Install();
		} else if (Input.GetKeyDown(removeKey)) {
			Structure structure = GetCursorStructure();
			if (structure != null) {
				structure.Remove();
			}
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
		if (currentStructure != null) {
			Destroy(currentStructure);
		}
		
		currentStructure = highlight.AddComponent(structureComponent.GetType()) as Structure;
		currentStructure.validityCheckPoints = structureComponent.validityCheckPoints;
		//currentStructure.SetValidityCheckPoints(structureComponent);
		currentStructure.canPlaceOnlyWhenSnapped = structureComponent.canPlaceOnlyWhenSnapped;
		highlightMesh.mesh = structure.GetComponent<MeshFilter>().sharedMesh;
		highlightRenderer.materials = new Material[structure.GetComponent<Renderer>().sharedMaterials.Length];
		Snap();
	}
	
	//Because Physics.CheckSphere doesn't work on concave mesh colliders, duplicate any and make them convex triggers to fix validity checking
	void Install () {
		if (canPlace && structurePrefab != null) {
			GameObject newStructure = Instantiate(structurePrefab, highlight.transform.position, highlight.transform.rotation, null);
			newStructure.layer = LayerMask.NameToLayer("Default");
			newStructure.GetComponent<Structure>().OnInstall();
			newStructure.layer = LayerMask.NameToLayer("Structure");
			
			MeshCollider[] cols = newStructure.GetComponents<MeshCollider>();
			for	(int i = 0; i < cols.Length; i++) {
				MeshCollider col = cols[i];
				if (!col.convex && !col.isTrigger) {
					MeshCollider newCol = newStructure.AddComponent<MeshCollider>();
					newCol.sharedMesh = col.sharedMesh;
					newCol.convex = true;
					newCol.isTrigger = true;
				}
			}
		}
	}
    
	//Toggles whether we can place a structure
    void SetValidity (bool valid) {
		Material mat = valid ? validMaterial : invalidMaterial;
		canPlace = valid;
		Material[] mats = new Material[highlightRenderer.sharedMaterials.Length];
		for (int i = 0; i < mats.Length; i++) {
			mats[i] = mat;
		}
		highlightRenderer.materials = mats;
	}

	void Snap () {		
		Collider[] cols = GetStructures();
		foreach (Collider col in cols) {
			Structure snapTo = col.GetComponent<Structure>();
			if (snapTo == null && currentStructure.canPlaceOnlyWhenSnapped) {
				Debug.LogWarning("GameObject was detected on Structure layer but has no Structure component");
				continue;
			}
			
			Stability stability = snapTo.GetComponent<Stability>();
			if (stability.fallen) {
				continue;
			}
			
			if (currentStructure.CheckSnap(snapTo, cursorPos, rotations)) {
				if (currentStructure.IsValid(validityCheckMask)) {
					SetValidity(true);
					return;
				}
			}
		}
		
		highlight.transform.position = cursorPos;
		highlight.transform.localEulerAngles = new Vector3(0, rotations * 90, 0);
		
		if (!currentStructure.canPlaceOnlyWhenSnapped) {
			if (currentStructure.IsValid(validityCheckMask)) {
				SetValidity(true);
				return;
			}
		}
		
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
	
	Structure GetCursorStructure () {
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, structureMask)) {
			return hit.collider.GetComponent<Structure>();
		}
		return null;
	}
}