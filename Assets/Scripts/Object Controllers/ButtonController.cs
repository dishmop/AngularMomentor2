using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour {
	
	public string instructionMessage = "", requiredColliderName = "";
	public Color colorUnpressed, colorPressed;
	public AudioClip soundPressed;
	public bool pressed = false;
	
	// Use this for initialization
	void Start () {
		if (pressed) {
			GetComponent<SpriteRenderer> ().color = colorPressed;
		} else {
			GetComponent<SpriteRenderer> ().color = colorUnpressed;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (!pressed) {
			if (requiredColliderName == "" || collider.name == requiredColliderName) {
				pressed = true;
				GameObject.Find ("Game Controller").GetComponent<InstructionController> ().MessageTrigger (instructionMessage);
				GetComponent<SpriteRenderer> ().color = colorPressed;
				if (soundPressed != null) {
					AudioSource.PlayClipAtPoint (soundPressed, transform.position);
				}
			}
		}
	}
}
