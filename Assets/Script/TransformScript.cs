using UnityEngine;

public class TransformScript : MonoBehaviour
{
	float speed = 8.0f;
	public GameObject robo;
	//float step;
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.A))
		{
			robo.gameObject.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
			//step = speed * Time.deltaTime;
			//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90f, 0), step);

		}

		if (Input.GetKey(KeyCode.D))
		{
			robo.gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
		}

		if (Input.GetKey(KeyCode.W))
		{
			robo.gameObject.transform.position += new Vector3(0, 0, speed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.S))
		{
			robo.gameObject.transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
		}
	}
}
		


