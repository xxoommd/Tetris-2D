using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour
{

	public void OnClickMain ()
	{
		GameController.instance.QuitGame ();
		Destroy (gameObject);
		Instantiate (Resources.Load ("UI/Main UI") as GameObject);
	}

	public void OnClickRestart ()
	{
		GameController.instance.RestartGame ();
		Destroy (gameObject);
	}
}
