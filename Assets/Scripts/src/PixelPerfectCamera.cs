using UnityEngine;
using System.Collections;

public class PixelPerfectCamera : MonoBehaviour {
	public float textureSize = 100f;
	float unitsPerPixel;
	void Start () {
		unitsPerPixel = 1f / textureSize;
		Camera.main.orthographicSize = (Screen.height / 2f) * unitsPerPixel;
	}
}