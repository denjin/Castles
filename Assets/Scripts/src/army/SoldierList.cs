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
			data = Resources.Load("soldiers") as TextAsset;
			//init json object
			json = new JSONObject(data.text);
			//init dictionary
			soldiers = new Dictionary<string , Soldier>();
			//parse the data
			parseData(json, soldiers);
		}

		public Soldier GetSoldier(string key) {
			return soldiers[key];
		}

		/**
		 * Parses the JSON object and plugs the data into the dictionary
		 * @param  	{JSON Object} 	obj 		the object to parse
		 * @param	{Dictionary}	Dictionary 	the dictionary to place the parsed data
		 */
		public void parseData(JSONObject obj, Dictionary<string, Soldier> dictionary) {
			//vars to add to each item
			string name;
			int hp;
			int mp;
			//references to the soldier's gear
			Dictionary<string, string> armour = new Dictionary<string, string>();
			Dictionary<string, string> weapons = new Dictionary<string, string>();
			//temporary JSONObject
			JSONObject item;

			//for each item in the whole list
			for (int i = 0; i < obj.list.Count; i++) {
				//store the item temporarily
				item = obj[i];
				name = "NULL";
				hp = 0;
				mp = 0;
				armour.Clear();
				weapons.Clear();

				//for each field in the item
				for (int j = 0; j < item.list.Count; j++) {
					switch (item.keys[j]) {
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
							switch (obj[i][j].keys[k]) {
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
							switch (obj[i][j].keys[k]) {
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
					}
					
				}

				dictionary.Add(name, new Soldier(name, hp, mp, armour, weapons));
				
			}

		}
	}
}