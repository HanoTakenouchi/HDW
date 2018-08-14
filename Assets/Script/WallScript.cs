using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class
WallScript : MonoBehaviour
{
	public GameObject wall;
	private bool flag = false;
	public GameObject lever;

	// Use this for initialization
	void Start()
	{

	}
	// Update is called once per frame
	void Update()
	{
		if (flag)
		{
			wall.gameObject.transform.position = Vector3.Lerp(wall.gameObject.transform.position, new Vector3(-93.41001f,-10.2f, 9.475004f), Time.deltaTime);
			lever.gameObject.transform.position = Vector3.Lerp(lever.gameObject.transform.position, new Vector3(151.11f, 7.079999f, -25.93f), Time.deltaTime);
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "denkiman")
		{
			if (Input.GetKey(KeyCode.E))
			{
				flag = true;
			}

		}
	}
}
   
