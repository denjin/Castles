using UnityEngine;
using System.Collections;
using Armies;

namespace Battle {
	public class ArmyManager {
		//the armies
		public Army[] armies;

		public ArmyManager(int _computerArmies) {
			armies = new Army[_computerArmies];
			armies[0] = new Army(true);
			for (int i = 1; i < _computerArmies; i++) {
				armies[i] = new Army();
			}
		}

		public Soldier[] GetSoldiers(int _army) {
			return armies[_army].soldiers;
		}
	}
}