using UnityEngine;
using System.Collections;

public class Tetris_O : Tetris
{

	protected override void InitialAllCoordinates ()
	{
		directionSize = 1;

		coordinates = new Vector2[1, 4] {
			{
				new Vector2 (0.5f, 0.5f),
				new Vector2 (0.5f, -0.5f),
				new Vector2 (-0.5f, 0.5f),
				new Vector2 (-0.5f, -0.5f),
			}
		};
	}
}
