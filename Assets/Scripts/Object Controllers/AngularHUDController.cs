using UnityEngine;
using System.Collections;

public class AngularHUDController : MonoBehaviour {

	private PlayerController pc;
	private GameObject angularHUD;
	private Transform angularVelocityArrow, angularVelocityArrow2, angularMomentumArrow;
	private LineRenderer mOIRingLineRenderer;
	private int circlePoints = 25;

	private float maxMOI;

	void Start () {
		pc = GetComponent<PlayerController> ();

		angularHUD = GameObject.Find ("Angular HUD");
		angularVelocityArrow = GameObject.Find ("Angular Velocity Arrow").transform;
		angularVelocityArrow2 = GameObject.Find ("Angular Velocity Arrow 2").transform;
		angularMomentumArrow = GameObject.Find ("Angular Momentum Arrow").transform;

		mOIRingLineRenderer = GetComponentInChildren<LineRenderer> ();
		mOIRingLineRenderer.SetVertexCount (circlePoints + 1);

		maxMOI = pc.GetMaxMomentOfInertia ();
	}

	void Update () {

		Vector2 cOM = pc.GetCentreOfMass ();
		//mOIRing.transform.localPosition = -cOM;
		angularVelocityArrow.transform.localPosition = -cOM;
		angularVelocityArrow2.transform.localPosition = -cOM;
		angularMomentumArrow.transform.localPosition = -cOM;

		float mOI = pc.GetMomentOfInertia ();
		float angularVelocity = pc.GetAngularVelocity ();
		float angularMomentum = mOI * angularVelocity * 0.2f;
		float scaleFactor = mOI / maxMOI;
		//mOIRing.localScale = new Vector3 (scaleFactor, scaleFactor, 1);
		float angleIncrement = 2 * Mathf.PI / circlePoints;
		for (int n = circlePoints - 1; n >= 0; n--) {
			mOIRingLineRenderer.SetPosition (n,
				scaleFactor * new Vector2 (Mathf.Cos (n * angleIncrement), Mathf.Sin (n * angleIncrement)));
		}
		mOIRingLineRenderer.SetPosition (circlePoints, new Vector2 (scaleFactor, 0));
		mOIRingLineRenderer.SetWidth (0.05f / scaleFactor, 0.05f / scaleFactor);
		angularVelocityArrow.localPosition += new Vector3 (0, scaleFactor + 0.2f, 0);
		angularVelocityArrow2.localPosition -= new Vector3 (0, scaleFactor + 0.2f, 0);
		if (angularVelocity < 0) {
			angularVelocityArrow.localScale = new Vector3 (1, 1, 1);
			angularVelocityArrow2.localScale = new Vector3 (1, 1, 1);
		} else {
			angularVelocityArrow.localScale = new Vector3 (-1, 1, 1);
			angularVelocityArrow2.localScale = new Vector3 (-1, 1, 1);
		}
		angularMomentumArrow.Rotate (0, 0, (-pc.GetAngularVelocity () + angularMomentum) * Time.deltaTime);

	}

	public void SetShow (bool show) {
		if (angularHUD != null) {
			angularHUD.SetActive (show);
		}
	}

	/*public void SetAlpha(float alpha) {
		mOIRing.GetComponent<SpriteRenderer> ().color = alpha * 150.0f / 255.0f;
		angularVelocityArrow.GetComponent<SpriteRenderer> ().color.a = alpha * 150.0f / 255.0f;
		angularMomentumArrow.GetComponent<SpriteRenderer> ().color.a = alpha * 150.0f / 255.0f;
	}*/
	
}