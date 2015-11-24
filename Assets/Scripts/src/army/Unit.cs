using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {
	//name
	public string myName;
	public int myId;
	//graphics
	public string portrait;
	//traits
	public int ballisticSkill;
	public int weaponSkill;
	public int strength;
	public int toughness;
	public int initiative;

	public int sightRange = 12;
	//stats
	public int experience;
	public int baseHealth;
	public int health;
	public int baseMorale;
	public int morale;
	//gear
	public string weapon1;
	public string weapon2;

	public int baseMovementPoints = 5;
	public int currentMovementPoints;

	public int baseActionPoints = 1;
	public int currentActionPoints;

	//for pathfinding
	public Vector2 target;
	public Vector2 newTarget;
	public Vector2[] path;
	//public MapManager map;
	int targetIndex;
	//current location
	public Vector2 oldPosition;
	public Vector2 newPosition;
	public Node currentNode;
	public Node newNode;
	public Vector2 direction;
	public float targetAngle;
	//battle modifiers
	public bool moving = false;
	public bool running = false;
	public bool stopping = false;

	//ai related stuff
	public bool human;
	public string behaviour;

	//events
	public delegate void PointsChanged(string name, int points, string type);
	public static event PointsChanged OnPointsChanged;

	public delegate void EnteredNewNode();
	public static event EnteredNewNode OnEnteredNewNode;

	public delegate void Arrived();
	public static event Arrived OnArrived;

	void Start() {
		target = Vector2.zero;
		newTarget = Vector2.zero;

		currentNode = newNode = Global.Instance.mapManager.WorldToNode(transform.position);
		StartTurn();
		BattleManager.OnStopMovement += StopMovement;
	}

	void Awake() {
		StartTurn();
	}

	void Update() {
		//check if this unit has been given a new target
		if (target != newTarget) {
			Debug.Log(newTarget);
			stopping = false;
			StopCoroutine("FollowPath");
			//clear our existing path
			ResetPath();
			//ask for a new path
			RequestNewPath();
			//set our current target to the new target
			target = newTarget;
		}
	}

	public void StartTurn() {
		currentMovementPoints = baseMovementPoints;
		currentActionPoints = baseActionPoints;
	}

	public void RequestNewPath() {
		Vector2[] path = Global.Instance.mapManager.pathing.GetPath(transform.position, newTarget);
		if (path.Length > 0) {
			OnPathFound(path, true);
		}
	}

	public void OnPathFound(Vector2[] newPath, bool pathSuccess) {
		if(pathSuccess) {
			path = new Vector2[Mathf.Min(currentMovementPoints, newPath.Length)];
			for (int i = 0; i < path.Length; i++) {
				path[i] = newPath[i];
			}
			if (path.Length > 0) {
				StartCoroutine("FollowPath");
			}
		}
	}

	private void ResetPath() {
		targetIndex = 0;
		path = new Vector2[0];
	}

	public void StopMovement() {
		if (moving) {
			stopping = true;
		}
		
	}

	IEnumerator FollowPath() {
		Vector2 currentWaypoint = path[0];
		while(true) {
			if (transform.position.x == currentWaypoint.x && transform.position.y == currentWaypoint.y) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					moving = false;
					ResetPath();
					OnArrived();
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			moving = true;
			//get the position we're going to move to
			newPosition = Vector2.MoveTowards(transform.position, currentWaypoint, Global.Instance.animationSpeed * Time.deltaTime);
			//get the direction we're moving in
			direction = newPosition - oldPosition;
			//get the angle I want to rotate towards
			targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			//rotate myself to that angle
			transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
			//set our new position
			transform.position = newPosition;
			newNode = Global.Instance.mapManager.WorldToNode(transform.position);
			if (stopping) {
				stopping = false;
				ResetPath();
			}
			if (newNode != currentNode) {
				newNode.occupied = true;
				currentNode.occupied = false;
				currentNode = newNode;
				currentMovementPoints -= 1;
				if (OnPointsChanged != null) {
					OnPointsChanged(myName, currentMovementPoints, "Movement");
				}
				if (OnEnteredNewNode != null) {
					OnEnteredNewNode();
				}
			}
			//record our new position as next frame's old position
			oldPosition = transform.position;
			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;
				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				} else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}