using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class inGameMusictoggle : MonoBehaviour {

	// Use this for initialization
	private bool IsPlaying = true;
	private AudioSource bgm;
    void Start(){
		
        bgm = AudioSource.FindObjectOfType<AudioSource>();
		if(bgm.mute){
			GameObject.Find("toggleMusic").GetComponentInChildren<Text>().text = "MUSIC:OFF";
		}
    }
	public void toggle(){
		
		if (IsPlaying) {
			//AudioListener.pause = true;
			
			bgm.mute = true;
			IsPlaying = false;
			GameObject.Find("toggleMusic").GetComponentInChildren<Text>().text = "MUSIC:OFF";

		} else {
			IsPlaying = true;
			//AudioListener.pause = false;
			bgm.mute = false;
			GameObject.Find("toggleMusic").GetComponentInChildren<Text>().text = "MUSIC:ON";
		}

	}



}
