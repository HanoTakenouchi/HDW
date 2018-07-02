using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KandentiScript : MonoBehaviour {

    public GameObject Base;
	public GameObject Denti;
	public GameObject MarkOmote;
	public GameObject MarkUra;
	public Material Grey;
	public bool flag;
    
    // Use this for initialization
    void Start () {
		flag = true;
    }
    
    // Update is called once per frame
    void Update () {
        Base.gameObject.transform.Rotate(0,0,90*Time.deltaTime);
    }
    
	private void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			KanOnclick();
		}
	}

	void KanOnclick()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (DenkiManager.CreateNumbers < 14)
			{
				if (flag == true)
				{
					MarkOmote.GetComponent<MeshRenderer>().material = Grey;
					DenkiManager.CreateNumbers += 3;
					Debug.Log(DenkiManager.CreateNumbers);
					flag = false;
				}
			}
		}
	}
}
