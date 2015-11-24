using UnityEngine;
using System;
using System.Collections.Generic;

public class FieldOfView {
	Node[,] map;
	//int[] octants = new int[] {1,2,3,4,5,6,7,8};
	List<Node> visibleNodes;

	public List<Node> GetVisibleNodes(Node source, int range) {
		map = Global.Instance.mapManager.nodes;
		visibleNodes = new List<Node>();
		for (int o = 1; o <= 8; o++) {
			ScanOctant(source, range, 1, o, 1.0, 0.0);
		}
		return visibleNodes;
	}

	//  Octant data
	//
	//    \ 1 | 2 /
	//   8 \  |  / 3
	//   -----+-----
	//   7 /  |  \ 4
	//    / 6 | 5 \
	//
	//  1 = NNW, 2 =NNE, 3=ENE, 4=ESE, 5=SSE, 6=SSW, 7=WSW, 8 = WNW
	protected void ScanOctant(Node source, int range, int depth, int octant, double startSlope, double endSlope) {
		int visrange2 = range * range;
		int x = 0;
		int y = 0;

		visibleNodes.Add(source);

		switch (octant) {
			case 1: //nnw
				y = source.y - depth;
				if (y < 0) return;
				x = source.x - Convert.ToInt32((startSlope * Convert.ToDouble(depth)));
				if (x < 0) x = 0;
				while (GetSlope(x, y, source.x, source.y, false) >= endSlope) {
					if (GetVisDistance(x, y, source.x, source.y) <= visrange2) {
						if (map[x, y].opaque) { //current cell blocked
							if (x - 1 >= 0 && !map[x - 1, y].opaque) {//prior cell within range AND open...
								//...incremenet the depth, adjust the endslope and recurse
								ScanOctant(source, range, depth + 1, octant, startSlope, GetSlope(x - 0.5, y + 0.5, source.x, source.y, false));
							}
						} else {
							if (x - 1 >= 0 && map[x - 1, y].opaque) {//prior cell within range AND not open...
								//..adjust the startslope
								startSlope = GetSlope(x - 0.5, y - 0.5, source.x, source.y, false);
								
							} 
							visibleNodes.Add(map[x, y]);	
						}
					}
					x++;
				}
				x--;
				break;

			case 2: //nne

				y = source.y - depth;
				if (y < 0) return;					
				
				x = source.x + Convert.ToInt32((startSlope * Convert.ToDouble(depth)));
				if (x >= map.GetLength(0)) x = map.GetLength(0) - 1;
				
				while (GetSlope(x, y, source.x, source.y, false) <= endSlope) {
					if (GetVisDistance(x, y, source.x, source.y) <= visrange2) {
						if (map[x, y].opaque) {
							if (x + 1 < map.GetLength(0) && !map[x + 1, y].opaque)
								ScanOctant(source, range, depth + 1, octant, startSlope, GetSlope(x + 0.5, y + 0.5, source.x, source.y, false));
						}
						else {
							if (x + 1 < map.GetLength(0) && map[x + 1, y].opaque)
								startSlope = -GetSlope(x + 0.5, y - 0.5, source.x, source.y, false);

							
						}	
						visibleNodes.Add(map[x, y]);						
					}
					x--;
				}
				x++;
				break;

			case 3:

				x = source.x + depth;
				if (x >= map.GetLength(0)) return;
				
				y = source.y - Convert.ToInt32((startSlope * Convert.ToDouble(depth))); 
				if (y < 0) y = 0;

				while (GetSlope(x, y, source.x, source.y, true) <= endSlope) {

					if (GetVisDistance(x, y, source.x, source.y) <= visrange2) {

						if (map[x, y].opaque) {
							if (y - 1 >= 0 && !map[x, y - 1].opaque)
								ScanOctant(source, range, depth + 1, octant, startSlope, GetSlope(x - 0.5, y - 0.5, source.x, source.y, true));
						} else {
							if (y - 1 >= 0 && map[x, y - 1].opaque)
								startSlope = -GetSlope(x + 0.5, y - 0.5, source.x, source.y, true);

							
						}
						visibleNodes.Add(map[x, y]);
					}
					y++;
				}
				y--;
				break;

			case 4:

				x = source.x + depth;
				if (x >= map.GetLength(0)) return;
				
				y = source.y + Convert.ToInt32((startSlope * Convert.ToDouble(depth)));
				if (y >= map.GetLength(1)) y = map.GetLength(1) - 1;

				while (GetSlope(x, y, source.x, source.y, true) >= endSlope) {

					if (GetVisDistance(x, y, source.x, source.y) <= visrange2) {

						if (map[x, y].opaque) {
							if (y + 1 < map.GetLength(1)&& !map[x, y + 1].opaque)
								ScanOctant(source, range, depth + 1, octant, startSlope, GetSlope(x - 0.5, y + 0.5, source.x, source.y, true));
						}
						else {
							if (y + 1 < map.GetLength(1) && map[x, y + 1].opaque)
								startSlope = GetSlope(x + 0.5, y + 0.5, source.x, source.y, true);

							 
						}	
						visibleNodes.Add(map[x, y]);					  
					}
					y--;
				}
				y++;
				break;

			case 5:

				y = source.y + depth;
				if (y >= map.GetLength(1)) return;
				
				x = source.x + Convert.ToInt32((startSlope * Convert.ToDouble(depth)));
				if (x >= map.GetLength(0)) x = map.GetLength(0) - 1;
				
				while (GetSlope(x, y, source.x, source.y, false) >= endSlope) {
					if (GetVisDistance(x, y, source.x, source.y) <= visrange2) {
						if (map[x, y].opaque) {
							if (x + 1 < map.GetLength(1) && !map[x+1, y].opaque) {
								ScanOctant(source, range, depth + 1, octant, startSlope, GetSlope(x + 0.5, y - 0.5, source.x, source.y, false));
							}
						} else {
							if (x + 1 < map.GetLength(1) && map[x + 1, y].opaque) {
								startSlope = GetSlope(x + 0.5, y + 0.5, source.x, source.y, false);
							}
						}
						visibleNodes.Add(map[x, y]);
					}
					x--;
				}
				x++;
				break;

			case 6:
				y = source.y + depth;
				if (y >= map.GetLength(1)) return;					
				
				x = source.x - Convert.ToInt32((startSlope * Convert.ToDouble(depth)));
				if (x < 0) x = 0;
				
				while (GetSlope(x, y, source.x, source.y, false) <= endSlope) {
					if (GetVisDistance(x, y, source.x, source.y) <= visrange2) {
						if (map[x, y].opaque) {
							if (x - 1 >= 0 && !map[x - 1, y].opaque) {
								ScanOctant(source, range, depth + 1, octant, startSlope, GetSlope(x - 0.5, y - 0.5, source.x, source.y, false));
							}
						} else {
							if (x - 1 >= 0 && map[x - 1, y].opaque) {
								startSlope = -GetSlope(x - 0.5, y + 0.5, source.x, source.y, false);
							}
						}
						visibleNodes.Add(map[x, y]);
					}
					x++;
				}
				x--;
				break;

			case 7:
				x = source.x - depth;
				if (x < 0) return;
				y = source.y + Convert.ToInt32((startSlope * Convert.ToDouble(depth)));					
				if (y >= map.GetLength(1)) y = map.GetLength(1) - 1;
				while (GetSlope(x, y, source.x, source.y, true) <= endSlope) {
					if (GetVisDistance(x, y, source.x, source.y) <= visrange2) {
						if (map[x, y].opaque) {
							if (y + 1 < map.GetLength(1) && !map[x, y+1].opaque) {
								ScanOctant(source, range, depth + 1, octant, startSlope, GetSlope(x + 0.5, y + 0.5, source.x, source.y, true));
							}
						} else {
							if (y + 1 < map.GetLength(1) && map[x, y + 1].opaque) {
								startSlope = -GetSlope(x - 0.5, y + 0.5, source.x, source.y, true);
							}
							
						}
						visibleNodes.Add(map[x, y]);
					}
					y--;
				}
				y++;
			break;

			case 8: //wnw
				x = source.x - depth;
				if (x < 0) return;
				y = source.y - Convert.ToInt32((startSlope * Convert.ToDouble(depth)));
				if (y < 0) y = 0;
				while (GetSlope(x, y, source.x, source.y, true) >= endSlope) {
					if (GetVisDistance(x, y, source.x, source.y) <= visrange2) {
						if (map[x, y].opaque) {
							if (y - 1 >=0 && !map[x, y - 1].opaque) {
								ScanOctant(source, range, depth + 1, octant, startSlope, GetSlope(x + 0.5, y - 0.5, source.x, source.y, true));
							}
						} else {
							if (y - 1 >= 0 && map[x, y - 1].opaque) {
								startSlope = GetSlope(x - 0.5, y - 0.5, source.x, source.y, true);
							}
							
						}
						visibleNodes.Add(map[x, y]);
					}
					y++;
				}
				y--;
			break;
		}


		if (x < 0)
			x = 0;
		else if (x >= map.GetLength(0))
			x = map.GetLength(0) - 1;

		if (y < 0)
			y = 0;
		else if (y >= map.GetLength(1))
			y = map.GetLength(1) - 1;

		if (depth < range & !map[x, y].opaque)
			ScanOctant(source, range, depth + 1, octant, startSlope, endSlope);

	}
	private double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert) {
		if (pInvert)
			return (pY1 - pY2) / (pX1 - pX2);
		else
			return (pX1 - pX2) / (pY1 - pY2);
	}

	private int GetVisDistance(int pX1, int pY1, int pX2, int pY2) {
		return ((pX1 - pX2) * (pX1 - pX2)) + ((pY1 - pY2) * (pY1 - pY2));
	}
}