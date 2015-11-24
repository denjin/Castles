using UnityEngine;
using System.Collections.Generic;

public class AIPlayer {
	public GameObject[] soldierObjects;
	public Unit[] soldierStats;

	MapManager map;

	public AIPlayer(MapManager _map) {
		map = _map;
		soldierObjects = new GameObject[0];
		soldierStats = new Unit[0];
	}

	public void LoadUnit(GameObject unit, Vector2 pos) {
		GameObject newUnit = GameObject.Instantiate(unit, pos, new Quaternion()) as GameObject;
		GameObject[] newObjectsArray = new GameObject[soldierObjects.Length + 1];
		Unit[] newStatsArray = new Unit[soldierObjects.Length + 1];
		int i;
		for (i = 0; i < soldierObjects.Length; i++) {
			newObjectsArray[i] = soldierObjects[i];
			newStatsArray[i] = soldierObjects[i].GetComponent<Unit>();
		}
		newObjectsArray[soldierObjects.Length] = newUnit;
		newUnit.GetComponent<Unit>().human = false;
		newUnit.GetComponent<Unit>().behaviour = "wait";
		newStatsArray[soldierObjects.Length] = newUnit.GetComponent<Unit>();

		soldierObjects = newObjectsArray;
		soldierStats = newStatsArray;
	}

	public void StartAITurn() {
		int i;
		for (i = 0; i < soldierStats.Length; i++) {
			if (soldierStats[i].behaviour == "wait") {
				SetBehaviour(soldierStats[i]);
			}
		}

		for (i = 0; i < soldierStats.Length; i++) {
			if (soldierStats[i].behaviour == "attack") {
				Attack(soldierStats[i]);
			}
		}

	}

	public void Attack(Unit unit) {
		
	}

	public void SetBehaviour(Unit unit) {
		bool enemiesSpotted = false;
		List<Node> visibleNodes = map.fov.GetVisibleNodes(map.WorldToNode(unit.gameObject.transform.position), unit.sightRange);
		List<Unit> visibleEnemies = GetVisibleUnits(visibleNodes, Global.Instance.battleManager.player.soldierStats);
		if (visibleEnemies.Count > 0) {
			enemiesSpotted = true;
		}
		if (enemiesSpotted) {
			unit.behaviour = "attack";
		}
	}

	public List<Unit> GetVisibleUnits(List<Node> visibleNodes, Unit[] enemies) {
		List<Unit> visibleUnits = new List<Unit>();
		for (int i = 0; i < enemies.Length; i++) {
			if (visibleNodes.Contains(enemies[i].currentNode)) {
				visibleUnits.Add(enemies[i]);
			}
		}
		return visibleUnits;
	}


}

