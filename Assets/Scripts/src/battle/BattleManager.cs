using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using Battle;
using Armies;
using Gear;

	public class BattleManager : MonoBehaviour {
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

		void Start() {
			map = gameObject.GetComponent<MapManager>();
			string[] characters = new string[2];
			characters[0] = "Dave";
			characters[1] = "Pete";
			InitArmies(characters);
			DeployArmies();
			rallyPoint = new GameObject();
			rallyPoint.AddComponent<SpriteRenderer>();
			rallyPoint.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("graphics/misc/rallypoint");
			rallyPoint.GetComponent<SpriteRenderer>().sortingLayerName = "Units";
		}

		void Update() {
			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (Input.GetMouseButtonDown(0)) {
				belligerents[human].troops[selectedDivision][0].GetComponent<Unit>().newTarget = pos;
			}
			UpdateVision();
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
			WeaponItem weapon = DataStore.Instance.GetWeaponItem(soldier.GetWeaponItem("1"));
			//add soldier
			GameObject soldierSprite = new GameObject();
			//add sprite renderer component to new sprite and load appropriate sprite asset into it
			soldierSprite.AddComponent<SpriteRenderer>();
			soldierSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(graphicsPath + soldier.GetSprite());
			soldierSprite.GetComponent<SpriteRenderer>().sortingLayerName = _sortingLayer;
			//add component to track id and army id
			soldierSprite.AddComponent<Unit>();
			soldierSprite.GetComponent<Unit>().armyId = _armyId;
			soldierSprite.GetComponent<Unit>().id = _soldierId;
			if (weapon.GetRange() == 0) {
				belligerents[_armyId].troops["infantry"].Add(soldierSprite);
			} else {
				belligerents[_armyId].troops["archers"].Add(soldierSprite);
			}
			if (_armyId == human) {
				Camera.main.GetComponent<OutlineEffect>().outlineRenderers.Add(soldierSprite.GetComponent<SpriteRenderer>());
			}
		}

		/**
		 * Creates the soldiers for the battle and adds them to the right army
		 * Also initialises the sprites on screen
		 * @param int _character
		 * @param Dictionary<string, int>       _data         [description]
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
			int i;
			int j;
			//set a random starting tile for each army
			for (i = 0; i < belligerents.Count; i++) {
				deploymentTile.x = (int)Mathf.Floor(Random.value * mapSize.x);
				deploymentTile.y = (int)Mathf.Floor(Random.value * mapSize.y);
				Vector3 pos = map.TileToWorld(deploymentTile);
				//set each soldier's position to the target tile
				for (j = 0; j < belligerents[i].troops["infantry"].Count; j++) {
					DeploySoldier(belligerents[i].troops["infantry"][j], new Vector3(pos.x + Random.value - 0.5f, pos.y + Random.value - 0.5f));
				}
				for (j = 0; j < belligerents[i].troops["archers"].Count; j++) {
					DeploySoldier(belligerents[i].troops["archers"][j], new Vector3(pos.x + Random.value - 0.5f, pos.y + Random.value - 0.5f));
				}
			}
		}

		private void DeploySoldier(GameObject _soldier, Vector3 _pos) {
			_soldier.transform.position = _pos;
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

		private void UpdateVision() {
			map.ClearVision();
			List<GameObject> division = belligerents[human].troops["infantry"];
			for (int i = 0; i < division.Count; i++) {
				map.GetBasicVision(map.WorldToNode(division[i].transform.position), 10);
			}
		}

		private void MoveDivision(Vector2 target) {
			List<GameObject> division = belligerents[human].troops[selectedDivision];
			//set the division's target
			belligerents[human].targets[selectedDivision] = target;
			int i;
			string formation = belligerents[human].formations[selectedDivision];
			switch(formation) {
				case ("column") :
				for (i = 0; i < division.Count; i++) {
					belligerents[human].troops[selectedDivision][i].GetComponent<Unit>().newTarget = new Vector2(0 + target.x, i * map.tileSize + target.y);
				}
				break;

				case ("line") :
				for (i = 0; i < division.Count; i++) {
					belligerents[human].troops[selectedDivision][i].GetComponent<Unit>().newTarget = new Vector2(i * map.tileSize + target.x, 0 + target.y);
				}
				break;

				case ("square") :
				int sqrt = (int)Mathf.Round(Mathf.Sqrt(division.Count));
				Debug.Log(sqrt);
				
				for (i = 0; i < division.Count; i++) {
					int row = (int)Mathf.Floor(i / sqrt);
					Vector2 position = Vector2.zero;
					position.x = i * map.tileSize + target.x - (row * sqrt * map.tileSize);
					position.y = row * map.tileSize + target.y;
					belligerents[human].troops[selectedDivision][i].GetComponent<Unit>().newTarget = position;
				}
				break;
			}
		}

		/**
		 * Called when the user selects a formation from the UI
		 * @param string _formation the selected formation
		 */
		public void FormationButtonPressed(string formation) {
			belligerents[human].formations[selectedDivision] = formation;
			Debug.Log(belligerents[human].formations[selectedDivision]);
			
		}

		/**
		 * Called when the user selects a division from the UI
		 * @param string _division the selected division
		 */
		public void DivisionButtonPressed(string _division) {
			selectedDivision = _division;
			Debug.Log("Selected division changed to: " + selectedDivision);
		}

		/**
		 * Applies the outline to the currently selected division
		 * @param List<GameObject> _division the selected division
		 */
		private void HighlightDivision(List<GameObject> _division) {
			//clear outline
			Camera.main.GetComponent<OutlineEffect>().outlineRenderers.Clear();
			for (int i = 0; i < _division.Count; i++) {
				Camera.main.GetComponent<OutlineEffect>().outlineRenderers.Add(_division[i].GetComponent<SpriteRenderer>());
			}
		}

		/**
		 * Sets the target positions for the selected division
		 * @param int              _belligerent the army
		 * @param List<GameObject> _division    the division
		 * @param string           _formation   the selected formation
		 */
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
					_division[i].GetComponent<Unit>().targetLocation = position;
				}
				break;

				case ("column") :
				for (i = 0; i < soldierCount; i++) {
					position.x = 0;
					position.y = i * spacing;
					_division[i].GetComponent<Unit>().targetLocation = position;
				}
				break;

				case ("square") :
				int sqrt = (int)Mathf.Round(Mathf.Sqrt(soldierCount));
				for (i = 0; i < soldierCount; i++) {
					int row = (int)Mathf.Floor(i / sqrt);
					position.x = (i * spacing - row);
					position.y = row * spacing;
					_division[i].GetComponent<Unit>().targetLocation = position;
				}
				break;
			}
			Debug.Log("Selected formation changed to: " + _formation + "for: " + selectedDivision);
		}
	}