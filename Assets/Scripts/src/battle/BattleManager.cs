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
		public MapManager map;

		private string graphicsPath = "graphics/units/";

		private GameObject[][] armySprites;
		private Soldier[][] armyData;

		void Awake() {
			//load the map
			map = new MapManager(mainCamera, 10, 10);
			//init armies
			
			string[] belligerents = new string[2];
			belligerents[0] = "Dave";
			belligerents[1] = "Pete";
			initArmies(belligerents);
			DeployArmies();
			//Debug.Log(DataStore.Instance.GetSoldier("Peasant").GetWeaponItem("1"));
			//Debug.Log(DataStore.Instance.GetSoldier("Leader").GetWeaponItem("1"));
		}

		private void initArmies(string[] _belligerents) {
			//Debug.Log(DataStore.Instance.GetCharacter("Dave").GetSoldier("peasant"));
			armySprites = new GameObject[_belligerents.Length][];
			armyData = new Soldier[_belligerents.Length][];

			Character character;
			int soldiers;
			
			for (int i = 0; i < _belligerents.Length; i++) {
				character = DataStore.Instance.GetCharacter(_belligerents[i]);
				soldiers = character.SoldierCount();
				

				
				armySprites[i] = new GameObject[soldiers];
				armyData[i] = new Soldier[soldiers];
				
				//go through each listed soldier type
				for (int j = 0; j < soldiers; j++) {
					


					/*
					//init data object
					armyData[i][j] = DataStore.Instance.GetSoldier("Peasant");
					//init display object
					armySprites[i][j] = new GameObject();
					//add sprite renderer
					armySprites[i][j].AddComponent<SpriteRenderer>();
					//load sprite
					armySprites[i][j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(graphicsPath + armyData[i][j].GetSprite());
					armySprites[i][j].GetComponent<SpriteRenderer>().sortingLayerName = "Units";
					*/
				}
				
			}
			
		}

		private void AddSoldier() {

		}

		private void DeployArmies() {
			Int2 mapSize = map.GetSize();
			Int2 deploymentTile = new Int2();
			Vector3 tilePosition = new Vector3();

			for (int i = 0; i < armyData.Length; i++) {
				deploymentTile.x = (int)Mathf.Floor(Random.value * mapSize.x);
				deploymentTile.y = (int)Mathf.Floor(Random.value * mapSize.y);
				for (int j = 0; j < armyData[i].Length; j++) {
					Vector3 pos = map.TileToWorld(deploymentTile, false);
					armySprites[i][j].transform.position = pos;
				}
			}
			
		}
	}
}