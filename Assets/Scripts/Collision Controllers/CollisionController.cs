using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollisionController : MonoBehaviour {

	private Rigidbody2D rb;
	private Vector2 previousVelocity;
	private float previousAngularVelocity;
	private Collision2D lastCollision;

	public Vector2 targetVelocity;
	public bool setVelocity;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		previousVelocity = Vector2.zero;
		previousAngularVelocity = 0.0f;
	}
	
	void FixedUpdate () {
		if (setVelocity) {
			rb.velocity = targetVelocity;
			setVelocity = false;
		}
	}
	
	void LateUpdate () {
		previousVelocity = rb.velocity;
		previousAngularVelocity = rb.angularVelocity;
	}

	public void ResetVelocity () {
		rb.velocity = previousVelocity;
		rb.angularVelocity = previousAngularVelocity;
	}
}