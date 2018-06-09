using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    int enemyHP = 3;
	public GameObject effect;
    public float offset;
    public float deleteTime;
    public GameObject Robo;
    float minAngle = 0.0F;
    float maxAngle = 90.0F;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Damage(){
		var instantiateEffect = GameObject.Instantiate(effect, transform.position + new Vector3(0f, offset, 0f), Quaternion.identity) as GameObject;
        Destroy(instantiateEffect, deleteTime);
        float angle = Mathf.LerpAngle(minAngle, maxAngle, Time.time);
        transform.eulerAngles = new Vector3(0, angle, 0);
        enemyHP -= 1;
        if (enemyHP == 0){
            Destroy(this.gameObject);
        }
    }

}
