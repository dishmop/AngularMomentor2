using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	static AudioController instance;
	
	void Awake()
	{
		if (instance == null) {
			//Debug.Log("Assigning instance of Audio Controller");
			instance = this;//new GameObject("Audio Controller").AddComponent<AudioController>();
		} else {
			Destroy (gameObject);
		}
		DontDestroyOnLoad(this);
	}
	
	public void OnApplicationQuit()
	{
		//Debug.Log("Audio Controller destroyed");
		instance = null;
		Destroy(this);
	}
}
