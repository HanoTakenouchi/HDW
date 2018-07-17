using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyWalkingScript : MonoBehaviour {
	
	public GameObject Gate;
	Animator EnemyAnimator;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "bullet")
        {
            EnemyAnimator.SetBool("Walking", false);
            EnemyAnimator.SetBool("Dying", true);
        }
    }

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "gate")
		{
			EnemyAnimator.SetBool("Walking", true);
		}
	}
}
