using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuEvent : MonoBehaviour
{
	public Image background;

	void Awake () {
		if (Screen.height > background.rectTransform.sizeDelta.x) {
			background.rectTransform.sizeDelta = new Vector2 (Screen.height, Screen.height);
		}
	}

	public void OnClickPlay ()
	{
		UIController.instance.Close ("Main UI");
		GameController.instance.NewGame ();
	}
}
