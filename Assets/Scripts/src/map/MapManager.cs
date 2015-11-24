using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using System;

public class MapManager : MonoBehaviour {
	public Camera cam;
	public int levelWidth;
	public int levelHeight;

	public Bounds bounds;

	public Node[,] nodes;
	public float tileSize;
	public TextAsset map;

	public FieldOfView fov = new FieldOfView();

	List<Node> reachableNodes;
	public Pathfinding pathing;

	public void Init(string _map) {
		reachableNodes = new List<Node>();
		map = Resources.Load(_map) as TextAsset;
		pathing = GetComponent<Pathfinding>();
		//initialise the camera
		Tile.SetCamera(cam);
		levelHeight = 50;
		levelWidth = 50;
		bounds = new Bounds(0, 0, 50, 50);
		Tile.LoadLevel(map);
		nodes = new Node[levelWidth, levelHeight];
		//create the level
		Tile.AddLayer(new Int2(levelWidth, levelHeight), 3, new Vector2(tileSize, tileSize), 0, LayerLock.None);
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

		LoadEnemyUnits();
		
		//set the cameras position to the center tile's position
		Int2 middle = new Int2(levelWidth / 2, levelHeight / 2);
		Vector3 tilePosition = Tile.MapToWorldPosition(middle, 0);
		Vector3 camPosition = new Vector3(tilePosition.x, tilePosition.y, -10f);
		cam.transform.position = camPosition;
	}

	public void LoadEnemyUnits() {
		for (int x = 0; x < levelWidth; x++) {
			for (int y = 0; y < levelHeight; y++) {
				int tile = Tile.GetTile(new Int2(x, y), 1).tile;
				if (tile != -1) {
					GameObject soldier = Global.Instance.battleManager.enemyTypes.soldierObjects[tile];
					Global.Instance.battleManager.ai.LoadUnit(soldier, TileToWorld(new Int2(x, y)));
				}
				Tile.DeleteTile(new Int2(x, y), 1);
			}
		}
	}





	public void MakeWall(Node node) {
		if (nodes[node.x, node.y].wall) {
			Tile.SetTile(new Int2(node.x, node.y), 0, 0, 16);
			node.wall = false;
			node.opaque = false;
			node.highCover = false;
		} else {
			Tile.SetTile(new Int2(node.x, node.y), 0, 0, 0);
			node.wall = true;
			node.opaque = true;
			node.highCover = true;
		}
		UpdateTile(node, 0, 0, true);
	}
	
	public void UpdateTile(Node node, int tLayer = 0, int tSet = 0, bool checkingReachability = false) {
		bool _base = checkingReachability ? _base = node.reachable : _base = node.wall;
		if (_base) {
			List<Node> neighbours = GetNeighbours(node, bounds, false);
		int mask = 0;
		for (int i = 0; i < neighbours.Count; i++) {
			bool check = checkingReachability ? check = neighbours[i].reachable : check = neighbours[i].wall;
			if (neighbours[i].x == node.x) {
				if (neighbours[i].y < node.y) {
					//south
					if (check) {
						mask += 4;
					}
				} else {
					//north
					if (check) {
						mask += 1;
					}
				}
			} else {
				if (neighbours[i].x < node.x) {
					//west
					if (check) {
						mask += 8;
					}
				} else {
					//east
					if (check) {
						mask += 2;
					}
				}
			}
		}
		Tile.SetTile(new Int2(node.x, node.y), tLayer, tSet, mask);
		}
		
	}

	public void SetTile(int _x, int _y) {
		Tile.SetTile(new Int2(_x, _y), 0, 0, 17);
	}


