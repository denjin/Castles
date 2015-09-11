using UnityEngine;
using Armies;
using System.Collections;
using System.Collections.Generic;

namespace Battle {
	public class Belligerent {
		public int id;
		public string name;
		public List<Soldier> soldiers;
		public List<GameObject> infantry;
		public List<GameObject> archers;
		public List<GameObject> cavalry;

		public Belligerent(int _id, string _name) {
			id = _id;
			name = _name;

			soldiers = new List<Soldier>();
			infantry = new List<GameObject>();
			archers = new List<GameObject>();
			cavalry = new List<GameObject>();
		}
	}
}