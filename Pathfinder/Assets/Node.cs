using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public int X;
	public int Y;

	public float? minCostToStart { get; set; }
	public Node nearestToStart { get; set; }
	public bool visited { get; set; }
	public float straightLineDistanceToEnd;
	public List<Edge> connections = new List<Edge>();

	public Vector2 gridCoordinates;

	public Node(Vector2 position, int x, int y)
	{
		X = x;
		Y = y;
		gridCoordinates = position;
	}

	public void FindStraightLineDistanceToEnd (Node end)
	{
		straightLineDistanceToEnd = Mathf.Sqrt(Mathf.Pow(X - end.X, 2) + Mathf.Pow(Y - end.Y, 2));
	}
}

public class Edge {
	public float Cost { get; set; }
	public Node ConnectedNode { get; set; }
}
