using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ConsoleBig2Script : MonoBehaviour
{
    public GameObject MissionCanvas;
    public GameObject Robokuro;
    Animation MissionCanvasAnimation;
    Animation RoboConsoleAnimation;
    private bool ConFlag;
    private bool ConFlag2;
    private int ConsoleCount = 1;

    // Use this for initialization
    void Start()
    {
        ConFlag = true;
        RoboConsoleAnimation = Robokuro.gameObject.GetComponent<Animation>();
        MissionCanvasAnimation = MissionCanvas.gameObject.GetComponent<Animation>();
        MissionCanvasAnimation.gameObject.SetActive(false);
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
				Big1Click();
            }
            if (ConsoleCount == 2)
            {
				Big2Click();
            }
        }
    }
	void Big1Click()
	{
		if (Input.GetMouseButtonDown(1)) 
        {

            if (ConFlag == true)
            {
                MissionCanvasAnimation.gameObject.SetActive(true);
                RoboConsoleAnimation.Play("RoboConsole Animation");
                MissionCanvasAnimation.Play();
                ConFlag = false;
                ConFlag2 = true;
                ConsoleCount = 2;
            }
        }
	}

	void Big2Click()
	{
		if (Input.GetMouseButtonDown(1)) 
        {
            if (ConFlag2 == true)
            {
                RoboConsoleAnimation.Play("RoboConsoleBack Animation");
                MissionCanvasAnimation.gameObject.SetActive(false);
                ConFlag2 = false;
                ConsoleCount = 1;
                ConFlag = true;
            }
        }
	}
}