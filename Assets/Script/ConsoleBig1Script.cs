using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleBig1Script : MonoBehaviour
{
	private GameObject CanvasObject;
	public Canvas Canvas1;
	private int Counter = 0;
    
	// Use this for initialization
	void Start()
	{
		CanvasObject = GameObject.Find("Canvas1");
		Debug.Log("kiteru");
		Canvas1.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnTriggerStay(Collider collider) 
	{
		if (collider.gameObject.tag == "Player")
		{
			if (Input.GetKeyDown(KeyCode.L))
			{
				if (Counter == 0)
				{
					CanvasObject.gameObject.SetActive(true);
					Debug.Log("kiteru2");
					Counter++;
				}

				else if (Counter == 1)
				{
					Canvas1.gameObject.SetActive(false);
					Counter--;
				}
			}
		}
	}
}
