using UnityEngine;

public class MouseFollow : MonoBehaviour {

	void Update() {
		Vector3 pos = Input.mousePosition;
		pos.y -= 50;
		transform.position = pos;
	}
}