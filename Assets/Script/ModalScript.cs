using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ModalScript : MonoBehaviour
{

	Animation SousaGroupAnimation;
	Animation Sousa2GroupAnimation;
	Animation Sousa3GroupAnimation;
   
	public GameObject SousaGroup;
	public GameObject Sousa2Group;
	public GameObject Sousa3Group;
	public GameObject Robo;
	private bool flag;
	private bool flag2;
	private bool flag3;
	private bool flag4;
	private bool flag5;
	private bool flag6;
   

	// Use this for initialization
	void Start()
	{
		SousaGroupAnimation = SousaGroup.gameObject.GetComponent<Animation>();
		Sousa2GroupAnimation = Sousa2Group.gameObject.GetComponent<Animation>();
		Sousa3GroupAnimation = Sousa3Group.gameObject.GetComponent<Animation>();
		Sousa2GroupAnimation.gameObject.SetActive(false);
		Sousa3GroupAnimation.gameObject.SetActive(false);
		flag = true;
		flag4 = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.E))
		{
			SousaGroupAnimation.gameObject.SetActive(false);
		}

		if (Robo.gameObject.transform.position.x <= 30)
		{
			if (flag)
			{
				Debug.Log("Sousa1");
				Sousa2GroupAnimation.gameObject.SetActive(true);
				flag = false;
				flag2 = true;
				Debug.Log("Sousa2");
			}
			if (flag2)
			{
				Sousa2GroupAnimation.Play();
				Debug.Log("Sousa3");
				flag2 = false;
				flag3 = true;
			}
			if (flag3)
			{
				if (Input.GetKey(KeyCode.E))
				{
					Debug.Log("Sousa4");
					Sousa2GroupAnimation.gameObject.SetActive(false);
					Debug.Log("Sousa5");
					flag3 = false;
				}
			}
		}

		if (Robo.gameObject.transform.position.x <= 14)
		{
			if (flag4)
			{
				Sousa3GroupAnimation.gameObject.SetActive(true);
				flag4 = false;
				flag5 = true;
			}
			if (flag5)
			{
				Sousa3GroupAnimation.Play();
				flag5 = false;
				flag6 = true;
			}
			if (flag6)
			{
				if (Input.GetKey(KeyCode.E))
				{
					Sousa3GroupAnimation.gameObject.SetActive(false);
					flag6 = false;
				}
			}

		}
	}
}

