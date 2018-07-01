using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DenkiManager : MonoBehaviour
{

	public GameObject Base14;

	public GameObject Base13;

	public GameObject Base12;

	public GameObject Base11;

	public GameObject Base10;

	public GameObject Base9;

	public GameObject Base8;

	public GameObject Base7;

	public GameObject Base6;

	public GameObject Base5;

	public GameObject Base4;

	public GameObject Base3;

	public GameObject Base2;

	public GameObject Base1;

	public GameObject Base0;

	public static int CreateNumbers = 14;

	// Use this for initialization
	void Start()
	{
		Base14.gameObject.SetActive(false);
		Base13.gameObject.SetActive(false);
		Base12.gameObject.SetActive(false);
		Base11.gameObject.SetActive(false);
		Base10.gameObject.SetActive(false);
		Base9.gameObject.SetActive(false);
		Base8.gameObject.SetActive(false);
		Base7.gameObject.SetActive(false);
		Base6.gameObject.SetActive(false);
		Base5.gameObject.SetActive(false);
		Base4.gameObject.SetActive(false);
		Base3.gameObject.SetActive(false);
		Base2.gameObject.SetActive(false);
		Base1.gameObject.SetActive(false);
		Base0.gameObject.SetActive(false);

	}

	// Update is called once per frame
	void Update()
	{
		if (CreateNumbers == 14)
        {
			Base13.gameObject.SetActive(false);
            Base14.gameObject.SetActive(true);
        }
        
		if (CreateNumbers == 13)
        {
			Base14.gameObject.SetActive(false);
            Base13.gameObject.SetActive(true);
            Base12.gameObject.SetActive(false);
        }
        
		if (CreateNumbers == 12)
		{
			Base13.gameObject.SetActive(false);
            Base12.gameObject.SetActive(true);
            Base11.gameObject.SetActive(false);
		}

		if (CreateNumbers == 11)
		{
			Base12.gameObject.SetActive(false);
			Base11.gameObject.SetActive(true);
			Base10.gameObject.SetActive(false);
		}
        
		if (CreateNumbers == 10)
		{
			Base11.gameObject.SetActive(false);
			Base10.gameObject.SetActive(true);
			Base9.gameObject.SetActive(false);
		}

		if (CreateNumbers == 9)
		{
			Base10.gameObject.SetActive(false);
			Base9.gameObject.SetActive(true);
			Base8.gameObject.SetActive(false);
		}

		if (CreateNumbers == 8)
		{
			Base9.gameObject.SetActive(false);
			Base8.gameObject.SetActive(true);
			Base7.gameObject.SetActive(false);
		}

		if (CreateNumbers == 7)
		{
			Base8.gameObject.SetActive(false);
			Base7.gameObject.SetActive(true);
			Base6.gameObject.SetActive(false);
		}

		if (CreateNumbers == 6)
		{
			Base7.gameObject.SetActive(false);
			Base6.gameObject.SetActive(true);
			Base5.gameObject.SetActive(false);
		}

		if (CreateNumbers == 5)
		{
			Base6.gameObject.SetActive(false);
			Base5.gameObject.SetActive(true);
			Base4.gameObject.SetActive(false);
		}

		if (CreateNumbers == 4)
		{
			Base5.gameObject.SetActive(false);
			Base4.gameObject.SetActive(true);
			Base3.gameObject.SetActive(false);
		}

		if (CreateNumbers == 3)
		{
			Base4.gameObject.SetActive(false);
			Base3.gameObject.SetActive(true);
			Base2.gameObject.SetActive(false);
		}

		if (CreateNumbers == 2)
		{
			Base3.gameObject.SetActive(false);
			Base2.gameObject.SetActive(true);
			Base1.gameObject.SetActive(false);
		}

		if (CreateNumbers == 1)
		{
			Base2.gameObject.SetActive(false);
			Base1.gameObject.SetActive(true);
			Base0.gameObject.SetActive(false);
		}

		if (CreateNumbers == 0)
		{
			Base1.gameObject.SetActive(false);
			Base0.gameObject.SetActive(true);
		}
	}
}


