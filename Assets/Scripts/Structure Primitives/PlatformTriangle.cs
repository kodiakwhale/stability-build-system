using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTriangle : Structure {
    
    public override bool CheckSnaps (Structure snapTo) {
        return false;
    } 

}
