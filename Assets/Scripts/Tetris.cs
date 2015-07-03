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
	private bool upMoveDisabled = false;
	private bool reachBottom = false;
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
		board = GameController.instance.board;
		StartCoroutine (AutoFall (0.5f));
	}

	protected virtual void Update ()
	{
		if (reachBottom) {
			StartCoroutine (TransferToBricksAndDestroy ());
			return;
		}
		// Define controllers: PC & Mac & Web & HTML5
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
		// Define key functions:

		/* 'Arrow Up' -> Turn */
		if (Input.GetButtonUp ("Fire1") || Input.GetKeyUp (KeyCode.UpArrow) && !turning) {
			StartCoroutine (Turn ());
		}

		//   'Arrow Left' or 'Right' -> Horizontal movement by one unit
		int h = (int)Input.GetAxisRaw ("Horizontal");
		if (h != 0 && CanHorizontalMove (h)) {
			StartCoroutine (HorizontalMove (h));
		}

		//   'Down' -> Fall one unit immediately
		int v = (int)Input.GetAxisRaw ("Vertical");
		if (v == -1 && CanVerticalMove (-1)) {
			StartCoroutine (VerticalMove(-1));
		}

		//   'Space' -> Fall down immediately
		if (Input.GetKeyUp(KeyCode.Space)) {
			FallDownToBottom();
		}

#elif UNITY_IOS
		// TODO: Touch Controllers
#endif
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

	bool CanHorizontalMove (int h)
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

		return false;
	}

	bool CanVerticalMove (int v)
	{
		if (v > 0 && upMoveDisabled) {
			return false;
		}

		if (v != 0) {
			if (falling) {
				return false;
			} else {
				foreach (GameObject brick in bricks) {
					int nextX = (int)brick.transform.position.x;
					int nextY = (int)brick.transform.position.y + v;
					if (nextY < 0 || nextY > (int)GameController.instance.boundary.y || board.brickBoxes [nextX, nextY] != null) {
						reachBottom = true;
						return false;
					}
				}

				return true;
			}
		}

		return false;
	}

	bool CanAutoFall ()
	{
		return CanVerticalMove (-1);
	}

	IEnumerator AutoFall (float fallingSpeed = 0.05f)
	{
		while (CanAutoFall()) {
			if (reachBottom || !autoFallingEnabled) {
				break;
			}

			Vector2 pos = transform.position;
			pos += new Vector2 (0, -1);
			transform.position = pos;
			
			yield return new WaitForSeconds (fallingSpeed);
		}
	}

	void FallDownToBottom ()
	{
		int fallDepth = 0;

		while (CanVerticalMove(fallDepth - 1)) {
			fallDepth -= 1;
		}

		StartCoroutine (VerticalMove (fallDepth));
	}

	IEnumerator HorizontalMove (int h, float moveDelay = 0.05f)
	{
		moving = true;

		Vector2 pos = transform.position;
		pos += new Vector2 (h, 0);
		transform.position = pos;

		yield return new WaitForSeconds (moveDelay);

		moving = false;
	}

	IEnumerator VerticalMove (int v, float moveDelay = 0.05f)
	{
		falling = true;

		Vector2 pos = transform.position;
		pos += new Vector2 (0, v);
		transform.position = pos;

		yield return new WaitForSeconds (moveDelay);

		falling = false;
	}

	IEnumerator TransferToBricksAndDestroy ()
	{
		foreach (GameObject brick in bricks) {
			int x = (int)brick.transform.position.x;
			int y = (int)brick.transform.position.y;

			board.brickBoxes [x, y] = brick;
			brick.transform.SetParent (board.transform);
		}

		Destroy (gameObject);
		board.CheckClear ();
		yield return null;
	}

	protected abstract void InitialAllCoordinates ();
}















