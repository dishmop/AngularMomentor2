using UnityEngine;
using System.Collections;

public class BackgroundScaler : MonoBehaviour {

	Camera cam;
	public float baseScaleFactor = 1.0f;

	// Use this for initialization
	void Start () {
		cam = transform.root.GetComponent<Camera> ();
		//Enable the sprite rendered (this is disabled because it grabs all mouse clicks in the editor
		GetComponent<SpriteRenderer> ().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3 (baseScaleFactor * cam.orthographicSize, baseScaleFactor * cam.orthographicSize, 1);
	}
}
