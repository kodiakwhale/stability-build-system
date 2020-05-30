﻿using UnityEngine;

namespace StabilityBuild {
	
    public static class Building {
		
		//TODO: move to generic function in StabilityBuild namespace
		public static Transform ClosestChild(Transform snap, Vector3 pos) {
			float closestDist = float.MaxValue;
			Transform closestSnap = snap.GetChild(0);
			for (int i = 0; i < snap.childCount; i++) {
				float dist = (snap.GetChild(i).position - pos).sqrMagnitude;
				if (dist < closestDist) {
					closestDist = dist;
					closestSnap = snap.GetChild(i);
				}
			}
			return closestSnap;
		}
		
		//TODO: move to generic function in StabilityBuild namespace
		public static int ClosestChildIndex(Transform snap, Vector3 pos) {
			float closestDist = float.MaxValue;
			int closestIndex = 0;
			for (int i = 0; i < snap.childCount; i++) {
				float dist = (snap.GetChild(i).position - pos).sqrMagnitude;
				if (dist < closestDist) {
					closestDist = dist;
					closestIndex = i;
				}
			}
			return closestIndex;
		}
		
    }
	
}
