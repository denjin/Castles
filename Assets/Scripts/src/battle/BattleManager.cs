using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Armies;
using Gear;

namespace Battle {
	public class BattleManager : MonoBehaviour {
		//the camera
		public Camera mainCamera;
		//the map
		public MapManager Map;

		private GameObject[,] armySprites;
		private Soldier[,] armyData;

		void Awake() {
			//load the map
			Map = new MapManager();
			//initialise the map
			Map.init(mainCamera);
			//load the armies
			//armyManager = new ArmyManager(1);
			//deploy the armies
			//DeployArmies();
			initArmies(2, 10);
			//Debug.Log(armies[0, 0]);
		}

		void initArmies(int _armies, int _soldiers) {
			armySprites = new GameObject[_armies, _soldiers];
			armyData = new Soldier[_armies, _soldiers];
			
			for (int i = 0; i < _armies; i++) {
				for (int j = 0; j < _soldiers; j++) {
					//init data object
					armyData[i,j] = DataStore.Instance.GetSoldier("Peasant");
					//init display object
					armySprites[i,j] = new GameObject();
					//add sprite renderer
					armySprites[i,j].AddComponent<SpriteRenderer>();
					//load sprite
					armySprites[i,j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("graphics/units/" + armyData[i,j].GetSprite());
					armySprites[i,j].transform.position = new Vector2(j, 0);
				}
			}
			
		}

		void DeployArmies() {
			//pick deployment zones
			//Debug.Log(armyManager.GetSoldiers(0).Length);
			
		}

		/*
		GameObject InstantiateSoldier(string _soldier) {

		}
		*/


	}
}