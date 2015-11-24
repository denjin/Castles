using UnityEngine;

public class InputManager : MonoBehaviour {
	public bool doubleClick;
	private float lastClickTime = 0f;
	private float catchTime = 0.5f;
	public bool shiftKey;

	public delegate void TabPressed();
	public static event TabPressed OnTabPressed;

	public delegate void ActionStart();
	public static event ActionStart OnActionStart;

	public delegate void ActionEnd();
	public static event ActionEnd OnActionEnd;

	void Update() {
		//is the player holding down the shift key?
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			shiftKey = true;
		} else {
			shiftKey = false;
		}

		if (Input.GetMouseButtonDown(0)) {
			//check if we did a double or single click
			doubleClick = Time.time - lastClickTime <= catchTime ? true : false;
			//check if we clicked inside the map area
			if (Input.mousePosition.y > Global.Instance.uiDockSizeY && Input.mousePosition.x < Global.Instance.uiDockSizeX) {
				//find out if we clicked on a soldier
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if(hit.collider != null) {
					Global.Instance.battleManager.SoldierSelected(hit.collider.gameObject.GetComponent<Unit>().myId);
				}
				if (doubleClick) {
					Global.Instance.cam.ScrollTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				}
			}
			lastClickTime = Time.time;
		}
		/*
		if (Input.GetMouseButtonDown(1)) {

			if (OnActionPressed != null) {
				OnActionPressed();
			}
		}
		*/
	
		if (Input.GetMouseButton(1) && Input.mousePosition.x < Global.Instance.uiDockSizeX && Input.mousePosition.y > Global.Instance.uiDockSizeY) {
			if (OnActionStart != null) {
				OnActionStart();
			}
		}

		if (Input.GetMouseButtonUp(1) && Input.mousePosition.x < Global.Instance.uiDockSizeX && Input.mousePosition.y > Global.Instance.uiDockSizeY) {
			if (OnActionEnd != null) {
				OnActionEnd();
			}
		}

		if (Input.GetKeyDown(KeyCode.Tab)) {
			if (OnTabPressed != null) {
				OnTabPressed();
			}
		}
	}
}