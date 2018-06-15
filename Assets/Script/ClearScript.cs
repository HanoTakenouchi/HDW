using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClearScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{

		this.GetComponent<TextMeshProUGUI>();
		DOScale(1.2f, 1.0f);
		SetLoops(-1, LoopType.Yoyo);
        Play();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}


}
