using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickShotScript : MonoBehaviour
{
	public Camera camera;
	public GameObject bullet;
	public GameObject test;
	public Transform muzzle;
	public float speed = 50;

	// Use this for initialization
	void Start()
	{
		test = new GameObject();
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log("Click1");
		ClickShot();
		//Debug.Log("Click4");
	}

	void ClickShot()
	{
		int distance = 100;
		Vector3 pos = Input.mousePosition + camera.transform.forward * -10;
		pos = camera.ScreenToWorldPoint(pos);

		Ray ray = new Ray(pos, camera.gameObject.transform.forward);

		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo, distance))
		{
			//Debug.Log(hitInfo.collider.name);
			if (Input.GetMouseButtonDown(0))
			{
				GameObject bullets = Instantiate(bullet) as GameObject;


				//	Vector3 force = hitInfo.point;
				//	// Rigidbodyに力を加えて発射
				//	bullets.GetComponent<Rigidbody>().AddForce(force);
				//	// 弾丸の位置を調整
				//bullets.transform.position = muzzle.position;
				bullets.transform.position = hitInfo.point;

				//	DenkiManager.CreateNumbers -= 1;   
			}
			Debug.DrawRay(pos, camera.gameObject.transform.forward, Color.red);
		}

	}
}
