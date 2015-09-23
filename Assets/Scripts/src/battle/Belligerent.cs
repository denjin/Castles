using UnityEngine;
using Armies;
using System.Collections;
using System.Collections.Generic;

namespace Battle {
	public class Belligerent {
		public int id;
		public string name;
		//a list of the divisions owned by this belligerent
		public List<string> divisions;
		//the troops that belong to each division
		public Dictionary<string, List<GameObject>> troops;
		//saves the current formation for the divisions
		public Dictionary<string, string> formations;
		//saves the current target location for the divisions
		public Dictionary<string, Vector2> targets;

		public Belligerent(int _id, string _name) {
			id = _id;
			name = _name;
			divisions = new List<string>();
			divisions.Add("infantry");
			divisions.Add("archers");
			troops = new Dictionary<string, List<GameObject>>();
			troops.Add("infantry", new List<GameObject>());
			troops.Add("archers", new List<GameObject>());
		}
	}
}