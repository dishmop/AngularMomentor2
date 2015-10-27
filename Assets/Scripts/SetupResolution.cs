using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class SetupResolution : MonoBehaviour {
	
	float targetFPS = 60;
	float outlierDuration = 0.2f;	// 5 FPS is clearly erroneous and we should just ignore it
	float sustainedBadFPSDuration = 10;
	float smoothedFrameDuration = 1/30f;
	float triggerStartTime = -100;
	float triggerDuration = 2;
	static Resolution[] resolutions;
	static int resIndex = -1;
	static int lastRestIndex = -1;
	static float aspect = 0;
	public static int numReductions = 0;
	static int maxNumReductions = 2;
	
	float lastTime = 0;
	float timerStart = 0;
	
	void Awake () {
		if (resolutions == null){
			resolutions = Screen.resolutions;
			resIndex = resolutions.Count() - 1;
			aspect = (float)resolutions[resIndex].width / (float)resolutions[resIndex].height;
		}
	}

	
	// Update is called once per frame
	void Update () {
		#if !UNITY_WEBPLAYER
		if (lastRestIndex != resIndex){
			Screen.SetResolution (resolutions[resIndex].width, resolutions[resIndex].height, true);
			lastRestIndex = resIndex;
		}
		
		float thisTime = Time.realtimeSinceStartup;
		float frameDuration = thisTime - lastTime;
		
		if (frameDuration <= outlierDuration){
			smoothedFrameDuration = Mathf.Lerp (smoothedFrameDuration, frameDuration, 0.05f);
		}
		else{
			// If we get an outlier restart the timer
			ResetTimer();
		}
		
		// The framerate has to be below 90% of the target for a sustained period before we switch resolutions
		float smoothedFPS = 1f/smoothedFrameDuration;
		if (smoothedFPS > targetFPS * 0.9f){
			ResetTimer();
		}
		
		if (Time.time > timerStart + sustainedBadFPSDuration && numReductions < maxNumReductions){
			ReduceResolution();
			
		}
		
		
		
		lastTime = thisTime;
		#endif
	}
	
	void ResetTimer(){
		timerStart = Time.time;
	}
	
	void ReduceResolution(){
		ResetTimer();
		int newResIndex = GetNextResDown();
		if (newResIndex != resIndex){
			Debug.Log ("Reducing resolution to " + resolutions[newResIndex].width + "x" + resolutions[newResIndex].height + " due to low framerate");
			Analytics.CustomEvent("ReduceResolution", new Dictionary<string, object>
			{
				{ "numReductions", numReductions},
				{ "gameTime", Time.fixedTime},
				{ "level", Application.loadedLevelName},
				{ "oldes", resolutions[resIndex].width + "x" + resolutions[resIndex].height},
				{ "newRes", resolutions[newResIndex].width + "x" + resolutions[newResIndex].height},
			});	
			triggerStartTime = Time.time;
			++numReductions;
		}
		resIndex = newResIndex;
		
	}
	
	int GetNextResDown(){
		for (int testIndex = resIndex-1; testIndex >= 0; --testIndex){
			float testAspect = (float)resolutions[testIndex].width / (float)resolutions[testIndex].height;
			if (Mathf.Abs (testAspect -  aspect) < 0.01f){
				return testIndex;
			}
		}
		return resIndex;
	}
	
	void OnGUI(){
		if (Time.time < triggerStartTime + triggerDuration){
			GUI.skin.label.fontSize = 20;
			string message = "Reducing resolution to " + resolutions[resIndex].width + "x" + resolutions[resIndex].height + " to try and improve framerate";
			Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(message));
			float widthRemaining = Screen.width - textSize.x;
			GUI.Label(new Rect(widthRemaining * 0.5f, 20, textSize.x, 50), message);
		}
		//		GUI.Label(new Rect(10, 20, 200, 50), 1f/smoothedFrameDuration + " fps");
	}
	
}