using UnityEngine;
using System.Collections;

namespace Armies {
	public class Army {
		public bool human;
		public Soldier[] soldiers;

		public Army(bool _human = false) {
			human = _human;
			soldiers = new Soldier[10];
		}
	}
}