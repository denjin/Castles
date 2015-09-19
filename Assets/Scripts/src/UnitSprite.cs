using UnityEngine;
using System.Collections;
using System;

namespace Battle{


public class UnitSprite : MonoBehaviour {
	public int id;
	public int armyId;
	public int division;
	public Vector2 target;
	public Vector2 newTarget;

	public Vector2 velocity;
	public Vector2 currentLocation;
	public Vector2 targetLocation;
	public Vector2[] path;
	int targetIndex;
	
	void Start() {
		target = Vector2.zero;
		newTarget = Vector2.zero;
		//PathRequestManager.Instance.RequestPath(transform.position, target, OnPathFound);
	}

	void Update() {
		if (target != newTarget) {
			ResetPath();
			PathRequestManager.Instance.RequestPath(transform.position, newTarget, OnPathFound);
			target = newTarget;
		}
	}

	public void OnPathFound(Vector2[] newPath, bool pathSuccess) {
		if(pathSuccess) {
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	private void ResetPath() {
		Debug.Log("Resetting path");
		
		targetIndex = 0;
		path = new Vector2[0];
	}

	IEnumerator FollowPath() {
		Vector2 currentWaypoint = path[0];
		while(true) {
			if (transform.position.x == currentWaypoint.x && transform.position.y == currentWaypoint.y) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					ResetPath();
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, 1f * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;

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