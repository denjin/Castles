using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : IHeapItem<Node> {
	public bool wall;

	//occlusion
	public bool opaque;
	public bool visible;

	public Vector2 worldPosition;

	public int gCost;
	public int hCost;

	public int gridX;
	public int gridY;

	public Node parent;

	public int heapIndex;

	public Node(bool _wall, bool _opaque, Vector2 _worldPosition, int _gridX, int _gridY) {
		wall = _wall;
		opaque = _opaque;
		worldPosition = _worldPosition;
		gridX = _gridX;
		gridY = _gridY;
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
			return !wall;
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