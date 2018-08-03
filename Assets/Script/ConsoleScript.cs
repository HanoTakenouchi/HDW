using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleScript : MonoBehaviour
{
	public GameObject doors;
	public GameObject doors2;
	public GameObject doors3;

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
			doors.transform.position = Vector3.Lerp(doors.gameObject.transform.position, new Vector3(1.010912f, -1.3963f, -37.96354f), Time.deltaTime);
			doors2.transform.position = Vector3.Lerp(doors.gameObject.transform.position, new Vector3(-1.1f, -1.4787f, 69.8f), Time.deltaTime);
			doors3.transform.position = Vector3.Lerp(doors.gameObject.transform.position, new Vector3(-1.8f, -1.4787f, 87.9f), Time.deltaTime);

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

