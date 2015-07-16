using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScene : MonoBehaviour {

	[HideInInspector] public Board board;
	[HideInInspector] public GameObject garbage;
	[HideInInspector] public InGameUI inGameUI;

	[HideInInspector] public bool isCleaning = false;

	private Dictionary<string, TetrisData> tetrisDictionary = null;
	private GameObject currentTetris = null;
	private LevelData levelData = null;
	private bool setDataFinished = false;

	private float timeLeft = 0f;
	private float dt = 0f;


	void Start () {
		// Init tetris templates.

		board = transform.FindChild ("Board").gameObject.GetComponent<Board> ();
		garbage = transform.FindChild ("Garbage").gameObject;
		inGameUI = transform.FindChild ("In Game UI").gameObject.GetComponent<InGameUI> ();
	}

	void Update () {
		if (!setDataFinished || GameController.instance.isGamePause || GameController.instance.isGameOver) {
			return;
		}

		dt += Time.deltaTime;
		if (dt >= 1f) {
			timeLeft -= 1f;
			UpdateTime ();
			dt = 0f;
		}

		if (GameController.instance.scoreDirty) {
			UpdateScore ();
		}

		if (CheckGameOver ()) {
			GameController.instance.GameOver ();
		}

		PlayGround playground = GameController.instance.playground;

		if (currentTetris == null && !isCleaning) {
			currentTetris = Instantiate (RandomTetrisTemplate ()) as GameObject;
			currentTetris.transform.position = new Vector3 (playground.width / 2, playground.height - 2, 0);
			currentTetris.transform.SetParent (transform);
		}
	}


	public void SetData (LevelData lvData) {
		levelData = lvData;
		timeLeft = levelData.limitTime;
		if (tetrisDictionary == null) {
			tetrisDictionary = new Dictionary<string, TetrisData> ();
		}

		tetrisDictionary.Clear ();

		string[] tetrisNames = {"I", "J", "L", "S", "Z", "O", "T"};
		for (int i = 0; i < tetrisNames.Length; i++) {

			string name = tetrisNames [i];
			byte weight = 0;

			switch (name) {
			case "I":
				weight = levelData.weightI;
				break;
			case "J":
				weight = levelData.weightJ;
				break;
			case "L":
				weight = levelData.weightL;
				break;
			case "S":
				weight = levelData.weightS;
				break;
			case "Z":
				weight = levelData.weightZ;
				break;
			case "O":
				weight = levelData.weightO;
				break;
			case "T":
				weight = levelData.weightT;
				break;
			}

			GameObject go = Resources.Load ("Tetris/Tetris " + name) as GameObject;
			TetrisData data = new TetrisData (weight, go);
			tetrisDictionary.Add (name, data);
		}
	}

	GameObject RandomTetrisTemplate () {
		ushort totalWeight = 0;
		
		foreach (TetrisData data in tetrisDictionary.Values) {
			totalWeight += data.weight;
		}
		
		int randomNumber = Random.Range (0, totalWeight) + 1;
		foreach (string key in tetrisDictionary.Keys) {
			TetrisData data = tetrisDictionary[key];
			if (data.weight < 1) {
				continue;
			}
			
			randomNumber -= data.weight;
			
			if (randomNumber < 1) {
				return data.template;
			}
		}
		
		return null;
	}

	bool CheckGameOver () {
		if (GameController.instance.playerData.score >= levelData.aimClearings) {
			return true;
		} else if (timeLeft <= float.Epsilon /* 0 */) {
			return true;
		} else {
			return false;
		}
	}

	void UpdateScore () {
		inGameUI.scoreText.text = "Score: " + GameController.instance.playerData.score;
		GameController.instance.scoreDirty = false;
	}

	void UpdateTime () {
		uint min = (uint)timeLeft / 60;
		uint sec = (uint)timeLeft % 60;
		inGameUI.timeText.text = min.ToString () + ":" + sec.ToString ();
	}
}
