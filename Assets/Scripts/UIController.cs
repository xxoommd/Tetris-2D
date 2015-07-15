using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{

	public static UIController instance;
	List<GameObject> uiList;
	public GameObject currentUI {
		get {
			if (uiList.Count > 0) {
				return uiList[uiList.Count - 1];
			} else {
				return null;
			}
		}
	}

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
		uiList = new List<GameObject> ();
	}

	public GameObject FindUI (string name) {
		foreach (GameObject ui in uiList) {
			if (ui.name == name) {
				return ui;
			}
		}

		return null;
	}

	public void Show (string name)
	{
		foreach (GameObject ui in uiList) {
			if (ui.name == name) {
				Debug.Log ("--- " + name + " already shown ---");
				return;
			}
		}

		Object uiObj = Resources.Load ("UI/" + name);
		if (uiObj) {
			GameObject newUI = Instantiate (uiObj) as GameObject;
			newUI.name = name;
			uiList.Add (newUI);
		} else {
			Debug.Log ("--- UI prefab not found: " + name + " ---");
		}
	}

	public void Close (string name)
	{
		foreach (GameObject ui in uiList) {
			if (ui.name == name) {
				uiList.Remove (ui);
				Destroy (ui.gameObject);
				return;
			}
		}
	}

	public void CloseTop ()
	{
		if (uiList.Count > 0) {
			GameObject last = uiList [uiList.Count - 1];

			if (last) {
				uiList.Remove (last);
				Destroy (last.gameObject);
			}
		}
	}

}
