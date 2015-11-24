using UnityEngine;

public static class Dice {
	public static int Roll(int dice = 1, int sides = 6) {
		int result = 0;
		for (int i = 0; i < dice; i++) {
			result += Random.Range(1, sides + 1);
		}
		return result;
	}
}