using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HensinScript : MonoBehaviour
{

	public Transform muzzle2;

	public GameObject prehabdenkiman;

	public GameObject copydenkiman;

	public bool isdenkiman;

	public GameObject Camera;


	// Use this for initialization
	void Start()
	{
		isdenkiman = false;
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerStay(Collider collider)
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (collider.gameObject.tag == "point")
			{
				if (isdenkiman == false)
				{
					copydenkiman = Instantiate(prehabdenkiman, muzzle2.position, transform.rotation) as GameObject;
					isdenkiman = true;
					gameObject.GetComponent<TransformScript>().enabled = false;
					copydenkiman.GetComponent<DenkimanScript>().genzaichi = collider.gameObject.GetComponent<PointScript>();
					Camera.transform.parent = null;
					Camera.transform.SetParent(copydenkiman.transform);
				}

			}
			else
			{
				if (copydenkiman.GetComponent<DenkimanScript>().genzaichi.gameObject == collider.gameObject)
				{
					isdenkiman = (false);
					Destroy(copydenkiman);
					gameObject.GetComponent<TransformScript>().enabled = true;
				}
			}
		}
	}
}


