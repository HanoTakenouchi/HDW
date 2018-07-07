using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPScript : MonoBehaviour
{

	public Slider slider;
	public int maxEnemyHP = 3;
	public GameObject SliderCanvas;
	public int enemyHP = 3;
	public GameObject Enemy;
	public Camera camera;
	public GameObject Cube;

	// Use this for initialization
	void Start()
	{
		slider.maxValue = maxEnemyHP;
		slider.value = enemyHP;
		Vector3 p = Cube.transform.position;
	    slider.transform.LookAt(p);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void Damage()
	{
		enemyHP -= 1;
		if (enemyHP == 0)
		{
			Destroy(Enemy.gameObject);
		}
	}
}
