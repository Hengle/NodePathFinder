using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour {

	public static PathGrid gridInstance;

	public Node[,] Nodes;
	public int testNodeX;
	public int testNodeY;

	[Range(2, 0.6f)]
	public float interval = 1f;
	public float showTime = 5f;

	private void Start()
	{
		gridInstance = this;
	}
}

