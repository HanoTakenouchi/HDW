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
			wall.gameObject.transform.position = Vector3.Lerp(wall.gameObject.transform.position, new Vector3(-40.11001f,-9.200001f, -24.42f), Time.deltaTime);
			lever.gameObject.transform.position = Vector3.Lerp(lever.gameObject.transform.position, new Vector3(-44.53f, 7.63f, -41.7f), Time.deltaTime);
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
   
