using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{
	[HideInInspector]
	public GameObject[,] brickBoxes;

	void Awake () {
		brickBoxes = new GameObject[20,30];
	}

	public void CheckClear() {
		for (int y = 0; y < 30; y++) {
			bool canClear = true;

			for (int x = 0; x < 20; x++) {
				if (brickBoxes[x, y] == null) {
					canClear = false;
					break;
				}
			}

			if (canClear) {

			}
		}
	}
}
