using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {
	public MapManager map;

	void Awake() {
		map = GetComponent<MapManager>();
	}
	/**
	 * Tells the FindPath Coroutine to start
	 * @param Vector2 startPosition the position the path should start at
	 * @param Vector2 endPosition   the position the path wants to finish at
	 */
	public void StartFindPath(Vector2 startPosition, Vector2 endPosition) {
		//start the coroutine
		StartCoroutine(FindPath(startPosition, endPosition));
	}

	/**
	 * Uses A* to calculate the most efficient path between the start position and the end position
	 * @param Vector2 _startPosition where we start from
	 * @param Vector2 _endPosition   where we want to end up
	 */
	public IEnumerator FindPath(Vector2 _startPosition, Vector2 _endPosition) {
		Vector2[] waypoints = new Vector2[0];
		bool pathSuccess = false;
		//get the grid nodes from the provided positions
		Node startNode = map.WorldToNode(_startPosition);
		Node endNode = map.WorldToNode(_endPosition);
		//create the open / closed sets
		Heap<Node> openSet = new Heap<Node>(map.levelWidth * map.levelHeight);
		HashSet<Node> closedSet = new HashSet<Node>();
		//add the starting node to the open set
		openSet.Add(startNode);
		//create temp node to check
		Node currentNode;
		//scan through the open set
		while (openSet.Count > 0) {
			//start with the first node in the open set
			currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);
			//check if the current node is the target node
			if (currentNode == endNode) {
				pathSuccess = true;
				//finish
				break;
			}
			//check node's neighbours
			foreach(Node neighbour in map.GetNeighbours(currentNode)) {
				//check if neighbour blocks the path or in the closed set
				if (!neighbour.Walkable || closedSet.Contains(neighbour)) {
					//skip this neighbour
					continue;
				}
				//calculate the cost to get from the current node to the neighbour
				int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				//check if this cost is less than the neighbours cost or neighbour is in the closed set
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					//set neighbours costs
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, endNode);
					//make this neighbour the current node
					neighbour.parent = currentNode;
					//if the neighbour isn't already in the open set
					if (!openSet.Contains(neighbour)) {
						//add this neighbour to the open set
						openSet.Add(neighbour);
					} else {
						//sort the item into the open set heap
						openSet.UpdateItem(neighbour);
					}
				}
			}
		}
		//after we've traversed through the open set, continue in the next frame
		yield return null;
		//if we've reached the end of the path
		if (pathSuccess) {
			//work back through the path, child to parent
			waypoints = RetracePath(startNode, endNode);
		}
		//tell the manager that this path is completed
		PathRequestManager.Instance.FinishedProcessingPath(waypoints, pathSuccess);
	}

	/**
	 * Go through the path between the end and the start via parents to generate the path
	 * @param Node startNode start
	 * @param Node endNode   end
	 */
	public Vector2[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector2[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
	}

	Vector2[] SimplifyPath(List<Node> path) {
		List<Vector2> waypoints = new List<Vector2>();
		for (int i = 0; i < path.Count; i ++) {
			waypoints.Add(path[i].worldPosition);
		}
		return waypoints.ToArray();
	}

	/**
	 * Computes the cost of getting from a to b
	 * @param Node nodeA start
	 * @param Node nodeB end
	 */
	public int GetDistance(Node nodeA, Node nodeB) {
		int dX = Mathf.Abs(nodeA.x - nodeB.x);
		int dY = Mathf.Abs(nodeA.y - nodeB.y);
		if (dX > dY) {
			return 14 * dY + 10 * (dX - dY);
		}
		return 14 * dX + 10 * (dY - dX);
	}
}