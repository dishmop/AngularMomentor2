using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	public static string startScreenName = "Start Screen",
		levelSelectScreenName = "Level Select Screen",
		helpScreenName = "Help Screen",
		angularMomentumScreenName = "Angular Momentum Screen",
		gameplayScreenName = "Gameplay Screen",
		aboutScreenName = "About Screen",
		quitScreenName = "Quit Screen",
		websiteText = "http://divf.eng.cam.ac.uk/gam2eng/Main/WebHome";
	public float defaultRightPosition = 0.4f, extendedRightPosition = 0.9f;
	public Color defaultTextColour, higlightedTextColour;

	public AudioClip soundMouseEnter, soundMouseExit;

	public GameObject[] buttons;
	private RectTransform[] buttonTransforms;
	private Text[] buttonTexts;
	private int selectedButton = -1;

    public string finalQuitURL;

	// Use this for initialization
	void Start () {
		buttons = GameObject.FindGameObjectsWithTag ("Menu Button");
		buttonTransforms = new RectTransform [buttons.Length];
		buttonTexts = new Text [buttons.Length];
		for (int n = buttons.Length - 1; n >= 0; n--) {
			buttonTransforms [n] = buttons [n].GetComponent<RectTransform> ();
			buttonTexts [n] = buttons [n].GetComponentInChildren<Text> ();
		}
		
		for (int n = buttonTransforms.Length - 1; n >= 0; n--) {
			if (!buttonTexts [n].text.Contains ("Level") || buttonTexts [n].text.Contains ("Select")) {
				buttonTransforms[n].anchorMax = new Vector2 (buttonTransforms[n].anchorMin.x,
					buttonTransforms[n].anchorMax.y);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int n = buttonTransforms.Length - 1; n >= 0; n--) {
			if (!buttonTexts [n].text.Contains ("Level") || buttonTexts [n].text.Contains ("Select")) {
				if (n != selectedButton) {
					buttonTransforms[n].anchorMax = new Vector2 (
					buttonTransforms[n].anchorMax.x * 0.8f + defaultRightPosition * 0.2f,
					buttonTransforms[n].anchorMax.y);
					buttonTexts[n].color = Color.Lerp (defaultTextColour, buttonTexts[n].color, 0.8f);
				}
			}
		}
		if (selectedButton >= 0 && selectedButton < buttonTransforms.Length) {
			if (!buttonTexts [selectedButton].text.Contains ("Level")
			    || buttonTexts [selectedButton].text.Contains ("Select")) {
				buttonTransforms[selectedButton].anchorMax = new Vector2 (
				buttonTransforms[selectedButton].anchorMax.x * 0.8f + extendedRightPosition * 0.2f,
				buttonTransforms[selectedButton].anchorMax.y);
				buttonTexts[selectedButton].color = Color.Lerp (higlightedTextColour, buttonTexts[selectedButton].color, 0.8f);
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (Application.loadedLevelName == startScreenName) {
				GoToQuitScreen ();
			} else {
				GoToStartScreen ();
			}
		}
	}
	
	public void GoToStartScreen () {
		Application.LoadLevel (startScreenName);
	}
	public void GoToLevelSelectScreen () {
		Application.LoadLevel (levelSelectScreenName);
	}
	public void GoToHelpScreen () {
		Application.LoadLevel (helpScreenName);
	}
	public void GoToAngularMomentumScreen () {
		Application.LoadLevel (angularMomentumScreenName);
	}
	public void GoToGameplayScreen () {
		Application.LoadLevel (gameplayScreenName);
	}
	public void GoToAboutScreen () {
		Application.LoadLevel (aboutScreenName);
	}
	public void GoToQuitScreen () {
		Application.LoadLevel (quitScreenName);
	}
	public void QuitGame () {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		if (finalQuitURL != ""){
			Application.OpenURL(finalQuitURL);
		}
#else
		if (finalQuitURL != ""){
			Application.OpenURL(finalQuitURL);
		}
		Application.Quit();
#endif
	}

	public void StartNewGame () {
		Application.LoadLevel ("Level 1");
	}
	public void StartLevel (GameObject callingButton) {
		for (int n = buttons.Length - 1; n >= 0; n--) {
			if (buttons [n].Equals (callingButton)) {
				string levelName = callingButton.GetComponentInChildren<Text> ()
					.text.Replace (" Button", "").Replace ("- ", "");
				//Debug.Log ("Start level: " + levelName);
				Application.LoadLevel (levelName);
				return;
			}
		}
	}
	public void CopyWebsiteText () {
		//To be filled in at some point maybe?
	}
	
	public void MouseEnter (GameObject callingButton) {
		for (int n = buttons.Length - 1; n >= 0; n--) {
			if (buttons [n].Equals (callingButton)) {
				selectedButton = n;
				if (soundMouseEnter != null) {
					AudioSource.PlayClipAtPoint (soundMouseEnter, transform.position, 0.5f);
				}
				return;
			}
		}
	}
	public void MouseExit (bool playSound = true) {
		if (selectedButton != -1) {
			if (playSound && soundMouseExit != null) {
				AudioSource.PlayClipAtPoint (soundMouseExit, transform.position, 0.5f);
			}
		}
		selectedButton = -1;
	}
}
