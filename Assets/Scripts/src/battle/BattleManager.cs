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
		public WeaponList weaponItems;
		public SoldierList soldiers;

		void Awake() {
			//load the children
			Map = new MapManager();
			armyManager = new ArmyManager(1);
			//initialise the map
			Map.init(mainCamera);
			//load gear
			armourItems = new ArmourList();
			weaponItems = new WeaponList();
			//load soldiers
			soldiers = new SoldierList();
			Debug.Log(soldiers.GetSoldier("Peasant").GetWeaponItem("1"));
		}
	}
}