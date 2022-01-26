using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class colorChanger : MonoBehaviour {

	public float speed = 0.99f;
	public Color startColor = Color.black;
	public Color endColor = Color.red;
	public bool repeatable = true;
	float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		
	}

	// Update is called once per frame
	void Update () {
		if (!repeatable)
		{
			float t = (Time.time/1.5f - startTime) * speed;
			GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
		}
		else
		{
			float t = (Mathf.Sin(Time.time/1.5f - startTime) * speed);
			GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
		}
	}
}
