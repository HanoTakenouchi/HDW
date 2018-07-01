using UnityEngine;
using System.Collections;

public class Lesson1 : MonoBehaviour
{

	GameObject[] objectArray;

	int createNumbers;

	public GameObject Base0;
	public GameObject Base1;
	public GameObject Base2;
	public GameObject Base3;
	public GameObject Base4;
	public GameObject Base5;
	public GameObject Base6;
	public GameObject Base7;
	public GameObject Base8;
	public GameObject Base9;
	public GameObject Base10;
	public GameObject Base11;
	public GameObject Base12;
	public GameObject Base13;
	public GameObject Base14;
	public GameObject bullet;

	// 弾丸発射点
	public Transform muzzle;

	// 弾丸の速度
	public float speed = 20;

	//続く

	void Start()
	{
		GameObject[] objectArray = { Base0, Base1, Base2, Base3, Base4, Base5, Base6, Base7, Base8, Base9, Base10, Base11, Base12, Base13, Base14 };
		createNumbers = 14;
	}



	void Update()
	{

	}

	void OnClick()
	{
		if (Input.GetMouseButton(0))
		{
			Hoge(createNumbers);
		}
	}

	private void Hoge(int number)
	{
		int numBelow = number - 1;
		int numAbove = number + 1;

		if (createNumbers != 0)
		{
			objectArray[number].gameObject.SetActive(true);

			try
			{
				objectArray[numBelow].gameObject.SetActive(false);
				objectArray[numAbove].gameObject.SetActive(false);
			}
			catch
			{

			}

			createNumbers--;
		}
		else
		{
			GetComponent<ShotScript>().enabled = false;
		}
	}
}