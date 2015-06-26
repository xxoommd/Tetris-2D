using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Boundary
{
	public int xMin, xMax, yMin, yMax;

	public Boundary (int xmin, int xmax, int ymin, int ymax)
	{
		xMin = xmin;
		xMax = xmax;
		yMin = ymin;
		yMax = ymax;
	}
}

public abstract class Tetris : MonoBehaviour
{

	// Public
	public GameObject[] brickTemplates;

	// Protected
	protected GameObject[] bricks;
	protected Vector2[,] coordinates;
	protected Boundary[] boundary;
	protected int directionSize;


	// private
	private int directionIndex;
	private Vector3 nextPosition;
	private bool moving = false;
	private Vector3 fallPosition;
	private bool falling = false;
	private float xDir = 0;
	private float yDir = 0;
	private Vector3 targetPosition;

	// Methods
	protected virtual void Awake ()
	{
		InitialAllCoordinates ();

		nextPosition = Vector3.zero;
		targetPosition = Vector3.zero;
		moving = false;

		GameObject brick = brickTemplates [Random.Range (0, brickTemplates.Length)];

		directionIndex = Random.Range (0, directionSize);
		bricks = new GameObject[4];

		for (int i = 0; i < 4; i++) {
			GameObject obj = Instantiate (brick);
			obj.transform.SetParent (transform);
			obj.transform.localPosition = coordinates [directionIndex, i];
			bricks [i] = obj;
		}

		nextPosition = transform.position;
	}

	protected virtual void Start () {
		xDir = transform.localPosition.x;
	}

	protected virtual void Update ()
	{
		if (Input.GetButtonUp ("Fire1")) {
			Turn ();
		}

		int h = (int)Input.GetAxisRaw ("Horizontal");
		if (!moving && CanMove(h)) {
			if (h != 0) {
				Move (h);
			}
		}

		if (!falling) {
			if (CanFall ()) {
				Fall ();
			} else {
				TransformToBricksAndDestroy ();
			}
		}
	}

	protected void FixedUpdate ()
	{
		Vector3 moveTranslate = Vector3.zero;
		if (moving) {
			if (Mathf.Abs(transform.localPosition.x - xDir) > 0.01f) {
				moveTranslate.x = (xDir - transform.localPosition.x) * GameController.instance.tetrisMoveSpeed * Time.deltaTime;
			} else {
				Vector3 newPosition = transform.localPosition;
				newPosition.x = xDir;
				transform.localPosition = newPosition;
				moving = false;
			}
		}

		if (falling) {
			if (Mathf.Abs(transform.localPosition.y - yDir) > 0.01f) {
				moveTranslate.y = (yDir - transform.localPosition.y) * GameController.instance.tetrisFallSpeed * Time.deltaTime;
			} else {
				Vector3 newPosition = transform.localPosition;
				newPosition.y = yDir;
				transform.localPosition = newPosition;
				falling = false;
			}
		}

		if (moveTranslate != Vector3.zero) {
			transform.Translate (moveTranslate);
		}
	}

	protected void Move (int h)
	{
		moving = true;
		xDir = transform.localPosition.x + h;
		Boundary bound = boundary [directionIndex];
		xDir = xDir < bound.xMin ? bound.xMin : xDir;
		xDir = xDir > bound.xMax ? bound.xMax : xDir;
	}

	protected bool CanMove(int h) {
		foreach (GameObject brick in bricks) {
			Vector3 pos = brick.transform.position;
			int x = (int)pos.x + h;
			x = x < 0 ? 0 : x;
			x = x > 19 ? 19 : x;
			int y = (int)pos.y;
			if (GameController.instance.boardScript.brickBoxes [x, y] != null) {
				return false;
			}
		}

		return true;
	}

	protected void Fall ()
	{
		falling = true;
		yDir = transform.localPosition.y - 1;
		Boundary bound = boundary [directionIndex];
		yDir = yDir < bound.yMin ? bound.yMin : yDir;
		yDir = yDir > bound.yMax ? bound.yMax : yDir;
	}

	protected bool CanFall ()
	{
		if (transform.localPosition.y < float.Epsilon) {
			return false;
		}

		foreach (GameObject brick in bricks) {
			Vector3 pos = brick.transform.position;
			int x = (int)pos.x;
			int y = (int)pos.y - 1;
			y = y < 0 ? 0 : y;
			if (GameController.instance.boardScript.brickBoxes [x, y] != null) {
				return false;
			}
		}

		return true;
	}

	protected void TransformToBricksAndDestroy ()
	{
		WaitForSeconds (0.5f);
		// Revise position
		Vector3 newPosition = new Vector3 (xDir, yDir, 0f);
		transform.localPosition = newPosition;

		foreach (GameObject brick in bricks) {
			brick.transform.SetParent (GameController.instance.board.transform);
			int x = (int)brick.transform.position.x;
			int y = (int)brick.transform.position.y;
			GameController.instance.boardScript.brickBoxes [x, y] = brick;
		}

		Destroy (gameObject);
		GameController.instance.boardScript.CheckClear ();
	}

	protected void Turn ()
	{
		directionIndex = (directionIndex + 1) % directionSize;

		for (int i = 0; i < 4; i++) {
			GameObject brick = bricks [i];
			brick.transform.localPosition = coordinates [directionIndex, i];
		}

		transform.position = RevisedPosition (transform.position);
	}

	private Vector3 RevisedPosition (Vector3 position)
	{
		Vector3 newPosition = position;
		Boundary by = boundary [directionIndex];
		newPosition.x = newPosition.x < by.xMin ? by.xMin : newPosition.x;
		newPosition.x = newPosition.x > by.xMax ? by.xMax : newPosition.x;
		newPosition.y = newPosition.y < by.yMin ? by.yMin : newPosition.y;
		newPosition.y = newPosition.y > by.yMax ? by.yMax : newPosition.y;

		return newPosition;
	}

	protected abstract void InitialAllCoordinates ();
}
