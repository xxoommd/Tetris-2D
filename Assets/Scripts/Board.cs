using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
	[HideInInspector]
	public bool[,] brickMatrix;
	GameController gameController;
	[HideInInspector]public int maxHeight = 0;
	public GameObject garbageTemplate;
	[HideInInspector]public GameObject garbage;

	void Start ()
	{
		gameController = GameController.instance;
		garbage = Instantiate (garbageTemplate) as GameObject;

		brickMatrix = new bool[gameController.playground.width, gameController.playground.height];
		InitBrickMatrix ();
	}

	void OnDestroy () {
		if (garbage) {
			Destroy (garbage);
		}
	}

	void InitBrickMatrix () {
		for (int x = 0; x < gameController.playground.width; x++) {
			for (int y = 0; y < gameController.playground.height; y++) {
				brickMatrix[x, y] = false;
			}
		}
	}

	bool CanClearLine (int y)
	{
		for (int x = 0; x < gameController.playground.width; x++) {
			if (!brickMatrix [x, y]) {
				return false;
			}
		}

		return true;
	}

	void ClearLines (ref List<int> pendingLines)
	{
		for (int i = 0; i < pendingLines.Count; i++) {
			int line = pendingLines[i];

			List<Transform> clearBricks = new List<Transform>();
			List<Transform> fallBricks = new List<Transform>();

			foreach (Transform brick in transform) {
				int currentY = (int)brick.position.y;
				if (currentY == line) {
					clearBricks.Add(brick);
				} else if (currentY > line) {
					fallBricks.Add(brick);
				}
			}

			if (clearBricks.Count > 0) {
				foreach(Transform b in clearBricks) {
					b.SetParent (garbage.transform);
					Destroy (b.gameObject);
				}
			}

			if (fallBricks.Count > 0) {
				foreach(Transform b in fallBricks) {
					b.position = new Vector2(b.position.x, b.position.y - 1f);
				}
			}
		}
	}

	void UpdateMatrix () {
		InitBrickMatrix ();

		foreach (Transform brick in transform) {
			brickMatrix[(int)brick.position.x, (int)brick.position.y] = true;
		}
	}

	void Checking ()
	{
		List<int> pendingLines = new List<int> ();
		pendingLines.Clear ();

		for (int y = maxHeight; y >= 0; y--) {
			if (CanClearLine (y)) {
				pendingLines.Add (y);
			}
		}

		if (pendingLines.Count > 0) {
			ClearLines (ref pendingLines);
			GameController.instance.AddScore (pendingLines.Count);
		}

		UpdateMatrix ();
		gameController.isCleaning = false;
	}

	public void CheckClear ()
	{
//		StartCoroutine (Checking ());
		Checking ();
	}
	
}
