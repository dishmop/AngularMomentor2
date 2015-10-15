using UnityEngine;
using System.Collections;

public class RagdollController : MonoBehaviour {
	
	public float totalMass;
	
	private static float headPercentage = 6.29f;
	private static float trunkPercentage = 44.11f;
	private static float upperArmPercentage = 2.71f;
	private static float lowerArmPercentage = 2.23f;
	private static float upperLegPercentage = 14.15f;
	private static float lowerLegPercentage = 4.34f;
	private static float footPercentage = 1.37f;
	//The following values are copy-pasted from the player controller (EnclosedPlayerController)
	private float minHeadAngle = -30.0f, maxHeadAngle = 10.0f;
	private float minUpperArmAngle = -15.0f, maxUpperArmAngle = 155.0f;
	private float minLowerArmAngle = 160.0f, maxLowerArmAngle = 15.0f;
	private float minUpperLegAngle = 150.0f, maxUpperLegAngle = 0.0f;
	private float minLowerLegAngle = -165.0f, maxLowerLegAngle = 0.0f;
	private float minFootAngle = 10.0f, maxFootAngle = -60.0f;
	
	// Use this for initialization
	void Start () {
		Rigidbody2D currentPart;
		JointAngleLimits2D limits = new JointAngleLimits2D ();
		currentPart = GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * trunkPercentage;

		currentPart = gameObject.transform.FindChild ("Head").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * headPercentage;
		limits.min = minHeadAngle; limits.max = maxHeadAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;
		//-----Left-----
		currentPart = transform.FindChild ("Left Upper Arm").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * upperArmPercentage;
		limits.min = minUpperArmAngle; limits.max = maxUpperArmAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;

		currentPart = transform.FindChild ("Left Upper Arm")
			.FindChild ("Left Lower Arm").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * lowerArmPercentage;
		limits.min = minLowerArmAngle; limits.max = maxLowerArmAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;

		currentPart = transform.FindChild ("Left Upper Leg").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * upperLegPercentage;
		limits.min = minUpperLegAngle; limits.max = maxUpperLegAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;

		currentPart = transform.FindChild ("Left Upper Leg")
			.FindChild ("Left Lower Leg").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * lowerLegPercentage;
		limits.min = minLowerLegAngle; limits.max = maxLowerLegAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;

		currentPart = transform.FindChild ("Left Upper Leg")
			.FindChild ("Left Lower Leg").FindChild ("Left Foot").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * footPercentage;
		limits.min = minFootAngle; limits.max = maxFootAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;
		//-----Right-----
		currentPart = transform.FindChild ("Right Upper Arm").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * upperArmPercentage;
		limits.min = minUpperArmAngle; limits.max = maxUpperArmAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;

		currentPart = transform.FindChild ("Right Upper Arm")
			.FindChild ("Right Lower Arm").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * lowerArmPercentage;
		limits.min = minLowerArmAngle; limits.max = maxLowerArmAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;

		currentPart = transform.FindChild ("Right Upper Leg").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * upperLegPercentage;
		limits.min = minUpperLegAngle; limits.max = maxUpperLegAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;

		currentPart = transform.FindChild ("Right Upper Leg")
			.FindChild ("Right Lower Leg").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * lowerLegPercentage;
		limits.min = minLowerLegAngle; limits.max = maxLowerLegAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;

		currentPart = transform.FindChild ("Right Upper Leg")
			.FindChild ("Right Lower Leg").FindChild ("Right Foot").GetComponent<Rigidbody2D> ();
		currentPart.mass = totalMass * footPercentage;
		limits.min = minFootAngle; limits.max = maxFootAngle;
		currentPart.GetComponent<HingeJoint2D> ().limits = limits;
	}
	
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Application.LoadLevel (Application.loadedLevelName);
		}
	}

	public void MatchTransform (Transform player) {
		transform.FindChild ("Head").rotation = player.FindChild ("Head").rotation;
		//-----Left-----
		transform.FindChild ("Left Upper Arm").rotation = player.FindChild ("Upper Arms").rotation;
		transform.FindChild ("Left Upper Arm").FindChild ("Left Lower Arm").rotation
			= player.FindChild ("Upper Arms").FindChild ("Lower Arms").rotation;
		transform.FindChild ("Left Upper Leg").rotation = player.FindChild ("Upper Legs").rotation;
		transform.FindChild ("Left Upper Leg").FindChild ("Left Lower Leg").rotation =
			player.FindChild ("Upper Legs").FindChild ("Lower Legs").rotation;
		transform.FindChild ("Left Upper Leg").FindChild ("Left Lower Leg").FindChild ("Left Foot").rotation
			= player.FindChild ("Upper Legs").FindChild ("Lower Legs").FindChild ("Feet").rotation;
		//-----Right-----
		transform.FindChild ("Right Upper Arm").rotation = player.FindChild ("Upper Arms").rotation;
		transform.FindChild ("Right Upper Arm").FindChild ("Right Lower Arm").rotation
			= player.FindChild ("Upper Arms").FindChild ("Lower Arms").rotation;
		transform.FindChild ("Right Upper Leg").rotation = player.FindChild ("Upper Legs").rotation;
		transform.FindChild ("Right Upper Leg").FindChild ("Right Lower Leg").rotation =
			player.FindChild ("Upper Legs").FindChild ("Lower Legs").rotation;
		transform.FindChild ("Right Upper Leg").FindChild ("Right Lower Leg").FindChild ("Right Foot").rotation
			= player.FindChild ("Upper Legs").FindChild ("Lower Legs").FindChild ("Feet").rotation;
	}
	public void SetVelocity (Vector2 velocity, float angularVelocity) {
		GetComponent<Rigidbody2D> ().velocity = velocity;
		GetComponent<Rigidbody2D> ().angularVelocity = angularVelocity;
		foreach (Transform child in transform.GetComponentsInChildren<Transform> ()) {
			child.GetComponent<Rigidbody2D> ().velocity = velocity;
			child.GetComponent<Rigidbody2D> ().angularVelocity = angularVelocity;
		}
	}
}
