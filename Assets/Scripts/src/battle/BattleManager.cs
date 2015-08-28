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

		private List<GameObject>[] armySprites;
		private List<Soldier>[] armyData;

		void Awake() {
			//load the map
			map = new MapManager(mainCamera, 10, 10);
			//init armies
			
			string[] belligerents = new string[2];
			belligerents[0] = "Dave";
			belligerents[1] = "Pete";
			InitArmies(belligerents);
			Debug.Log(armyData[0].Count);
			
			DeployArmies();
			//Debug.Log(DataStore.Instance.GetSoldier("Peasant").GetWeaponItem("1"));
			//Debug.Log(DataStore.Instance.GetSoldier("Leader").GetWeaponItem("1"));
		}

		private void InitArmies(string[] _belligerents) {
			//Debug.Log(DataStore.Instance.GetCharacter("Dave").GetSoldier("peasant"));
			armySprites = new List<GameObject>[_belligerents.Length];
			armyData = new List<Soldier>[_belligerents.Length];

			Character character;
			Dictionary<string, int> soldiers;
			
			for (int i = 0; i < _belligerents.Length; i++) {
				character = DataStore.Instance.GetCharacter(_belligerents[i]);
				soldiers = character.GetSoldiers();
				//armySprites[i] = new GameObject[soldiers.Count];
				AddSoldiers(i, character.GetSoldiers());
			}
		}

		private void AddSoldiers(int _character, Dictionary<string, int> _data) {
			//create lists to store soldiers
			armySprites[_character] = new List<GameObject>();
			armyData[_character] = new List<Soldier>();
			//for each soldier type in the data
			foreach (KeyValuePair<string, int> type in _data) {
				//for each required soldier of this type
				for (int i = 0; i < type.Value; i++) {
					//create temp soldier data
					Soldier soldier = DataStore.Instance.GetSoldier(type.Key);
					//add soldier data
					armyData[_character].Add(soldier);
					//add soldier sprite
					GameObject soldierSprite = new GameObject();
					//add sprite renderer component to new sprite and load appropriate sprite into it
					soldierSprite.AddComponent<SpriteRenderer>();
					soldierSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(graphicsPath + soldier.GetSprite());
					soldierSprite.GetComponent<SpriteRenderer>().sortingLayerName = "Units";
				}
			}
		}

		private void DeployArmies() {
			Int2 mapSize = map.GetSize();
			Int2 deploymentTile = new Int2();
			Vector3 tilePosition = new Vector3();

			for (int i = 0; i < armyData.Length; i++) {
				deploymentTile.x = (int)Mathf.Floor(Random.value * mapSize.x);
				deploymentTile.y = (int)Mathf.Floor(Random.value * mapSize.y);
				for (int j = 0; j < armyData[i].Count; j++) {
					Vector3 pos = map.TileToWorld(deploymentTile, false);
					armySprites[i][j].transform.position = pos;
				}
			}
			
		}
	}
}