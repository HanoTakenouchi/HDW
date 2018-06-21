using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ClearScript : MonoBehaviour {

	Animation MissionAnimation;
	Animation ClearAnimation;
	public GameObject Mission;
	public GameObject Clear;
	public GameObject Robo;
	public GameObject ClearGroup;
	private bool flag;
	private bool flag2;

  
	// Use this for initialization
	void Start () 
	{
		MissionAnimation = Mission.gameObject.GetComponent<Animation>();
		ClearAnimation = Clear.gameObject.GetComponent<Animation>();
		ClearGroup.gameObject.SetActive(false);
		flag = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Robo.transform.position.z >= 38)
		{
			ClearGroup.gameObject.SetActive(true);
			if (flag)
			{
				MissionAnimation.Play();
				flag = false;
				flag2 = true;
				if (flag2)
                {
                    ClearAnimation.Play();
                    flag2 = false;
                }
			}


		//	gameObject.GetComponent<TransformScript>().enabled = false;
		}
        
	}


}
