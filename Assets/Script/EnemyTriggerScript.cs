using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyTriggerScript : MonoBehaviour
{

	public GameObject Robo;
	public Transform target;
	public GameObject bullet;
	public GameObject muzzle;
	public GameObject Groove;
	public Animator EnemyAnimator;
	public NavMeshAgent agent;


	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			agent.SetDestination(target.position);

			EnemyAnimator.SetInteger("AnimationInt", 1);

			Groove.GetComponent<SpriteRenderer>().color = Color.red;

			GameObject bullets = Instantiate(bullet) as GameObject;

			Vector3 force = Robo.gameObject.transform.position;

			bullets.GetComponent<Rigidbody>().AddForce(force);

			bullets.transform.position = muzzle.transform.position;

		}

	}
    
	public void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			EnemyAnimator.SetInteger("AnimationInt", 0);
			Groove.GetComponent<SpriteRenderer>().color = Color.yellow;
		}

	}
}
