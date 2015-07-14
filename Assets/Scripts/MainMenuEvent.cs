using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuEvent : MonoBehaviour
{
	public Image background;

	void Awake () {
		if (Screen.height > 1024) {
			background.flexibleWidth = Screen.height;
			background.flexibleHeight = Screen.height;
		}
	}

	public void OnClickPlay ()
	{
		UIController.instance.Close ("Main UI");
		GameController.instance.NewGame ();
	}
}
