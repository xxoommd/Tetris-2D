using UnityEngine;
using System.Collections;

public class Tetris_T : Tetris {

	protected override void InitialAllCoordinates ()
	{
		directionSize = 4;

		coordinates = new Vector2[4,4] {
			{
				new Vector2 (0, 0),
				new Vector2 (1f, 0),
				new Vector2 (-1f, 0),
				new Vector2 (0, 1f),
			},
			{
				new Vector2 (0, 1f),
				new Vector2 (0, 0),
				new Vector2 (0, -1f),
				new Vector2 (1f, 0),
			},
			{
				new Vector2 (-1f, 0),
				new Vector2 (0, 0),
				new Vector2 (1f, 0),
				new Vector2 (0, -1f),
			},
			{
				new Vector2 (0, 1f),
				new Vector2 (0, 0),
				new Vector2 (0, -1),
				new Vector2 (-1, 0),
			},
		};
	}
}
