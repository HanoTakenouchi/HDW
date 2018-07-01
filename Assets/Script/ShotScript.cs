using UnityEngine;
using System.Collections;

public class ShotScript : MonoBehaviour
{

	// bullet prefab
	public GameObject bullet;

	// 弾丸発射点
	public Transform muzzle;

	// 弾丸の速度
	public float speed = 20;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		OnClick();
	}

	void OnClick()
	{
		if (Input.GetMouseButton(0))
		{
			if (DenkiManager.CreateNumbers >= 1)
			{
				GameObject bullets = Instantiate(bullet) as GameObject;

				Vector3 force = Input.mousePosition;
				// Rigidbodyに力を加えて発射
				bullets.GetComponent<Rigidbody>().AddForce(force);
				// 弾丸の位置を調整
				bullets.transform.position = muzzle.position;

				DenkiManager.CreateNumbers -= 1;            
			}

			if (DenkiManager.CreateNumbers <= 0)
			{
				GetComponent<ShotScript>().enabled = false;
			}
		}

	}
}


