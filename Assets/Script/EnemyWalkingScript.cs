using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyWalkingScript : MonoBehaviour
{
	Animator EnemyAnimator;
	// Use this for initialization11
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			EnemyAnimator.SetInteger("AnimationInt",1);
		}
	}

}

