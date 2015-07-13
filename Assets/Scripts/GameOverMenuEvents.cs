using UnityEngine;
using System.Collections;

public class GameOverMenuEvents : MonoBehaviour {

	public void OnClickMain () {
		GameController.instance.DestroyGame ();
		Destroy (gameObject);
		GameController.instance.ShowMainUI ();
	}

	public void OnClickRestart () {
		GameController.instance.RestartGame ();
		Destroy (gameObject);
	}
}
