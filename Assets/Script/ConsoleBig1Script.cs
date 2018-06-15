using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ConsoleBig1Script : MonoBehaviour
{
	public GameObject Canvas1;
	private bool consoleflag;
	Animation CanvasAnimation;

    
	// Use this for initialization
	void Start()
	{
		Debug.Log("kiteru");
	}

	// Update is called once per frame
	void Update()
	{
		
	}
   
	void OnTriggerStay(Collider collider) 
	{
		if (collider.gameObject.tag == "Player")
		{
			if (Input.GetKeyUp(KeyCode.L))
			{
				CanvasAnimation = Canvas1.gameObject.GetComponent<Animation>();
				CanvasAnimation.Play();
				Debug.Log("kiteru2");
			}
		}
	}
}
