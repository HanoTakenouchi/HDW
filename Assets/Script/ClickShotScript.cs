using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickShotScript : MonoBehaviour
{
	public GameObject bullet;
	public GameObject test;
	public Transform muzzle;
	public float speed;
	public Camera gameCamera;
    
	// Use this for initialization
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{

			//ここから
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.Log(Camera.main.transform.position);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				Vector3 targetPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
				//ここまで*/


				GameObject bullets = Instantiate(bullet) as GameObject;

				bullets.transform.position =  muzzle.position;

				//Vector2 touchScreenPosition = Input.mousePosition;

				//Vector3 touchWorldPosition = gameCamera.ScreenToWorldPoint(touchScreenPosition);


				bullets.GetComponent<Rigidbody>().AddForce((targetPos - muzzle.position).normalized * speed);
			}
		}
	}
}
