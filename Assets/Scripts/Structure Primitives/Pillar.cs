﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : Structure {
    
	public override bool CheckSnap (Structure snapTo, Vector3 cursorPos) {
        return false;
    } 
	
}
