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
	private bool moving;

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
				Boundary by = boundary [directionIndex];
				nextPosition.x += h;
				nextPosition.x = nextPosition.x < by.xMin ? by.xMin : nextPosition.x;
				nextPosition.x = nextPosition.x > by.xMax ? by.xMax : nextPosition.x;
			} else if (v != 0) {
				moving = true;
				nextPosition = transform.localPosition;
				Boundary by = boundary [directionIndex];
				nextPosition.y += v;
				nextPosition.y = nextPosition.y < by.yMin ? by.yMin : nextPosition.y;
				nextPosition.y = nextPosition.y > by.yMax ? by.yMax : nextPosition.y;
			}
		}
	}

	protected void Turn ()
	{
		directionIndex = (directionIndex + 1) % directionSize;

		for (int i = 0; i < 4; i++) {
			GameObject brick = bricks [i];
			brick.transform.localPosition = coordinates [directionIndex, i];
		}
	}

	protected abstract void InitialAllCoordinates ();
}
