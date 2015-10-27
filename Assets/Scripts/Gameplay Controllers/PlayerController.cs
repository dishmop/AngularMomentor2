using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class PlayerController : MonoBehaviour {
	
	Camera cam;
	GameController gc;
	InstructionController ic;
	AngularHUDController ac;
	public bool hasHUD = false;

	public AudioClip soundHooked, soundJump, soundHatchGrabbed, soundHeadImpact;

	//Body variables
	private Rigidbody2D rb;
	private Transform body;
	private Transform head;
	private Transform upperArms;
	private Transform upperLegs;
	private Transform lowerLegs;
	private Transform lowerArms;
	private Transform feet;
	private float minHeadAngle = -30.0f, maxHeadAngle = 10.0f;
	private float minUpperArmAngle = -15.0f, maxUpperArmAngle = 155.0f;
	private float minLowerArmAngle = 160.0f, maxLowerArmAngle = 15.0f;
	private float minUpperLegAngle = 150.0f, maxUpperLegAngle = 0.0f;
	private float minLowerLegAngle = -165.0f, maxLowerLegAngle = 0.0f;
	private float minFootAngle = 10.0f, maxFootAngle = -60.0f;
	
	private GameObject freeControls, startControls, restartControls;
	private Image powerArrow, spinArrow;

	public GameObject ragdoll;
	public ParticleSystem impactEffect;

	//Control variables
	public float extension, lastExtension, desiredExtension;//(0-1)
	public float mOI;
	public float minMOI = 1, maxMOI = 4;
	public float angularVelocity;
	public Vector2 cOM;
	public enum State { None,
		LevelStart, Free,
		FeetLanded, ChoosingPower, Extending, ChoosingSpin,
		HandsGrabbed, Exiting, BeingTaught };
	public float jumpSpeed, jumpSpin, jumpAngle;
	public float minJumpSpeed = 0.5f, maxJumpSpeed = 5.0f, maxJumpSpin = 120.0f;
	public float maxJumpAngle = 45.0f;
	public float maxPowerMouseDistance = 3.0f, arrowBaseScale = 2.4f;

	public State state, previousState, lockToState;

	private ContactPoint2D currentContact;
	public float landTimeout = 1.5f;
	private float landTime;
	public Vector2 desiredPosition, jumpDirection;
	public float desiredAngle, wallAngle;
	public float crouchFootDistance = 0.4f;
	private Vector2 lastVelocity;
	private float lastAngularVelocity;

	public string nextScene;

	public float startTime;
	
	// Use this for initialization
	void Start () {
		cam = Camera.main;
		gc = GameObject.Find ("Game Controller").GetComponent<GameController> ();
		ic = GameObject.Find ("Game Controller").GetComponent<InstructionController> ();
		if (hasHUD) {
			ac = GetComponent<AngularHUDController> ();
		}

		//rb = transform.FindChild ("Body").GetComponent<Rigidbody2D> ();
		body = transform.FindChild ("Body");
		head = body.FindChild ("Head").transform;
		upperArms = body.FindChild ("Upper Arms").transform;
		lowerArms = body.FindChild ("Upper Arms").FindChild ("Lower Arms").transform;
		upperLegs = body.FindChild ("Upper Legs").transform;
		lowerLegs = body.FindChild ("Upper Legs").FindChild ("Lower Legs").transform;
		feet = body.FindChild ("Upper Legs").FindChild ("Lower Legs").FindChild ("Feet").transform;
		rb = GetComponent<Rigidbody2D> ();
		//hud = GetComponent<HUDController> ();
		freeControls = GameObject.Find ("Free Controls");
		startControls = GameObject.Find ("Start Controls");
		restartControls = GameObject.Find ("Restart Controls");
		//Debug.Log (freeControls.ToString () + ", " + startControls.ToString () + ", " + restartControls.ToString ());
		freeControls.SetActive (false);
		startControls.SetActive (false);
		restartControls.SetActive (false);
		powerArrow = GameObject.Find ("Power Arrow").GetComponent<Image> ();
		spinArrow = GameObject.Find ("Spin Arrow").GetComponent<Image> ();
		powerArrow.enabled = false;
		spinArrow.enabled = false;
		/*upperLegs = transform.FindChild ("Upper Legs").transform;
		lowerLegs = transform.FindChild ("Upper Legs").FindChild ("Lower Legs").transform;
		upperArms = transform.FindChild ("Upper Arms").transform;
		lowerArms = transform.FindChild ("Upper Arms").FindChild ("Lower Arms").transform;*/

		lockToState = State.None;
		SetState (State.LevelStart);
		//nextState = State.Free;
		lastVelocity = Vector2.zero;
		lastAngularVelocity = 0.0f;
		startTime = Time.time;
//		Debug.Log ("levelStart - levelName: " + Application.loadedLevelName);
		Analytics.CustomEvent("levelStart", new Dictionary<string, object>
		{
			{ "levelName", Application.loadedLevelName },
		});		
		
	}
	
	// Update is called once per frame
	void Update () {
		//gc.SetCameraTarget(rb.position);
		switch (state) {
		case (State.LevelStart) : {
			gc.SetCameraTarget(cam.ScreenToWorldPoint (Input.mousePosition));
			if (Input.GetButtonDown ("Fire1")) {
				SetState (State.Free);
			}
			break;
		}
		case (State.Free) : {
			break;
		}
		case (State.FeetLanded) : {
			if (((rb.position - desiredPosition).sqrMagnitude < 0.01f
			     && Mathf.Abs (Mathf.DeltaAngle(desiredAngle, rb.rotation)) < 0.01f && extension < 0.01f)
			    || landTime > landTimeout) {
				SetState (State.ChoosingPower);
			}
			break;
		}
		case (State.ChoosingPower) : {
			//-----HUD-----
			powerArrow.transform.localScale = arrowBaseScale * Vector3.one / cam.orthographicSize;
			powerArrow.rectTransform.position = (Vector2)cam.WorldToScreenPoint (rb.position
				+ jumpDirection * maxPowerMouseDistance * jumpSpeed / maxJumpSpeed);
			powerArrow.fillAmount = jumpSpeed / maxJumpSpeed;
			powerArrow.rectTransform.rotation = Quaternion.Euler (0, 0, jumpAngle);
			if (Input.GetButtonDown ("Fire1")) {
				SetState (State.Extending);
			}
			break;
		}
		case (State.Extending) : {
			if (extension > 0.99f) {
				SetState (State.ChoosingSpin);
			}
			break;
		}
		case (State.ChoosingSpin) : {
			Vector2 mousePosition = cam.ScreenToWorldPoint (Input.mousePosition);
			jumpSpin = -Mathf.Clamp (-2.0f * (Mathf.DeltaAngle (jumpAngle,
				(Mathf.Atan2 (mousePosition.y - rb.position.y, mousePosition.x - rb.position.x) * Mathf.Rad2Deg - 90.0f))),
				-maxJumpSpin, maxJumpSpin);
			//-----HUD-----
			powerArrow.transform.localScale = arrowBaseScale * Vector3.one / cam.orthographicSize;
			spinArrow.transform.localScale = arrowBaseScale * Vector3.one / cam.orthographicSize;
			spinArrow.rectTransform.rotation = Quaternion.Euler (0, 0, jumpAngle + jumpSpin + 90.0f);
			float proportion = jumpSpin / 360.0f;
			spinArrow.fillAmount = Mathf.Abs (proportion);
			spinArrow.rectTransform.localScale = new Vector3 (1, -Mathf.Sign (proportion), 1);
			if (Input.GetButtonDown ("Fire1") || maxJumpSpin == 0.0f) {
				SetState (State.Free);
				rb.velocity = jumpDirection * jumpSpeed;
				rb.angularVelocity = jumpSpin;
			}
			break;
		}
		case (State.HandsGrabbed) : {
			break;
		}
		case (State.Exiting) : {
			if (extension < 0.01f && (rb.position - desiredPosition).sqrMagnitude < 0.01f
			    && rb.velocity.sqrMagnitude < 0.01f && Mathf.Abs (rb.angularVelocity) < 0.2f) {
				Application.LoadLevel(nextScene);
//				Debug.Log ("levelComplete - levelName: " + Application.loadedLevelName + ", levelTime = " + (Time.time - startTime));
				Analytics.CustomEvent("levelStart", new Dictionary<string, object>
				                      {
					{ "levelName", Application.loadedLevelName },
					{ "levelTime",  (Time.time - startTime)},
				});
				
			}
			break;
		}
		case (State.BeingTaught) : {
			break;
		}
		}
	}

	void FixedUpdate () {

		lastVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
		lastExtension = extension;


		//----------Update extension
		switch (state) {
		case (State.LevelStart) : {
			break;
		}
		case (State.Free) : {
			desiredExtension = (((Input.mousePosition.y / Screen.height) - 0.5f) * 1.25f) + 0.5f;
			extension = extension * 0.8f + desiredExtension * 0.2f;
			/*if (rb.angularVelocity > 360) {
				extension = extension * 0.5f + 0.5f;
			}*/
			GameObject.Find ("Game Controller").GetComponent<GameController> ().SetCameraAngle(false, 0);
			break;
		}
		case (State.FeetLanded) : {
			landTime += Time.deltaTime;
			extension *= 0.4f;
			rb.rotation = Mathf.LerpAngle (rb.rotation, desiredAngle, 0.5f);//rb.rotation * 0.5f + desiredAngle * 0.5f;
			desiredPosition = currentContact.point + currentContact.normal * (extension + crouchFootDistance);
			rb.position = Vector2.Lerp (rb.position, desiredPosition, 0.2f);
			rb.velocity = Vector2.zero;
			break;
		}
		case (State.ChoosingPower) : {
			Vector2 mousePosition = cam.ScreenToWorldPoint (Input.mousePosition);
			jumpSpeed = Mathf.Clamp ((mousePosition - rb.position).magnitude * 1.0f, minJumpSpeed, maxJumpSpeed);
			jumpAngle = Mathf.Atan2 (mousePosition.y - rb.position.y, mousePosition.x - rb.position.x) * Mathf.Rad2Deg - 90.0f;
			//float deltaAngle = Mathf.DeltaAngle (jumpAngle, wallAngle);
			//if (deltaAngle
			//jumpAngle = Mathf.Clamp (jumpAngle, wallAngle - maxJumpAngle, wallAngle + maxJumpAngle);
			jumpAngle = Utilities.ClampAngle (jumpAngle, wallAngle - maxJumpAngle, wallAngle + maxJumpAngle);
			rb.rotation = jumpAngle;
			jumpDirection = (Quaternion.AngleAxis (jumpAngle, Vector3.forward) * Vector2.up).normalized;
			desiredPosition = currentContact.point + jumpDirection * crouchFootDistance;
			rb.position = Vector2.Lerp (rb.position, desiredPosition, 0.5f);
			break;
		}
		case (State.Extending) : {
			extension = extension * 0.4f + 0.6f;
			desiredPosition = currentContact.point + jumpDirection * (extension + crouchFootDistance);
			rb.position = Vector2.Lerp (rb.position, desiredPosition, 0.5f);
			break;
		}
		case (State.ChoosingSpin) : {
			break;
		}
		case (State.HandsGrabbed) : {
			//extension += Input.GetAxis ("Vertical") * 0.025f;
			break;
		}
		case (State.Exiting) : {
			extension *= 0.98f;
			rb.position = Vector2.Lerp (rb.position, desiredPosition - extension * (Vector2)transform.up, 0.05f);
			rb.velocity = Vector2.Lerp (rb.velocity, Vector2.zero, 0.2f);
			rb.angularVelocity = Mathf.Lerp (rb.angularVelocity, 0.0f, 0.02f);
			break;
		}
		case (State.BeingTaught) : {
			break;
		}
		}
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
		//rb.angularVelocity -= Input.GetAxis("Horizontal");
		//-----
		if (extension == 1.0f && lastExtension <= 1.0f) {
			ic.MessageTrigger ("Fully Extended");
		}
		if (extension == 0.0f && lastExtension >= 0.0f) {
			ic.MessageTrigger ("Fully Crouched");
		}
	}

	void SetState (State newState) {
		//This is a bit dodgy because it depends on the cyclical order of the states in the enum...
		//if (lockToState == State.None || newState == lockToState || newState == lockToState - 1) {
		//Only change state if we aren't in the desired lock state or there isn't a valid lock state
		if (state != lockToState || lockToState == State.None) {
			//Do any actions related to leaving the current state
			switch (state) {
			case (State.LevelStart):
				{
					/*Cursor.lockState = CursorLockMode.Locked;
					Cursor.lockState = CursorLockMode.None;
					gc.SetCameraTarget (Vector2.zero, true);*/
					//This ^ was screwing with the Instruction Controller
					extension = 0.5f;
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.lockState = CursorLockMode.None;
					break;
				}
			case (State.Free):
				{
					break;
				}
			case (State.FeetLanded):
				{
					break;
				}
			case (State.ChoosingPower):
				{
					break;
				}
			case (State.Extending):
				{
					break;
				}
			case (State.ChoosingSpin):
				{
					extension = 1.0f;
					powerArrow.enabled = false;
					spinArrow.enabled = false;
					break;
				}
			case (State.HandsGrabbed):
				{
					break;
				}
			case (State.Exiting):
				{
					break;
				}
			case (State.BeingTaught):
				{
					break;
				}
			}
			//Update things for new state
			switch (newState) {
			case (State.LevelStart):
				{
					gc.SetCameraShouldFollowPlayer (false);
					ic.MessageTrigger ("State Level Start");
					startControls.SetActive (true);
					freeControls.SetActive (false);
					break;
				}
			case (State.Free):
				{
					ic.MessageTrigger ("State Free");
					rb.freezeRotation = false;
					startControls.SetActive (false);
					freeControls.SetActive (true);
					if (ac != null) {
						ac.SetShow (true);
					}
					//gc.SetCameraTarget (Vector2.zero, true);
					Time.timeScale = 1.0f;
					if (state == State.ChoosingSpin && soundJump != null) {
						AudioSource.PlayClipAtPoint (soundJump, transform.position);
					}
					break;
				}
			case (State.FeetLanded):
				{
					ic.MessageTrigger ("State FeetLanded");
					//GameObject.Find ("Game Controller").GetComponent<GameController> ().SetCameraAngle(true,
					//	Vector2.Angle (currentContact.normal, Vector2.up));
					rb.freezeRotation = true;
					freeControls.SetActive (false);
					if (ac != null) {
						ac.SetShow (false);
					}
					if (soundHooked != null) {
						AudioSource.PlayClipAtPoint (soundHooked, transform.position);
					}
					break;
				}
			case (State.ChoosingPower):
				{
					ic.MessageTrigger ("State ChoosingPower");
					rb.position = desiredPosition;
					rb.rotation = desiredAngle;
					extension = 0.0f;
					powerArrow.fillAmount = 0.0f;
					powerArrow.enabled = true;
					break;
				}
			case (State.Extending):
				{
					ic.MessageTrigger ("State Extending");
					break;
				}
			case (State.ChoosingSpin):
				{
					ic.MessageTrigger ("State ChoosingSpin");
					extension = 1.0f;
					spinArrow.rectTransform.position = cam.WorldToScreenPoint (rb.position);
					spinArrow.fillAmount = 0.0f;
					spinArrow.enabled = true;
					Time.timeScale = 0.0f;
					break;
				}
			case (State.HandsGrabbed):
				{
					ic.MessageTrigger ("State HandsGrabbed");
					break;
				}
			case (State.Exiting):
				{
					if (soundHatchGrabbed != null) {
						AudioSource.PlayClipAtPoint (soundHatchGrabbed, transform.position);
					}
					gc.SetPlayerExiting ();
					ic.MessageTrigger ("State Exiting");
					startControls.SetActive (false);
					freeControls.SetActive (false);
					break;
				}
			case (State.BeingTaught):
				{
					ic.MessageTrigger ("State BeingTaught");
					startControls.SetActive (false);
					freeControls.SetActive (false);
					break;
				}
			}
			previousState = state;
			state = newState;
		}
	}
	/*public void RevertState () {
		SetState (previousState);
	}*/
	public State GetState () {
		return state;
	}
	/*public void SetLockState (State lockState) {
		lockToState = lockState;
	}
	public void LockCurrentState () {
		lockToState = state;
	}
	public void UnlockState () {
		lockToState = State.None;
	}*/

	public float GetMomentOfInertia () {
		return mOI;
	}
	public float GetMaxMomentOfInertia () {
		return maxMOI;
	}
	public float GetAngularVelocity () {
		return rb.angularVelocity;
	}
	public void SetAngularVelocity (float newAngularVel) {
		rb.angularVelocity = newAngularVel;
	}
	public float GetAngularMomentum () {
		return rb.angularVelocity * mOI;
	}
	public void SetAngularMomentum (float newAngularMomentum) {
		rb.angularVelocity = newAngularMomentum / mOI;
	}
	public Vector2 GetCentreOfMass () {
		return rb.centerOfMass;
	}

	public void SetMaxJumpAngle (float newMaxJumpAngle) {
		maxJumpAngle = newMaxJumpAngle;
	}

	void OnCollisionEnter2D (Collision2D collision) {
		//Debug.Log (collision.collider.ToString () + " has collided");
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere (transform.TransformPoint (rb.centerOfMass), 0.05f);
	}
	
	public void FeetCollided(Collision2D collision, Vector2 normal) {
		//Debug.Log ("Feet collided with " + collision.collider.name);
		if (state == State.Free) {
			if (collision.collider.tag == "Landable") {//If we're free (i.e. not performing a jump already)
				//tell the state machine that we've landed and store the collision data
				currentContact = collision.contacts [0];
				wallAngle = Utilities.SignedAngle (Vector2.up, /*currentContact.*/normal);
				//currentContact.normal = normal;
				desiredAngle = Utilities.SignedAngle (Vector2.up, /*currentContact.*/normal);
				/*Vector2 */
				desiredPosition = collision.contacts [0].point;
				//Stick to the collided object; prepare for next jump
				/*rb.freezeRotation = true;
			rb.velocity = Vector2.zero;
			landTime = 0.0f;
			nextState = State.FeetLanded;*/
				SetState (State.FeetLanded);
			} else {
				rb.AddForceAtPosition (collision.contacts[0].normal * Mathf.Abs ((extension - lastExtension)
					/ Time.deltaTime * 200.0f * Mathf.Cos (Mathf.DeltaAngle (
					rb.rotation, Vector2.Angle (collision.contacts[0].normal, Vector2.up)))), collision.contacts[0].point);
				//Debug.Log (Mathf.DeltaAngle (rb.rotation, Vector2.Angle (collision.contacts[0].normal, Vector2.up)));
				//rb.velocity += collision.contacts[0].normal * (extension - lastExtension) / Time.deltaTime * 0.1f;
				//Resolve this in direction of player; non-direct impacts give less velocity
			}
		}
	}
	public void HandsCollided(Collision2D collision) {
		//Debug.Log ("Hands collided with " + collision.collider.name);
		if (state == State.Free) {
			rb.AddForceAtPosition (collision.contacts[0].normal * Mathf.Abs ((extension - lastExtension)
				/ Time.deltaTime * 200.0f * Mathf.Cos (Mathf.DeltaAngle (
				rb.rotation, Vector2.Angle (collision.contacts[0].normal, Vector2.up)))), collision.contacts[0].point);
			//Debug.Log (Mathf.DeltaAngle (rb.rotation, Vector2.Angle (collision.contacts[0].normal, Vector2.up)));
			//Resolve this in direction of player; non-direct impacts give less velocity
		}
	}
	public void HandsTriggered(Collider2D collider) {
		//Debug.Log ("Hands triggered on " + collider.name);
		if (state == State.Free) {
			if (collider.tag == "Exit") {
				if (collider.GetComponent<ExitController> ().GetIsUnlocked ()) {
					desiredPosition = collider.transform.position;
					nextScene = collider.gameObject.GetComponent<ExitController> ().GetToSceneName ();
					collider.gameObject.GetComponent<ExitController> ().StartAnimation ();
					SetState (State.Exiting);
				}
			}
		}
	}
	public void HeadCollided(Collision2D collision) {
		//Debug.Log ("Head collided with " + collision.collider.name);
		//if (Vector2.Dot (collision.relativeVelocity, collision.contacts[0].normal)/ < -2.0f) {
		if (collision.collider.tag != "No Knockout"
		    && collision.relativeVelocity.sqrMagnitude > 3.5f + ((collision.collider.tag == "Soft")? 3.5f : 0.0f)) {
			if (state == State.Extending || state == State.ChoosingSpin) {
				lastVelocity = jumpDirection * jumpSpeed;
				/*nextState = State.Free;
				state = State.Free;*/
				SetState (State.Free);
			}
			/*ParticleSystem impact = */Instantiate (impactEffect, collision.contacts[0].point,
				Quaternion.FromToRotation (Vector3.up, collision.contacts[0].normal));
			ConvertToRagdoll ();
			if (soundHeadImpact != null) {
				AudioSource.PlayClipAtPoint (soundHeadImpact, transform.position, 0.5f);
			}
			//Cause the impact to happen again with the ragdoll
			if (collision.collider.gameObject.isStatic == false) {
				collision.collider.GetComponent<CollisionController> ().ResetVelocity ();
			}
		}
	}

	private void ConvertToRagdoll () {
		if (!gc.GetPlayerDead ()) {
			powerArrow.enabled = false;
			spinArrow.enabled = false;
			startControls.SetActive (false);
			freeControls.SetActive (false);
			restartControls.SetActive (true);
			gc.SetPlayerDead ();
			GameObject curRagdoll = (GameObject)Instantiate (ragdoll, transform.position,
			                                                 transform.rotation * Quaternion.Euler (0, 0, 7));
			//Hacky rotation ^ to match the player's body within the 'Player' object
			curRagdoll.GetComponent<RagdollController> ().MatchTransform (/*curRagdoll.transform, */transform.FindChild ("Body"));
			curRagdoll.GetComponent<RagdollController> ().SetVelocity (lastVelocity, lastAngularVelocity);

			Destroy (gameObject);
		}
	}

	public void SetExtension (float value) {
		extension = value;
	}

	/*private void SetMouseToExtension () {

	}*/
}