using UnityEngine;
using System.Collections;
using SpriteTile;
using DG.Tweening;

public class CameraController : MonoBehaviour {
	public Vector2 mapMinBounds;
	public Vector2 mapMaxBounds;

	public float currentZoom;
	public float maxZoom;
	public float minZoom;

	private float baseCameraSpeed = 5f;

	Camera cam;
	
	void Awake () {
		cam = Camera.main;
		maxZoom = 3f;
		minZoom = 10f;
		cam.orthographicSize = 5f;
		currentZoom = Camera.main.orthographicSize;
	}
	
	void Update () {
		currentZoom -= Input.GetAxis("ScrollWheel");
		if (currentZoom < maxZoom) {
			currentZoom = maxZoom;
		} else if (currentZoom > minZoom) {
			currentZoom = minZoom;
		}
		cam.orthographicSize = currentZoom;

		if (Input.GetKey(KeyCode.RightArrow)){
			transform.position += Vector3.right * baseCameraSpeed * Global.Instance.cameraScrollSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
			transform.position += Vector3.left * baseCameraSpeed * Global.Instance.cameraScrollSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.UpArrow)){
			transform.position += Vector3.up * baseCameraSpeed * Global.Instance.cameraScrollSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.DownArrow)){
			transform.position += Vector3.down * baseCameraSpeed * Global.Instance.cameraScrollSpeed * Time.deltaTime;
		}
	}

	void LateUpdate() {
		float verticalExtent = cam.orthographicSize;
		float horizontalExtent = verticalExtent * Screen.width / Screen.height;
		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x, mapMinBounds.x, mapMaxBounds.x),
			Mathf.Clamp(transform.position.y, mapMinBounds.y, mapMaxBounds.y),
			-10f
		);
	}

	public void ScrollTo(Vector2 target) {
		float x = Mathf.Clamp(target.x, mapMinBounds.x, mapMaxBounds.x);
		float y = Mathf.Clamp(target.y, mapMinBounds.y, mapMaxBounds.y);
		cam.transform.DOMove(new Vector3(x, y, -10f), 1);
	}
}