using UnityEngine;
using System.Collections;

public class InGameUiEvents : MonoBehaviour {

	public void OnClickPause () {
		GameController.instance.PauseGame ();
	}
}
