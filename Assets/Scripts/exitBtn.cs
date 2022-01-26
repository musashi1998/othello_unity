using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitBtn : MonoBehaviour {

	public void quit(){
		Debug.Log ("exit btn pressed");
		Application.Quit();
	}
}
