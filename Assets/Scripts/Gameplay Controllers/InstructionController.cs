using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionController: MonoBehaviour {
	
	private GameController gc;
	private PlayerController pc;
	private GameObject dummyPlayer;

	public AudioClip soundAdvance;
	public bool usesDummy = false;

	public int levelNumber = 1;
	public int instructionNumber = -1;
	private GameObject instructionUI;
	/*public*/ Text instructionTextComponent;
	public string[] instructionTexts;
	public bool[] instructionContinues;

	public GameObject[] enableObjects;
	
	// Use this for initialization
	void Start () {
		gc = GameObject.Find ("Game Controller").GetComponent<GameController> ();
		pc = GameObject.FindWithTag ("Player").GetComponent<PlayerController> ();
		if (usesDummy) {
			dummyPlayer = GameObject.Find ("Dummy Player");
			//dummyPlayer.SetActive (false);
		}

		instructionUI = GameObject.Find ("Instruction UI");
		if (instructionTextComponent == null) {
			instructionTextComponent = GameObject.Find ("Instruction Text").GetComponent<Text> ();
		}
		instructionUI.SetActive (false);

		//Replace #@ with newline
		for (int n = instructionTexts.Length - 1; n >= 0; n--) {
			if (instructionContinues[n]) {
				instructionTexts[n] = instructionTexts[n] + System.Environment.NewLine + "[Press the Spacebar to continue]";
			}
			instructionTexts[n] = instructionTexts[n].Replace("#@", System.Environment.NewLine);
		}
	}
	
	void Update () {
		if (Input.GetButtonDown ("Jump")) {

		}
	}
	
	public void SetInstruction (string text) {
		if (instructionTextComponent != null) {
			instructionTextComponent.text = text;
		}
		
	}
	public void FinishInstruction () {
		if (instructionNumber > -1 && instructionNumber < instructionTexts.Length) {
			if (instructionContinues [instructionNumber]) {
				NextInstruction();
			} else {
				instructionUI.SetActive (false);
			}
		}
	}
	public void NextInstruction (int goToInstruction = -1) {
		instructionUI.SetActive (true);
		if (goToInstruction == -1) {
			instructionNumber++;
		} else {
			instructionNumber = goToInstruction;
		}
		if (instructionNumber > -1 && instructionNumber < instructionTexts.Length) {
			if (instructionTextComponent != null) {
				instructionTextComponent.text = instructionTexts [instructionNumber];
			}
			InstructionNumberTrigger ();
		} else {
			//Debug.Log ("Instruction number out of range");
			instructionUI.SetActive (false);
		}
		if (soundAdvance != null) {
			AudioSource.PlayClipAtPoint (soundAdvance, transform.position, 0.75f);
		}
	}
	
	public void InstructionNumberTrigger () {
		switch (levelNumber) {
			#region Angular Momentum Tutorial
		case (-1): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target to below centre");
				gc.SetCameraTarget (Vector2.down * 2.0f);
				gc.SetCameraShouldFollowPlayer (false);
				dummyPlayer.GetComponent<DummyPlayerController> ().SetAngularMomentum (-360.0f);
				GameObject.Find ("Start Controls").SetActive (false);
				//This ^ has to be done because the play object which would usually handle it doesn't exist in this scene
				break;
			}
			}
			break;
		}
			#region Gameplay Tutorial
			#endregion
		case (-2): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target to below centre");
				gc.SetCameraTarget (Vector2.down * 2.0f);
				gc.SetCameraShouldFollowPlayer (false);
				dummyPlayer.GetComponent<DummyPlayerController> ().SetAngularMomentum (45.0f);
				GameObject.Find ("Start Controls").SetActive (false);
				//This ^ has to be done because the play object which would usually handle it doesn't exist in this scene
				break;
			}
			case (1): {
				//Debug.Log ("Setting camera target to foot hooks");
				gc.SetCameraTarget (GameObject.Find ("Foot Hooks").transform.position + Vector3.down);
				break;
			}
			case (2): {
				//Debug.Log ("Setting camera target to button");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Button").transform.position);
				break;
			}
			case (3): {
				//Debug.Log ("Setting camera target to entry hatch");
				gc.SetCameraTarget (GameObject.Find ("Entry Hatch").transform.position);
				break;
			}
			case (4): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 1
		case (1): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target to below centre");
				gc.SetCameraTarget (Vector2.down * 2.0f);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			case (3): {
				pc.SetExtension (0.5f);
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.lockState = CursorLockMode.None;
				break;
			}
			case (5): {
				pc.SetAngularMomentum(-90.0f);
				break;
			}
			case (7): {
				pc.SetExtension (0.5f);
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.lockState = CursorLockMode.None;
				break;
			}
			case (9): {
				enableObjects [0].SetActive (true);
				enableObjects [0].GetComponent<Rigidbody2D> ().angularVelocity = -22.5f;
				//Debug.Log (pc.transform.rotation.z * Mathf.Rad2Deg);
				enableObjects [0].transform.rotation = pc.transform.rotation * Quaternion.Euler (0, 0, 90.0f);
				//Debug.Log(enableObjects[0].transform.rotation.z * Mathf.Rad2Deg);
				break;
			}
			case (11): {
				gc.MessageTrigger ("Unlock Exit Hatch");
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 2
		case (2): {
			switch (instructionNumber) {
			case (1): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			case (2): {
				//Debug.Log ("Setting camera target to foot hooks");
				gc.SetCameraTarget (GameObject.Find ("Foot Hooks Start").transform.position);
				break;
			}
			case (3): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 3
		case (3): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target to button");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Button").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			case (6): {
				gc.MessageTrigger ("Unlock Exit Hatch");
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 4
		case (4): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target to foot hooks");
				gc.SetCameraTarget (GameObject.Find ("Foot Hooks Target").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			case (1): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.MessageTrigger ("Unlock Exit Hatch");
				pc.SetMaxJumpAngle (45.0f);
				break;
			}
			case (4): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.MessageTrigger ("Unlock Exit Hatch");
				pc.SetMaxJumpAngle (45.0f);
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 5
		case (5): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target to foot hooks");
				gc.SetCameraTarget (GameObject.Find ("Foot Hooks Target").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 6
		case (6): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target to button");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Button").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 7
		case (7): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target to button");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Button").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 8
		case (8): {
			switch (instructionNumber) {
			case (0): {
				//Debug.Log ("Setting camera target below centre");
				gc.SetCameraTarget (Vector3.down * 2.0f);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			case (1): {
				//Debug.Log ("Setting camera target below cargo");
				gc.SetCameraTarget (GameObject.Find ("Cargo").transform.position + Vector3.down * 4.0f);
				break;
			}
			case (3): {
				//Debug.Log ("Setting camera target to foot hooks");
				gc.SetCameraTarget (GameObject.Find ("Foot Hooks Target").transform.position);
				break;
			}
			case (4): {
				//Debug.Log ("Setting camera target below cargo");
				gc.SetCameraTarget (GameObject.Find ("Cargo").transform.position + Vector3.down * 2.0f);
				break;
			}
			case (7): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			}
			break;
		}
			#endregion
			
			#region Level X
		case (200): {
			switch (instructionNumber) {
			case (1): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			case (2): {
				gc.SetCameraShouldFollowPlayer (true);
				break;
			}
			case (3): {
				Cursor.lockState = CursorLockMode.Locked;
				pc.SetExtension (0.5f);
				enableObjects [0].SetActive (true);
				break;
			}
			case (4): {
				Cursor.lockState = CursorLockMode.None;
				break;
			}
			}
			break;
		}
			#endregion
			#region Level Y
		case (300): {
			switch (instructionNumber) {
			case (1): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			case (2): {
				//Debug.Log ("Setting camera target to foot hooks");
				gc.SetCameraTarget (GameObject.Find ("Foot Hooks Target").transform.position);
				break;
			}
			case (3): {
				gc.SetCameraShouldFollowPlayer (true);
				enableObjects [0].SetActive (true);//Enable the foot hooks at the start position
				Cursor.lockState = CursorLockMode.None;
				break;
			}
			case (4): {
				//pc.SetLockState (PlayerController.State.FeetLanded);
				dummyPlayer.SetActive (true);//Enable the dummy player
				//Debug.Log ("Setting camera target to just below dummy player");
				gc.SetCameraTarget (dummyPlayer.transform.position + Vector3.down);
				gc.SetCameraShouldFollowPlayer (false);
				//dummyPlayer.GetComponent<DummyPlayerController> ().SetAngularVelocity (60.0f);
				//Not sure why this ^ gets overridden; something to do with activation of the dummy?
				break;
			}
			case (5): {
				dummyPlayer.GetComponent<DummyPlayerController> ().SetAngularVelocity (-90.0f);
				////Debug.Log ("Dummy Player angular velocity = "
				//           + dummyPlayer.GetComponent<DummyPlayerController> ().GetAngularVelocity ());
				break;
			}
			case (11): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				dummyPlayer.SetActive (false);
				//pc.UnlockState ();
				break;
			}
			}
			break;
		}
			#endregion
		}
	}
	public void MessageTrigger (string message) {
		////Debug.Log ("Instruction controller recieved message: " + message);
		if (message == "Spacebar Pressed") {
			if (instructionNumber > -1 && instructionNumber < instructionTexts.Length) {
				if (instructionContinues[instructionNumber]) {
					NextInstruction ();
				}
			}
			return;
		}
		if (message == "Fire1 Released") {
			if (instructionNumber == -1) {
				NextInstruction ();
			}
			return;
		}
		switch (levelNumber) {
			#region Level 1
		case (1): {
			switch (message) {
			case ("Fully Extended"): {
				if (instructionNumber == 3 || instructionNumber == 8) {
					NextInstruction ();
				}
				break;
			}
			case ("Fully Crouched"): {
				if (instructionNumber == 4 || instructionNumber == 7) {
					NextInstruction ();
				}
				break;
			}
			case ("Button 1 Pressed"): {
				if (instructionNumber == 9 || instructionNumber == 10) {
					NextInstruction ();
				}
				break;
			}
			case ("Button 2 Pressed"): {
				if (instructionNumber == 9 || instructionNumber == 10) {
					NextInstruction ();
				}
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 2
		case (2): {
			switch (message) {
			case ("Feet Hooked"): {
				if (instructionNumber <= 2) {
					NextInstruction (3);
				}
				break;
			}
			case ("Fully Extended"): {
				if (instructionNumber <= 3) {
					NextInstruction (4);
				}
				break;
			}
			case ("Hatch Reached"): {
				if (instructionNumber <= 4) {
					NextInstruction (5);
				}
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 3
		case (3): {
			switch (message) {
			case ("Fire1 Pressed"): {
				if (instructionNumber == 2) {
					NextInstruction ();
				}
				break;
			}
			case ("Feet Hooked"): {
				if (instructionNumber <= 1) {
					NextInstruction (2);
				}
				break;
			}
			case ("State ChoosingSpin"): {
				if (instructionNumber <= 2) {
					NextInstruction (3);
				}
				break;
			}
			case ("State Free"): {
				if (instructionNumber >= 2 && instructionNumber <= 4) {
					NextInstruction (5);
				}
				break;
			}
			case ("Button Pressed"): {
				if (instructionNumber <= 5) {
					NextInstruction (6);
				}
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 4
		case (4): {
			switch (message) {
			case ("Feet Hooked"): {
				if (instructionNumber <= 0) {
					NextInstruction (1);
				}
				break;
			}
			case ("Hatch Reached"): {
				if (instructionNumber == 1 || instructionNumber == 2) {
					NextInstruction (3);
				}
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 5
		case (5): {
			switch (message) {
			case ("Feet Hooked"): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 6
		case (6): {
			switch (message) {
			case ("Button Pressed"): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				gc.MessageTrigger ("Unlock Exit Hatch");
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 7
		case (7): {
			switch (message) {
			case ("Button Pressed"): {
				//Debug.Log ("Setting camera target to exit hatch");
				gc.SetCameraTarget (GameObject.FindGameObjectWithTag ("Exit").transform.position);
				gc.SetCameraShouldFollowPlayer (false);
				gc.MessageTrigger ("Unlock Exit Hatch");
				break;
			}
			}
			break;
		}
			#endregion
			#region Level 8
		case (8): {
			switch (message) {
			case ("Great Stuff"): {
				if (instructionNumber <= 4) {
					NextInstruction (5);
				}
				break;
			}
			case ("Lower Foot Hooks"): {
				if (instructionNumber <= 6) {
					NextInstruction (7);
				}
				break;
			}
			}
			break;
		}
			#endregion
			
			#region Level X
		case (200): {
			switch (message) {
			case ("Fire1 Pressed"): {
				if (instructionNumber == -1 || instructionNumber == 5 || instructionNumber == 6) {
					NextInstruction ();
				}
				break;
			}
			case ("Feet Hooked"): {
				if (instructionNumber == 4) {
					NextInstruction ();
				}
				break;
			}
			case ("Can Restart"): {
				if (instructionNumber == 7) {
					NextInstruction ();
				}
				break;
			}
			case ("Grab Hatch"): {
				if (instructionNumber == 8) {
					NextInstruction ();
				}
				break;
			}
			}
			break;
		}
			#endregion
			#region Level Y
		case (300): {
			switch (message) {
			case ("Fire1 Pressed"): {
				if (instructionNumber == -1) {
					NextInstruction ();
				}
				break;
			}
			case ("Explain Momentum"): {
				if (instructionNumber <= 3) {
					NextInstruction ();
				}
				break;
			}
			}
			break;
		}
			#endregion
		}
	}
}