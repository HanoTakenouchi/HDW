using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleBig1Script : MonoBehaviour
{
	public GameObject Canvas1;
	private int Counter = 0;
    
	// Use this for initialization
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnTriggerStay(Collider collider) 
	{
		if (collider.gameObject.tag == "Player")
		{
			if (Input.GetKey(KeyCode.L))
			{
				Canvas1.SetActive(true);
				Debug.Log("kiteru1");
				Counter++;

				if (Counter == 1)
				{
					if (Input.GetKey(KeyCode.L))
					{
						Canvas1.SetActive(false);
						Counter--;
					}
				}
			}
		}
	}
}
