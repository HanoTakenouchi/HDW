using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point4Script : MonoBehaviour {

    public GameObject Doramukan;
    bool flag = false;
    public float speed = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (flag)
        {
            Doramukan.gameObject.transform.position = Vector3.Lerp(Doramukan.gameObject.transform.position, new Vector3(-8, 2, 3.6f),speed);
        }
	}

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "denkiman")
        {
			if (Input.GetKeyUp(KeyCode.L))
            {
                Doramukan.GetComponent<Rigidbody>().useGravity = true;

            }

        }
    }
}
