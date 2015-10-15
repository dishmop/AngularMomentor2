using UnityEngine;
using System.Collections;

public class HeadCollisionNotifier : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter2D (Collision2D collision) {
		GameObject.FindWithTag ("Player").GetComponent<PlayerController> ().HeadCollided (collision);
	}
	/*void OnTriggerEnter2D (Collider2D collider) {
		GameObject.FindWithTag ("Player").GetComponent<PlayerController> ().HeadCollided (collider);
	}*/
}
