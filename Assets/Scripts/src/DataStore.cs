using UnityEngine;
using Armies;
using Gear;

public class DataStore : MonoBehaviour {
	public ArmourList armourItems;
	public WeaponList weaponItems;
	public SoldierList soldiers;

	private static DataStore instance;
	public static DataStore Instance {
		get { 
			return instance ?? (instance = new GameObject("DataStore").AddComponent<DataStore>());
		}
	}

	void Awake() {
		//load the data
		armourItems = new ArmourList();
		weaponItems = new WeaponList();
		soldiers = new SoldierList();
	}

	public ArmourItem GetArmourItem(string _key) {
		return armourItems.GetItem(_key);
	}

	public WeaponItem GetWeaponItem(string _key) {
		return weaponItems.GetItem(_key);
	}

	public Soldier GetSoldier(string _key) {
		return soldiers.GetSoldier(_key);
	}
}