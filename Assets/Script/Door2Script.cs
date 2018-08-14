using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door2Script : MonoBehaviour {

	public GameObject doors;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
			doors.transform.position = Vector3.Lerp(doors.gameObject.transform.position, new Vector3(173.1891f,-1.3611f,-29.98645f), Time.deltaTime);
        }
    }

}
