using UnityEngine;

public class Bounds {
	public int minx;
	public int miny;
	public int maxx;
	public int maxy;

	public Bounds(int _minx, int _miny, int _maxx, int _maxy) {
		minx = _minx;
		miny = _miny;
		maxx = _maxx;
		maxy = _maxy;
	}

	public int Clamp(int val, string dir) {
		if (dir == "x") {
			return Mathf.Clamp(val, minx, maxx);
		}
		return Mathf.Clamp(val, miny, maxy);
	}
}