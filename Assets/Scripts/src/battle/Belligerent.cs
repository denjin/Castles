using UnityEngine;
using Armies;
using System.Collections;
using System.Collections.Generic;

namespace Battle {
	public class Belligerent {
		public int id;
		public string name;
		public List<Soldier> soldiers;
		public List<int> infantry;
		public List<int> archers;
		public List<int> cavalry;

		public Belligerent(int _id, string _name) {
			id = _id;
			name = _name;

			soldiers = new List<Soldier>();
			infantry = new List<int>();
			archers = new List<int>();
			cavalry = new List<int>();
		}
	}
}