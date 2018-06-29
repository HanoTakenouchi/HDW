using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KandentiScript : MonoBehaviour {

    public GameObject Base;
	public GameObject Denti;
	public GameObject MarkOmote;
	public GameObject MarkUra;
	public Material Grey;
    
    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        Base.gameObject.transform.Rotate(0,0,90*Time.deltaTime);
    }

	private void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			if (Input.GetKeyDown(KeyCode.Z)){
				Debug.Log("Kan1");
				MarkOmote.GetComponent<MeshRenderer>().material = Grey;
				DenkiManager.CreateNumbers++;
				Debug.Log("kan2");
			}
		}
	}
}
