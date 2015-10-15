using UnityEngine;
using System.Collections;

public class SetupResolution : MonoBehaviour {

	int count = 0;
	static bool firstTime = true;
	
	
	void Awake () {
		if (firstTime){
			Screen.SetResolution (1024, 768, false);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		if (count++ == 60){
			if (firstTime){
				Screen.SetResolution (Screen.currentResolution.width, Screen.currentResolution.height, true);
				firstTime = false;
			}
			
		}
		
	}
}
