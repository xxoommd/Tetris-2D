using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUI : MonoBehaviour
{
	public Text scoreText;

	public void OnClickPause ()
	{
		GameController.instance.PauseGame ();
	}
}
