using UnityEngine;
using System.Collections;

public class MainMenuEvent : MonoBehaviour {

	public void OnClickPlay () {
		Debug.Log ("--- On Click Play ---");
		Destroy (gameObject);

		// Start Playing...
		GameController.instance.InitGame ();
	}
}
