using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using Battle;
using Armies;
using Gear;

namespace Battle {
	public class BattleManager : MonoBehaviour {
		//the camera
		public Camera mainCamera;
		//the map
		public MapManager Map;
		//the armies
		public ArmyManager armyManager;

		public ArmourList armourItems;

		void Awake() {
			//load the children
			Map = new MapManager();
			armyManager = new ArmyManager(1);
			//initialise the map
			Map.init(mainCamera);
			armourItems = new ArmourList();
			//Debug.Log(armourItems.getItem("Cloth Cap").getName());
			
		}
	}
}