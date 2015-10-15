using UnityEngine;
using System.Collections;

public class HandCollisionNotifier : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter2D (Collision2D collision) {
		GameObject.FindWithTag ("Player").GetComponent<PlayerController> ().HandsCollided (collision);
	}
	
	//void OnTriggerEnter2D (Collider2D collider) {
	void OnTriggerStay2D (Collider2D collider) {
		GameObject.FindWithTag ("Player").GetComponent<PlayerController> ().HandsTriggered (collider);
	}
}
