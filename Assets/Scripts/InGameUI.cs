using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUI : MonoBehaviour
{
	public static InGameUI instance = null;

	public Text scoreText;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	public void OnClickPause ()
	{
		GameController.instance.PauseGame ();
	}
}
