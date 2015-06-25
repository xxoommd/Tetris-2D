using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{
	[HideInInspector]
	public GameObject[,] brickBoxes;

	void Awake () {
		brickBoxes = new GameObject[20,30];
	}
}
