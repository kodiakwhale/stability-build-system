using UnityEngine;

//Implements Structure functionality
public class PlatformSquare : Structure {
	
	//returns true if there is a snap available, then snaos
	public override bool CheckSnap (Structure snap) {
		//string typeName = snap.GetType().Name;
		//after snapping, check validity
		
		SetSnap();
		
		//why snap in Structure?
		//make another script just for the highlight?
		//or a bool to tell if this is a highlight?
		
		if (snap is PlatformSquare) {
			//snap PlatformSquare to PlatformSquare
		} else if (snap is PlatformTriangle) {
			//snap PlatformSquare to PlatformTriangle
		} else if (snap is Wall) {
			//snap PlatformSquare to Wall
		} else {
			ReturnSnap();
			return false;
		}
		
		ReturnSnap();
		return true;
	}

}
