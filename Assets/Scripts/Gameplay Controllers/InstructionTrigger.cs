using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionTrigger : MonoBehaviour {

	public string message, requiredColliderName = "";
	public bool destroyOnTrigger = false;

	void OnTriggerEnter2D (Collider2D collider) {
		if (requiredColliderName == "" || collider.name == requiredColliderName) {
			GameObject.Find ("Game Controller").GetComponent<InstructionController> ().MessageTrigger (message);
			if (destroyOnTrigger) {
				Destroy (gameObject);
			}
		}
	}

}