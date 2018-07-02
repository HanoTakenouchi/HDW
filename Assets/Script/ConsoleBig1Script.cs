using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ConsoleBig1Script : MonoBehaviour
{
	public GameObject MAPCanvas;
	public GameObject MissionCanvas;
	public GameObject Robokuro;
	Animation MAPCanvasAnimation;
	Animation RoboConsoleAnimation;
	private bool ConFlag;
	private bool ConFlag2;
	private int ConsoleCount = 1;
    
	// Use this for initialization
	void Start()
	{
		ConFlag = true;
		ConFlag2 = true;
		RoboConsoleAnimation = Robokuro.gameObject.GetComponent<Animation>();
		MAPCanvasAnimation = MAPCanvas.gameObject.GetComponent<Animation>();
		MAPCanvasAnimation.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
   
	void OnTriggerStay(Collider collider) 
	{
		if (collider.gameObject.tag == "Player")
		{
			if (ConsoleCount == 1)
			{
				if (Input.GetKeyUp(KeyCode.E))
				{
                    
					if (ConFlag == true)
					{
						MAPCanvasAnimation.gameObject.SetActive(true);
						RoboConsoleAnimation.Play("RoboConsole Animation");
						MAPCanvasAnimation.Play();
						ConFlag = false;
						ConsoleCount = 2;
					}
				}
			}
			if (ConsoleCount == 2)
            {
				if(Input.GetKey(KeyCode.E))
			    {
					if (ConFlag2 == true)
					{
						RoboConsoleAnimation.Play("RoboConsoleBack Animation");
						MAPCanvasAnimation.gameObject.SetActive(false);
						ConFlag2 = false;
						ConsoleCount = 1;
					}
				}
			}
		}
	}
}
