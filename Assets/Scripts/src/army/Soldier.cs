using UnityEngine;
using System.Collections;

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
		protected ArmourItem[] armour;
		//weapons
		protected WeaponItem[] weapons;

		public Soldier () {}

		public Soldier(int _id, string _name, int _hp, int _mp, ArmourItem[] _armour, WeaponItem[] _weapons) {
			id = _id;
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
	}
}

