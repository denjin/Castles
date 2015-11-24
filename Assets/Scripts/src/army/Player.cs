using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Player {
	public string name;
	public GameObject[] soldierObjects;
	public Unit[] soldierStats;

	public Player (string _name, GameObject[] _soldiers) {
		name = _name;
		soldierObjects = _soldiers;
		soldierStats = new Unit[soldierObjects.Length];
		for (int i = 0; i < soldierObjects.Length; i++) {
			soldierStats[i] = soldierObjects[i].GetComponent<Unit>();
		}
	}
}