using UnityEngine;
using Armies;
using System.Collections;
using System.Collections.Generic;

namespace Battle {
	public class Belligerent {
		public int id;
		public string name;
		public List<Soldier> soldiers;
		public List<string> divisions;
		public Dictionary<string, List<GameObject>> troops;

		public Belligerent(int _id, string _name) {
			id = _id;
			name = _name;

			soldiers = new List<Soldier>();
			divisions = new List<string>();
			divisions.Add("infantry");
			divisions.Add("archers");
			troops = new Dictionary<string, List<GameObject>>();
			troops.Add("infantry", new List<GameObject>());
			troops.Add("archers", new List<GameObject>());
		}
	}
}