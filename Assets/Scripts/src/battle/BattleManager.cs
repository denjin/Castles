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

		private GameObject[,] armySprites;
		private Soldier[,] armyData;

		void Awake() {
			//load the map
			map = new MapManager(mainCamera, 10, 10);
			//init armies
			initArmies(2, 10);
			DeployArmies();
		}

		private void initArmies(int _armies, int _soldiers) {
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
					armySprites[i,j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(graphicsPath + armyData[i,j].GetSprite());
					armySprites[i,j].transform.position = new Vector2(0f + Random.value - 0.5f, 0f + Random.value - 0.5f);
				}
			}
			
		}

		private void DeployArmies() {
			Int2 mapSize = map.GetSize();
			Int2 deploymentTile = new Int2((int)Mathf.Floor(Random.value * mapSize.x), (int)Mathf.Floor(Random.value * mapSize.y));
			Debug.Log(deploymentTile);
		}
	}
}