using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playBtn : MonoBehaviour {

	// Use this for initialization
	 Button pb;
	void Awake ()
    {
      pb = GameObject.Find("PlayButton").GetComponent<Button>();
	  pb.Select();
    }
	public void playGame(){
		SceneManager.LoadScene ("GAME");
	}
}
