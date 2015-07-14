using UnityEngine;
using System.Collections;

public class GameOverMenuEvents : MonoBehaviour {

	public void OnClickMain () {
		GameController.instance.QuitGame ();
		Destroy (gameObject);
		UIController.instance.Show ("Main UI");
	}

	public void OnClickRestart () {
		GameController.instance.RestartGame ();
		Destroy (gameObject);
	}
}
