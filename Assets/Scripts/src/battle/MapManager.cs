using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using Map;

namespace Battle {

public class MapManager {
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
		private float tileSize = 0.16f;
		//how many different tiles do we have?
		private int numTiles = 4;
		
		private float seed;
		private float noise;

		public MapManager(Camera _mainCamera, int _levelWidth = 100, int _levelHeight = 100) {
			//initialise the camera
			mainCamera = _mainCamera;
			Tile.SetCamera(mainCamera);

			//setup map variables
			levelWidth = _levelWidth;
			levelHeight = _levelHeight;
			//setup the node list
			nodes = new Node[_levelWidth, _levelHeight];
			//add a buffer to the size of the level
			buffer = new Int2(levelWidth * 2, levelHeight * 2);
			//create the level
			Tile.NewLevel(new Int2(buffer.x, buffer.y), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
			Tile.AddLayer(new Int2(buffer.x, buffer.y), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
			//save the size of the map
			mapSize = Tile.GetMapSize();
			
			//setup the noise function
			seed = Random.value;
			
			//build the level
			for (int tY = 0; tY < levelHeight; tY++) {
				for (int tX = 0; tX < levelWidth; tX++) {
					//select a tile
					int tile1 = 1;
					//position the tile
					Int2 position = new Int2(tX, tY);
					//set the tile
					Tile.SetTile(position, 0, 0, tile1);
					//add the node to the node list
					nodes[tX,tY] = new Node(true, new Vector2(tX * tileSize, tY * tileSize), tX, tY);
				}
			}
			
			//set the cameras position to the first tiles position
			Int2 middle = new Int2(levelWidth / 2, levelHeight / 2);
			Vector3 tilePosition = Tile.MapToWorldPosition(middle, 0);
			Vector3 camPosition = new Vector3(tilePosition.x, tilePosition.y, -10f);
			mainCamera.transform.position = camPosition;
		}

		public void FindPath(Vector2 _startPosition, Vector2 _endPosition) {
			Debug.Log("Finding path between: " + _startPosition + " and " + _endPosition);
			//get the grid nodes from the provided positions
			Node startNode = WorldToNode(_startPosition);
			Node endNode = WorldToNode(_endPosition);
			//create the open / closed sets
			List<Node> openSet = new List<Node>();
			HashSet<Node> closedSet = new HashSet<Node>();
			//add the starting node to the open set
			openSet.Add(startNode);
			//create temp node to check
			Node currentNode;
			//scan through the open set
			while (openSet.Count > 0) {
				//start with the first node in the open set
				currentNode = openSet[0];
				//check all other nodes in the open set
				for (int i = 1; i < openSet.Count; i++) {
					//check if this node's fCost is less than the current node's fCost or if they're the same, then check if it's hCost is lower
					if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
						//set this node as the current node
						currentNode = openSet[i];
					}
				}
				//remove current node from the closed set and add it to the closed set
				openSet.Remove(currentNode);
				closedSet.Add(currentNode);
				//check if the current node is the target node
				if (currentNode == endNode) {
					RetracePath(startNode, endNode);
					//finish
					return;
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
						}
					}
				}
			}
		}

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

		public int GetDistance(Node nodeA, Node nodeB) {
			int dX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int dY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
			if (dX > dY) {
				return 14 * dY + 10 * (dX - dY);
			}
			return 14 * dX + 10 * (dY - dX);
		}

		public void RetracePath(Node startNode, Node endNode) {
			List<Node> path = new List<Node>();
			Node currentNode = endNode;
			while (currentNode != startNode) {
				path.Add(currentNode);
				currentNode = currentNode.parent;
			}
			path.Reverse();
			for(int i = 0; i < path.Count; i++) {
				Debug.Log(i + " : [" + path[i].gridX + "][" + path[i].gridY + "]");
			}
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
		
		private int GetPerlinNoise(int x, int y, float _seed, int _numTiles) {
			float noiseVal = Mathf.PerlinNoise(_seed + x, _seed + y) * (_numTiles + 1);
			int _int = (int)Mathf.Floor(noiseVal);
			if (_int < 0) {
				_int = 0;
			}
			return _int;
		}
	}
}