	/**
	 * Gets a list of nodes adjacent to the target node
	 * @param Node _node the node to check
	 */
	public List<Node> GetNeighbours(Node node, Bounds _bounds, bool full = true) {
		List<Node> neighbours = new List<Node>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {//if we're in the centre of the check
				if (x == 0 && y == 0) {
					continue;//skip over this one because it's the supplied node
				}
				int checkX = node.x + x;
				int checkY = node.y + y;
				//check if this is a valid node
				if (checkX >= _bounds.minx && checkX < _bounds.maxx && checkY >= _bounds.miny && checkY < _bounds.maxy) {
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

	public Int2 GetSize(bool trim = false) {
		if (trim) {
			return new Int2(levelWidth - 1, levelHeight - 1);
		}
		return new Int2(levelWidth, levelHeight);
	}

	public Vector2 TileToWorld(Int2 _tile) {
		return Tile.MapToWorldPosition(_tile);
	}

	public Node WorldToNode(Vector2 position) {
		float percentX = position.x / (levelWidth * tileSize);
		float percentY = position.y / (levelHeight * tileSize);
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
				SetNodeVisibility(nodes[x, y]);
			}
		}
	}

	public void GetBasicVision(Node source, int range) {
		List<Node> visible = fov.GetVisibleNodes(source, range);
		for (int i = 0; i < visible.Count; i++) {
			visible[i].visible = true;
			
		}
		for (int x = 0; x < levelWidth; x++) {
			for (int y = 0; y < levelHeight; y++) {
				SetNodeVisibility(nodes[x, y]);
			}
		}
	}

	public void SetNodeVisibility(Node target) {
		Color color = new Color(255f, 255f, 255f, 0.25f);
		if (target.visible) {
			color.a = 1f;
		}
		Tile.SetColor(new Int2(target.x, target.y), 0, color);
	}

	public List<Node> GetLine(Node source, Node target) {
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

	public void ClearPreview() {
		LineRenderer line = Global.Instance.lineDrawer;
		line.SetVertexCount(0);
	}

	public void PreviewPath(Vector2[] path) {
		LineRenderer line = Global.Instance.lineDrawer;
		line.SetVertexCount(path.Length);
		for (int i = 0; i < path.Length; i++) {
			line.SetPosition(i, path[i]);
		}
	}

	public void PreviewShot(Vector2 source, Vector2 target) {
		LineRenderer line = Global.Instance.lineDrawer;
		line.SetVertexCount(2);
		line.SetPosition(0, source);
		line.SetPosition(1, target);
	}

	public void GetReachableNodes(Node source, int range, Unit unit) {
		ClearReachableTiles();
		
		//get the bounds of the area to check
		int minx = Mathf.Max(source.x - range, 0);
		int miny = Mathf.Max(source.y - range, 0);
		int maxx = Mathf.Min(source.x + range, levelWidth);
		int maxy = Mathf.Min(source.y + range, levelHeight);
		//make a list to store the nodes that need checking
		List<Node> block = new List<Node>();
		//add the nodes to the list
		for (int x = minx; x < maxx; x++) {
			for (int y = miny; y < maxy; y++) {
				block.Add(nodes[x,y]);
			}
		}
		//keep a record of the block for next time we need to check
		reachableNodes = block;
		//for each block in the list
		for (int i = 0; i < block.Count; i++) {
			//get a path from the unit to that block
			Vector2[] path = pathing.GetPath(source.worldPosition, block[i].worldPosition);
			//if that path is valid and shorter than the unit's available movement points
			if (path.Length > 0 && path.Length <= unit.currentMovementPoints) {
				//that node is reachable so make it so
				Node node = WorldToNode(path[path.Length - 1]);
				node.reachable = true;
				Tile.SetTile(new Int2(node.x, node.y), 2, 1, 1);
			}
		}

		for (int i = 0; i < block.Count; i++) {
			UpdateTile(block[i], 2, 1, true);
		}
		
	}

	public void ClearReachableTiles() {
		//clear previous reachable node tiles
		for (int x = 0; x < levelWidth; x++) {
			for (int y = 0; y < levelHeight; y++) {
				Tile.DeleteTile(new Int2(x, y), 2);
			}
		}

		//clear previous collection of reachable nodes
		for (int i = 0; i < reachableNodes.Count; i++) {
			reachableNodes[i].reachable = false;
			SetNodeVisibility(reachableNodes[i]);
		}

		reachableNodes = new List<Node>();
	}

}