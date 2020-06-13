using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StabilityBuild;

[RequireComponent(typeof(Structure))]
public class Stability : MonoBehaviour {
	
	private Renderer rend;
	private bool fallen = false;
	
	private bool anchored;
	private Stability mostStableNeighbor;
	
	[SerializeField]
    private int maxStability = 100;
    public int stability { get; private set; } = 100;
	
	[Tooltip("How much stability is lost from the most stable neighboring structure")]
	[SerializeField]
	private int stabilityDegredation = 11;
	
	private List<Stability> neighbors = new List<Stability>();
	
	void Start () {
		RefreshNeighborList();
		GetComponent<Structure>().deathEvent += Fall;
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
			//if (neighbor == mostStableNeighbor) {
				UpdateStability();
			//}
		}
	}
	
	public void RefreshNeighborList() {
		rend = GetComponent<Renderer>();
		(Vector3 a, Vector3 b, Quaternion c) CheckParams = (rend.bounds.center, rend.bounds.extents * 1.01f, Quaternion.identity); //C# tuples are kind of weird
		if (Physics.CheckBox(CheckParams.a, CheckParams.b, CheckParams.c, Building.terrainMask)) {
			anchored = true;
			stability = maxStability;
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
			/*foreach (Stability neighbor in neighbors) {
				if (neighbor != null) {
					neighbor.UpdateStability();
				}
			}*/
		}
		
		foreach (Material mat in rend.sharedMaterials) {
			mat.SetInt("_Stability", stability);
		}
		
		/*if (stability <= 0) {
			GetComponent<Structure>().Remove();
		}*/
	}
	
}
