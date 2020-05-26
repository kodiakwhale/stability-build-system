using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour {
	
	/*
	This script handles snap point pooling and keeps track of all placed structures.
	It detects groups of collapsed structures and groups them into one rigidbody.
	*/

	[SerializeField]
	private Transform[] structurePrefabs;
	
	//singleton pattern
	private static StructureManager instance;
	public static StructureManager Instance { get { return instance; } }

	private static Transform snapStorage;
	
	private static List<Structure> structures;
	private static List<Structure> destroyed;

	void Awake () {
		//only want one instance of this script in the world
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
		
		snapStorage = transform.Find("Snaps");
		structures = new List<Structure>();
		destroyed = new List<Structure>();
		
		//store two copies of every snap, in case we need to snap two of the same structure together
		for	(int i = 0; i < structurePrefabs.Length; i++) {
			StoreSnaps(structurePrefabs[i]);
			StoreSnaps(structurePrefabs[i]);
		}
	}
	
	void Update () {
		if (destroyed.Count > 0) {
			Glue();
			destroyed = new List<Structure>();
		}
	}
	
	void Glue () {
		for	(int i = 0; i < destroyed.Count; i++) {
			Structure structure = destroyed[i];
			if (structure != null) {
				if (!structure.glued) {
					structure.Glue(structure.transform);
				}
			}
		}
	}
	
	#region Snap Managing
	public static void StoreSnaps (Transform snap) {
		string snapName = snap.name;
		if (snapStorage.Find(snapName) != null) {
			snapName = snapName + "B";
		}
		if (snapStorage.Find(snapName) == null) {
			Transform newSnap = new GameObject(snapName).transform;
			newSnap.SetParent(snapStorage);
			
			if (snap.childCount > 0) {
				StoreSubSnaps(snap, newSnap);
			}
		}
	}
	
	private static void StoreSubSnaps (Transform original, Transform newSnap) {
		foreach (Transform child in original) {
			Transform subSnap = new GameObject(child.name).transform;
			subSnap.SetParent(newSnap);
			subSnap.localPosition = child.localPosition;
			if (child.childCount > 0) {
				StoreSubSnaps(child, subSnap);
			}
		}
	}
	
	public static Transform GetSnap (Structure structure) {
		string snapName = structure.GetType().Name;
		Transform snap = snapStorage.Find(snapName);
		if (snap == null) {
			snap = snapStorage.Find(snapName + "B");
		}
		return snap;
	}
	#endregion
	
	#region Structure Managing
	public static void AddStructure (Structure structure) {
		if (!structures.Contains(structure)) {
			structures.Add(structure);
		}
	}
	
	public static void RemoveStructure (Structure structure) {
		//when a structure is removed, call this
		if (structures.Contains(structure)) {
			structures.Remove(structure);
		}
		
	}
	
	public static void AddDestroyed (Structure structure) {
		if (!destroyed.Contains(structure)) {
			destroyed.Add(structure);
		}
		RemoveStructure(structure);
		
	}
	#endregion
	
}
