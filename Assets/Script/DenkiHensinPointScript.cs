using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class DenkiHensinPointScript : MonoBehaviour {

	Animation Sousa4GroupAnimation;
	public GameObject Sousa4Group;
	private bool flag7;
    private bool flag8;
    private bool flag9;

	// Use this for initialization
	void Start () {
		Sousa4GroupAnimation = Sousa4Group.gameObject.GetComponent<Animation>();
		Sousa4GroupAnimation.gameObject.SetActive(false);
		flag7 = true;
		flag8 = false;
		flag9 = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (flag7)
            {
                Sousa4GroupAnimation.gameObject.SetActive(true);
                flag7 = false;
                flag8 = true;
            }
            if (flag8)
            {
                Sousa4GroupAnimation.Play();
                flag8 = false;
                flag9 = true;
            }
            if (flag9)
            {
				if (Input.GetKey(KeyCode.E))
                {
                    Sousa4GroupAnimation.gameObject.SetActive(false);
                    flag9 = false;
                }
            }
        }
    }
}
