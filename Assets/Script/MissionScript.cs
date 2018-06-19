using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MissionScript : MonoBehaviour {

	public GameObject Mission;
    Animation MissionAnimation;
    
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		MissionAnimation = this.gameObject.GetComponent<Animation>();
		MissionAnimation.Play();
	}
}
