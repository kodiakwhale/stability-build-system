using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class StructureManager : MonoBehaviour {
	
	/*
	This script handles snap point pooling and keeps track of all placed structures.
	It detects groups of collapsed structures and groups them into one rigidbody.
	*/

	[SerializeField] Transform[] structurePrefabs;
	[SerializeField] KeyCode debugKey = KeyCode.P;
	[SerializeField] KeyCode saveKey = KeyCode.O;
	[SerializeField] KeyCode loadKey = KeyCode.L;
	static bool showStability = true;
	static Shader debugShader;
	
	static StructureManager instance;
	public static StructureManager Instance { get { return instance; } }

	static Transform snapStorage;
	static List<Structure> structures;
	static List<Stability> destroyed;

	void Awake() {
		//Only want one instance of this script in the world
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
		
		snapStorage = transform.Find("Snaps");
		structures = new List<Structure>();
		destroyed = new List<Stability>();
		debugShader = Shader.Find("kodiakwhale/Stability Build Debug");
		
		//Store two copies of every snap, in case we need to snap two of the same structure together
		for	(int i = 0; i < structurePrefabs.Length; i++) {
			StoreSnaps(structurePrefabs[i]);
			StoreSnaps(structurePrefabs[i]);
		}
	}
	
	void Update() {
		if (destroyed.Count > 0) {
			Glue();
			destroyed = new List<Stability>();
		}
		
		if (Input.GetKeyDown(debugKey)) {
			//Toggle stability debug on all structures
			ToggleStability();
		} else if (Input.GetKeyDown(saveKey)) {
			SaveStructures("test");
		} else if (Input.GetKeyDown(loadKey)) {
			LoadStructures("test");
		}
	}
	
	void Glue() {
		for	(int i = 0; i < destroyed.Count; i++) {
			Stability structure = destroyed[i];
			if (structure != null) {
				if (!structure.glued) {
					structure.Glue(structure.transform);
				}
			}
		}
	}
	
	void ToggleStability() {
		showStability = !showStability;
		for	(int i = 0; i < structures.Count; i++) {
			UpdateStructureDebug(structures[i].GetComponent<Renderer>());
		}
	}
	
	static void UpdateStructureDebug(Renderer rend) {
		int matsLength = rend.materials.Length;
		Material[] mats = rend.sharedMaterials;
		for	(int i = 0; i < matsLength; i++) {
			if (mats[i].shader = debugShader) {
				mats[i].SetFloat("_ShowStability", showStability ? 1 : 0);
			}
		}
	}
	
	#region Snap Managing
	public static void StoreSnaps(Transform snap) {
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
	
	private static void StoreSubSnaps(Transform original, Transform newSnap) {
		foreach (Transform child in original) {
			Transform subSnap = new GameObject(child.name).transform;
			subSnap.SetParent(newSnap);
			subSnap.localPosition = child.localPosition;
			if (child.childCount > 0) {
				StoreSubSnaps(child, subSnap);
			}
		}
	}
	
	public static Transform GetSnap(Structure structure) {
		string snapName = structure.GetType().Name;
		Transform snap = snapStorage.Find(snapName);
		if (snap == null) {
			snap = snapStorage.Find(snapName + "B");
		}
		return snap;
	}
	#endregion
	
	#region Structure Managing
	public static void AddStructure(Structure structure) {
		if (!structures.Contains(structure)) {
			structures.Add(structure);
			UpdateStructureDebug(structure.GetComponent<Renderer>());
		}
	}
	
	//when a structure is removed, call this
	public static void RemoveStructure(Structure structure) {
		if (structures.Contains(structure)) {
			structures.Remove(structure);
		}
		
	}
	
	public static void AddDestroyed(Stability structure) {
		if (!destroyed.Contains(structure)) {
			destroyed.Add(structure);
		}
	}
	#endregion
	
	//Saving and loading based on Jasper Flick's implementation from https://catlikecoding.com/unity/tutorials/object-management/persisting-objects/
	#region Saving and Loading
	
	void SaveStructures(string saveName) {
		string savePath = Application.persistentDataPath;
		savePath = Path.Combine(savePath, saveName);
		using (
			BinaryWriter bwriter = new BinaryWriter(File.Open(savePath, FileMode.Create))
		) {
			GameWriter writer = new GameWriter(bwriter);
			writer.Write(Application.version);
			
			writer.Write(structures.Count);
			for (int i = 0; i < structures.Count; i++) {
				Structure structureComponent = structures[i];
				Stability stabilityComponent = structureComponent.GetComponent<Stability>();
				
				//Write the name of the Structure type, so we can spawn the right one when loading
				writer.Write(structureComponent.GetType().Name);
				writer.Write(structureComponent.health);
				
				//Write transform information
				Transform t = structureComponent.transform;
				writer.Write(t.position);
				writer.Write(t.rotation);
				writer.Write(t.localScale);
				
				//Write a bool to indicate if a structure has a Stability component
				bool hasStability = stabilityComponent != null;
				writer.Write(hasStability);
				if (hasStability) {
					writer.Write(stabilityComponent.stability);
				}
				
			}
		}
	}
	
	void LoadStructures(string saveName) {
		string savePath = Application.persistentDataPath;
		savePath = Path.Combine(savePath, saveName);
		using (
			BinaryReader breader = new BinaryReader(File.Open(savePath, FileMode.Open))
		) {
			GameReader reader = new GameReader(breader);
			string fileVersion = reader.ReadString();
			if (fileVersion != Application.version) {
				Debug.LogError("Incompatible save file version.");
				return;
			}
			
			int numStructures = reader.ReadInt();
			for (int i = 0; i < numStructures; i++) {
				string structureTypeName = reader.ReadString();
				int structureHealth = reader.ReadInt();
				
				int structureIndex = -1;
				for (int j = 0; j < structurePrefabs.Length; j++) {
					if (structurePrefabs[j].GetComponent<Structure>().GetType().Name == structureTypeName) {
						structureIndex = j;
						break;
					}
				}
				
				if (structureIndex == -1) {
					Debug.LogError("Corrupted save file! Could not spawn Structure of type: " + structureTypeName);
					return;
				}
				
				//Spawns a new object, reading in the position and rotation
				GameObject structureObj = Instantiate(structurePrefabs[structureIndex].gameObject, reader.ReadVector3(), reader.ReadQuaternion(), null);
				structureObj.transform.localScale = reader.ReadVector3();
				
				//Make sure to add a stability component if there wasn't one on the prefab
				if (reader.ReadBool()) {
					Stability structureStability = structureObj.GetComponent<Stability>();
					if (structureStability == null) {
						structureStability = structureObj.AddComponent<Stability>();
					}
					structureStability.stability = reader.ReadInt();
				}
				
				AddStructure(structureObj.GetComponent<Structure>());
			}
		}
	}
	
	#endregion
}
