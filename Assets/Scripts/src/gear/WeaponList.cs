using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class WeaponList {
	public static Dictionary<string, WeaponItem> GetWeaponList (string source) {
		//load the data
		TextAsset data = Resources.Load(source) as TextAsset;
		//init json object
		JSONObject obj = new JSONObject(data.text);
		Dictionary<string, WeaponItem> dictionary = new Dictionary<string , WeaponItem>();
		//vars to add to each item
		string name;
		int weight;
		string type;
		int shortRange;
		int longRange;
		int shortModifier;
		int longModifier;
		int damage;
		int strength;
		int reliability;
		//temporary JSONObject
		JSONObject item;
		//for each item in the whole list
		for (int i = 0; i < obj.list.Count; i++) {
			//store the item temporarily
			item = obj[i];
			//set default values
			name = "NULL";
			weight = 0;
			type = "NULL";
			shortRange = 0;
			longRange = 0;
			shortModifier = 0;
			longModifier = 0;
			damage = 0;
			strength = 0;
			reliability = 0;
			//for each field in the item
			for (int j = 0; j < item.list.Count; j++) {
				//get their stats, only if they correspond to the correct fields
				switch (item.keys[j].ToLower()) {
					case "name" :
					name = obj[i].list[j].str;
					break;

					case "weight" :
					weight = (int)obj[i].list[j].n;
					break;

					case "type" :
					type = obj[i].list[j].str;
					break;

					case "shortrange" :
					shortRange = (int)obj[i].list[j].n;
					break;

					case "longrange" :
					longRange = (int)obj[i].list[j].n;
					break;

					case "shortmodifier" :
					shortModifier = (int)obj[i].list[j].n;
					break;

					case "longmodifier" :
					longModifier = (int)obj[i].list[j].n;
					break;

					case "damage" :
					damage = (int)obj[i].list[j].n;
					break;

					case "strength" :
					strength = (int)obj[i].list[j].n;
					break;

					case "reliability" :
					reliability = (int)obj[i].list[j].n;
					break;
					
				}
			}

			//add the item to the dictionary
			dictionary.Add(name, new WeaponItem(name, weight, type, shortRange, longRange, shortModifier, longModifier, damage, strength, reliability));
		}
		return dictionary;
	}
}