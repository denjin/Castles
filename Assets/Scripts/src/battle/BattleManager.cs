using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using SpriteTile;
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
		public WeaponList weaponItems;
		public SoldierList soldiers;

		void Awake() {
			//load the map
			Map = new MapManager();
			//initialise the map
			Map.init(mainCamera);
			//load the armies
			armyManager = new ArmyManager(1);
			//load gear
			armourItems = new ArmourList();
			weaponItems = new WeaponList();
			//load soldiers
			soldiers = new SoldierList();
		}

		IEnumerator DeployUnits() {
			//runs forever
			for (;;) {
				
			}
		}
	}
}