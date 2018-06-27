using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyScript : MonoBehaviour {

    int enemyHP = 1;
    public GameObject Robo;
	NavMeshAgent agent;
	public Transform target; 
	public GameObject bullet;
    public Transform muzzle;
    public float speed = 50;
	public GameObject Groove;

    // 弾丸の速度
	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    void Damage(){
        enemyHP -= 1;
        if (enemyHP == 0){
            Destroy(this.gameObject);
        }
    }

	private void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			Groove.GetComponent<SpriteRenderer>().color = Color.red;

			agent.SetDestination(target.position);

			GameObject bullets = Instantiate(bullet) as GameObject;

			Vector3 force = Robo.gameObject.transform.position;

            bullets.GetComponent<Rigidbody>().AddForce(force);

            bullets.transform.position = muzzle.position;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		Groove.GetComponent<SpriteRenderer>().color = Color.cyan;
	}
}