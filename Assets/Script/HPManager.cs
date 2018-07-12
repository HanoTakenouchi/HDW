using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour {
    
	public GameObject HP14;

    public GameObject HP13;
    
    public GameObject HP12;

    public GameObject HP11;
    
    public GameObject HP10;

    public GameObject HP9;
    
    public GameObject HP8;
    
    public GameObject HP7;
    
    public GameObject HP6;
    
    public GameObject HP5;

    public GameObject HP4;
    
    public GameObject HP3;

    public GameObject HP2;

    public GameObject HP1;

    public GameObject HP0;

    public static int HPNumbers = 14;

    // Use this for initialization
    void Start()
    {
		HP14.gameObject.SetActive(false);
		HP13.gameObject.SetActive(false);
        HP12.gameObject.SetActive(false);
        HP11.gameObject.SetActive(false);
        HP10.gameObject.SetActive(false);
        HP9.gameObject.SetActive(false);
        HP8.gameObject.SetActive(false);
        HP7.gameObject.SetActive(false);
        HP6.gameObject.SetActive(false);
        HP5.gameObject.SetActive(false);
        HP4.gameObject.SetActive(false);
        HP3.gameObject.SetActive(false);
        HP2.gameObject.SetActive(false);
        HP1.gameObject.SetActive(false);
        HP0.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
		if (HPNumbers == 14)
        {
            HP13.gameObject.SetActive(false);
			HP14.gameObject.SetActive(true);
        }
        
		if (HPNumbers== 13)
        {
            HP14.gameObject.SetActive(false);
            HP13.gameObject.SetActive(true);
            HP12.gameObject.SetActive(false);
        }
        
		if (HPNumbers == 12)
        {
            HP13.gameObject.SetActive(false);
            HP12.gameObject.SetActive(true);
            HP11.gameObject.SetActive(false);
        }
        
		if (HPNumbers == 11)
        {
            HP12.gameObject.SetActive(false);
            HP11.gameObject.SetActive(true);
            HP10.gameObject.SetActive(false);
        }
        
		if (HPNumbers == 10)
        {
            HP11.gameObject.SetActive(false);
            HP10.gameObject.SetActive(true);
            HP9.gameObject.SetActive(false);
        }
        
		if (HPNumbers == 9)
        {
            HP10.gameObject.SetActive(false);
            HP9.gameObject.SetActive(true);
            HP8.gameObject.SetActive(false);
        }

		if (HPNumbers == 8)
        {
            HP9.gameObject.SetActive(false);
            HP8.gameObject.SetActive(true);
            HP7.gameObject.SetActive(false);
        }
        
		if (HPNumbers == 7)
        {
            HP8.gameObject.SetActive(false);
            HP7.gameObject.SetActive(true);
            HP6.gameObject.SetActive(false);
        }

		if (HPNumbers == 6)
        {
            HP7.gameObject.SetActive(false);
            HP6.gameObject.SetActive(true);
            HP5.gameObject.SetActive(false);
        }

		if (HPNumbers == 5)
        {
            HP6.gameObject.SetActive(false);
            HP5.gameObject.SetActive(true);
            HP4.gameObject.SetActive(false);
        }
        
		if (HPNumbers == 4)
        {
            HP5.gameObject.SetActive(false);
            HP4.gameObject.SetActive(true);
            HP3.gameObject.SetActive(false);
        }

		if (HPNumbers == 3)
        {
            HP4.gameObject.SetActive(false);
            HP3.gameObject.SetActive(true);
            HP2.gameObject.SetActive(false);
        }

		if (HPNumbers == 2)
        {
            HP3.gameObject.SetActive(false);
            HP2.gameObject.SetActive(true);
            HP1.gameObject.SetActive(false);
        }
        
		if (HPNumbers == 1)
        {
            HP2.gameObject.SetActive(false);
            HP1.gameObject.SetActive(true);
            HP0.gameObject.SetActive(false);
        }

		if (HPNumbers == 0)
        {
            HP1.gameObject.SetActive(false);
            HP0.gameObject.SetActive(true);
        }
    }
}

