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
			initArmies(2, 10);
			DeployArmies();
		}

		private void initArmies(int _armies, int _soldiers) {
			armySprites = new GameObject[_armies][];
			armyData = new Soldier[_armies][];
			
			for (int i = 0; i < _armies; i++) {
				armySprites[i] = new GameObject[_soldiers];
				armyData[i] = new Soldier[_soldiers];
				for (int j = 0; j < _soldiers; j++) {
					//init data object
					armyData[i][j] = DataStore.Instance.GetSoldier("Peasant");
					//init display object
					armySprites[i][j] = new GameObject();
					//add sprite renderer
					armySprites[i][j].AddComponent<SpriteRenderer>();
					//load sprite
					armySprites[i][j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(graphicsPath + armyData[i][j].GetSprite());
					armySprites[i][j].GetComponent<SpriteRenderer>().sortingLayerName = "Units";
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
				for (int j = 0; j < armyData[i].Length; j++) {
					Vector3 pos = map.TileToWorld(deploymentTile, false);
					armySprites[i][j].transform.position = pos;
				}
			}
			
		}
	}
}