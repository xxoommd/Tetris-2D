using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
	[HideInInspector]
	public GameObject[,]
		brickBoxes;
	public GameObject wall;
	GameController gameController;
	//
	private List<int> rowsToClear;

	void Start ()
	{
		gameController = GameController.instance;

		InitWalls ();

		brickBoxes = new GameObject[gameController.playground.width, gameController.playground.height];
		rowsToClear = new List<int> ();
	}

	void InitWalls ()
	{
		for (int x = 0; x < gameController.playground.width; x++) {
			Instantiate (wall, new Vector3 (x, -1, 0), Quaternion.identity);
			Instantiate (wall, new Vector3 (x, gameController.playground.height, 0), Quaternion.identity);
		}

		for (int y = -1; y < gameController.playground.height + 1; y++) {
			Instantiate (wall, new Vector3 (-1, y, 0), Quaternion.identity);
			Instantiate (wall, new Vector3 (gameController.playground.width, y, 0), Quaternion.identity);
		}
	}

	public void CheckClear ()
	{
		rowsToClear.Clear ();
		for (int y = 0; y < gameController.playground.height; y++) {
			bool canClear = true;

			for (int x = 0; x < gameController.playground.width; x++) {
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
				for (int x = 0; x < gameController.playground.width; x++) {
					Destroy (brickBoxes [x, y]);
					brickBoxes [x, y] = null;
				}
			}

			foreach (int y0 in rowsToClear) {
				for (int y = y0; y < gameController.playground.height; y++) {
					for (int x = 0; x < gameController.playground.width; x++) {
						if (brickBoxes [x, y] != null) {
							Vector2 newPos = brickBoxes [x, y].transform.position;
							newPos.y--;
							brickBoxes [x, y].transform.position = newPos;
						}
					}
				}
			}

			RelocateBricksByTheirPositions ();

		}
	}

	private void RelocateBricksByTheirPositions ()
	{
		GameObject[,] newBrickBoxes = new GameObject[gameController.playground.width, gameController.playground.height];

		for (int x = 0; x < gameController.playground.width; x++) {
			for (int y = 0; y < gameController.playground.height; y++) {
				if (brickBoxes [x, y] != null) {
					int x0 = (int)brickBoxes [x, y].transform.position.x;
					int y0 = (int)brickBoxes [x, y].transform.position.y;
					newBrickBoxes [x0, y0] = brickBoxes [x, y];
				}
			}
		}

		brickBoxes = newBrickBoxes;
	}
}
