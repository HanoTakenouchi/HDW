using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

   

	// Use this for initialization
	void Start()
	{
		Invoke("DelayMethod", 1);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    private void OnCollisionEnter(Collision collision)
    { 
        collision.gameObject.SendMessage("Damage");
    }

	void DelayMethod()
    {
        Destroy(this.gameObject);
    }
    
}

