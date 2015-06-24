using UnityEngine;
using System.Collections;

public class Tetris_Z : Tetris {

	protected override void InitialAllCoordinates ()
	{
		directionSize = 2;
		
		coordinates = new Vector2[2, 4] {
			{
				new Vector2 (0, 0.5f),
				new Vector2 (0, -0.5f),
				new Vector2 (1f, -0.5f),
				new Vector2 (-1f, 0.5f),
			},
			{
				new Vector2 (0.5f, 0),
				new Vector2 (-0.5f, 0),
				new Vector2 (0.5f, 1f),
				new Vector2 (-0.5f, -1f),
			}
		};
	}
}
