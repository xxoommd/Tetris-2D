using UnityEngine;
using System.Collections;

public class MainMenuEvent : MonoBehaviour
{

	public void OnClickPlay ()
	{
		UIController.instance.Close ("Main UI");
		GameController.instance.NewGame ();
	}
}
