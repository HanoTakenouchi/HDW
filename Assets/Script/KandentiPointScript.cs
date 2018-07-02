using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class KandentiPointScript : MonoBehaviour
{
    
    Animation Sousa5GroupAnimation;
    public GameObject Sousa5Group;
    private bool flag10;
    private bool flag11;
    private bool flag12;

    // Use this for initialization
    void Start()
    {
		Sousa5GroupAnimation = Sousa5Group.gameObject.GetComponent<Animation>();
        Sousa5GroupAnimation.gameObject.SetActive(false);
		flag10 = true;
		flag11 = false;
		flag12 = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (flag10)
            {
                Sousa5GroupAnimation.gameObject.SetActive(true);
                flag10 = false;
                flag11 = true;
            }
            if (flag11)
            {
                Sousa5GroupAnimation.Play();
                flag11 = false;
                flag12 = true;
            }
            if (flag12)
            {
				if (Input.GetKey(KeyCode.E))
                {
                    Sousa5GroupAnimation.gameObject.SetActive(false);
                    flag12 = false;
                }
            }
        }
    }
}
