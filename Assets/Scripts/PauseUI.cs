using UnityEngine;
using System.Collections;

public class PauseUI : MonoBehaviour
{
	private GameScene gameScene = null;
	
	void Start () {
		gameScene = GameController.instance.gameScene;
	}


	public void OnClickQuit ()
	{
		GameController.instance.QuitGame ();
		GameController.instance.ShowUI ("Main UI");
		Destroy (gameObject);
	}

	public void OnClickResume ()
	{
		GameController.instance.isGamePause = false;
		gameScene.inGameUI.pauseButton.interactable = true;

		Destroy (gameObject);
	}
}
