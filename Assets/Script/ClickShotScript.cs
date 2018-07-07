using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickShotScript : MonoBehaviour {
	public Camera camera;
	public GameObject bullet;
    public Transform muzzle;
    public float speed = 50;

	// Use this for initialization
	void Start () 
	{
		
	}

	// Update is called once per frame
	void Update () 
	{
		Debug.Log("Click1");
		ClickShot();
		Debug.Log("Click4");
	}

	void ClickShot()
	{
		int distance = 50;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo, distance))
		{
			if (Input.GetMouseButtonDown(0))
			{
				GameObject bullets = Instantiate(bullet) as GameObject;

				Vector3 force = hitInfo.point;
				// Rigidbodyに力を加えて発射
				bullets.GetComponent<Rigidbody>().AddForce(force);
				// 弾丸の位置を調整
				bullets.transform.position = muzzle.position;

				DenkiManager.CreateNumbers -= 1;   
			}
			Debug.DrawRay(ray.origin, hitInfo.point, Color.red);
    }   
		}

}
