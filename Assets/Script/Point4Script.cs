using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point4Script : MonoBehaviour {

    public GameObject Doramukan;
    public float speed = 0.5f;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update()
	{
	}

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "denkiman")
        {
			if (Input.GetKeyUp(KeyCode.E))
            {
				
                Doramukan.GetComponent<Rigidbody>().useGravity = true;
            }

        }
    }
}
