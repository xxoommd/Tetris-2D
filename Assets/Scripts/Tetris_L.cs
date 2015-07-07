using UnityEngine;
using System.Collections;

public class Tetris_L : Tetris
{

	protected override void InitialAllCoordinates ()
	{
		directionSize = 4;

		coordinates = new Vector2[4, 4] {
			{
				new Vector2 (0, 0),
				new Vector2 (-1f, 0),
				new Vector2 (1f, 0),
				new Vector2 (1f, 1f),
			},
			
			{
				new Vector2 (0, 0),
				new Vector2 (0, 1f),
				new Vector2 (0, -1f),
				new Vector2 (1f, -1f),
			},
			
			{
				new Vector2 (0, 0),
				new Vector2 (1f, 0),
				new Vector2 (-1f, 0),
				new Vector2 (-1f, -1f),
			},
			
			{
				new Vector2 (0, 0),
				new Vector2 (0, 1f),
				new Vector2 (0, -1f),
				new Vector2 (-1f, 1f),
			}
		};
	}

	protected override GameObject getBrickTemplate ()
	{
		return GameController.instance.brickL;
	}
}
