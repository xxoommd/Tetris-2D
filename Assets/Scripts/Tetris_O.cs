using UnityEngine;
using System.Collections;

public class Tetris_O : Tetris
{

	protected override void InitialAllCoordinates ()
	{
		directionSize = 1;

		coordinates = new Vector2[1, 4] {
			{
				new Vector2 (0, 0),
				new Vector2 (0, 1),
				new Vector2 (1, 0),
				new Vector2 (1, 1),
			}
		};
	}
}
