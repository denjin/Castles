namespace Gear {
	public class ArmourItem : Item {
		private string location;
		private int damageReduction;

		public ArmourItem() {}

		public ArmourItem(string _name, int _weight, string _location, int _damageReduction) : base (_name, _weight) {
			location = _location;
			damageReduction = _damageReduction;
		}

		public void setLocation (string _location) {
			location = _location;
		}
		
		public string getLocation () {
			return location;
		}
		
		public void setDamageReduction (int _damageReduction) {
			damageReduction = _damageReduction;
		}
		
		public int getDamageReduction () {
			return damageReduction;
		}
	}
}