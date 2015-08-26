using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gear {
	/**
	 * This class loads all the different types of armour from an external JSON file
	 */
	
	public class ArmourList {
		//the data that will be loaded
		TextAsset data;
		//the json that will store the loaded data
		JSONObject json;
		//dictionary to store values in easily accessible form
		private Dictionary<string, ArmourItem> armour;

		public ArmourList() {
			//load the data
			data = Resources.Load("common/armour_items") as TextAsset;
			//init json object
			json = new JSONObject(data.text);
			//init dictionary
			armour = new Dictionary<string , ArmourItem>();
			//parse the data
			ParseData(json, armour);
		}


		/**
		 * Returns an item from the dictionary, assumes entry exists for speed
		 * @param  {string} key		the key to select
		 * @return {ArmourItem}		the item
		 */
		public ArmourItem GetItem(string _key) {
			return armour[_key];
		}

		/**
		 * Parses the JSON object and plugs the data into the dictionary
		 * @param  	{JSON Object} 	obj 		the object to parse
		 * @param	{Dictionary}	Dictionary 	the dictionary to place the parsed data
		 */
		void ParseData(JSONObject obj, Dictionary<string, ArmourItem> dictionary) {
			//vars to add to each item
			string name;
			int weight;
			string location;
			int damageReduction;
			//temporary JSONObject
			JSONObject item;
			//for each item in the whole list
			for (int i = 0; i < obj.list.Count; i++) {
				//store the item temporarily
				item = obj[i];
				//set default values
				name = "NULL";
				weight = 0;
				location = "NULL";
				damageReduction = 0;
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

						case "location" :
						location = obj[i].list[j].str;
						break;

						case "damage_reduction" :
						damageReduction = (int)obj[i].list[j].n;
						break;
						
					}
				}

				//add the item to the dictionary
				dictionary.Add(name, new ArmourItem(name, weight, location, damageReduction));
			}
		}
	}
}