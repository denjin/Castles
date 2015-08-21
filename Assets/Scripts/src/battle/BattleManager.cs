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
			//load gear data
			armourItems = new ArmourList();
			weaponItems = new WeaponList();
			//load soldiers data
			soldiers = new SoldierList();

			//load the armies
			armyManager = new ArmyManager(1);

			//deploy the armies
			
		}

		void DeployArmies() {
			//pick deployment zones
			Debug.Log(armyManager.GetSoldiers(0));
			
		}


	}
}