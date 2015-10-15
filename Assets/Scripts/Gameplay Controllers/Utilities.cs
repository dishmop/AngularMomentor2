using UnityEngine;
using System.Collections;

public class Utilities {

	public static float SignedAngle (Vector2 from, Vector2 to) {
		float ang = Vector2.Angle (from, to);
		Vector3 cross = Vector3.Cross(from, to);
		if (cross.z < 0)
			ang = -ang;//360 - ang;
		return ang;
	}

	public static float ClampAngle (float angle, float min, float max) {
		float mid = (min + max) * 0.5f;
		while (angle < mid - 180.0f) {
			angle += 360.0f;
		}
		while (angle >= mid + 180.0f) {
			angle -= 360.0f;
		}
		angle = Mathf.Clamp (angle, min, max);
		return angle;
	}

}
