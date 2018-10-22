using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathGrid))]
public class PathGridEditor : Editor {

	Node[,] nodes;
	public bool updateGrid;
	public bool showGrid = false;

	public int obstacleLayer = 9;

	private int height;
	private int width;
	PathGrid grid;

	public override void OnInspectorGUI()
	{

		using (var check = new EditorGUI.ChangeCheckScope())
		{
			base.OnInspectorGUI();
		}
		if (GUILayout.Button("Update Grid"))
		{
			GenerateGrid();
		}
		showGrid = GUILayout.Toggle(showGrid, "Show Grid");
	}

	public void GenerateGrid()
	{
		//Declare variables
		grid = (PathGrid)target;
		width = Mathf.RoundToInt(Mathf.Abs((int)grid.transform.localScale.x) / (grid.interval * 2));
		height = Mathf.RoundToInt(Mathf.Abs((int)grid.transform.localScale.y) / grid.interval);
		Vector2 offset = grid.transform.position;

		nodes = new Node[width, height];
		Handles.color = Color.yellow;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				float pointX = grid.interval * 2*x + offset.x;
				float pointY = grid.interval * y + offset.y - height * grid.interval;
				if (y % 2 == 0)
				{
					pointX += grid.interval;
				}

				Node node = new Node(new Vector2(pointX, pointY), x, y);
				nodes[x, y] = node;
			}
		}
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				InitializeNodeConnections(nodes[x, y]);
			}
		}

		grid.Nodes = nodes;
		if (EditorApplication.isPlaying)
		{
			PathFinder pathfinder = new PathFinder(nodes[0, 0], nodes[grid.testNodeX, grid.testNodeY]);
			List<Node> path = pathfinder.GetShortestPath();
			for (int i = 0; i < path.Count - 1; i++)
			{
				Debug.DrawLine(path[i].gridCoordinates, path[i + 1].gridCoordinates, Color.green, grid.showTime);
			}
		}

		if (showGrid)
		{
			foreach (Node node in nodes)
			{
				Debug.DrawLine(node.gridCoordinates, node.gridCoordinates + new Vector2(0.05f, 0.05f), Color.red, grid.showTime);
			}
		}
	}


	void InitializeNodeConnections(Node node)
	{
		RaycastHit2D hit;
		float diagonalDistance = Mathf.Sqrt(Mathf.Pow(grid.interval, 2) + Mathf.Pow(grid.interval, 2));
		Node connectedNode;
		//Top
		if (node.Y < height - 2)
		{
			hit = Physics2D.Raycast(node.gridCoordinates, new Vector2(0, 1), grid.interval * 2);
			if (hit.collider != null && hit.collider.gameObject.layer == obstacleLayer) { } else
			{
				connectedNode = nodes[node.X, node.Y + 2];
				TwoWayConnection(node, connectedNode, 2);
				Edge connection = new Edge { ConnectedNode = connectedNode, Cost = 2 };
				node.connections.Add(connection);
				if (showGrid)
				{
					Debug.DrawLine(node.gridCoordinates, connectedNode.gridCoordinates, Color.yellow, grid.showTime);
				}
			}
		}
		//Top Left
		if (node.Y < height - 1 && node.X != 0)
		{
			hit = Physics2D.Raycast(node.gridCoordinates, new Vector2(-1, 1), diagonalDistance);
			if (hit.collider != null && hit.collider.gameObject.layer == obstacleLayer) { } else
			{
				if (node.Y % 2 == 0)
				{
					connectedNode = nodes[node.X, node.Y + 1];
				}
				else
				{
					connectedNode = nodes[node.X - 1, node.Y + 1];
				}
				Edge connection = new Edge { ConnectedNode = connectedNode, Cost = 1.4f };
				node.connections.Add(connection);
				TwoWayConnection(node, connectedNode, 1.4f);
				if (showGrid)
				{
					Debug.DrawLine(node.gridCoordinates, connectedNode.gridCoordinates, Color.yellow, grid.showTime);
				}
			}
		}

		//Top Right
		if (node.Y < height - 1 && node.X < width - 1)
		{
			hit = Physics2D.Raycast(node.gridCoordinates, new Vector2(1, 1), diagonalDistance);
			if (hit.collider != null && hit.collider.gameObject.layer == obstacleLayer) { } else
			{
				if (node.Y % 2 == 0)
				{
					connectedNode = nodes[node.X + 1, node.Y + 1];
				}
				else
				{
					connectedNode = nodes[node.X, node.Y + 1];
				}
				Edge connection = new Edge { ConnectedNode = connectedNode, Cost = 1.4f };
				node.connections.Add(connection);
				TwoWayConnection(node, connectedNode, 1.4f);
				if (showGrid)
				{
					Debug.DrawLine(node.gridCoordinates, connectedNode.gridCoordinates, Color.yellow, grid.showTime);
				}
			}
		}
		//Right
		if (node.X < width - 1)
		{
			hit = Physics2D.Raycast(node.gridCoordinates, new Vector2(1, 0), grid.interval * 2);
			if (hit.collider != null && hit.collider.gameObject.layer == obstacleLayer) { } else
			{
				connectedNode = nodes[node.X + 1, node.Y];
				Edge connection = new Edge { ConnectedNode = connectedNode, Cost = 2 };
				node.connections.Add(connection);
				TwoWayConnection(node, connectedNode, 2);
				if (showGrid)
				{
					Debug.DrawLine(node.gridCoordinates, connectedNode.gridCoordinates, Color.yellow, grid.showTime);
				}
			}
		}
	}

	public void TwoWayConnection(Node node, Node connectedNode, float _cost)
	{
		Edge returnConnection = new Edge { ConnectedNode = node, Cost = _cost};
		connectedNode.connections.Add(returnConnection);
	}
}
