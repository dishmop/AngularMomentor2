using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExitController : MonoBehaviour {

	public string toScene;
	public bool isUnlocked = true;
	public Sprite spriteUnlocked, spriteLocked;
	public AudioClip soundUnlocked, soundLocked;

	// Use this for initialization
	void Start () {
		if (isUnlocked) {
			GetComponent<SpriteRenderer> ().sprite = spriteUnlocked;
		} else {
			GetComponent<SpriteRenderer> ().sprite = spriteLocked;
		}
	}

	public void StartAnimation () {
		//Debug.Log ("Starting exit hatch animation");
	}
	
	public string GetToSceneName () {
		return toScene;
	}
	public bool GetIsUnlocked () {
		return isUnlocked;
	}
	public void SetIsUnlocked (bool unlocked) {
		isUnlocked = unlocked;
		if (isUnlocked) {
			GetComponent<SpriteRenderer> ().sprite = spriteUnlocked;
			if (soundUnlocked != null) {
				AudioSource.PlayClipAtPoint(soundUnlocked, transform.position);
			}
		} else {
			GetComponent<SpriteRenderer> ().sprite = spriteLocked;
			if (soundLocked != null) {
				AudioSource.PlayClipAtPoint(soundLocked, transform.position);
			}
		}
	}
}