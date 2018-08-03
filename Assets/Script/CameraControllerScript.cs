using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControllerScript : MonoBehaviour
{

	public CinemachineVirtualCamera camera;

	public static int priority = 1;
    
	//public BoxCollider Box1;
	//public BoxCollider Box2;
	//public BoxCollider Box3;
    

	// Use this for initialization
	void Start()
	{
		
	}

	// Update is called once per frames
	void Update()
	{

	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			priority += 1;
			camera.Priority = priority;

		}      
	}
}