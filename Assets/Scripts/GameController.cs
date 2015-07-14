using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;

[System.Serializable]
public class PlayGround
{
	public int width;
	public int height;
}

public class GameController : MonoBehaviour
{

	// Singleton instance
	public static GameController instance = null;

	// Tetris templates
	[HideInInspector] public GameObject brickI;
	[HideInInspector] public GameObject brickJ;
	[HideInInspector] public GameObject brickL;
	[HideInInspector] public GameObject brickS;
	[HideInInspector] public GameObject brickZ;
	[HideInInspector] public GameObject brickO;
	[HideInInspector] public GameObject brickT;

	// Public attributes which should not show in unity inspector
	[HideInInspector] public Board board;
	[HideInInspector] public bool isCleaning = false;
	[HideInInspector] public bool isPaused = false;
	[HideInInspector] public bool isGameOver = true;

	// Public attributes which will be specified in unity editor
	public GameObject wallTemplate;
	public GameObject[] tetrisTemplates;
	public GameObject boardObject;
	public float fallingUnitTime = 0.1f;
	public PlayGround playground;
	
	// Private attributes
	private GameObject currentTetris = null;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);

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
	}

	void Start ()
	{
		// Adjust main camera.
		Camera camera = Camera.main;
		camera.transform.position = new Vector3 (playground.width / 2 - 0.5f, playground.height / 2 - 0.5f, camera.transform.position.z);
		camera.orthographicSize = playground.height / 2 + 1f;

		UIController.instance.Show ("Main UI");
	}

	public void NewGame ()
	{
		GameObject boardObj = Instantiate (boardObject) as GameObject;
		board = boardObj.GetComponent<Board> ();

		InitWalls ();

		UIController.instance.CloseTop ();
		UIController.instance.Show ("In Game UI");

		isGameOver = false;
		isPaused = false;
	}

	public void QuitGame ()
	{
		isGameOver = true;

		GameObject wall = GameObject.Find ("Walls");
		Destroy (wall);
		Destroy (board.gameObject);

		if (currentTetris) {
			Destroy (currentTetris);
		}

		UIController.instance.Close ("In Game UI");
	}

	public void RestartGame ()
	{
		QuitGame ();
		NewGame ();
	}

	public void PauseGame ()
	{
		UIController.instance.Show ("Pause UI");
		isPaused = true;
	}

	void Update ()
	{
		if (isGameOver) {
			return;
		}

		if (Input.GetKeyUp (KeyCode.P)) {
			PauseGame ();
		}

		if (currentTetris == null && !isCleaning) {
			currentTetris = Instantiate (tetrisTemplates [Random.Range (0, tetrisTemplates.Length)]) as GameObject;
			currentTetris.transform.position = new Vector3 (playground.width / 2, playground.height - 2, 0);
		}
	}

	void InitWalls ()
	{
		GameObject wall = new GameObject ("Walls");

		for (int x = 0; x < playground.width; x++) {
			GameObject wall1 = Instantiate (wallTemplate, new Vector3 (x, -1, 0), Quaternion.identity) as GameObject;
			GameObject wall2 = Instantiate (wallTemplate, new Vector3 (x, playground.height, 0), Quaternion.identity) as GameObject;
			wall1.transform.SetParent (wall.transform);
			wall2.transform.SetParent (wall.transform);
		}
		
		for (int y = -1; y < playground.height + 1; y++) {
			GameObject wall1 = Instantiate (wallTemplate, new Vector3 (-1, y, 0), Quaternion.identity) as GameObject;
			GameObject wall2 = Instantiate (wallTemplate, new Vector3 (playground.width, y, 0), Quaternion.identity) as GameObject;
			wall1.transform.SetParent (wall.transform);
			wall2.transform.SetParent (wall.transform);
		}
	}

	public void GameOver ()
	{
		isGameOver = true;
		UIController.instance.Show ("GameOver UI");
	}
}














