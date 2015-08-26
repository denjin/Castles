using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Armies {
	/**
	 * This class contains all the different types of soldier
	 */
	public class SoldierList {
		//the data that will be loaded
		TextAsset data;
		//the json that will store the loaded data
		JSONObject json;
		//dictionary to store values in easily accessible form
		private Dictionary<string, Soldier> soldiers;

		public SoldierList() {
			//load the data
			data = Resources.Load("common/soldiers") as TextAsset;
			//init json object
			json = new JSONObject(data.text);
			//init dictionary
			soldiers = new Dictionary<string , Soldier>();
			//parse the data
			parseData(json);
		}

		public Soldier GetSoldier(string key) {
			return soldiers[key];
		}

		/**
		 * Parses the JSON object and plugs the data into the dictionary
		 * @param  	{JSON Object} 	obj 		the object to parse
		 */
		public void parseData(JSONObject obj) {
			//temporary JSONObject
			JSONObject item;
			//for each item in the whole list
			for (int i = 0; i < obj.list.Count; i++) {
				//store the item temporarily
				item = obj[i];
				string name = "";
				int hp = 0;
				int mp = 0;
				Dictionary<string, string> armour = new Dictionary<string, string>();
				Dictionary<string, string> weapons = new Dictionary<string, string>();
				string sprite = "";

				//for each field in the item
				for (int j = 0; j < item.list.Count; j++) {
					switch (item.keys[j].ToLower()) {
						case "name" :
						name = obj[i].list[j].str;
						break;

						case "hp" :
						hp = (int)obj[i].list[j].n;
						break;

						case "mp" :
						mp = (int)obj[i].list[j].n;
						break;

						case "armour" :
						for (int k = 0; k < obj[i][j].Count; k++) {
							switch (obj[i][j].keys[k].ToLower()) {
								case "head" :
								armour.Add("head", obj[i][j].list[k].str);
								break;

								case "torso" :
								armour.Add("torso", obj[i][j].list[k].str);
								break;

								case "legs" :
								armour.Add("legs", obj[i][j].list[k].str);
								break;

								case "hands" :
								armour.Add("hands", obj[i][j].list[k].str);
								break;

								case "feet" :
								armour.Add("feet", obj[i][j].list[k].str);
								break;
							}
						}
						break;

						case "weapons" :
						for (int k = 0; k < obj[i][j].Count; k++) {
							switch (obj[i][j].keys[k].ToLower()) {
								case "1" :
								weapons.Add("1", obj[i][j].list[k].str);
								break;

								case "2" :
								weapons.Add("2", obj[i][j].list[k].str);
								break;

								case "3" :
								weapons.Add("3", obj[i][j].list[k].str);
								break;

								case "4" :
								weapons.Add("4", obj[i][j].list[k].str);
								break;
							}
						}
						break;

						case "sprite" :
						sprite = obj[i].list[j].str;
						break;
					}
					
				}
				soldiers.Add(name, new Soldier(name, hp, mp, armour, weapons, sprite));
			}
		}
	}
}