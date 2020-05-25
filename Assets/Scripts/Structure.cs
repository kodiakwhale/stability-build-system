using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: if we never need to provide base implementations (virtual methods), switch to IStructure interface
public abstract class Structure : MonoBehaviour {

    [SerializeField]
    protected int maxHealth = 100;
    [SerializeField]
    protected int health = 100;
    
    [SerializeField]
    protected int stability = 100;

    [SerializeField]
    private SnapPoint[] snapPoints;

    [SerializeField]
    private Vector3[] validityCheckPoints; //points in local space that must not overlap any colliders, used in validity checking
    private static float validityCheckRadius = 0.01f;
    
    public bool glued { get; private set; } = false;

    //snaps could be different for any 2 structures, so they must define their own snapping behavior or inherit functionality from another structure
    public abstract bool CheckSnap (Structure snapTo);

    //validity checking: base implementation checks to make sure there are no colliders at each point in validityCheckPoints
    //can use OverlapSphere instead of CheckSphere to do additional logic on any found colliders
    public virtual bool IsValid (LayerMask validityCheckMask) {
        for (int i = 0; i < validityCheckPoints.Length; i++) {
            if (Physics.CheckSphere(validityCheckPoints[i], validityCheckRadius, validityCheckMask)) {
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

[System.Serializable]
struct SnapPoint {
    [SerializeField]
    private Structure snapTo;
    [SerializeField]
    private Vector3 point;
    [SerializeField]
    private int sides;
}
