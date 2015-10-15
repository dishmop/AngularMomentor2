using UnityEngine;
using System.Collections;

public class FootCollisionNotifier : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter2D (Collision2D collision) {
		//Debug.Log (collision.gameObject.transform.up.ToString ());
		GameObject.FindWithTag ("Player").GetComponent<PlayerController> ().FeetCollided (collision,
		                                                                                  collision.gameObject.transform.up);
		//GameObject.Find ("Game Controller").GetComponent<InstructionController> ().FeetCollided (collision);
	}
}
