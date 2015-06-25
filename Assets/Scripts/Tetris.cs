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

	// Methods
	protected virtual void Awake ()
	{
		InitialAllCoordinates ();

		nextPosition = Vector3.zero;
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

	protected virtual void Update ()
	{
		if (Input.GetButtonUp ("Fire1")) {
			Turn ();
		}

		int h = 0, v = 0;
		h = (int)Input.GetAxisRaw ("Horizontal");
		v = (int)Input.GetAxisRaw ("Vertical");
		if (h > 0) {
			h = 1;
		} else if (h < 0) {
			h = -1;
		} else {
			h = 0;
		}

		if (h != 0) {
			if (v > 0) {
				v = 1;
			} else if (v < 0) {
				v = -1;
			} else {
				v = 0;
			}
		}

		if (moving) {
			if (transform.localPosition == nextPosition) {
				moving = false;
			} else {
				float speed = GameController.instance.tetrisMoveSpeed * Time.deltaTime;
				transform.localPosition = Vector3.MoveTowards (transform.localPosition, nextPosition, speed);
			}
		} else {
			if (h != 0) {
				moving = true;
				nextPosition = transform.localPosition;
				nextPosition.x += h;
				nextPosition = RevisedPosition (nextPosition);
			} else if (v != 0) {
				moving = true;
				nextPosition = transform.localPosition;
				nextPosition.y += v;
				nextPosition = RevisedPosition (nextPosition);
			}
		}
	}

	protected void FixedUpdate ()
	{
		Fall ();
	}

	protected void Fall ()
	{
		if (falling) {
			if (fallPosition == transform.localPosition) {
				if (!CheckFall()) {
					TransformToBricksAndDestroy ();
					return;
				}
				falling = false;
			} else {
				float speed = GameController.instance.tetrisFallSpeed * Time.deltaTime;
				transform.localPosition = Vector3.MoveTowards (transform.localPosition, fallPosition, speed);
			}
		} else {
			fallPosition = transform.localPosition;
			if (CheckFall ()) {
				fallPosition.y -= 1;
				falling = true;
			} else {
				TransformToBricksAndDestroy ();
				return;
			}
		}
	}

	protected bool CheckFall ()
	{
		if (transform.localPosition.y < 1f) {
			return false;
		}

		return true;
	}

	protected void TransformToBricksAndDestroy ()
	{
		foreach (GameObject brick in bricks) {
			brick.transform.SetParent(GameController.instance.board.transform);
			int x = (int)brick.transform.position.x;
			int y = (int)brick.transform.position.y;
			GameController.instance.boardScript.brickBoxes[x, y] = brick;
		}

//		Destroy (gameObject);
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
