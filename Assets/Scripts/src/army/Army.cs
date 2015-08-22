using UnityEngine;
using System.Collections;

namespace Armies {
	public class Army {
		public bool human;
		public Soldier[] soldiers;

		public bool deployed = false;

		public Army(bool _human = false) {
			human = _human;
			soldiers = new Soldier[10];
			for (int i = 0; i < 10; i++) {
				soldiers[i] = DataStore.Instance.GetSoldier("Peasant");
				soldiers[i].SetID(i);
			}
		}
	}
}