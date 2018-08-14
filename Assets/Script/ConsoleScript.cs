using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleScript : MonoBehaviour
{
	public GameObject doors;
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
		private void OnTriggerStay(Collider collider)
		{
			if (collider.gameObject.tag == "Player")
			{
			doors.transform.position = Vector3.Lerp(doors.gameObject.transform.position, new Vector3(105.5891f, -1.3963f, -29.94f), Time.deltaTime);
			}
		}

		//private void OnTriggerExit(Collider collider)
		//{
		//	if (collider.gameObject.tag == "player")
		//	{
		//		doors.gameObject.transform.position = Vector3.Lerp(doors.gameObject.transform.position, new Vector3(9.46f, -1.3963f, -54.66354f), Time.deltaTime);
		//	}
		//}

}

