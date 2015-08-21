using UnityEngine;
using System.Collections;

namespace Armies {
	public class Army {
		public Soldier[] soldiers;

		public Army() {
			soldiers = new Soldier[10];
		}
	}
}