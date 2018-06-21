using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickShotScript : MonoBehaviour {
	public Camera camera;
    

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Debug.Log("Click1");
		ClickShot();
		Debug.Log("Click4");
	}

	void ClickShot()
	{
		int distance = 100;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo, distance))
		{
			Debug.DrawRay(ray.origin, hitInfo.point, Color.red);
    }   
		}

}
