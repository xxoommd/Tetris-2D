using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Boundary {
	public int xMin, xMax, yMin, yMax;

	public Boundary (int xmin, int xmax, int ymin, int ymax) {
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

	// Methods
	protected virtual void Awake ()
	{
		InitialAllCoordinates ();

		GameObject brick = brickTemplates [Random.Range (0, brickTemplates.Length)];

		directionIndex = Random.Range (0, directionSize);
		bricks = new GameObject[4];

		for (int i = 0; i < 4; i++) {
			GameObject obj = Instantiate (brick, coordinates [directionIndex, i], Quaternion.identity) as GameObject;
			bricks [i] = obj;
			obj.transform.SetParent (transform);
		}

		transform.localPosition = new Vector3 (10f, 10f, 0f);
	}

	protected virtual void Update ()
	{
		if (Input.GetButtonUp ("Fire1")) {
			Turn ();
		}

		int h = 0;
		h = (int)Input.GetAxisRaw ("Horizontal");
		if (h > 0) {
			h = 1;
		} else if (h < 0) {
			h = -1;
		} else {
			h = 0;
		}

		if (h != 0) {
			Vector3 newPosition = transform.position;
			newPosition.x += h;
			Boundary by = boundary[directionIndex];
			newPosition.x = newPosition.x < by.xMin ? by.xMin : newPosition.x;
			newPosition.x = newPosition.x > by.xMax ? by.xMax : newPosition.x;

			transform.position = Vector3.MoveTowards(transform.position, newPosition, 1f);
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
