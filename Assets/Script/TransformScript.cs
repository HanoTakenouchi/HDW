using UnityEngine;

public class TransformScript : MonoBehaviour {
    float speed = 5.0f;
	public GameObject robo;
	//float step;
	// Use this for initialization
	void Start () {
	
	}
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
			robo.gameObject.transform.position += new Vector3(0, 0, speed * Time.deltaTime);
			//step = speed * Time.deltaTime;
			//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90f, 0), step);

        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
			robo.gameObject.transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
			robo.gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
			robo.gameObject.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }
       
    }
}
		


