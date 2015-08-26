using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Gear;

namespace Armies {
	public class Character : Soldier {
		private int diplomacy;
		private int martial;
		private int intrigue;
		public Dictionary<string, int> soldiers;

		public Character () {}

		public Character (
			string _name,
			int _diplomacy,
			int _martial,
			int _intrigue,
			int _hp,
			int _mp,
			Dictionary<string, string> _armour,
			Dictionary<string, string> _weapons,
			Dictionary<string, int> _soldiers,
			string _sprite) : base (_name, _hp, _mp, _armour, _weapons, _sprite) {
			
			diplomacy = _diplomacy;
			martial = _martial;
			intrigue = _intrigue;
			soldiers = _soldiers;
		}

		public int GetDiplomacy() {
			return diplomacy;
		}

		public void SetDiplomacy(int _value) {
			diplomacy = _value;
		}

		public int GetMartial() {
			return martial;
		}

		public void SetMartial(int _value) {
			martial = _value;
		}

		public int GetIntrigue() {
			return intrigue;
		}

		public void SetIntrigue(int _value) {
			intrigue = _value;
		}

		public int SoldierCount() {
			return soldiers["peasant"];
		}

		public Dictionary<string, int> GetSoldiers() {
			return soldiers;
		}

		public void SetSoldier(int _item, string _key) {
			if (soldiers.ContainsKey(_key)) {
				soldiers[_key] = _item;
			}
		}

		public int GetSoldier(string _key) {
			if (soldiers.ContainsKey(_key)) {
				return soldiers[_key];
			}
			return 0;
		}
	}
}