using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
	[HideInInspector]
	public GameObject[,]
		brickBoxes;
	
	//
	private List<int> rowsToClear;

	void Awake ()
	{
		brickBoxes = new GameObject[20, 30];
		rowsToClear = new List<int> ();
	}

	public void CheckClear ()
	{
		rowsToClear.Clear ();
		for (int y = 0; y < 30; y++) {
			bool canClear = true;

			for (int x = 0; x < 20; x++) {
				if (brickBoxes [x, y] == null) {
					canClear = false;
					break;
				}
			}

			if (canClear) {
				rowsToClear.Add (y);
			}
		}

		if (rowsToClear.Count > 0) {
			foreach (int y in rowsToClear) {
				for (int x = 0; x < 20; x++) {
					Destroy (brickBoxes [x, y]);
					brickBoxes [x, y] = null;
				}
			}

			foreach (int y0 in rowsToClear) {
				for (int y = y0; y < 30; y++) {
					for (int x = 0; x < 20; x++) {
						if (brickBoxes[x, y] != null) {
							Vector2 newPos = brickBoxes[x, y].transform.position;
							newPos.y--;
							brickBoxes[x, y].transform.position = newPos;
						}
					}
				}
			}

			RelocateBricksByTheirPositions();

		}
	}

	private void RelocateBricksByTheirPositions() {
		GameObject[,] newBrickBoxes = new GameObject[20, 30];

		for (int x = 0; x < 20; x++) {
			for (int y = 0; y < 30; y++) {
				if (brickBoxes[x, y] != null) {
					int x0 = (int)brickBoxes[x, y].transform.position.x;
					int y0 = (int)brickBoxes[x, y].transform.position.y;
					newBrickBoxes[x0, y0] = brickBoxes[x, y];
				}
			}
		}

		brickBoxes = newBrickBoxes;
	}
}
