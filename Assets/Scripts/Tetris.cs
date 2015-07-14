using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public abstract class Tetris : MonoBehaviour
{
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

	// Keyboard: Long press makes move faster
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
	private bool leftPressedEnabled = false;
	private float leftPressedTime = 0f;
	private bool rightPressedEnabled = false;
	private float rightPressedTime = 0f;
#endif

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
	private Vector2 touchOrigin = -Vector2.zero;
	private bool touchEnabled = false;
	private float fromTouchBegan = 0f;
	private bool isMoved = false;
	private bool isHorizontalMoved = false;
	private bool isVerticalMoved = false;
#endif


	// Inheritated Methods from MonoBehaviour
	protected virtual void Awake ()
	{
		InitialAllCoordinates ();

		GameObject brick = getBrickTemplate ();

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

	void ReviseInitPosition ()
	{
		int leakY = 0;
		foreach (GameObject brick in bricks) {
			int y = (int)brick.transform.position.y;
			int maxHeight = GameController.instance.playground.height - 2;
			if (y - maxHeight > leakY) {
				leakY = y - maxHeight;
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

		if (GameController.instance.isPaused) {
			return;
		}

		if (reachBottom) {
			TransferToBricksAndDestroy ();
			return;
		}


#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
		// Define controllers: Keyboard

		//   'Fire1' or 'Arrow Up' -> Turn
		if (Input.GetKeyUp (KeyCode.UpArrow) || Input.GetKeyUp (KeyCode.W)) {
			int revisedX = 0;
			if (CanTurn (ref revisedX)) {
				StartCoroutine (Turn (revisedX));
			}
		}

		//   'Arrow Left' or 'Right' -> Horizontal movement by one unit
		if (Input.GetKey (KeyCode.LeftArrow)) {
			leftPressedEnabled = true;
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			rightPressedEnabled = true;
		}

		if (leftPressedEnabled) {
			leftPressedTime += Time.deltaTime;
		}

		if (rightPressedEnabled) {
			rightPressedTime += Time.deltaTime;
		}

		int h = (int)Input.GetAxisRaw ("Horizontal");
		if (h != 0 && CanHorizontalMove (h)) {
			float moveDelay = 0.1f;
			if ((h < 0 && leftPressedTime > 0.1f) || (h > 0 && rightPressedTime > 0.1f)) {
				moveDelay = 0.01f;
			}

			StartCoroutine (HorizontalMove (h, moveDelay));
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			leftPressedTime = 0f;
			leftPressedEnabled = false;
		}

		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			rightPressedTime = 0f;
			rightPressedEnabled = false;
		}

		//   'Down' -> Fall one unit immediately
		int v = (int)Input.GetAxisRaw ("Vertical");
		if (v == -1 && CanMoveDownByOneStep ()) {
			StartCoroutine (MoveDownOneStep ());
		}

		//   'Space' -> Fall down immediately
		if (Input.GetKeyUp (KeyCode.Space)) {
			FallDownToBottom ();
		}

#endif

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
		if (Input.touchCount > 0) {
			Touch myTouch = Input.touches[0];

			if (myTouch.phase == TouchPhase.Began) {
				touchEnabled = true;
				touchOrigin = myTouch.position;
				fromTouchBegan = 0f;
			} else if (myTouch.phase == TouchPhase.Ended) {
				if (touchEnabled && fromTouchBegan < 0.2f && !isMoved) {
					int revisedX = 0;
					if (CanTurn (ref revisedX)) {
						StartCoroutine (Turn (revisedX));
					}
				}

				isMoved = false;
				isHorizontalMoved = false;
				isVerticalMoved = false;
			} else if (myTouch.phase == TouchPhase.Moved) {
				isMoved = true;

				do {
					float movedX = myTouch.position.x - touchOrigin.x;
					float movedY = myTouch.position.y - touchOrigin.y;

					if (!touchEnabled) {
						break;
					}

					if (Mathf.Abs(movedX) > Mathf.Abs(movedY)) {
						if (isVerticalMoved) {
							break;
						}

						int ht = 0;
						if (movedX > 0) {
							ht = 1;
						} else if (movedX < 0) {
							ht = -1;
						}
						
						if (ht != 0 && CanHorizontalMove (ht)) {
							isHorizontalMoved = true;
							StartCoroutine (HorizontalMove (ht, 0.07f));
						}
					} else {
						if (isHorizontalMoved) {
							break;
						}

						int vt = 0;
						if (movedY < 0) {
							vt = -1;
						}

						if (vt != 0 && CanVerticalMove(vt)) {
							isVerticalMoved = true;
							StartCoroutine (VerticalMove(vt, 0.04f));
						}
					}

				} while (false);

				touchOrigin = myTouch.position;
			} else if (myTouch.phase == TouchPhase.Ended) {
				isMoved = false;
				isHorizontalMoved = false;
				isVerticalMoved = false;
			}

			fromTouchBegan += Time.deltaTime;
		}
#endif
	}

	// Self-defined methods
	public bool CheckValid ()
	{
		foreach (GameObject brick in bricks) {
			int x = (int)brick.transform.position.x;
			int y = (int)brick.transform.position.y;

			if (GameController.instance.board.brickMatrix [x, y]) {
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
		for (int absX = 0; absX < GameController.instance.playground.width; absX++) {
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
						
						if (x < 0 || x >= GameController.instance.playground.width || board.brickMatrix [x, y]) {
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
					if (nextX < 0 || nextX >= GameController.instance.playground.width || board.brickMatrix [nextX, nextY]) {
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
				if (nextY < 0 || nextY >= GameController.instance.playground.height || board.brickMatrix [nextX, nextY]) {
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

			if (GameController.instance.isPaused) {
				yield return new WaitForSeconds (0.1f);
				continue;
			}

			if (autoFallDisabled) {
				yield return new WaitForSeconds (0.1f);
				continue;
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

	void TransferToBricksAndDestroy ()
	{
		foreach (GameObject brick in bricks) {
			brick.transform.SetParent (board.transform);

			int x = (int)brick.transform.position.x;
			int y = (int)brick.transform.position.y;
			board.brickMatrix [x, y] = true;

			if (board.maxHeight < y) {
				board.maxHeight = y;
			}
		}

		GameController.instance.isCleaning = true;
		board.CheckClear ();

		Destroy (gameObject);

	}

	protected abstract void InitialAllCoordinates ();
	protected abstract GameObject getBrickTemplate ();
}















