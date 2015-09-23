using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using System;

public class MapManager : MonoBehaviour {
	public Camera cam;
	public int levelWidth;
	public int levelHeight;
	public Node[,] nodes;
	public float tileSize;
	public TextAsset map;

	void Awake() {
		map = Resources.Load("maps/test_map") as TextAsset;
		//initialise the camera
		Tile.SetCamera(cam);
		levelHeight = 50;
		levelWidth = 50;
		Tile.LoadLevel(map);
		nodes = new Node[levelWidth, levelHeight];
		//create the level
		//Tile.NewLevel(new Int2(levelWidth, levelHeight), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
		Tile.AddLayer(new Int2(levelWidth, levelHeight), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
		//build the level
		int x;
		int y;
		bool wall;
		for (x = 0; x < levelWidth; x++) {
			for (y = 0; y < levelHeight; y++) {
				if (Tile.GetTile(new Int2(x, y)).tile == 0) {
					wall = true;
				} else {
					wall = false;
				}
				//set the node
				nodes[x,y] = new Node(wall, new Vector2(x * tileSize, y * tileSize), x, y);
			}
		}
		for (x = 0; x < levelWidth; x++) {
			for (y = 0; y < levelHeight; y++) {
				if (nodes[x,y].wall) {
					UpdateTile(nodes[x,y]);
				}
			}
		}
		
		
		//set the cameras position to the center tile's position
		Int2 middle = new Int2(levelWidth / 2, levelHeight / 2);
		Vector3 tilePosition = Tile.MapToWorldPosition(middle, 0);
		Vector3 camPosition = new Vector3(tilePosition.x, tilePosition.y, -10f);
		cam.transform.position = camPosition;
	}

	public void MakeWall(Node node) {
		Tile.SetTile(new Int2(node.gridX, node.gridY), 0, 0, 0);
		node.wall = true;
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
			Tile.SetTile(new Int2(node.gridX, node.gridY), 0, 0, mask);
			if (full) {
				for (int i = 0; i < neighbours.Count; i++) {
					UpdateTile(neighbours[i]);
				}
			}

		}
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
