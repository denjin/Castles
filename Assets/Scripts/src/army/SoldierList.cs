using System.Collections;
using System.Collections.Generic;

namespace Armies {
	/**
	 * This class contains all the different types of soldier
	 */
	public class SoldierList {
		private Dictionary<string, Soldier> soldiers;

		public SoldierList() {
			soldiers = new Dictionary<string, Soldier>();

			Soldier peasant = new Soldier();
		}

		/**
		 * Returns an item from the dictionary, assumes entry exists for speed
		 * @param  {string} key		the key to select
		 * @return {ArmourItem}		the item
		 */
		public Soldier getItem(string key) {
			return soldiers[key];
		}
	}
}