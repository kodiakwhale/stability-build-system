using UnityEngine;
using UnityEngine.UI;

public class SelectStructure : MonoBehaviour {
	
	[SerializeField]
	private BuildController buildController;
	[SerializeField]
	private GameObject structureUIprefab;
    [SerializeField]
	private GameObject[] structures;
	int selectedStructure = 0;
	
	[SerializeField]
	private Transform uiParent;
	
	void Start () {
		for	(int i = 0; i < structures.Length; i++) {
			GameObject uiObj = Instantiate(structureUIprefab, Vector3.zero, Quaternion.identity, uiParent);
			string structureName = structures[i].name;
			uiObj.name = structureName;
			uiObj.transform.GetChild(0).GetComponent<Text>().text = structureName;
		}
		
		Select(0);
	}
	
	void Select (int selection) {
		if (selection > structures.Length - 1) {
			selection = 0;
		} else if (selection < 0) {
			selection = structures.Length - 1;
		}
		uiParent.GetChild(selectedStructure).GetComponent<Image>().color = Color.white;
		selectedStructure = selection;
		uiParent.GetChild(selectedStructure).GetComponent<Image>().color = Color.green;
		buildController.ChangeStructure(structures[selectedStructure]);
	}
	
	void Update () {
		if (Input.mouseScrollDelta.y > 0) {
			Select(selectedStructure - 1);
		} else if (Input.mouseScrollDelta.y < 0) {
			Select(selectedStructure + 1);
		}
		
		if (Input.GetKeyDown(KeyCode.Q)) {
			buildController.Stop();
		}
	}
}
