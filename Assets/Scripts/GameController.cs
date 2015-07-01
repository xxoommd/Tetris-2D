using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
	//
	public static GameController instance = null;
	public GameObject[] tetrisTemplates;
	public GameObject board;
	public Transform spawnSpot;
	public float fallingUnitTime = 0.1f;
	public float movingUnitTime = 0.1f;
	public Vector2 boundary;

	//
	[HideInInspector]
	public Board
		boardScript;

	//
	private GameObject currentTetris = null;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		boardScript = board.GetComponent<Board> ();
	}

	void Update ()
	{
		if (currentTetris == null) {
			currentTetris = Instantiate (tetrisTemplates [Random.Range (0, tetrisTemplates.Length)]) as GameObject;
			currentTetris.transform.SetParent (board.transform);
			currentTetris.transform.position = spawnSpot.position;
		}
	}
}
