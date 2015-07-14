using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{

	public static UIController instance;
	List<GameObject> uiStack;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
		uiStack = new List<GameObject> ();
	}

	public void Show (string name)
	{
		Object uiObj = Resources.Load (name);
		if (uiObj) {
			GameObject newUI = Instantiate (uiObj) as GameObject;
			newUI.name = name;
			uiStack.Add (newUI);
		}
	}

	public void Close (string name)
	{
		foreach (GameObject ui in uiStack) {
			if (ui.name == name) {
				uiStack.Remove (ui);
				Destroy (ui.gameObject);
				return;
			}
		}
	}

	public void CloseTop ()
	{
		if (uiStack.Count > 0) {
			GameObject last = uiStack[uiStack.Count - 1];

			if (last) {
				uiStack.Remove (last);
				Destroy (last.gameObject);
			}
		}
	}

}
