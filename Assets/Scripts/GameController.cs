using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{

	public static GameController instance = null;
	public GameObject[] tetrisTemplates;
	public Transform board;
	public Transform spawnSpot;
	public float tetrisMoveSpeed = 20f;
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

	void Update ()
	{
		if (currentTetris == null) {
			currentTetris = Instantiate (tetrisTemplates[Random.Range (0, tetrisTemplates.Length)]) as GameObject;
			currentTetris.transform.SetParent (board);
			currentTetris.transform.position = spawnSpot.position;
		}
	}
}
