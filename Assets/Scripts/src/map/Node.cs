using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : IHeapItem<Node> {
	public bool wall;

	//occlusion
	public bool opaque;
	public bool visible;
	//can i be reached
	public bool reachable;

	public bool occupied;

	//cover
	public bool lowCover = false;
	public bool highCover = false;

	public Vector2 worldPosition;

	public int gCost;
	public int hCost;

	public int x;
	public int y;

	public Node parent;

	public int heapIndex;

	public Node(bool _wall, bool _opaque, Vector2 _worldPosition, int _x, int _y) {
		wall = _wall;
		opaque = _opaque;
		worldPosition = _worldPosition;
		x = _x;
		y = _y;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get;
		set;
	}

	public bool Walkable {
		get {
			if (wall || occupied) {
				return false;
			} else {
				return true;
			}
		}
	}

	public bool Wall {
		get;
		set;
	}

	public int CompareTo (Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}