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

	public GameObject Robo;

	public Collider collider;


	// Use this for initialization
	void Start()
	{
		isdenkiman = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P) && GetComponent<Collider>() != null)
		{
			Debug.Log("kireru??");
			if (GetComponent<Collider>().gameObject.tag == "point")
			{
				if (isdenkiman == false)
				{
					copydenkiman = Instantiate(prehabdenkiman, muzzle2.position, transform.rotation) as GameObject;
					isdenkiman = true;
					gameObject.GetComponent<TransformScript>().enabled = false;
					copydenkiman.GetComponent<DenkimanScript>().genzaichi = GetComponent<Collider>().gameObject.GetComponent<PointScript>();
					Debug.Log("kireru");
					Camera.transform.SetParent(copydenkiman.transform);
				}
				else
				{
					Debug.Log("kireru1");
					if (copydenkiman.GetComponent<DenkimanScript>().genzaichi.gameObject == GetComponent<Collider>().gameObject)
					{
						Debug.Log("kireru2");
						isdenkiman = (false);
						Destroy(copydenkiman);
						gameObject.GetComponent<TransformScript>().enabled = true;
						Camera.transform.SetParent(Robo.transform);
					}
				}
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		this.collider = collider;
	}

	private void OnTriggerExit(Collider collider)
	{
		this.collider = null;
	}
}


