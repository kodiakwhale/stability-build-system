using System;
using UnityEngine;

public abstract class Structure : MonoBehaviour {
    
    public bool canPlaceOnlyWhenSnapped = false;
    
    //stablity and health should be it's own script
    //snap to that, not structure?
    [SerializeField]
    protected int maxHealth = 100;
    [SerializeField]
    protected int health = 100;

    [SerializeField]
    public Vector3[] validityCheckPoints = { Vector3.zero }; //points in local space that must not overlap any colliders, used in validity checking
    protected static float validityCheckRadius = 0.01f;
    
    public bool glued { get; private set; } = false;
    
    private Transform snap;
	public event Action deathEvent;

    //snaps could be different for any 2 structures, so they must define their own snapping behavior or inherit functionality from another structure
    public abstract bool CheckSnap (Structure snapTo, Vector3 cursorPos, int rotations);
    
    //By default, every structure removes its snaps when placed
    //Could override if you have a structure with additional children (e.g. a torch with a point light)
    public virtual void OnInstall () {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
		StructureManager.AddStructure(this);
		
		validityCheckPoints = null;
    }
    
    //Call this instead of Destroy() on every structure
    public virtual void Remove () {
		deathEvent();
        StructureManager.RemoveStructure(this);
        Destroy(gameObject);
    }
    
    public Transform SetSnap () {
        snap = StructureManager.GetSnap(this);
		if (snap == null) {
			return null;
		}
        snap.SetParent(transform);
		snap.transform.localPosition = Vector3.zero;
		snap.transform.localEulerAngles = Vector3.zero;
        return snap;
    }
    
    public bool ReturnSnap (bool snapped, Structure snapToReturn = null) {
		if (snapToReturn != null) {
			snapToReturn.ReturnSnap(snapped);
		}
		
		if (snap == null) {
			return snapped;
		}
        StructureManager.StoreSnaps(snap);
        Destroy(snap.gameObject);
        snap = null;
		
		return snapped;
    }

    //validity checking: base implementation checks to make sure there are no colliders at each point in validityCheckPoints
    //can use OverlapSphere instead of CheckSphere to do additional logic on any found colliders, or check for mesh colliders
    public virtual bool IsValid (LayerMask validityCheckMask) {
		Vector3 pos = transform.position;
        for (int i = 0; i < validityCheckPoints.Length; i++) {
            if (Physics.CheckSphere(transform.TransformPoint(validityCheckPoints[i]), validityCheckRadius, validityCheckMask, QueryTriggerInteraction.Collide)) {
                return false;
            }
        }
        
        return true;
    }
    
    public virtual void Glue (Transform glueTo) {
        // :)
        glued = true;
    }
    
}
