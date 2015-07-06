using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	//
	public static GameController instance = null;
	public GameObject[] tetrisTemplates;
	[HideInInspector]
	public Board board;
	public float fallingUnitTime = 0.1f;
	public Vector2 boundary;
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
	}

	void Start ()
	{
		board = GameObject.Find ("Board").GetComponent<Board> ();
	}

	void Update ()
	{
		if (isGameOver) {
			return;
		}

		if (currentTetris == null) {
			currentTetris = Instantiate (tetrisTemplates [Random.Range (0, tetrisTemplates.Length)]) as GameObject;
			currentTetris.transform.SetParent (board.transform);
			currentTetris.transform.position = new Vector3 (10, 28, 0);
		}
	}

	public void GameOver () {
		isGameOver = true;
		gameOverText.enabled = true;
	}
}














