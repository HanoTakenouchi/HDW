using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ModalScript : MonoBehaviour
{
    
	Animation SousaGroupAnimation;
	Animation Sousa2GroupAnimation;
	public GameObject SousaGroup;
	public GameObject Sousa2Group;
	public GameObject Robo;
	private bool flag;
	private bool flag2;
	private bool flag3;

	private void Awake()
	{
		
	}
	// Use this for initialization
	void Start()
	{
		SousaGroupAnimation = SousaGroup.gameObject.GetComponent<Animation>();
		Sousa2GroupAnimation = Sousa2Group.gameObject.GetComponent<Animation>();
		SousaGroupAnimation.Play();
		Sousa2GroupAnimation.gameObject.SetActive(false);
		flag = true;
		flag2 = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.L))
		{
			SousaGroupAnimation.gameObject.SetActive(false);
		}

		if (Robo.gameObject.transform.position.z >= -60)
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
                if (Input.GetKey(KeyCode.L))
                {
                    Debug.Log("Sousa4");
                    Sousa2GroupAnimation.gameObject.SetActive(false);
                    Debug.Log("Sousa5");
                    flag3 = false;
                }
            }
		}
	}
}
