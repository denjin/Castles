using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using Battle;
using System;

public class MapManager : MonoBehaviour {
		private Camera mainCamera;
		//base size of the map
		private int levelWidth;
		private int levelHeight;
		//array to store node data, purely for pathfinding purposes
		public Node[,] nodes;
		//amount to add to the map to get it to fit properly into the iso map
		private Int2 buffer;
		//height / width of the generated iso map
		private Int2 mapSize;
		//the size of each tile
		private float tileSize;
		//how many different tiles do we have?
		private int numTiles = 4;


		void Awake() {
			//initialise the camera
			mainCamera = Camera.main;
			Tile.SetCamera(mainCamera);
			//setup map variables
			levelWidth = 100;
			levelHeight = 100;
			tileSize = 0.16f;
			//setup the node list
			nodes = new Node[levelWidth, levelHeight];
			//add a buffer to the size of the level
			buffer = new Int2(levelWidth * 2, levelHeight * 2);
			//create the level
			Tile.NewLevel(new Int2(buffer.x, buffer.y), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
			Tile.AddLayer(new Int2(buffer.x, buffer.y), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
			//save the size of the map
			mapSize = Tile.GetMapSize();
			//build the level
			for (int tY = 0; tY < levelHeight; tY++) {
				for (int tX = 0; tX < levelWidth; tX++) {
					//select a tile
					int tile1 = 0;
					//position the tile
					Int2 position = new Int2(tX, tY);
					//set the tile
					Tile.SetTile(position, 0, 0, tile1);
					//add the node to the node list
					nodes[tX,tY] = new Node(true, new Vector2(tX * tileSize, tY * tileSize), tX, tY);
				}
			}

			for (int x = 10; x < 90; x++) {
				for (int y = 40; y < 60; y++) {
					Tile.SetTile(new Int2(x, y), 0, 0, 3);
					nodes[x, y].walkable = false;
				}
			}
			
			//set the cameras position to the first tiles position
			Int2 middle = new Int2(levelWidth / 2, levelHeight / 2);
			Vector3 tilePosition = Tile.MapToWorldPosition(middle, 0);
			Vector3 camPosition = new Vector3(tilePosition.x, tilePosition.y, -10f);
			//mainCamera.transform.position = camPosition;
		}

		public void StartFindPath(Vector2 startPosition, Vector2 endPosition) {
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
			Node startNode = WorldToNode(_startPosition);
			Node endNode = WorldToNode(_endPosition);

			if (startNode.walkable && endNode.walkable) {
				//create the open / closed sets
				Heap<Node> openSet = new Heap<Node>(levelWidth * levelHeight);
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
					foreach(Node neighbour in GetNeighbours(currentNode)) {
						//check if neighbour blocks the path or in the closed set
						if (!neighbour.walkable || closedSet.Contains(neighbour)) {
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
								openSet.UpdateItem(neighbour);
							}
						}
					}
				}
			}

			
			yield return null;
			if (pathSuccess) {
				waypoints = RetracePath(startNode, endNode);
			}
			PathRequestManager.Instance.FinishedProcessingPath(waypoints, pathSuccess);
		}

		/**
		 * Gets a list of nodes adjacent to the target node
		 * @param Node _node the node to check
		 */
		public List<Node> GetNeighbours(Node _node) {
			List<Node> neighbours = new List<Node>();
			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					//if we're in the centre of the check
					if (x == 0 && y == 0) {
						//skip over this one because it's the supplied node
						continue;
					}
					//values to check if this is valid
					int checkX = _node.gridX + x;
					int checkY = _node.gridY + y;
					//check if this is a valid node
					if (checkX >= 0 && checkX < levelWidth && checkY >= 0 && checkY < levelHeight) {
						//add this node to the neighbours
						neighbours.Add(nodes[checkX, checkY]);
					}
				}
			}
			return neighbours;
		}

		/**
		 * Computes the cost of getting from a to b
		 * @param Node nodeA start
		 * @param Node nodeB end
		 */
		public int GetDistance(Node nodeA, Node nodeB) {
			//get the distance in each axis
			int dX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int dY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
			//calculate the cost
			if (dX > dY) {
				return 14 * dY + 10 * (dX - dY);
			}
			return 14 * dX + 10 * (dY - dX);
		}

		/**
		 * Go through the path between the end and the start via parents to generate the path
		 * @param Node startNode start
		 * @param Node endNode   end
		 */
		public Vector2[] RetracePath(Node startNode, Node endNode) {
			//create a list to store the nodes
			List<Node> path = new List<Node>();
			//set the current node to the last node
			Node currentNode = endNode;
			//trace back through the path going from node to parent
			while (currentNode != startNode) {
				path.Add(currentNode);
				currentNode = currentNode.parent;
			}
			//flip the order of the path
			Vector2[] waypoints = SimplifyPath(path);
			Array.Reverse(waypoints);
			return waypoints;
		}

		Vector2[] SimplifyPath(List<Node> path) {
			List<Vector2> waypoints = new List<Vector2>();
			
			Vector2 directionOld = Vector2.zero;

			for (int i = 1; i < path.Count; i ++) {
				Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
				if (directionNew != directionOld) {
					waypoints.Add(path[i].worldPosition);
				}
				directionOld = directionNew;
			}
			return waypoints.ToArray();
		}

		public Int2 GetSize() {
			return new Int2(levelWidth, levelHeight);
		}
		

		public Vector3 TileToWorld(Int2 _tile) {
			return Tile.MapToWorldPosition(_tile);
		}

		public Node WorldToNode(Vector2 _position) {
			float percentX = _position.x / (levelWidth * tileSize);
			float percentY = _position.y / (levelHeight * tileSize);
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);
			int x = Mathf.RoundToInt((levelWidth - 1) * percentX);
			int y = Mathf.RoundToInt((levelHeight - 1) * percentY);
			return nodes[x,y];
		}
	}
