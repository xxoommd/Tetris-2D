using UnityEngine;
using System.Collections;

public class MainMenuEvent : MonoBehaviour {

	public void OnClickPlay () {
		Destroy (gameObject);

		// Start Playing...
		GameController.instance.NewGame ();
	}
}
