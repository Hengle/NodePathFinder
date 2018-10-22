using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder {

	public Node end;
	public Node start;
	public int NodeVisits { get; private set; }

	public PathFinder(Node _start, Node _end)
	{
		start = _start;
		end = _end;
	}

	public List<Node> GetShortestPath ()
	{
		foreach (var node in PathGrid.gridInstance.Nodes)
			node.FindStraightLineDistanceToEnd(end);
		AStarSearch();
		List<Node> shortestPath = new List<Node>();
		shortestPath.Add(end);
		BuildShortestPath(shortestPath, end);
		shortestPath.Reverse();
		return shortestPath;
	}

	private void AStarSearch()
	{
		NodeVisits = 0;
		start.minCostToStart = 0;
		var prioQueue = new List<Node>();
		prioQueue.Add(start);
		do
		{
			prioQueue = prioQueue.OrderBy(x => x.minCostToStart + x.straightLineDistanceToEnd).ToList();
			var node = prioQueue.First();
			prioQueue.Remove(node);
			NodeVisits++;
			foreach (var cnn in node.connections.OrderBy(x => x.Cost))
			{
				var childNode = cnn.ConnectedNode;
				if (childNode.visited)
					continue;
				if (childNode.minCostToStart == null ||
					node.minCostToStart + cnn.Cost < childNode.minCostToStart)
				{
					childNode.minCostToStart = node.minCostToStart + cnn.Cost;
					childNode.nearestToStart = node;
					if (!prioQueue.Contains(childNode))
						prioQueue.Add(childNode);
				}
			}
			node.visited = true;
			if (node == end)
				return;
		} while (prioQueue.Any());
	}

	private void BuildShortestPath(List<Node> list, Node node)
	{
		if (node.nearestToStart == null)
			return;
		list.Add(node.nearestToStart);
		BuildShortestPath(list, node.nearestToStart);
	}
}
