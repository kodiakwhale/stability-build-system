using System.Collections.Generic;
using UnityEngine;
using StabilityBuild;

[RequireComponent(typeof(Structure))]
public class Stability : MonoBehaviour {
	
	private Renderer rend;
	public bool fallen { get; private set; } = false;
	public bool glued { get; private set; } = false;
	
	private bool anchored;
	private Stability mostStableNeighbor;
	
	private Structure structureComponent;
	
	[SerializeField]
	private int maxStability = 100;
	public int stability = 100;
	
	[Tooltip("How much stability is lost from the most stable neighboring structure")]
	[SerializeField]
	private int stabilityDegredation = 11;
	
	private List<Stability> neighbors = new List<Stability>();
	
	void Awake() {
		rend = GetComponent<Renderer>();
		RefreshNeighborList();
		structureComponent = GetComponent<Structure>();
		structureComponent.deathEvent += Fall;
	}
	
	public void Fall() {
		if (fallen) {
			return;
		}
		fallen = true;
		foreach (Stability neighbor in neighbors) {
			if (neighbor == null) {
				continue;
			}
			neighbor.RemoveNeighbor(this);
		}
		MeshCollider col = GetComponent<MeshCollider>();
		if (col != null && !col.convex) {
			col.convex = true;
		}
		gameObject.AddComponent<Rigidbody>();
	}
	
	public bool HasNeighbor(Stability nieghbor) {
		return neighbors.Contains(nieghbor);
	}
	
	public void AddNeighbor(Stability neighbor) {
		if (neighbor == this) {
			return;
		}
		if (!neighbors.Contains(neighbor)) {
			neighbors.Add(neighbor);
			if (neighbor.stability - stabilityDegredation > stability) {
				UpdateStability();
			}
		}
	}
	
	public void RemoveNeighbor(Stability neighbor) {
		if (neighbor == this) {
			return;
		}
		if (neighbors.Contains(neighbor)) {
			neighbors.Remove(neighbor);
			UpdateStability();
		}
	}
	
	/// <summary>
	/// Refreshes this structure's list of neighbors
	/// </summary>
	public void RefreshNeighborList() {
		(Vector3 a, Vector3 b, Quaternion c) CheckParams = (rend.bounds.center, rend.bounds.extents * 1.01f, Quaternion.identity); //I just wanted an excuse to try C# tuples :D
		if (Physics.CheckBox(CheckParams.a, CheckParams.b, CheckParams.c, Building.terrainMask)) {
			anchored = true;
			stability = maxStability;
			//This is only for the debug material; delete this loop if you replace the material
			foreach (Material mat in rend.sharedMaterials) {
				mat.SetInt("_Stability", stability);
			}
		}
		Collider[] cols = Physics.OverlapBox(CheckParams.a, CheckParams.b, CheckParams.c, Building.structureMask);
		neighbors = new List<Stability>();
		foreach (Collider col in cols) {
			Stability neighbor = col.GetComponent<Stability>();
			if (neighbor == null) {
				Debug.LogWarning("Object was on Structure layer, but had no Stability component", col.gameObject);
				continue;
			}
			if (neighbor == this) {
				continue;
			}
			
			if (!neighbors.Contains(neighbor)) {
				neighbors.Add(neighbor);
			}
			if (!neighbor.HasNeighbor(this)) {
				neighbor.AddNeighbor(this);
			}
		}
		
		UpdateStability();
	}
	
	/// <summary>
	/// Refreshes the stability of this structure and propagates updates to its neighbors
	/// </summary>
	public void UpdateStability() {
		if (anchored || this == null) {
			return;
		}
		
		int beforeStability = stability;
		
		int highestStability = 0;
		foreach (Stability neighbor in neighbors) {
			if (neighbor.stability > highestStability) {
				highestStability = neighbor.stability;
				mostStableNeighbor = neighbor;
			}
		}
		
		stability = highestStability - stabilityDegredation;
		if (beforeStability != stability) {
			for (int i = 0; i < neighbors.Count; i++) {
				if (neighbors[i] == null) {
					continue;
				}
				neighbors[i].UpdateStability();
			}
		}
		
		//This is only for the debug material; delete this loop if you replace the material
		foreach (Material mat in rend.sharedMaterials) {
			mat.SetInt("_Stability", stability);
		}
		
		if (stability <= 0) {
			StructureManager.AddDestroyed(this);
			fallen = true;
		}
	}
	
	public void Glue (Transform glueTo) {
		if (glued || !fallen) {
			return;
		}
		glued = true;
		StructureManager.RemoveStructure(structureComponent);
		MeshCollider col = GetComponent<MeshCollider>();
		if (col != null && !col.convex) {
			col.convex = true;
		}
		for (int i = 0; i < neighbors.Count; i++) {
			neighbors[i].Glue(glueTo);
		}
		
		if (glueTo == transform) {
			gameObject.AddComponent<Rigidbody>();
			return;
		}
		
		transform.SetParent(glueTo);
		
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null) {
			Destroy(rb);
		}
	}
	
}
