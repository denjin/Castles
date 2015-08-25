using UnityEngine;
using Armies;
using Gear;

public class DataStore : MonoBehaviour {
	public ArmourList armourItems;
	public WeaponList weaponItems;
	public SoldierList soldiers;
	public CharacterList characters;

	private static DataStore instance;

	/**
	 * If this oject already exists, return the existing instance of it, or if it's yet to be instantiated then create it
	 */
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
		characters = new CharacterList();
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

	public Character GetCharacter(string _key) {
		return characters.GetCharacter(_key);
	}
}