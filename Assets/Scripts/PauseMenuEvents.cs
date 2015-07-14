using UnityEngine;
using System.Collections;

public class PauseMenuEvents : MonoBehaviour {

	public void OnClickQuit () {
		GameController.instance.QuitGame ();
		UIController.instance.Close ("Pause UI");
		UIController.instance.Show ("Main UI");
	}

	public void OnClickResume () {
		GameController.instance.isPaused = false;
		UIController.instance.Close ("Pause UI");
	}
}
