using UnityEngine;
using System.Collections;
using Armies;

namespace Battle {
	public class ArmyManager {
		//the human players army
		public Army PlayerArmy;
		//the ai controlled armies
		public Army[] ComputerArmies;

		public ArmyManager(int _computerArmies) {
			PlayerArmy = new Army();
			ComputerArmies = new Army[_computerArmies];
			for (int i = 0; i < _computerArmies; i++) {
				ComputerArmies[i] = new Army();
			}
		}
	}
}