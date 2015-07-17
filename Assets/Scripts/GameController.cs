using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class PlayGround
{
	public int width;
	public int height;
}

public class TetrisData
{
	public byte weight;
	public GameObject template;

	public TetrisData (byte wt, GameObject go) {
		weight = wt;
		template = go;
	}
}

public class PlayerData 
{
	public ushort world;
	public ushort level;
	public uint score;
}

public class GameController : Singleton <GameController>
{

	// Tetris templates
	[HideInInspector] public GameObject brickI;
	[HideInInspector] public GameObject brickJ;
	[HideInInspector] public GameObject brickL;
	[HideInInspector] public GameObject brickS;
	[HideInInspector] public GameObject brickZ;
	[HideInInspector] public GameObject brickO;
	[HideInInspector] public GameObject brickT;

	// Public attributes which should not show in unity inspector
	[HideInInspector] public bool isGamePause = false;
	[HideInInspector] public bool isGameOver = true;
	[HideInInspector] public GameScene gameScene;
	[HideInInspector] public PlayerData playerData;
	[HideInInspector] public bool scoreDirty = false;


	// Public attributes which will be specified in unity editor
	public float fallingUnitTime = 0.1f;
	public PlayGround playground;
	
	// Private attributes
	private GameObject gameSceneTemplate;

		

	// Inherited methods:
	protected override void Awake ()
	{
		base.Awake ();

		// Init bricks.
		Object[] temp = Resources.LoadAll ("Bricks");
		GameObject[] brickTemplates = new GameObject[temp.Length];
		for (int i = 0; i < temp.Length; i++) {
			brickTemplates [i] = temp [i] as GameObject;
		}

		brickI = brickTemplates [0];
		brickJ = brickTemplates [1];
		brickL = brickTemplates [2];
		brickS = brickTemplates [3];
		brickZ = brickTemplates [4];
		brickO = brickTemplates [5];
		brickT = brickTemplates [6];

		gameSceneTemplate = Resources.Load ("Game Scene") as GameObject;
	}

	void Start ()
	{
		// Adjust main camera.
		Camera camera = Camera.main;
		camera.transform.position = new Vector3 (playground.width / 2 - 0.5f, playground.height / 2 - 0.5f, camera.transform.position.z);
		camera.orthographicSize = playground.height / 2 + 2f;

		float viewWidth = camera.orthographicSize * 2 * camera.aspect;
		if (viewWidth < playground.width) {
			float newSize = (playground.width + 2f) / camera.aspect / 2;
			camera.orthographicSize = newSize;
		}

		// First run: Show main menu
		ShowUI ("Main UI");

		playerData = new PlayerData ();
		playerData.world = 0;
		playerData.level = 0;
	}

	void Update ()
	{
		if (isGameOver) {
			return;
		}

		if (Input.GetKeyUp (KeyCode.P)) {
			PauseGame ();
			return;
		}
	}

	// Private methods:
	void CreateGameScene () {
		if (gameScene == null) {
			gameScene = (Instantiate (gameSceneTemplate) as GameObject).GetComponent<GameScene> ();
			WorldData currentWorldData = WorldController.instance.FindWorldData (playerData.world);
			if (currentWorldData != null) {
				if (currentWorldData.levels.Length <= playerData.level) {
					playerData.world ++;
					playerData.level = 0;

					if (playerData.world >= WorldController.instance.worlds.Length) {
						Debug.Log ("--- NO AVAILABLE WORLD OF ID: " + playerData.world.ToString() + " ---");
						return;
					}
				}

				LevelData currentLevelData = WorldController.instance.FindLevelData (playerData.world, playerData.level);
				gameScene.SetData (currentLevelData);
			} else {
				Debug.Log ("--- NO AVAILABLE WORLD OF ID: " + playerData.world.ToString() + " ---");
			}
		}
	}

	// Public methods:
	public void ShowUI (string name, GameObject parent = null) {
		GameObject uiObject = Instantiate (Resources.Load ("UI/" + name) as GameObject);

		if (parent != null) {
			uiObject.transform.SetParent (parent.transform);
		}
	}

	public void NewGame ()
	{
		isGameOver = false;
		isGamePause = false;

		playerData.score = 0;
		scoreDirty = true;

		CreateGameScene ();
	}
	
	public void QuitGame ()
	{
		isGameOver = true;

		if (gameScene) {
			Destroy (gameScene.gameObject);
			gameScene = null;
		}
	}
	
	public void RestartGame ()
	{
		QuitGame ();
		NewGame ();
	}
	
	public void PauseGame ()
	{
		ShowUI ("Pause UI", gameScene.gameObject);
		isGamePause = true;
	}
	
	public void GameOver ()
	{
		isGamePause = true;
		isGameOver = true;

		if (gameScene.inGameUI) {
			gameScene.inGameUI.GameOver ();
		}
	}
	
	public void GameWin () {
		GameOver ();

		ShowUI ("Game Win UI", gameScene.gameObject);
	}

	public void GameLose () {
		GameOver ();

		ShowUI ("Game Lose UI", gameScene.gameObject);
	}

	public void GoToNextLevel () {
		QuitGame ();
		playerData.level++;
		NewGame ();
	}

	public void AddScore (uint scoreInc) {
		playerData.score += scoreInc;
		scoreDirty = true;
	}

}














