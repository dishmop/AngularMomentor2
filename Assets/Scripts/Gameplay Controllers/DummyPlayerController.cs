using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DummyPlayerController : MonoBehaviour {

	GameController gc;
	DummyAngularHUDController ac;

	//Body variables
	private Rigidbody2D rb;
	private Transform body;
	private Transform head;
	private Transform upperArms;
	private Transform upperLegs;
	private Transform lowerLegs;
	private Transform lowerArms;
	private Transform feet;
	private float minHeadAngle = -20.0f, maxHeadAngle = 30.0f;
	private float minUpperArmAngle = -15.0f, maxUpperArmAngle = 155.0f;
	private float minLowerArmAngle = 160.0f, maxLowerArmAngle = 15.0f;
	private float minUpperLegAngle = 150.0f, maxUpperLegAngle = 0.0f;
	private float minLowerLegAngle = -165.0f, maxLowerLegAngle = 0.0f;
	private float minFootAngle = 10.0f, maxFootAngle = -60.0f;
	
	private GameObject freeControls;

	//Control variables
	public float extension, desiredExtension;//(0-1)
	public float mOI;
	public float minMOI = 1, maxMOI = 4;
	public float angularVelocity;
	public Vector2 cOM;

	// Use this for initialization
	void Start () {
		gc = GameObject.Find ("Game Controller").GetComponent<GameController> ();
		ac = GetComponent<DummyAngularHUDController> ();
		rb = GetComponent<Rigidbody2D> ();

		body = transform.FindChild ("Body");
		head = body.FindChild ("Head").transform;
		upperArms = body.FindChild ("Upper Arms").transform;
		lowerArms = body.FindChild ("Upper Arms").FindChild ("Lower Arms").transform;
		upperLegs = body.FindChild ("Upper Legs").transform;
		lowerLegs = body.FindChild ("Upper Legs").FindChild ("Lower Legs").transform;
		feet = body.FindChild ("Upper Legs").FindChild ("Lower Legs").FindChild ("Feet").transform;

		//freeControls = GameObject.Find ("Dummy Free Controls");
		//freeControls.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
	}
	void FixedUpdate () {

		desiredExtension = (((Input.mousePosition.y / Screen.height) - 0.5f) * 1.25f) + 0.5f;
		extension = extension * 0.8f + desiredExtension * 0.2f;

		extension = Mathf.Clamp01 (extension);
		//-----Update associated physics-----
		float newMOI = Mathf.Lerp (minMOI, maxMOI, extension);//This should be a quadratic interp
		rb.angularVelocity *= mOI / newMOI;
		mOI = newMOI;
		//Store COM
		Vector2 oldCOM = rb.centerOfMass;
		//Update limbs rotations (this changes the COM)
		head.localRotation = Quaternion.Euler (0, 0, Mathf.Lerp (minHeadAngle, maxHeadAngle, extension));
		upperArms.localRotation = Quaternion.Euler (0, 0, Mathf.Lerp (minUpperArmAngle, maxUpperArmAngle, extension));
		lowerArms.localRotation = Quaternion.Euler (0, 0, Mathf.Lerp (minLowerArmAngle, maxLowerArmAngle, extension));
		upperLegs.localRotation = Quaternion.Euler (0, 0, Mathf.Lerp (minUpperLegAngle, maxUpperLegAngle, extension));
		lowerLegs.localRotation = Quaternion.Euler (0, 0, Mathf.Lerp (minLowerLegAngle, maxLowerLegAngle, extension));
		feet.localRotation = Quaternion.Euler (0, 0, Mathf.Lerp (minFootAngle, maxFootAngle, extension));
		//Move the body to keep the COM as the centre of rotation
		body.localPosition -= (Vector3)(rb.centerOfMass - oldCOM);
		//Reset the COM position to avoid weird momentum-gaining effects (but store it for debugging first)

		cOM = transform.TransformPoint (rb.centerOfMass);

		angularVelocity = rb.angularVelocity;
		cOM = transform.TransformPoint (rb.centerOfMass);
		//----------

		//Debug
		rb.angularVelocity -= Input.GetAxis("Horizontal");
	}

	public float GetMomentOfInertia () {
		return mOI;
	}
	public float GetMaxMomentOfInertia () {
		return maxMOI;
	}

	public float GetAngularVelocity () {
		//Debug.Log (transform.name + " angular velocity is " + rb.angularVelocity);
		return rb.angularVelocity;
	}
	public void SetAngularVelocity (float angularVel) {
		//Debug.Log (transform.name + " angular velocity set to " + angularVel);
		rb.angularVelocity = angularVel;
	}
	public void SetAngularMomentum (float newAngularMomentum) {
		rb.angularVelocity = newAngularMomentum / mOI;
	}

	public Vector2 GetCentreOfMass () {
		return rb.centerOfMass;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere (transform.TransformPoint (rb.centerOfMass), 0.05f);
	}
}