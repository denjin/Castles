using UnityEngine;

public class Sector {
	
	public float alpha;
	public float theta;

	public float Beta {
		get {
			return alpha + theta;
		}
	}

	public Sector(float _alpha = 0f, float _theta = 0f) {
		alpha = _alpha;
		theta = _theta;
	}

	public void Clear() {
		alpha = 0f;
		theta = 0f;
	}

	public void SetFullCircle() {
		alpha = 0;
		theta = Angle.twoPi;
	}

	public void SetCone(Vector2 dir, float coneAngle, bool normalise = false) {
		alpha = Mathf.Atan2(dir.y, dir.x) - 0.5f * coneAngle;
		if(normalise) {
			alpha = Angle.NormaliseRad2(alpha);
		}
		theta = coneAngle;
	}

	public void SetFromCoords(Vector2 c, Vector2 a, Vector2 b, bool normalise) {
		alpha = Mathf.Atan2(a.y - c.y, a.x - c.x);
		float newBeta = Mathf.Atan2(b.y - c.y, b.x - c.x);
		if (normalise) {
			alpha = Angle.NormaliseRad2(alpha);
			newBeta = Angle.NormaliseRad2(newBeta);
		}
		if(alpha >= newBeta) {
			Clear();
		} else {
			theta = newBeta - alpha;
		}
	}

	public void Copy(Sector sector) {
		alpha = sector.alpha;
		theta = sector.theta;
	}

	public void SetIntersection(Sector a, Sector b) {
		if(a.theta == 0 || b.theta == 0) {
			Clear();
		} else {
			alpha = Mathf.Max(a.alpha, b.alpha);
			float newBeta = Mathf.Min(a.alpha + a.theta, b.alpha + b.theta);
			if(newBeta <= alpha) {
				Clear();
			} else {
				theta = newBeta - alpha;
			}
		}
	}

	public void SetUnion(Sector a, Sector b) {
		if(a.theta == 0) {
			//copy b
			Copy(b);
		} else if(b.theta == 0) {
			//copy a
			Copy(a);
		} else {
			alpha = Mathf.Min(a.alpha, b.alpha);
			float newBeta = Mathf.Max(a.Beta, b.Beta);
			if(newBeta <= alpha) {
				Clear();
			} else {
				theta = newBeta - alpha;
			}
		}
	}
}