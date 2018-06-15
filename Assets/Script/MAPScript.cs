using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MAPScript : MonoBehaviour {

	public GameObject MAP;
	Animation MAPAnimation;
    
	// Use this for initialization
	void Start () 
	{
		MAPAnimation = this.gameObject.GetComponent<Animation>();
	}

	// Update is called once per frame
	void Update () 
	{
		MAPAnimation.Play();
	}
}
