using UnityEngine;
using System.Collections;

namespace Armies {
	public class Army {
		public bool human;

		public GameObject[] soldiers;

		public bool deployed = false;

		public Army(bool _human = false) {
			human = _human;
			soldiers = new GameObject[10];

			GameObject gameObject;

			for (int i = 0; i < 10; i++) {
				gameObject = GameObject.Instantiate(Resources.Load("Prefabs/SoldierGameObject")) as GameObject;
				soldiers[i] = gameObject;

				//soldiers[i] = DataStore.Instance.GetSoldier("Peasant");
				//soldiers[i].SetID(i);
			}
		}
	}
}