using UnityEngine;
using System.Collections;

public class Tetris_I : Tetris
{

	protected override void InitialAllCoordinates ()
	{
		directionSize = 2;

		coordinates = new Vector2[2, 4] {
			{
				new Vector2 (0.5f, 0.5f),
				new Vector2 (1.5f, 0.5f),
				new Vector2 (2.5f, 0.5f),
				new Vector2 (3.5f, 0.5f),
			},
			
			{
				new Vector2 (0.5f, 0.5f),
				new Vector2 (0.5f, 1.5f),
				new Vector2 (0.5f, 2.5f),
				new Vector2 (0.5f, 3.5f),
			},
		};

		boundary = new Boundary[] {
			new Boundary (0, 0, 16, 29),
			new Boundary (0, 0, 19, 26)
		};
	}

	protected override void Update ()
	{
		Debug.Log ("Bricks World Position: ");
		for (int i = 0; i < 4; i++) {
			GameObject brick = bricks [i];
			Debug.Log (brick.transform.position);
		}
		base.Update ();
	}
}
