namespace Gear {
	public class Item {
		//the name of the item
		protected string name;
		protected int weight;

		public Item() {}

		public Item(string _name, int _weight) {
			name = _name;
			weight = _weight;
		}

		public string GetName() {
			return name;
		}

		public void SetName(string _name) {
			name = _name;
		}

		public void SetWeight (int _weight) {
			weight = _weight;
		}
		
		public int GetWeight () {
			return weight;
		}
	}
}