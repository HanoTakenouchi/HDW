using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ModalScript : MonoBehaviour
{

	Animation SousaAnimation;
	GameObject SousaGroup;

	// Use this for initialization
	void Start()
	{
		SousaGroup = GameObject.Find("/CanvasGroup/SousaCanvas/SousaGroup");
		SousaAnimation = SousaGroup.gameObject.GetComponent<Animation>();
		SousaAnimation.Play();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.L))
		{
			SousaAnimation.gameObject.SetActive(false);
		}
	}
}
