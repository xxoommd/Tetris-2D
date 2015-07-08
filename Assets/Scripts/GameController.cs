﻿using UnityEngine;
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
	public GameObject wallTemplate;
	public static GameController instance = null;
	public GameObject[] tetrisTemplates;
	[HideInInspector]public bool isCleaning = false;
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

		InitWalls ();
	}

	void Update ()
	{
		if (isGameOver) {
			return;
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
		gameOverText.enabled = true;
	}
}














