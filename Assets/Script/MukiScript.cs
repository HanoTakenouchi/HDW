using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MukiScript : MonoBehaviour
{
	public float run_speed;
	public float smooth = 10;

	void Start()
	{

	}
    
	void Update()
	{
		Vector3 target_dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		//Run
		if (target_dir.magnitude > 0.01f)
		{
			//体の向きを変更
			Quaternion rotation = Quaternion.LookRotation(target_dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);
			//前方へ移動
			transform.Translate(Vector3.forward * Time.deltaTime * run_speed);
		}
	}
}