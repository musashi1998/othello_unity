using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class toggleMusic : MonoBehaviour {

	// Use this for initialization
	private bool musicIsPlaying = true;
	public AudioSource BGM;
	public void toggle(){
		if (musicIsPlaying) {
			//AudioListener.pause = true;
			BGM.mute = true;
			musicIsPlaying = false;
			GameObject.Find("toggleMusic").GetComponentInChildren<Text>().text = "MUSIC:OFF";

		} else {
			musicIsPlaying = true;
			//AudioListener.pause = false;
			BGM.mute = false;
			GameObject.Find("toggleMusic").GetComponentInChildren<Text>().text = "MUSIC:ON";
		}

	}



}
