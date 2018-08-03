using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTriggerScript : MonoBehaviour {

	NavMeshAgent agent;
    public Transform target;
    public GameObject Robo;
    public GameObject bullet;
    public GameObject muzzle;
    public GameObject Groove;


	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {

            agent.SetDestination(target.position);

            Groove.GetComponent<SpriteRenderer>().color = Color.red;

            agent.SetDestination(target.position);

            GameObject bullets = Instantiate(bullet) as GameObject;

            Vector3 force = Robo.gameObject.transform.position;

            bullets.GetComponent<Rigidbody>().AddForce(force);

            bullets.transform.position = muzzle.transform.position;
        }

    }
}
