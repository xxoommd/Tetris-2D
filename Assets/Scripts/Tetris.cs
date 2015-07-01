using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public abstract class Tetris : MonoBehaviour
{

	// Public variables showing in inspector
	public GameObject[] brickTemplates;

	// Protected variables
	protected GameObject[] bricks;
	protected Vector2[,] coordinates;
	protected int directionSize;
	protected int directionIndex;

	// Private variables
	private bool turning = false;
	private bool moving = false;
	private bool falling = false;
	private Board board;

	// Inheritated Methods from MonoBehaviour
	protected virtual void Awake ()
	{
		InitialAllCoordinates ();

		GameObject brick = brickTemplates [Random.Range (0, brickTemplates.Length)];

		directionIndex = Random.Range (0, directionSize);
		bricks = new GameObject[4];

		for (int i = 0; i < 4; i++) {
			GameObject obj = Instantiate (brick);
			obj.transform.SetParent (transform);
			obj.transform.localPosition = coordinates [directionIndex, i];
			bricks [i] = obj;
		}
	}

	protected virtual void Start ()
	{
		board = GameController.instance.boardScript;
	}

	protected virtual void Update ()
	{
		// Define key functions:
		//   'Up' -> Turn
		if (Input.GetButtonUp ("Fire1")/*Input.GetKeyUp (KeyCode.UpArrow)*/ && !turning) {
			StartCoroutine (Turn ());
		}

		//   'Down' -> Fall one unit immediately
		int v = (int)Input.GetAxisRaw ("Vertical");
		if (v != 0 && CanMove (0, v)) {
			StartCoroutine (Move (0, v));
		}
		//   'Left' or 'Right' -> Horizontal movement by one unit
		int h = (int)Input.GetAxisRaw ("Horizontal");
		if (h != 0 && CanMove (h, 0)) {
			StartCoroutine (Move (h, 0));
		}

		//   'Space' -> Fall down immediately

	}

	// Self-defined methods
	IEnumerator Turn ()
	{
		turning = true;
		directionIndex = (directionIndex + 1) % directionSize;

		Vector3 revisedPos = Vector3.zero;
		for (int i = 0; i < 4; i++) {
			GameObject brick = bricks [i];
			brick.transform.localPosition = coordinates [directionIndex, i];

			if (brick.transform.position.x < 0) {
				revisedPos.x = 0 - brick.transform.position.x;
			} else if (brick.transform.position.x > GameController.instance.boundary.x) {
				revisedPos.x = GameController.instance.boundary.x - brick.transform.position.x;
			}
		}

		if (revisedPos != Vector3.zero) {
			transform.position += revisedPos;
		}

		yield return new WaitForSeconds (0.1f);
		turning = false;
	}

	bool CanMove (int h, int v)
	{
		if (h != 0) {
			if (moving) {
				return false;
			} else {
				foreach (GameObject brick in bricks) {
					int nextX = (int)brick.transform.position.x + h;
					int nextY = (int)brick.transform.position.y;
					if (nextX < 0 || nextX > (int)GameController.instance.boundary.x || board.brickBoxes [nextX, nextY] != null) {
						return false;
					}
				}

				return true;
			}
		}

		if (v != 0) {
			if (falling) {
				return false;
			} else {
				foreach (GameObject brick in bricks) {
					int nextX = (int)brick.transform.position.x;
					int nextY = (int)brick.transform.position.y + v;
					if (nextY < 0 || nextY > (int)GameController.instance.boundary.y || board.brickBoxes [nextX, nextY] != null) {
						return false;
					}
				}

				return true;
			}
		}

		return false;
	}

	IEnumerator Move (int h, int v)
	{
		if (h != 0) {
			moving = true;
		}

		if (v != 0) {
			falling = true;
		}

		Vector2 pos = transform.position;
		pos += new Vector2 (h, v);
		transform.position = pos;

		yield return new WaitForSeconds (0.05f);

		if (h != 0) {
			moving = false;
		}
		
		if (v != 0) {
			falling = false;
		}
	}

	protected abstract void InitialAllCoordinates ();
}















