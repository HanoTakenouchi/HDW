using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPScript : MonoBehaviour {

	public int HP = 14;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision collision)
	{
		collision.gameObject.tag = "Enemy";
		HP -= 1;
		HPManager.HPNumbers -= 1;
	}
}
