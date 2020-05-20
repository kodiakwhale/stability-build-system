using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Structure {
    public override bool CheckSnaps (Structure snapTo) {
        return false;
    } 
}
