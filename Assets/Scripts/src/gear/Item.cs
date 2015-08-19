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

		public string getName() {
			return name;
		}

		public void setName(string _name) {
			name = _name;
		}

		public void setWeight (int _weight) {
			weight = _weight;
		}
		
		public int getWeight () {
			return weight;
		}
	}
}