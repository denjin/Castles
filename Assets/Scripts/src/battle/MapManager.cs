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
					//position the tile
					Int2 position = new Int2(tX, tY);
					//set the tile
					Tile.SetTile(position, 0, 0, 0);
					//add the node to the node list
					nodes[tX,tY] = new Node(true, false, new Vector2(tX * tileSize, tY * tileSize), tX, tY);
				}
			}

			for (int x = 40; x <= 60; x++) {
				for (int y = 40; y <= 60; y++) {
					Tile.SetTile(new Int2(x, y), 0, 1, 0);
					nodes[x, y].walkable = false;
					nodes[x, y].wall = true;
				}
			}

			for (int x = 41; x <= 59; x++) {
				for (int y = 41; y <= 59; y++) {
					Tile.SetTile(new Int2(x, y), 0, 0, 0);
					nodes[x, y].walkable = true;
					nodes[x, y].wall = false;
				}
			}

			Tile.SetTile(new Int2(50, 60), 0, 0, 0);
			nodes[50, 60].walkable = true;
			nodes[50, 60].wall = false;

			for (int x = 0; x < levelWidth; x++) {
				for (int y = 0; y < levelHeight; y++) {
					UpdateTile(nodes[x,y]);
				}
			}
			
			//set the cameras position to the first tiles position
			Int2 middle = new Int2(levelWidth / 2, levelHeight / 2);
			Vector3 tilePosition = Tile.MapToWorldPosition(middle, 0);
			Vector3 camPosition = new Vector3(tilePosition.x, tilePosition.y, -10f);
			mainCamera.transform.position = camPosition;
		}

		public void MakeWall(Node node) {
			Tile.SetTile(new Int2(node.gridX, node.gridY), 0, 1, 0);
			node.wall = true;
			node.walkable = false;
			UpdateTile(node, true);
		}
		public void UpdateTile(Node node, bool full = false) {
			if (node.wall) {
				List<Node> neighbours = GetNeighbours(node, false);
				int mask = 0;
				for (int i = 0; i < neighbours.Count; i++) {
					if (neighbours[i].gridX == node.gridX) {
						if (neighbours[i].gridY < node.gridY) {
							//south
							if (neighbours[i].wall) {
								mask += 4;
							}
						} else {
							//north
							if (neighbours[i].wall) {
								mask += 1;
							}
						}
					} else {
						if (neighbours[i].gridX < node.gridX) {
							//west
							if (neighbours[i].wall) {
								mask += 8;
							}
						} else {
							//east
							if (neighbours[i].wall) {
								mask += 2;
							}
						}
					}
				}
				Tile.SetTile(new Int2(node.gridX, node.gridY), 0, 1, mask);
				if (full) {
					for (int i = 0; i < neighbours.Count; i++) {
						UpdateTile(neighbours[i]);
					}
				}

			}
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
			Node startNode = WorldToNode(_startPosition);
			Node endNode = WorldToNode(_endPosition);
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
				Debug.Log(waypoints.Length);
				
			}
			//tell the manager that this path is completed
			PathRequestManager.Instance.FinishedProcessingPath(waypoints, pathSuccess);
		}

		/**
		 * Gets a list of nodes adjacent to the target node
		 * @param Node _node the node to check
		 */
		public List<Node> GetNeighbours(Node node, bool full = true) {
			List<Node> neighbours = new List<Node>();
			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {//if we're in the centre of the check
					if (x == 0 && y == 0) {
						continue;//skip over this one because it's the supplied node
					}
					int checkX = node.gridX + x;
					int checkY = node.gridY + y;
					//check if this is a valid node
					if (checkX >= 0 && checkX < levelWidth && checkY >= 0 && checkY < levelHeight) {
						if (!full) {
							if (nodes[checkX, checkY].gridX != node.gridX && nodes[checkX, checkY].gridY != node.gridY) {
								continue;
							}
						}
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
			int dX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int dY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
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
