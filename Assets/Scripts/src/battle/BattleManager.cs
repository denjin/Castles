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
		private int totalSoldiers;

		void Awake() {
			map = new MapManager(mainCamera, 10, 10);
			string[] belligerents = new string[2];
			belligerents[0] = "Dave";
			belligerents[1] = "Pete";
			InitArmies(belligerents);
			DeployArmies();
			
		}

		void Update() {
			//sort soldiers
			for (int i = 0; i < armySprites.Length; i++) {
				for (int j = 0; j < armySprites[i].Count; j++) {
					SortSprite(armySprites[i][j]);
				}
			}
		}

		/**
		 * Starts the initialisation of the soldiers and characters for the battle, sets up the various arrays & lists
		 * @param {[type]} string[] _belligerents array of characters names
		 */
		private void InitArmies(string[] _belligerents) {
			//Debug.Log(DataStore.Instance.GetCharacter("Dave").GetSoldier("peasant"));
			armySprites = new List<GameObject>[_belligerents.Length];
			armyData = new List<Soldier>[_belligerents.Length];

			Character character;
			Dictionary<string, int> soldiers;
			
			for (int i = 0; i < _belligerents.Length; i++) {
				character = DataStore.Instance.GetCharacter(_belligerents[i]);
				
				//create lists to store soldiers
				armyData[i] = new List<Soldier>();
				armySprites[i] = new List<GameObject>();
				//add leader soldier
				CreateSoldier(i, 0, "leader");

				soldiers = character.GetSoldiers();
				AddSoldiers(i, soldiers);
			}
		}

		/**
		 * Creates a soldier and an on screen soldier asset, sets up it's graphics etc
		 * @param {int}    _armyId       the id of the army this soldier belongs to
		 * @param {int}    _soldierId    the id of this soldier
		 * @param {int}    _type         the type of soldier to create
		 * @param {String} _sortingLayer the sorting layer, defaults to the units layer
		 */
		private void CreateSoldier(int _armyId, int _soldierId, string _type, string _sortingLayer = "Units") {
			//create temp soldier data
			Soldier soldier = DataStore.Instance.GetSoldier(_type);
			//add soldier data
			armyData[_armyId].Add(soldier);
			//add soldier sprite
			GameObject soldierSprite = new GameObject();
			//add sprite renderer component to new sprite and load appropriate sprite asset into it
			soldierSprite.AddComponent<SpriteRenderer>();
			soldierSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(graphicsPath + soldier.GetSprite());
			soldierSprite.GetComponent<SpriteRenderer>().sortingLayerName = _sortingLayer;
			//add component to track id and army id
			soldierSprite.AddComponent<UnitSprite>();
			soldierSprite.GetComponent<UnitSprite>().armyId = _armyId;
			soldierSprite.GetComponent<UnitSprite>().id = _soldierId;
			//add sprite to list
			armySprites[_armyId].Add(soldierSprite);
		}

		/**
		 * Creates the soldiers for the battle and adds them to the right army
		 * Also initialises the sprites on screen
		 * @param {[type]} int _character
		 * @param {[type]} Dictionary<string, int>       _data         [description]
		 */
		private void AddSoldiers(int _character, Dictionary<string, int> _data) {
			totalSoldiers = 1;
			//for each soldier type in the data
			foreach (KeyValuePair<string, int> type in _data) {
				//for each required soldier of this type
				for (int i = 0; i < type.Value; i++) {
					//add a soldier
					CreateSoldier(_character, totalSoldiers, type.Key);
					totalSoldiers += 1;
				}
			}
			//reset num of soldiers
			totalSoldiers = 1;
		}

		/**
		 * Positions the soldiers in a randomly assigned tile
		 */
		private void DeployArmies() {
			//sets the size of the map
			Int2 mapSize = map.GetSize();
			Int2 deploymentTile = new Int2();
			Vector3 tilePosition = new Vector3();
			int q;
			//set a random starting tile for each army
			for (int i = 0; i < armyData.Length; i++) {
				deploymentTile.x = (int)Mathf.Floor(Random.value * mapSize.x);
				deploymentTile.y = (int)Mathf.Floor(Random.value * mapSize.y);
				//set each soldier's position to the target tile
				for (int j = 0; j < armyData[i].Count; j++) {
					Vector3 pos = map.TileToWorld(deploymentTile, false);
					//shuffle the position slightly
					pos.x += (Random.value - 0.5f);
					pos.y += (Random.value - 0.5f);
					armyData[i][j].SetPos(pos.x, pos.y);
					armySprites[i][j].transform.position = pos;
				}
			}
			
		}

		private void SortSprite(GameObject _sprite) {
			//int pos = Mathf.RoundToInt(_sprite.transform.position.y);
			//pos /= 3;
			//Debug.Log(pos);
			float y = _sprite.transform.position.y;
			Vector3 pos = _sprite.transform.position;
			pos.z = y;
			_sprite.transform.position = pos;
		}
	}
}