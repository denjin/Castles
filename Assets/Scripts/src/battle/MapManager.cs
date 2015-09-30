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
		Tile.AddLayer(new Int2(levelWidth, levelHeight), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
		//build the level
		int x;
		int y;
		bool wall;
		bool opaque;
		for (x = 0; x < levelWidth; x++) {
			for (y = 0; y < levelHeight; y++) {
				if (Tile.GetTile(new Int2(x, y)).tile == 0) {
					wall = true;
					opaque = true;
				} else {
					wall = false;
					opaque = false;
				}
				//set the node
				nodes[x,y] = new Node(wall, opaque, new Vector2(x * tileSize, y * tileSize), x, y);
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

	void Update() {
		Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		if (Input.GetMouseButtonDown(1)) {
			Node node = WorldToNode(pos);
			MakeWall(node);
		}
	}

	

	public void MakeWall(Node node) {
		if (nodes[node.x, node.y].wall) {
			Tile.SetTile(new Int2(node.x, node.y), 0, 0, 16);
			node.wall = false;
			node.opaque = false;
		} else {
			Tile.SetTile(new Int2(node.x, node.y), 0, 0, 0);
			node.wall = true;
			node.opaque = true;
		}
		
		UpdateTile(node, true);
	}
	
	public void UpdateTile(Node node, bool full = false) {
		if (node.wall) {
			List<Node> neighbours = GetNeighbours(node, false);
			int mask = 0;
			for (int i = 0; i < neighbours.Count; i++) {
				if (neighbours[i].x == node.x) {
					if (neighbours[i].y < node.y) {
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
					if (neighbours[i].x < node.x) {
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
			Tile.SetTile(new Int2(node.x, node.y), 0, 0, mask);
			if (full) {
				for (int i = 0; i < neighbours.Count; i++) {
					UpdateTile(neighbours[i]);
				}
			}

		}
	}

	public void SetTile(int _x, int _y) {
		Tile.SetTile(new Int2(_x, _y), 0, 0, 17);
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
				int checkX = node.x + x;
				int checkY = node.y + y;
				//check if this is a valid node
				if (checkX >= 0 && checkX < levelWidth && checkY >= 0 && checkY < levelHeight) {
					if (!full) {
						if (nodes[checkX, checkY].x != node.x && nodes[checkX, checkY].y != node.y) {
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
		int x = Mathf.RoundToInt((levelWidth) * percentX);
		int y = Mathf.RoundToInt((levelHeight) * percentY);
		return nodes[x,y];
	}

	public float Distance(Node a, Node b) {
		float dx = b.x - a.x;
		float dy = b.y - a.y;
		return Mathf.Sqrt(dx * dx + dy * dy);
	}

	public void ClearVision() {
		for (int x = 0; x < levelWidth; x++) {
			for (int y = 0; y < levelHeight; y++) {
				nodes[x, y].visible = false;
				SetNodeVisibility(nodes[x, y], 0.25f);
			}
		}
	}

	public void GetBasicVision(Node source, int range) {
		int minx = Mathf.Max(source.x - range, 0);
		int miny = Mathf.Max(source.y - range, 0);
		int maxx = Mathf.Min(source.x + range, levelWidth - 1);
		int maxy = Mathf.Min(source.y + range, levelHeight - 1);

		List<Node> line;
		for (int x = minx; x <= maxx; x++) {
			line = GetLine(source, nodes[x,miny]);
			ScanLine(line);
			line = GetLine(source, nodes[x,maxy]);
			ScanLine(line);
		}

		for (int y = miny; y <= maxy; y++) {
			line = GetLine(source, nodes[minx,y]);
			ScanLine(line);
			line = GetLine(source, nodes[maxx,y]);
			ScanLine(line);
		}
		for (int x = 0; x < levelWidth; x++) {
			for (int y = 0; y < levelHeight; y++) {
				if (nodes[x,y].visible) {
					float dist = Distance(source, nodes[x,y]);
					if (dist < range) {
						SetNodeVisibility(nodes[x, y], 1f);
					}
				}
			}
		}
	}


	public void ScanLine(List<Node> line) {
		for (int i = 0; i < line.Count; i++) {
			line[i].visible = true;
			if (line[i].opaque) {
				break;
			}
		}
	}

	public void SetNodeVisibility(Node target, float vis) {
		Tile.SetColor(new Int2(target.x, target.y), 0, new Color(255f, 255f, 255f, vis));
	}

	public List<Node> GetLine (Node source, Node target) {
		List<Node> line = new List<Node>();
		//source positions
		int x0 = source.x;
		int y0 = source.y;
		//target positions
		int x1 = target.x;
		int y1 = target.y;
		//distances
		int dx = Mathf.Abs((x1 - x0));
		int dy = Mathf.Abs((y1 - y0));
		//increment direction
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		//init start position
		int err = dx - dy;
		while (true) {
			line.Add(nodes[x0, y0]);
			if (x0 == x1 && y0 == y1) {
				break;
			}
			int err2 = err * 2;
			if (err2 > -dx) {
				err -= dy;
				x0 += sx;
			}
			if (err2 < dx) {
				err += dx;
				y0 += sy;
			}
		}
		return line;
	}

}