namespace Gear {
	public class WeaponItem : Item {
		private string type;
		private int requiredHands;
		private int range;
		private int damage;
		private string damageType;

		public WeaponItem() {}

		public WeaponItem (string _name, int _weight, string _type, int _requiredHands, int _range, int _damage, string _damageType) : base (_name, _weight) {
			type = _type;
			requiredHands = _requiredHands;
			range = _range;
			damage = _damage;
			damageType = _damageType;
		}

		//--------------------
		//getters / setters
		//--------------------

		public void SetType (string _type) {
			type = _type;
		}
		
		public string GetType () {
			return type;
		}
		
		public void SetRequiredHands (int _requiredHands) {
			requiredHands = _requiredHands;
		}
		
		public int GetRequiredHands () {
			return requiredHands;
		}

		public void SetRange (int _range) {
			range = _range;
		}
		
		public int GetRange () {
			return range;
		}
		
		public void SetDamage (int _damage) {
			damage = _damage;
		}
		
		public int GetDamage () {
			return damage;
		}

		public void SetDamageType (string _damageType) {
			damageType = _damageType;
		}
		
		public string GetDamageType () {
			return damageType;
		}
		
		
	}
}