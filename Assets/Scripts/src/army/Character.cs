using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Gear;

namespace Armies {
	public class Character : Soldier {
		private int diplomacy;
		private int martial;
		private int intrigue;
		private Dictionary<string, string> soldiers;

		public Character () {}

		public Character (string _name, int _diplomacy, int _martial, int _intrigue, int _hp, int _mp, Dictionary<string, string> _armour, Dictionary<string, string> _weapons, Dictionary<string, string> _soldiers, string _sprite) : base (_name, _hp, _mp, _armour, _weapons, _sprite) {
			diplomacy = _diplomacy;
			martial = _martial;
			intrigue = _intrigue;
			soldiers = _soldiers;
		}
	}
}