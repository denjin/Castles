using UnityEngine;
public static class Angle {

	public static float pi = Mathf.PI;
	public static float twoPi = pi * 2;

	//converts angle in degrees to radians
	public static float DegToRad(float angle) {
		return angle * pi / 180;
	}

	//converts angle in radians to angle in degrees
	public static float RadToDeg(float angle) {
		return angle * 180 / pi;
	}

	//normalises angle to -pi and +pi
	public static float NormaliseRad(float angle) {
		while(angle < -pi) {
			angle += twoPi;
		}
		while(angle > pi) {
			angle -= twoPi;
		}
		return angle;
	}

	//normalises angle to 0 and 2 pi
	public static float NormaliseRad2(float angle) {
		while(angle < 0) {
			angle += twoPi;
		}
		while(angle > twoPi) {
			angle -= twoPi;
		}
		return angle;
	}

	//normalises angle to -180 and +180
	public static float NormaliseDeg(float angle) {
		while(angle < -180) {
			angle += 360;
		}
		while(angle > 180) {
			angle -= 360;
		}
		return angle;
	}

	//normalises angle to 0 and +360
	public static float NormaliseDeg2(float angle) {
		while(angle < 0) {
			angle += 360;
		}
		while(angle > 360) {
			angle -= 360;
		}
		return angle;
	}

	//is normalised angle between min and max?
	public static bool IsEnclosedNormalised(float angle, float max, float min) {
		return (angle > min && angle < max);
	}

	//is angle in degrees between min and max?
	public static bool IsEnclosedDeg(float angle, float max, float min) {
		while(angle > max) {
			angle -= 360;
		}
		while(angle < min) {
			angle += 360;
		}
		return (angle > min && angle < max);
	}

	//is angle in radians between min and max?
	public static bool IsEnclosedRad(float angle, float max, float min) {
		while(angle > max) {
			angle -= pi;
		}
		while(angle < min) {
			angle += pi;
		}
		return (angle > min && angle < max);
	}

	//gets cone angle in radians with [0,0] as centre
	public static float ConeAngle(Vector2 a, Vector2 b) {
		return Mathf.Atan2(a.y, a.x) - Mathf.Atan2(b.y, b.x);
	}

	//gets polar angle in radians
	public static float PolarAngle(Vector2 a) {
		return Mathf.Atan2(a.y, a.x);
	}
}