using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gear {
	/**
	 * This class loads all the different types of weapon from an external JSON file
	 */
	
	public class WeaponList {
		//the data that will be loaded
		TextAsset data;
		//the json that will store the loaded data
		JSONObject json;
		//dictionary to store values in easily accessible form
		private Dictionary<string, WeaponItem> weapons;

		public WeaponList() {
			//load the data
			data = Resources.Load("weapon_items") as TextAsset;
			//init json object
			json = new JSONObject(data.text);
			//init dictionary
			weapons = new Dictionary<string , WeaponItem>();
			//parse the data
			ParseData(json, weapons);
		}


		/**
		 * Returns an item from the dictionary, assumes entry exists for speed
		 * @param  {string} key		the key to select
		 * @return {WeaponItem}		the item
		 */
		public WeaponItem GetItem(string _key) {
			return weapons[_key];
		}

		/**
		 * Parses the JSON object and plugs the data into the dictionary
		 * @param  	{JSON Object} 	obj 		the object to parse
		 * @param	{Dictionary}	Dictionary 	the dictionary to place the parsed data
		 */
		void ParseData(JSONObject obj, Dictionary<string, WeaponItem> dictionary) {
			//vars to add to each item
			string name;
			int weight;
			string type;
			int requiredHands;
			int range;
			int damage;
			string damageType;
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
				requiredHands = 0;
				range = 0;
				damage = 0;
				damageType = "NULL";
				//for each field in the item
				for (int j = 0; j < item.list.Count; j++) {
					//get their stats, only if they correspond to the correct fields
					switch (item.keys[j]) {
						case "name" :
						name = obj[i].list[j].str;
						break;

						case "weight" :
						weight = (int)obj[i].list[j].n;
						break;

						case "type" :
						type = obj[i].list[j].str;
						break;

						case "required_hands" :
						requiredHands = (int)obj[i].list[j].n;
						break;

						case "range" :
						range = (int)obj[i].list[j].n;
						break;

						case "damage" :
						damage = (int)obj[i].list[j].n;
						break;

						case "damage_type" :
						damageType = obj[i].list[j].str;
						break;
						
					}
				}

				//add the item to the dictionary
				dictionary.Add(name, new WeaponItem(name, weight, type, requiredHands, range, damage, damageType));
			}
		}
	}
}