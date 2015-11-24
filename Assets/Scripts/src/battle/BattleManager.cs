using UnityEngine;
using UnityEngine.UI;
//using System;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BattleManager : MonoBehaviour {
	//the map
	private MapManager map;
	//the ui
	public UIManager ui;
	//the human player
	public Player player;
	//the ai controlled player
	public AIPlayer ai;
	//array to store which soldiers are currently selected
	public int selectedSoldier = 0;
	private string selectedOrder = "move";
	//a list storing the various different types of weapons
	Dictionary<string, WeaponItem> weapons;
	public Player enemyTypes;


	Node targetNode;
	Node oldTargetNode;

	//GameObject canvas;

	public bool humanTurn = true;
	public bool resolving = false;

	public delegate void StopMovement();
	public static event StopMovement OnStopMovement;
	
	void Awake() {
		Global.Instance.Init();
		player = PlayerData.Load("common/characters");
		enemyTypes = PlayerData.Load("common/enemy_types");
		ai = new AIPlayer(map);
	}

	void Start() {
		map = Global.Instance.mapManager;
		map.Init("maps/test_map");
		ui = GetComponent<UIManager>();
		

		
		
		Unit.OnEnteredNewNode += UpdateVision;
		Unit.OnArrived += UnitArrived;
		InputManager.OnTabPressed += NextUnit;
		InputManager.OnActionStart += StartOrder;
		InputManager.OnActionEnd += EndOrder;
		ui.InitUI();
		
		//canvas = GameObject.Find("ButtonCanvas");
		Global.Instance.SetMapBounds(new Vector2(0 - map.tileSize / 2, 0 - map.tileSize / 2), new Vector2(map.levelWidth * map.tileSize + map.tileSize / 2, map.levelHeight * map.tileSize + map.tileSize / 2));
		weapons = WeaponList.GetWeaponList("common/weapon_items");
		
		DeployArmies();

		UpdateVision();
		Global.Instance.cam.ScrollTo(player.soldierObjects[selectedSoldier].transform.position);
		SoldierSelected(0);
	}	

	public void EndTurn() {
		humanTurn = !humanTurn;
		for (int i = 0; i < player.soldierStats.Length; i++) {
			player.soldierStats[i].currentMovementPoints = player.soldierStats[i].baseMovementPoints;
			player.soldierStats[i].currentActionPoints = player.soldierStats[i].baseActionPoints;
		}
		SoldierSelected(0);
	}

	

	/**
	 * Positions the soldiers in a randomly assigned tile
	 */
	private void DeployArmies() {
		//sets the size of the map
		Int2 mapSize = map.GetSize();
		//set a random starting tile for each army
		Int2 deploymentTile = new Int2((int)Mathf.Floor(Random.value * mapSize.x), (int)Mathf.Floor(Random.value * mapSize.y));
		
		for (int i = 0; i < player.soldierObjects.Length; i++) {
			Node node = GetRandomNode(map.nodes[deploymentTile.x, deploymentTile.y], 4);
			node.occupied = true;
			player.soldierObjects[i].transform.position = node.worldPosition;
		}
	}

	private Node GetRandomNode(Node baseNode, int range) {
		while(true) {
			Node randomNode;
			Vector2 shuffle = new Vector2((Random.value - 0.5f) * (map.tileSize * range), (Random.value - 0.5f) * (map.tileSize * range));
			float x = baseNode.worldPosition.x + shuffle.x;
			float y = baseNode.worldPosition.y + shuffle.y;
			Node targetNode = map.WorldToNode(new Vector2(Mathf.Clamp(x, Global.Instance.mapMinBounds.x, Global.Instance.mapMaxBounds.x), Mathf.Clamp(y, Global.Instance.mapMinBounds.y, Global.Instance.mapMaxBounds.y)));
			if (targetNode.Walkable) {
				randomNode = targetNode;
				return randomNode;
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

	private void UpdateVision() {
		map.ClearVision();
		if (selectedOrder != "shoot") {
			for (int i = 0; i < player.soldierObjects.Length; i++) {
				map.GetBasicVision(map.WorldToNode(player.soldierObjects[i].transform.position), player.soldierStats[i].sightRange);
			}
		} else {
			map.GetBasicVision(map.WorldToNode(player.soldierObjects[selectedSoldier].transform.position), player.soldierStats[selectedSoldier].sightRange);
		}
	
	}

	private void PreviewPath(Vector2 target) {
		if (!resolving) {
			PathRequestManager.Instance.RequestPath(player.soldierObjects[selectedSoldier].transform.position, target, OnPathFound);
		}
	}

	private void OnPathFound(Vector2[] path, bool pathSuccess) {
		int length = Mathf.Min(player.soldierStats[selectedSoldier].currentMovementPoints, path.Length);
		Vector2[] newPath = new Vector2[length + 1];
		newPath[0] = player.soldierObjects[selectedSoldier].transform.position;
		
		for (int i = 0; i < length; i++) {
			newPath[i+1] = path[i];
		}
		map.PreviewPath(newPath);
	}

	private void IssueMoveCommand(Vector2 target) {
		map.ClearPreview();
		if (!player.soldierStats[selectedSoldier].moving) {
			resolving = true;
			player.soldierStats[selectedSoldier].newTarget = target;
		}
	}

	private void IssueShootCommand(Vector2 target) {
		Shoot(player.soldierStats[selectedSoldier], map.WorldToNode(target));
	}

	public void UnitArrived() {
		resolving = false;
		map.GetReachableNodes(map.WorldToNode(player.soldierObjects[selectedSoldier].transform.position), player.soldierStats[selectedSoldier].currentMovementPoints + 1, player.soldierStats[selectedSoldier]);
	}

	private void NextUnit() {
		if(Global.Instance.inputManager.shiftKey) {
			SoldierSelected(selectedSoldier - 1);
		} else {
			SoldierSelected(selectedSoldier + 1);
		}
		
	}
	

	public void SoldierSelected(int selection) {

		player.soldierObjects[selectedSoldier].transform.Find("glow").GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0f);
		if (Global.Instance.inputManager.doubleClick) {
			Global.Instance.cam.ScrollTo(player.soldierObjects[selectedSoldier].transform.position);
		}
		
		selectedSoldier = selection;
		if (selectedSoldier >= player.soldierObjects.Length) {
			selectedSoldier = 0;
		} else if (selectedSoldier < 0) {
			selectedSoldier = player.soldierObjects.Length - 1;
		}

		player.soldierObjects[selectedSoldier].transform.Find("glow").GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 255f);
		
		ui.HighlightSoldier(selectedSoldier);
		map.GetReachableNodes(map.WorldToNode(player.soldierObjects[selectedSoldier].transform.position), player.soldierStats[selectedSoldier].currentMovementPoints + 1, player.soldierStats[selectedSoldier]);
		UpdateVision();
	}

	

	public void OrderSelected(string order) {
		selectedOrder = order;
		map.ClearReachableTiles();

		switch (order) {
			case "move" :
				map.GetReachableNodes(map.WorldToNode(player.soldierObjects[selectedSoldier].transform.position), player.soldierStats[selectedSoldier].currentMovementPoints + 1, player.soldierStats[selectedSoldier]);
			break;
		}
		UpdateVision();
	}

	public GameObject GetSoldierInNode(Node node) {
		Debug.Log(node.x + "-" + node.y);
		//check humans first
		for (int i = 0; i < player.soldierObjects.Length; i++) {
			if (player.soldierStats[i].currentNode == node) {
				return player.soldierObjects[i];
			}
		}
		//then check ai
		for (int j = 0; j < ai.soldierObjects.Length; j++) {
			if (ai.soldierStats[j].currentNode == node) {
				return ai.soldierObjects[j];
			}
		}
		return null;
	}

	public bool SoldierIsHuman(Unit unit) {
		if (System.Array.Exists(player.soldierStats, element => element == unit)) {
			return true;
		}
		return false;
	}

	

	public void StartOrder() {
		//map.ClearPreview();
		Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		targetNode = map.WorldToNode(pos);
		switch (selectedOrder) {
			case ("move") :
				if (targetNode != oldTargetNode) {
					PreviewPath(pos);
					oldTargetNode = targetNode;
				}
				
			break;

			case ("shoot") :
			UpdateVision();
				if (targetNode != oldTargetNode) {
					List<Node> visibleNodes = map.fov.GetVisibleNodes(map.WorldToNode(player.soldierObjects[selectedSoldier].transform.position), player.soldierStats[selectedSoldier].sightRange);
					GameObject targetSoldier = GetSoldierInNode(targetNode);
					if (visibleNodes.Contains(targetNode) && targetSoldier != null && !SoldierIsHuman(targetSoldier.GetComponent<Unit>())) {
						map.PreviewShot(player.soldierObjects[selectedSoldier].transform.position, targetNode.worldPosition);
					} else {
						map.ClearPreview();
					}
					
					oldTargetNode = targetNode;
				}
			break;
		}
	}

	public void EndOrder() {
		map.ClearPreview();
		switch (selectedOrder) {
			case ("move") :
				IssueMoveCommand(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			break;

			case("shoot") :
				IssueShootCommand(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			break;

		}
		selectedOrder = "move";
		UpdateVision();
	}

	public void Shoot(Unit shooter, Node targetNode) {
		//get the node we're shooting at
		Node originNode = map.WorldToNode(shooter.gameObject.transform.position);
		//get the unit sitting in the node
		Unit targetUnit = GetSoldierInNode(targetNode).GetComponent<Unit>();
		//if there is no target, just pretend this never happened
		if (targetUnit == null) {
			return;
		}
		//get the number we require to be rolled
		int required = Shooting.GetRequiredHitRoll(shooter);
		//get ready to record the shot modifiers
		int modifier = 0;
		//get the path of the shot
		List<Node> shotPath = map.GetLine(originNode, targetNode);
		//apply cover modifier
		modifier += Shooting.GetCoverModifier(shotPath);
		//apply modifiers from the unit shooting and their weapon
		modifier += Shooting.GetShooterModifier(shooter, weapons[shooter.weapon1], (int)Mathf.Round(map.Distance(originNode, targetNode)));
		//apply modifiers from the unit being shot at
		modifier += Shooting.GetTargetModifier(targetUnit, weapons[shooter.weapon1]);
		//roll a die
		int roll = Dice.Roll();
		//always miss on a 1 or if the modified roll is lower than the required roll
		if (roll == 1 || roll + modifier < required) {
			Debug.Log("miss");
		} else {
			Debug.Log("hit");
		}

		
	}
}