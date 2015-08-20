using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Gear;

namespace Armies {
	public class Soldier {
		//the soldiers unique id
		protected int id;
		//name of the soldier
		protected string name;
		//hit points
		protected int hp;
		//morale points
		protected int mp;
		//armour
		private Dictionary<string, string> armour;
		//weapons
		private Dictionary<string, string> weapons;

		public Soldier () {}

		public Soldier(string _name, int _hp, int _mp, Dictionary<string, string> _armour, Dictionary<string, string> _weapons) {
			name = _name;
			hp = _hp;
			mp = _mp;

			armour = _armour;
			weapons = _weapons;
		}
		

		//--------------------
		//getters / setters
		//--------------------

		public int getID() {
			return id;
		}

		public void setID(int _id) {
			id = _id;
		}

		public string getName() {
			return name;
		}

		public void setName(string _name) {
			name = _name;
		}

		public int getHP() {
			return hp;
		}

		public void setHP(int _hp) {
			hp = _hp;
		}

		public int getMP() {
			return mp;
		}

		public void setMP(int _mp) {
			mp = _mp;
		}

		public string getArmourItem(string key) {
			if (armour.ContainsKey(key)) {
				return armour[key];
			}
			return null;
		}

		public string getWeaponItem(string key) {
			if (weapons.ContainsKey(key)) {
				return weapons[key];
			}
			return null;
		}
	}
}

