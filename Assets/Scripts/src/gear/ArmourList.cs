using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gear {
	/**
	 * This class contains all the different types of armour
	 */
	
	public class ArmourList {
		TextAsset data;

		JSONObject json;

		//dictionary to store values in easily accessible form
		private Dictionary<string, ArmourItem> armour;

		public ArmourList() {
			//load the data
			data = Resources.Load("armour_items") as TextAsset;
			//create json object
			json = new JSONObject(data.text);
			//init dictionary
			armour = new Dictionary<string , ArmourItem>();
			//parse the data
			parseData(json, armour);
		}


		/**
		 * Returns an item from the dictionary, assumes entry exists for speed
		 * @param  {string} key		the key to select
		 * @return {ArmourItem}		the item
		 */
		public ArmourItem getItem(string key) {
			return armour[key];
		}

		/**
		 * Parses the JSON object and plugs the data into the dictionary
		 * @param  	{JSON Object} 	obj 		the object to parse
		 * @param	{Dictionary}	Dictionary 	the dictionary to place the parsed data
		 */
		void parseData(JSONObject obj, Dictionary dictionary) {
			string name;
			int weight;
			string location;
			int damageReduction;
			for (int i = 0; i < obj.list.Count; i++) {
				name = obj.list[i][0].str;
				weight = (int)obj.list[i][1].n;
				location = obj.list[i][2].str;
				damageReduction = (int)obj.list[i][3].n;
				dictionary.Add(name, new ArmourItem(name, weight, location, damageReduction));
			}
		}
	}
}