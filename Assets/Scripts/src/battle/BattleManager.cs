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
			
			//DeployArmies();
			//Debug.Log(DataStore.Instance.GetSoldier("Peasant").GetWeaponItem("1"));
			//Debug.Log(DataStore.Instance.GetSoldier("Leader").GetWeaponItem("1"));
		}

		private void InitArmies(string[] _belligerents) {
			//Debug.Log(DataStore.Instance.GetCharacter("Dave").GetSoldier("peasant"));
			//armySprites = new List<GameObject>[_belligerents.Length];
			armyData = new List<Soldier>[_belligerents.Length];

			Character character;
			Dictionary<string, int> soldiers;
			
			for (int i = 0; i < _belligerents.Length; i++) {
				character = DataStore.Instance.GetCharacter(_belligerents[i]);
				soldiers = character.GetSoldiers();
				//armySprites[i] = new GameObject[soldiers.Count];
				armyData[i] = AddSoldiers(character.GetSoldiers());
			}
		}

		private List<Soldier> AddSoldiers(Dictionary<string, int> _data) {
			//create list to store soldiers
			List<Soldier> soldiers = new List<Soldier>();
			//for each soldier type in the data
			foreach (KeyValuePair<string, int> type in _data) {
				//for each required soldier of this type
				for (int i = 0; i < type.Value; i++) {
					soldiers.Add(DataStore.Instance.GetSoldier(type.Key));
				}
			}
			return soldiers;
		}

		/*
		private void AddSoldier(int _i, int _j, string _type) {
			//init data object
			armyData[_i][_j] = DataStore.Instance.GetSoldier(_type);
			//init display object
			armySprites[_i][_j] = new GameObject();
			//add sprite renderer
			armySprites[_i][_j].AddComponent<SpriteRenderer>();
			//load sprite
			armySprites[_i][_j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(graphicsPath + armyData[_i][_j].GetSprite());
			armySprites[_i][_j].GetComponent<SpriteRenderer>().sortingLayerName = "Units";
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
		*/
	}
}