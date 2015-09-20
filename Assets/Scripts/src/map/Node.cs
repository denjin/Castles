using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Battle {
	public class Node : IHeapItem<Node> {
		public bool walkable;
		public bool wall;
		public Vector2 worldPosition;

		public int gCost;
		public int hCost;

		public int gridX;
		public int gridY;

		public Node parent;

		public int heapIndex;

		public Node(bool _walkable, bool _wall, Vector2 _worldPosition, int _gridX, int _gridY) {
			walkable = _walkable;
			wall = _wall;
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
			get {
				return heapIndex;
			}

			set {
				heapIndex = value;
			}
		}

		public int CompareTo (Node nodeToCompare) {
			int compare = fCost.CompareTo(nodeToCompare.fCost);
			if (compare == 0) {
				compare = hCost.CompareTo(nodeToCompare.hCost);
			}
			return -compare;
		}
	}
}