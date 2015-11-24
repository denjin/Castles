using UnityEngine;
using System.Collections.Generic;

public static class Shooting {
	public static int GetCoverModifier(List<Node> path) {
		bool inPartialCover = false;
		bool inFullCover = false;
		int i;
		for (i = 0; i < path.Count; i++) {
			if (path[i].highCover) {
				inFullCover = true;
				break;
			}
		}

		if (!inFullCover) {
			for (i = 0; i < path.Count; i++) {
				if (path[i].lowCover) {
					inPartialCover = true;
					break;
				}
			}
		}
		
		if (inFullCover) {
			Debug.Log("shooter modifier = " + -2);
			return -2;
		}

		if (inPartialCover) {
			Debug.Log("shooter modifier = " + -1);
			return -1;
		}
		Debug.Log("cover modifier = " + 0);
		return 0;
	}

	public static int GetShooterModifier(Unit shooter, WeaponItem weapon, int range) {
		int modifier = 0;
		if (range <= weapon.shortRange) {
			modifier += weapon.shortModifier;
		} else if (range > weapon.shortRange && range <= weapon.longRange) {
			modifier += weapon.longModifier;
		}
		Debug.Log("shooter modifier = " + modifier);
		
		return modifier;
	}

	public static int GetTargetModifier(Unit target, WeaponItem weapon) {
		int modifier = 0;
		if (target.running) {
			modifier -= 1;
		}
		Debug.Log("target modifier = " + modifier);
		return modifier;
	}

	public static int GetRequiredHitRoll(Unit target) {
		return 7 - target.ballisticSkill;
	}

	public static int GetRequiredWoundRoll(Unit target, WeaponItem weapon, int roll) {
		int requiredRoll;
		int t = target.toughness;
		int s = weapon.strength;
		requiredRoll = t - s + 4;
		if (requiredRoll > 7) {
			requiredRoll = 100;
		} else if (requiredRoll == 7) {
			requiredRoll = 6;
		}
		return requiredRoll;
	}
}