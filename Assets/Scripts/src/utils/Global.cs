using UnityEngine;
using System.Collections.Generic;

public class Global : MonoBehaviour {
	private static Global instance;
	//key references
	public BattleManager battleManager;
	public MapManager mapManager;
	public CameraController cam;
	public InputManager inputManager;
	public LineRenderer lineDrawer;
	//camera and animation speed
	public float cameraScrollSpeed = 1f;
	public float animationSpeed = 1f;
	//ui viewport size
	public float uiDockSizeX;
	public float uiDockSizeY;

	public Vector2 mapMinBounds;
	public Vector2 mapMaxBounds;

	public static Global Instance {
		get { 
			return instance ?? (instance = new GameObject("Global").AddComponent<Global>());
		}
	}

	public void Init() {
		battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
		mapManager = GameObject.Find("BattleManager").GetComponent<MapManager>();
		cam = Camera.main.GetComponent<CameraController>();
		inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
		lineDrawer = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
		uiDockSizeX = Screen.width / 5 * 4;
		uiDockSizeY = Screen.height / 5;
	}

	public void SetMapBounds(Vector2 min, Vector2 max) {
		cam.mapMinBounds = mapMinBounds = min;
		cam.mapMaxBounds = mapMaxBounds = max;
	}
}