using UnityEngine;
using System.Collections;

public class PauseMenuEvents : MonoBehaviour {

	public void OnClickQuit () {
		Debug.Log ("=== Click Quit button ===");
		GameController.instance.DestroyGame ();
		Destroy (gameObject);

		GameController.instance.ShowMainUI ();
	}

	public void OnClickResume () {
		Debug.Log ("=== Click Resume button ===");
	}
}
