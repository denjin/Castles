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

		public void setType (string _type) {
			type = _type;
		}
		
		public string getType () {
			return type;
		}
		
		public void setRequiredHands (int _requiredHands) {
			requiredHands = _requiredHands;
		}
		
		public int getRequiredHands () {
			return requiredHands;
		}

		public void setRange (int _range) {
			range = _range;
		}
		
		public int getRange () {
			return range;
		}
		
		public void setDamage (int _damage) {
			damage = _damage;
		}
		
		public int getDamage () {
			return damage;
		}

		public void setDamageType (string _damageType) {
			damageType = _damageType;
		}
		
		public string getDamageType () {
			return damageType;
		}
		
		
	}
}