using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Gear;

namespace Armies {
	public class Soldier {
		//the soldiers unique id
		protected int i;
		//name of the soldier
		protected string name;
		//hit points
		protected int hp;
		//morale points
		protected int mp;
		//armour
		protected Dictionary<string, string> armour;
		//weapons
		protected Dictionary<string, string> weapons;
		//graphics asset
		protected string sprite;
		//position on the map
		protected float posX;
		protected float posY;

		public Soldier () {}

		public Soldier(string _name, int _hp, int _mp, Dictionary<string, string> _armour, Dictionary<string, string> _weapons, string _sprite) {
			name = _name;
			hp = _hp;
			mp = _mp;
			armour = _armour;
			weapons = _weapons;
			sprite = _sprite;
		}
		

		//--------------------
		//getters / setters
		//--------------------
		
		public int GetI() {
			return i;
		}

		public void SetI(int _i) {
			i = _i;
		}

		public float GetPosX() {
			return posX;
		}

		public float GetPosY() {
			return posY;
		}

		public void SetPos(float _x = -1f, float _y = -1f) {
			if (_x != -1f) {
				posX = _x;
			}
			if (_y != -1f) {
				posY = _y;
			}
		}

		public string GetName() {
			return name;
		}

		public void GetName(string _name) {
			name = _name;
		}

		public int GetHP() {
			return hp;
		}

		public void GetHP(int _hp) {
			hp = _hp;
		}

		public int GetMP() {
			return mp;
		}

		public void GetMP(int _mp) {
			mp = _mp;
		}

		public string GetArmourItem(string key) {
			if (armour.ContainsKey(key)) {
				return armour[key];
			}
			return null;
		}

		public void SetArmourItem(string item, string key) {
			if (armour.ContainsKey(key)) {
				armour[key] = item;
			}
		}

		public string GetWeaponItem(string key) {
			if (weapons.ContainsKey(key)) {
				return weapons[key];
			}
			return null;
		}

		public void SetWeaponItem(string item, string key) {
			if (weapons.ContainsKey(key)) {
				weapons[key] = item;
			}
		}

		public string GetSprite() {
			return sprite;
		}

		public void SetSprite(string _sprite) {
			sprite = _sprite;
		}
	}
}

