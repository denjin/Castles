namespace Gear {
	public class ArmourItem : Item {
		private string location;
		private int damageReduction;

		public ArmourItem() {}

		public ArmourItem(string _name, int _weight, string _location, int _damageReduction) : base (_name, _weight) {
			location = _location;
			damageReduction = _damageReduction;
		}

		public void SetLocation (string _location) {
			location = _location;
		}
		
		public string GetLocation () {
			return location;
		}
		
		public void SetDamageReduction (int _damageReduction) {
			damageReduction = _damageReduction;
		}
		
		public int GetDamageReduction () {
			return damageReduction;
		}
	}
}