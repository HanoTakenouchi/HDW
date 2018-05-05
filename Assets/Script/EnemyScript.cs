using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    int enemyHP = 3;

	// Use this for initialization
	void Start () {
		
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

}
