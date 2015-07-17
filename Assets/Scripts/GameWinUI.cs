using UnityEngine;
using System.Collections;

public class GameWinUI : GameOverUI {

	public void OnClickGoOn () {
		GameController.instance.GoToNextLevel ();
	}
}
