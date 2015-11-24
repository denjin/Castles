
public class WeaponItem : Item {
	public string type;
	public int shortRange;
	public int longRange;
	public int shortModifier;
	public int longModifier;
	public int damage;
	public int strength;
	public int reliability;

	public WeaponItem() {}

	public WeaponItem (string _name, int _weight, string _type, int _shortRange, int _longRange, int _shortModifier, int _longModifier, int _damage, int _strength, int _reliability) : base (_name, _weight) {
		type = _type;
		shortRange = _shortRange;
		longRange = _longRange;
		shortModifier = _shortModifier;
		longModifier = _longModifier;
		damage = _damage;
		strength = _strength;
		reliability = _reliability;
	}
}