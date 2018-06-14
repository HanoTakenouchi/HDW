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
			wall.gameObject.transform.position = Vector3.Lerp(wall.gameObject.transform.position, new Vector3(13.38f, -8.63f, 7.69f), Time.deltaTime);
			lever.gameObject.transform.position = Vector3.Lerp(lever.gameObject.transform.position, new Vector3(-4.06f, 7.56f, 12.12f), Time.deltaTime);
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "denkiman")
		{
			if (Input.GetKey(KeyCode.L))
			{
				flag = true;
			}

		}
	}
}
   
