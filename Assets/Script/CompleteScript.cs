using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CompleteScript : MonoBehaviour {

    Animation CompleteAnimation;

	// Use this for initialization
	void Start () 
	{
		
    }
	
	// Update is called once per frame
	void Update () 
	{
		if (transform.position.z >= 50)
		{
			CompleteAnimation = this.gameObject.GetComponent<Animation>();
			CompleteAnimation.Play();
		}
	}
}
 