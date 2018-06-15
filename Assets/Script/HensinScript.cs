using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HensinScript : MonoBehaviour
{

	public Transform muzzle2;

	public GameObject prehabdenkiman;

	public GameObject copydenkiman;

	bool isdenkiman;

	public GameObject Camera;

	public GameObject Robo;

	bool isPoint = true;
	//public Collider collider;

	public GameObject MainCam;

	public GameObject SubCam;

	public GameObject ZZZ;

	public GameObject eye;


	// Use this for initialization
	void Start()
	{
		isdenkiman = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.O))
        {
            Debug.Log("kireru1");
            isdenkiman = (false);
            Debug.Log("1");
            Destroy(copydenkiman);
            Debug.Log("2");
            gameObject.GetComponent<TransformScript>().enabled = true;
            MainCam.SetActive(true);
            SubCam.SetActive(false);
            Camera.transform.SetParent(Robo.transform);
			ZZZ.gameObject.SetActive(false);
			eye.gameObject.SetActive(true);
        }
	}

	private void OnTriggerStay(Collider col)
	{
		//Debug.Log(col.gameObject.tag);
		if (Input.GetKeyDown(KeyCode.P) && col.gameObject.tag == "point")
		{
			Debug.Log("change");
			isPoint = false;
			if (isdenkiman == false)
			{
				copydenkiman = Instantiate(prehabdenkiman, muzzle2.position, transform.rotation) as GameObject;
				isdenkiman = true;
				gameObject.GetComponent<TransformScript>().enabled = false;
				copydenkiman.GetComponent<DenkimanScript>().genzaichi = col.gameObject.GetComponent<PointScript>();
				MainCam.SetActive(false);
				SubCam.SetActive(true);
				eye.gameObject.SetActive(false);
				ZZZ.gameObject.SetActive(true);
				Debug.Log("kiteru");

			}
		}
	}
}

