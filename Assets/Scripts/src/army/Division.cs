using UnityEngine;
using System.Collections;

namespace Armies {
	public class Division {
		private string name;
		public Soldier[] soldiers;

		/**
		 * gets the name of the soldier
		 * @return {string}
		 */
		public string getName() {
			return name;
		}

		/**
		 * sets the name of the soldier
		 * @param {string}
		 */
		public void setName(string _name) {
			name = _name;
		}
	}
}