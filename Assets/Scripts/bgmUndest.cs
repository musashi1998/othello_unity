using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmUndest : MonoBehaviour {

	// Use this for initialization
	void Awake(){
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("bgm");
		if (objs.Length > 1) {
			Destroy (this.gameObject);
		} else {
			DontDestroyOnLoad (this.gameObject);
		}		
	}
}
