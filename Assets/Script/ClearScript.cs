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
	public GameObject ClearCanvas;
    
	// Use this for initialization
	void Start () 
	{
		MissionAnimation = Mission.gameObject.GetComponent<Animation>();
		ClearAnimation = Clear.gameObject.GetComponent<Animation>();
		ClearCanvas.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Robo.transform.position.z >= 38)
		{
			Debug.Log("clear1");
			ClearCanvas.gameObject.SetActive(true);
			Debug.Log("clear2");
			MissionAnimation.Play();
			Debug.Log("clear3");
            ClearAnimation.Play();
			Debug.Log("clear4");
		}

	}


}
