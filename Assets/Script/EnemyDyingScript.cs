using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDyingScript : MonoBehaviour {
	Animator EnemyAnimator;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
			EnemyAnimator.SetInteger("AnimationInt",2);
        }
    }

}
