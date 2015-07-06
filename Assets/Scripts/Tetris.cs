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
	private bool initialOK = false;
	private bool turning = false;
	private bool moving = false;
	private bool upMoveDisabled = false;
	private bool autoFallDisabled = false;
	private bool movingDown = false;
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
		ReviseInitPosition ();
		if (!CheckValid ()) {
			GameController.instance.GameOver ();
		}

		board = GameController.instance.board;
		StartCoroutine (AutoFall ());
	}

	void ReviseInitPosition () {
		int leakY = 0;
		foreach (GameObject brick in bricks) {
			int y = (int)brick.transform.position.y;
			if (y - 28 > leakY) {
				leakY = y - 28;
			}
		}
		
		if (leakY != 0) {
			Vector3 newPosition = transform.position;
			newPosition.y -= leakY;
			transform.position = newPosition;
		}
		initialOK = true;
	}
	
	protected virtual void Update ()
	{
		if (!initialOK) {
			return;
		}

		if (reachBottom) {
			StartCoroutine (TransferToBricksAndDestroy ());
			return;
		}


#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
		// Define controllers: Keyboard

		//   'Fire1' or 'Arrow Up' -> Turn
		int revisedX = 0;
		if (Input.GetButtonUp ("Fire1") || Input.GetKeyUp (KeyCode.UpArrow)) {
			if (CanTurn (ref revisedX)) {
				StartCoroutine (Turn (revisedX));
			}
		}

		//   'Arrow Left' or 'Right' -> Horizontal movement by one unit
		int h = (int)Input.GetAxisRaw ("Horizontal");
		if (h != 0 && CanHorizontalMove (h)) {
			StartCoroutine (HorizontalMove (h, GameController.instance.movingUnitTime));
		}

		//   'Down' -> Fall one unit immediately
		int v = (int)Input.GetAxisRaw ("Vertical");
		if (v == -1 && CanMoveDownByOneStep ()) {
			StartCoroutine (MoveDownOneStep ());
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
	public bool CheckValid ()
	{
		foreach (GameObject brick in bricks) {
			int x = (int)brick.transform.position.x;
			int y = (int)brick.transform.position.y;

			if (GameController.instance.board.brickBoxes [x, y] != null) {
				return false;
			}
		}

		return true;
	}

	void Move (int h, int v)
	{
		Vector2 pos = transform.position;
		pos += new Vector2 (h, v);
		transform.position = pos;
	}

	bool CanTurn (ref int revisedX)
	{
		if (turning) {
			return false;
		}

		GameObject tetrisClone = new GameObject ();
		tetrisClone.transform.SetParent (transform.parent);
		tetrisClone.transform.localPosition = transform.localPosition;
		GameObject[] bricksClone = new GameObject[4];

		int nextDirectionIndex = (directionIndex + 1) % directionSize;

		for (int i = 0; i < 4; i++) {
			bricksClone [i] = new GameObject ();
			bricksClone [i].transform.SetParent (tetrisClone.transform);
			bricksClone [i].transform.localPosition = coordinates [nextDirectionIndex, i];
		}

		// Calculating Revised X;
		int[] directionFactors = new int[]{-1, 1};
		bool[] directionEnabled = new bool[] {true, true};
		for (int absX = 0; absX < 20; absX++) {
			for (int i = 0; i < 2; i++) {
				if (!directionEnabled [i]) {
					continue;
				}

				int directionFactor = directionFactors [i];
				int movedX = absX * directionFactor;

				if (CanHorizontalMove (movedX)) {
					int potentialX = (int)(transform.position.x) + absX * directionFactor;
					
					Vector2 pos = transform.position;
					pos.x = potentialX;
					tetrisClone.transform.position = pos;
					
					bool isOK = true;
					foreach (GameObject brick in bricksClone) {
						int x = (int)brick.transform.position.x;
						int y = (int)brick.transform.position.y;
						
						if (x < 0 || x > 19 || board.brickBoxes [x, y] != null) {
							isOK = false;
							break;
						}
					}
					
					if (isOK) {
						revisedX = potentialX - (int)transform.position.x;
						Destroy (tetrisClone);
						return true;
					}
				} else {
					directionEnabled [i] = false;
				}

			}
		}

		Destroy (tetrisClone);
		return false;
	}

	IEnumerator Turn (int revisedX = 0)
	{
		turning = true;
		directionIndex = (directionIndex + 1) % directionSize;

		for (int i = 0; i < 4; i++) {
			GameObject brick = bricks [i];
			brick.transform.localPosition = coordinates [directionIndex, i];
		}

		if (revisedX != 0) {
			Move (revisedX, 0);
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
		} else {
			return true;
		}
	}

	bool CanVerticalMove (int v)
	{
		if (v > 0 && upMoveDisabled) {
			return false;
		}

		if (v != 0) {

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

		return false;
	}

	bool CanAutoFall ()
	{
		return CanVerticalMove (-1);
	}

	IEnumerator AutoFall ()
	{
		while (CanAutoFall()) {
			if (reachBottom) {
				break;
			}

			if (autoFallDisabled) {
				yield return new WaitForSeconds (0.1f);
			}

			Move (0, -1);
			
			yield return new WaitForSeconds (GameController.instance.fallingUnitTime);
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

		Move (h, 0);

		yield return new WaitForSeconds (moveDelay);

		moving = false;
	}

	IEnumerator VerticalMove (int v, float moveDelay = 0.05f)
	{
		Move (0, v);

		yield return new WaitForSeconds (moveDelay);
	}

	bool CanMoveDownByOneStep ()
	{
		return !movingDown && CanVerticalMove (-1);
	}

	IEnumerator MoveDownOneStep ()
	{
		autoFallDisabled = true;
		movingDown = true;

		Move (0, -1);

		yield return new WaitForSeconds (0.05f);
		autoFallDisabled = false;
		movingDown = false;
	}

	IEnumerator TransferToBricksAndDestroy ()
	{
		yield return new WaitForSeconds (0.15f);

		foreach (GameObject brick in bricks) {
			int x = (int)brick.transform.position.x;
			int y = (int)brick.transform.position.y;

			board.brickBoxes [x, y] = brick;
			brick.transform.SetParent (board.transform);
		}

		Destroy (gameObject);
		board.CheckClear ();
	}

	protected abstract void InitialAllCoordinates ();
}















