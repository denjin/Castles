using UnityEngine;
using System.Collections;

namespace Battle{


public class UnitSprite : MonoBehaviour {
	public int id;
	public int armyId;
	public int division;
	public Vector2 target;

	public Vector2 velocity;
	public Vector2 currentLocation;
	public Vector2 targetLocation;
	public Vector2[] path;
	int targetIndex;
	
	void Start() {
		target = Vector2.zero;
		PathRequestManager.Instance.RequestPath(transform.position, target, OnPathFound);
	}

	public void OnPathFound(Vector2[] newPath, bool pathSuccess) {
		if(pathSuccess) {
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath() {
		Vector2 currentWaypoint = path[0];

		while(true) {
			if (transform.position.x == currentWaypoint.x && transform.position.y == currentWaypoint.y) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					targetIndex = 0;
					path = new Vector2[0];
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			//Debug.Log("moving towards " + currentWayPoint);
			
			transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, 1f * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], new Vector3(0.1f, 0.1f, 0.1f));

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}
}