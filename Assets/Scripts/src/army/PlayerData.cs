using UnityEngine;
using System.Collections.Generic;

public static class PlayerData {
	public static Player Load(string targetData) {
		//load the data
		TextAsset data = Resources.Load(targetData) as TextAsset;
		//init json object
		JSONObject obj = new JSONObject(data.text);
		//init data
		string playerName = "";
		GameObject[] soldiers = new GameObject[0];
		
		for (int i = 0; i < obj.list.Count; i++) {
			switch(obj.keys[i].ToLower()) {
				case "name" : 
				playerName = obj.list[i].str;
				break;
				
				case "soldiers" :
				soldiers = new GameObject[obj.list[i].Count];
				//each soldier
				for (int j = 0; j < obj.list[i].Count; j++) {
					JSONObject s = obj.list[i][j];
					//init game object
					GameObject soldier = new GameObject();
					//add graphics
					soldier.AddComponent<SpriteRenderer>();
					soldier.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("graphics/units/soldier");
					//assign graphics to correct layer
					soldier.GetComponent<SpriteRenderer>().sortingLayerName = "Units";
					//add data component
					soldier.AddComponent<Unit>();
					Unit unit = soldier.GetComponent<Unit>();
					//each property
					for (int k = 0; k < s.list.Count; k++) {
						unit.myId = j;
						switch (s.keys[k]) {
							case ("name") :
							soldier.name = s.list[k].str;
							unit.myName = s.list[k].str;
							break;

							case ("portrait") :
							unit.portrait = s.list[k].str;
							break;

							case ("weaponSkill") :
							unit.weaponSkill = (int)s.list[k].n;
							break;

							case ("ballisticSkill") :
							unit.ballisticSkill = (int)s.list[k].n;
							break;

							case ("strength") :
							unit.strength = (int)s.list[k].n;
							break;

							case ("toughness") :
							unit.toughness = (int)s.list[k].n;
							break;

							case ("initiative") :
							unit.initiative = (int)s.list[k].n;
							break;

							case ("experience") :
							unit.experience = (int)s.list[k].n;
							break;

							case ("health") :
							unit.health = (int)s.list[k].n;
							break;

							case ("morale") :
							unit.morale = (int)s.list[k].n;
							break;

							case ("weapon1") :
							unit.weapon1 = s.list[k].str;
							break;

							case ("weapon2") :
							unit.weapon2 = s.list[k].str;
							break;
						}
					}
					soldier.AddComponent<BoxCollider2D>();

					GameObject glow = new GameObject();
					glow.name = "glow";
					glow.AddComponent<SpriteRenderer>();
					glow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("graphics/units/glow");
					glow.GetComponent<SpriteRenderer>().sortingLayerName = "Outline";
					glow.transform.SetParent(soldier.transform);
					glow.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0f);

					soldiers[j] = soldier;
				}
				break;
			}
		}
		Player player = new Player(playerName, soldiers);
		return player;
	}
}