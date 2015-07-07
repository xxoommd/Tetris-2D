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
	// Tetris templates
	public GameObject[] brickTemplates;
	[HideInInspector]
	public GameObject brickI;
	[HideInInspector]
	public GameObject brickJ;
	[HideInInspector]
	public GameObject brickL;
	[HideInInspector]
	public GameObject brickS;
	[HideInInspector]
	public GameObject brickZ;
	[HideInInspector]
	public GameObject brickO;
	[HideInInspector]
	public GameObject brickT;


	//
	public static GameController instance = null;
	public GameObject[] tetrisTemplates;
	[HideInInspector]
	public Board board;
	public float fallingUnitTime = 0.1f;
	public PlayGround playground;
	public Text gameOverText;
	//
	private GameObject currentTetris = null;
	private bool isGameOver = false;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
		gameOverText.enabled = false;

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
		board = GameObject.Find ("Board").GetComponent<Board> ();

		// Adjust main camera.
		Camera camera = Camera.main;
		camera.transform.position = new Vector3 (playground.width / 2 - 0.5f, playground.height / 2 - 0.5f, camera.transform.position.z);
		camera.orthographicSize = playground.height / 2 + 1f;
	}

	void Update ()
	{
		if (isGameOver) {
			return;
		}

		if (currentTetris == null) {
			currentTetris = Instantiate (tetrisTemplates [Random.Range (0, tetrisTemplates.Length)]) as GameObject;
			currentTetris.transform.SetParent (board.transform);
			currentTetris.transform.position = new Vector3 (playground.width / 2, playground.height - 2, 0);
		}
	}

	public void GameOver ()
	{
		isGameOver = true;
		gameOverText.enabled = true;
	}
}














