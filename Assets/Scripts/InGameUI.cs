using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUI : MonoBehaviour
{
	[HideInInspector] public Text scoreText;
	[HideInInspector] public Text timeText;
	[HideInInspector] public Button pauseButton;


	void Start () {
		scoreText = transform.FindChild ("Score Text").gameObject.GetComponent<Text> ();
		timeText = transform.FindChild ("Time Text").gameObject.GetComponent<Text> ();
		pauseButton = transform.FindChild ("Pause Button").gameObject.GetComponent<Button> ();
	}

	public void OnClickPause ()
	{
		GameController.instance.PauseGame ();
	}

	public void GamePause () {
		pauseButton.interactable = false;
	}

	public void GameOver () {
		pauseButton.gameObject.SetActive (false);

		
	}
}
