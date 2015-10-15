using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	Camera cam;
	GameObject player;
	Rigidbody2D rb_player;
	InstructionController ic;

	Vector2 rollingPosition;// = Vector3.zero;
	float rollingSize;
	public float minCameraSize = 3.5f;
	public Vector2 playerPosition;
	public Vector2 targetPosition;
	public bool followPlayer;
	bool playerDead, exiting;
	bool fixCameraAngle;

	// Use this for initialization
	void Start () {
	}
	void Awake () {
		//The following only applies for 2.5D games/ scenes
		/*foreach (Rigidbody body in FindObjectsOfType<Rigidbody> ()) {
			body.constraints = RigidbodyConstraints.FreezePositionZ
				| RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
		}*/
		cam = Camera.main;
		player = GameObject.FindGameObjectWithTag ("Player");
		rb_player = player.GetComponent<Rigidbody2D> ();
		playerPosition = player.transform.TransformPoint (rb_player.centerOfMass);
		ic = GetComponent<InstructionController> ();


		rollingPosition = targetPosition;
		rollingSize = 0.5f;

		followPlayer = true;
		playerDead = false;
		exiting = false;
		fixCameraAngle = true;

		//GameObject.Find ("Canvas").SetActive (true);
		GameObject.FindObjectOfType<Canvas> ().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel ("Start Screen");
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel (Application.loadedLevelName);
		}
		if (!playerDead && ic != null) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				ic.MessageTrigger ("Spacebar Pressed");
			}
			if (Input.GetButtonDown ("Fire1")) {
				ic.MessageTrigger ("Fire1 Pressed");
			}
			if (Input.GetButtonUp ("Fire1")) {
				ic.MessageTrigger ("Fire1 Released");
			}
		}
	}

	void FixedUpdate () {
		if (player != null && rb_player != null) {
			playerPosition = player.transform.TransformPoint (rb_player.centerOfMass);
			if (followPlayer) {
				rollingPosition = playerPosition * 0.1f + rollingPosition * 0.9f;
				rollingSize = minCameraSize * 0.05f + rollingSize * 0.95f;
			} else {
				rollingPosition = (playerPosition + targetPosition) * 0.5f * 0.1f + rollingPosition * 0.9f;
				rollingSize = Mathf.Max (minCameraSize, (playerPosition - targetPosition).magnitude * 0.6f) * 0.05f + rollingSize * 0.95f;
			}
			//targetPosition = playerPosition;//Vector2.zero;//
			//Debug.Log (Mathf.Max (3.5f, (playerPosition - targetPosition).magnitude * 0.5f));
			if (!fixCameraAngle) {
				//cam.transform.localRotation = Quaternion.Euler (0, 0, rb_player.rotation);
			}
		} else {
			player = GameObject.FindGameObjectWithTag ("Player");
			rb_player = player.GetComponent<Rigidbody2D> ();
		}
		if (exiting) {
			cam.transform.position = new Vector3 (playerPosition.x * 0.1f + cam.transform.position.x * 0.9f,
			                                      playerPosition.y * 0.1f + cam.transform.position.y * 0.9f,
			                                      cam.transform.position.z);
			cam.orthographicSize *= 0.992f;
			cam.orthographicSize = Mathf.Max (cam.orthographicSize, 0.5f);
		} else {
			cam.transform.position = new Vector3 (rollingPosition.x, rollingPosition.y, cam.transform.position.z);
			cam.orthographicSize = rollingSize;
		}
	}

	/*void OnGUI()
	{
		GUI.Label(new Rect(0,0,100,100), new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).ToString());
	}*/
	
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (playerPosition, 0.05f);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere (targetPosition, 0.05f);
	}
	
	public void SetCameraTarget (Vector2 target) {
		targetPosition = target;
		Vector2 relativePosition = Vector2.ClampMagnitude (targetPosition - rb_player.position, 20);
		targetPosition = rb_player.position + relativePosition;
	}
	public void SetCameraShouldFollowPlayer (bool shouldFollowPlayer) {
		followPlayer = shouldFollowPlayer;
	}
	
	public void SetCameraAngle (bool fixAngle, float angle) {
		fixCameraAngle = fixAngle;
		if (fixCameraAngle) {
			//cam.transform.rotation = Quaternion.Euler (0, 0, angle);
		}
	}
	
	public bool GetPlayerDead () {
		return playerDead;
	}
	public void SetPlayerDead () {
		playerDead = true;
	}
	public void SetPlayerExiting () {
		exiting = true;
	}

	public void MessageTrigger (string message) {
		Debug.Log ("Instruction controller recieved message: " + message);
		//General level messages
		switch (message) {
		case ("Unlock Exit Hatch") :
		{
			GameObject.FindGameObjectWithTag ("Exit").GetComponent<ExitController> ().SetIsUnlocked (true);
			break;
		}
		}
		//Specific level messages
		switch (ic.levelNumber) {
		case (4) :
		{
			switch (message) {
			case ("Message For Level 4") :
			{
				GameObject.FindGameObjectWithTag ("Exit").GetComponent<ExitController> ().SetIsUnlocked (true);
				break;
			}
			}
			break;
		}
		}
	}
}
