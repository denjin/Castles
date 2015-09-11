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
		//the belligerent who is the human player
		private int human = 0;
		private string selectedDivision = "all";

		private string graphicsPath = "graphics/units/";

		private List<Belligerent> belligerents;
		private List<GameObject>[] armySprites;
		private int totalSoldiers;

		GameObject rallyPoint;

		void Awake() {
			map = new MapManager(mainCamera, 100, 100);
			string[] characters = new string[2];
			characters[0] = "Dave";
			characters[1] = "Pete";
			InitArmies(characters);
			//Debug.Log(belligerents[0].soldiers[1].GetI());
			DeployArmies();
			rallyPoint = new GameObject();
			rallyPoint.AddComponent<SpriteRenderer>();
			rallyPoint.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("graphics/misc/rallypoint");
			rallyPoint.GetComponent<SpriteRenderer>().sortingLayerName = "Units";
		}

		void Update() {
			/*
			if (Input.GetMouseButton(0)) {
				Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
				pos.z = 0;
				rallyPoint.transform.position = pos;
			}
			*/
			for (int i = 0; i < /*belligerents.Count*/1; i++) {
				for (int j = 0; j < belligerents[i].infantry.Count; j++) {
					Vector3 velocity = SteerForSeek(belligerents[i].infantry[j].GetComponent<UnitSprite>().velocity, belligerents[i].infantry[j].transform.position, belligerents[i].infantry[j].GetComponent<UnitSprite>().targetLocation, 0.05f);
					belligerents[i].infantry[j].GetComponent<UnitSprite>().velocity = velocity;
					belligerents[i].infantry[j].transform.position += velocity;
				}
				for (int j = 0; j < belligerents[i].archers.Count; j++) {
					Vector3 velocity = SteerForSeek(belligerents[i].archers[j].GetComponent<UnitSprite>().velocity, belligerents[i].archers[j].transform.position, belligerents[i].archers[j].GetComponent<UnitSprite>().targetLocation, 0.05f);
					belligerents[i].archers[j].GetComponent<UnitSprite>().velocity = velocity;
					belligerents[i].archers[j].transform.position += velocity;
				}
			}
			/*
			for (int i = 0; i < armySprites.Length; i++) {
				for (int j = 0; j < armySprites[i].Count; j++) {
					if (i == 0) {
						Vector3 velocity = SteerForSeek(armySprites[i][j].GetComponent<UnitSprite>().velocity, armySprites[i][j].transform.position, armySprites[i][j].GetComponent<UnitSprite>().targetLocation, 0.05f);
						//armySprites[i][j].GetComponent<UnitSprite>().velocity = velocity;
						//armySprites[i][j].transform.position += velocity;
					}
					//sort soldier graphics
					SortSprite(armySprites[i][j]);
				}
			}
			*/
		}


		/**
		 * Starts the initialisation of the soldiers and characters for the battle, sets up the various arrays & lists
		 * @param {[type]} string[] _belligerents array of characters names
		 */
		private void InitArmies(string[] _belligerents) {
			armySprites = new List<GameObject>[_belligerents.Length];
			//armyData = new List<Soldier>[_belligerents.Length];
			belligerents = new List<Belligerent>();
			Character character;
			Dictionary<string, int> soldiers;
			
			for (int i = 0; i < _belligerents.Length; i++) {
				character = DataStore.Instance.GetCharacter(_belligerents[i]);
				//add new belligerent
				Belligerent belligerent = new Belligerent(i, character.GetName());
				belligerents.Add(belligerent);
				//create list to store soldier sprites
				armySprites[i] = new List<GameObject>();
				//add leader soldier
				CreateSoldier(i, 0, "leader");
				//add the rest of the soldiers
				soldiers = character.GetSoldiers();
				AddSoldiers(i, soldiers);
			}
			Debug.Log(belligerents[0].infantry.Count);
			
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
			soldier.SetI(_soldierId);
			WeaponItem weapon = DataStore.Instance.GetWeaponItem(soldier.GetWeaponItem("1"));
			//add soldier data to belligrent
			belligerents[_armyId].soldiers.Add(soldier);
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
			if (weapon.GetRange() > 0) {
				belligerents[_armyId].archers.Add(soldierSprite);
			} else {
				belligerents[_armyId].infantry.Add(soldierSprite);
			}
			if (_armyId == human) {
				mainCamera.GetComponent<OutlineEffect>().outlineRenderers.Add(soldierSprite.GetComponent<SpriteRenderer>());
			}
			
			//add sprite to list
			//armySprites[_armyId].Add(soldierSprite);
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
			int i;
			int j;
			//set a random starting tile for each army
			for (i = 0; i < belligerents.Count; i++) {
				deploymentTile.x = (int)Mathf.Floor(Random.value * mapSize.x);
				deploymentTile.y = (int)Mathf.Floor(Random.value * mapSize.y);
				Vector3 pos = map.TileToWorld(deploymentTile);
				//set each soldier's position to the target tile
				for (j = 0; j < belligerents[i].infantry.Count; j++) {
					//shuffle the position slightly
					pos.x += (Random.value - 0.5f);
					pos.y += (Random.value - 0.5f);
					//armyData[i][j].SetPos(pos.x, pos.y);
					belligerents[i].infantry[j].transform.position = pos;
				}
				for (j = 0; j < belligerents[i].archers.Count; j++) { 
					//shuffle the position slightly
					pos.x += (Random.value - 0.5f);
					pos.y += (Random.value - 0.5f);
					//armyData[i][j].SetPos(pos.x, pos.y);
					belligerents[i].archers[j].transform.position = pos;
				}
			}
			
		}

		/**
		 * Sorts a sprite by it's x position
		 * @param GameObject _sprite the sprite to sort
		 */
		private void SortSprite(GameObject _sprite) {
			float y = _sprite.transform.position.y;
			Vector3 pos = _sprite.transform.position;
			pos.z = y;
			_sprite.transform.position = pos;
		}

		public Vector3 SteerForSeek(Vector3 _currentVelocity, Vector3 _currentLocation, Vector3 _targetLocation, float _maxSpeed) {
			Vector3 desiredVelocity = Truncate(_targetLocation - _currentLocation, _maxSpeed);
			Vector3 steering = Truncate(desiredVelocity - _currentVelocity, _maxSpeed);
			Vector3 newVelocity = Truncate(_currentVelocity + steering, _maxSpeed);
			return newVelocity;
		}

		/**
		 * Limits a vector's magnitude by a maximum size
		 * @param Vector3 _input the vector to limit
		 * @param float   _max   the maximum length
		 */
		public Vector3 Truncate(Vector3 _input, float _max) {
			return Vector3.Normalize(_input) * _max;
		}

		public void FormationButtonPressed(string _formation) {
			List<GameObject> division;
			division = new List<GameObject>();
			switch (selectedDivision) {
				case ("all") :
				division.AddRange(belligerents[human].infantry);
				division.AddRange(belligerents[human].archers);
				break;

				case ("infantry") :
				division = belligerents[human].infantry;
				break;

				case ("archers") :
				division = belligerents[human].archers;
				break;
			}
			Debug.Log(division.Count);

			SetFormation(human, division, _formation);
		}

		public void DivisionButtonPressed(string _division) {
			selectedDivision = _division;
			Debug.Log("Selected division changed to: " + selectedDivision);
			//clear outline
			mainCamera.GetComponent<OutlineEffect>().outlineRenderers.Clear();
			switch (_division) {
				case "all" :
				for (int i = 0; i < belligerents[human].infantry.Count; i++) {
					mainCamera.GetComponent<OutlineEffect>().outlineRenderers.Add(belligerents[human].infantry[i].GetComponent<SpriteRenderer>());
				}
				for (int i = 0; i < belligerents[human].archers.Count; i++) {
					mainCamera.GetComponent<OutlineEffect>().outlineRenderers.Add(belligerents[human].archers[i].GetComponent<SpriteRenderer>());
				}
				break;
				
				case "infantry" :
				for (int i = 0; i < belligerents[human].infantry.Count; i++) {
					mainCamera.GetComponent<OutlineEffect>().outlineRenderers.Add(belligerents[human].infantry[i].GetComponent<SpriteRenderer>());
				}
				break;

				case "archers" :
				for (int i = 0; i < belligerents[human].archers.Count; i++) {
					mainCamera.GetComponent<OutlineEffect>().outlineRenderers.Add(belligerents[human].archers[i].GetComponent<SpriteRenderer>());
				}
				break;
			}
		}

		public void SetFormation(int _belligerent, List<GameObject> _division, string _formation) {
			int soldierCount = _division.Count;
			float spacing = 0.25f;
			int i;
			Vector2 position = new Vector2();
			switch (_formation) {
				case ("line") :
				for (i = 0; i < soldierCount; i++) {
					position.x = i * spacing;
					position.y = 0;
					_division[i].GetComponent<UnitSprite>().targetLocation = position;
				}
				break;

				case ("column") :
				for (i = 0; i < soldierCount; i++) {
					position.x = 0;
					position.y = i * spacing;
					_division[i].GetComponent<UnitSprite>().targetLocation = position;
				}
				break;

				case ("square") :
				int sqrt = (int)Mathf.Round(Mathf.Sqrt(soldierCount));
				for (i = 0; i < soldierCount; i++) {
					int row = (int)Mathf.Floor(i / sqrt);
					position.x = (i * spacing - row) - row * spacing;
					position.y = row * spacing;
					_division[i].GetComponent<UnitSprite>().targetLocation = position;
				}
				break;
			}
			Debug.Log("Selected formation changed to: " + _formation + "for: " + selectedDivision);
		}
	}
}