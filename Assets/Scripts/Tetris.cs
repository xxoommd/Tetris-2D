using UnityEngine;
using System.Collections;

public class Tetris : MonoBehaviour
{

	public GameObject brick;
	protected GameObject[] bricks;
	protected int brickCount;

	protected virtual void Awake ()
	{
		brickCount = 4;
		bricks = new GameObject[brickCount];

		for (int i = 0; i < brickCount; i++) {
			GameObject obj = Instantiate (brick, Vector3.zero, Quaternion.identity) as GameObject;
			bricks [i] = obj;
			obj.transform.SetParent (transform);
		}

		transform.localPosition = new Vector3 (9.5f, 15f, 0f);
	}
}
