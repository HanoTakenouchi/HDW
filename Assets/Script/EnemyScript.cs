using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour {

    int enemyHP = 1;
    public GameObject Robo;
	NavMeshAgent agent;
	public Transform target; 

    // 弾丸の速度
	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		
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
			agent.SetDestination(target.position);
		}
	}
}