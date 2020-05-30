using UnityEngine;

//TODO: if we never need to provide base implementations (virtual methods), switch to IStructure interface
public abstract class Structure : MonoBehaviour {
    
    [SerializeField]
	private bool canPlaceOnlyWhenSnapped;
    
    //stablity and health should be it's own script
    //snap to that, not structure?
    [SerializeField]
    protected int maxHealth = 100;
    [SerializeField]
    protected int health = 100;
    
    [SerializeField]
    protected int stability = 100;

    [SerializeField]
    private Vector3[] validityCheckPoints; //points in local space that must not overlap any colliders, used in validity checking
    private static float validityCheckRadius = 0.01f;
    
    public bool glued { get; private set; } = false;
    
    private Transform snap;

    //snaps could be different for any 2 structures, so they must define their own snapping behavior or inherit functionality from another structure
    public abstract bool CheckSnap (Structure snapTo, Vector3 cursorPos);
    
    //By default, every structure removes its snaps when placed
    //Could override if you have a structure with additional children (e.g. a torch with a point light)
    public virtual void OnInstall () {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    
    //Call this instead of Destroy() on every structure
    public virtual void Remove () {
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
    
    public void ReturnSnap () {
		if (snap == null) {
			return;
		}
        StructureManager.StoreSnaps(snap);
        Destroy(snap.gameObject);
        snap = null;
    }

    //validity checking: base implementation checks to make sure there are no colliders at each point in validityCheckPoints
    //can use OverlapSphere instead of CheckSphere to do additional logic on any found colliders
    public virtual bool IsValid (LayerMask validityCheckMask) {
        /*for (int i = 0; i < validityCheckPoints.Length; i++) {
            if (Physics.CheckSphere(validityCheckPoints[i], validityCheckRadius, validityCheckMask)) {
                return false;
            }
        }*/
        
        return true;
    }
    
    public virtual void Glue (Transform glueTo) {
        // :)
        glued = true;
    }
    
}
