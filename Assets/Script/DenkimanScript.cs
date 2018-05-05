//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class DenkimanScript : MonoBehaviour
{

    public PointScript genzaichi;

    // Use this for initialization
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        {
            if (Input.GetKeyUp("up"))
            {
                if (genzaichi.up != null)
                {
                    transform.position = genzaichi.up.transform.position;
                    genzaichi = genzaichi.up.GetComponent<PointScript>();
                }
            }

            if (Input.GetKeyUp("down"))
            {
                if (genzaichi.down != null)
                 {
                    transform.position = genzaichi.down.transform.position;
                    genzaichi = genzaichi.down.GetComponent<PointScript>();
                 }
             }

            if (Input.GetKeyUp("left"))
             {
                if (genzaichi.left != null)
                {
                    transform.position = genzaichi.left.transform.position;
                    genzaichi = genzaichi.left.GetComponent<PointScript>();
                }
            }

            if (Input.GetKeyUp("right"))
            {
                if (genzaichi.right != null)
                {
                    transform.position = genzaichi.right.transform.position;
                    genzaichi = genzaichi.right.GetComponent<PointScript>();
                }
            }
        }
     }
}

