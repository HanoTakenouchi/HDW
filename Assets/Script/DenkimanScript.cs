using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
			if (Input.GetKeyDown("up"))
            {
                if (genzaichi.up != null)
                {
					transform.DOMove(genzaichi.up.transform.position,1);
                    genzaichi = genzaichi.up.GetComponent<PointScript>();
                }
            }

            if (Input.GetKeyUp("down"))
            {
                if (genzaichi.down != null)
                 {
					transform.DOMove(genzaichi.down.transform.position, 1);
                    genzaichi = genzaichi.down.GetComponent<PointScript>();
                 }
             }

            if (Input.GetKeyUp("left"))
             {
                if (genzaichi.left != null)
                {
					transform.DOMove(genzaichi.left.transform.position, 1);
                    genzaichi = genzaichi.left.GetComponent<PointScript>();
                }
            }

            if (Input.GetKeyUp("right"))
            {
                if (genzaichi.right != null)
                {
					transform.DOMove(genzaichi.right.transform.position, 1);
                    genzaichi = genzaichi.right.GetComponent<PointScript>();
                }
            }
        }
     }
}

