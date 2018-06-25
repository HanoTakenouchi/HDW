using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript: MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void KaigiClick()
	{
		SceneManager.LoadScene("Main");
	}

	public void KenkyuuClick()
	{
		SceneManager.LoadScene("");
	}

	public void DataClick()
	{
		SceneManager.LoadScene("");
	}

	public void AIClick()
	{
		SceneManager.LoadScene("");
	}

	public void ModoruClick()
	{
		SceneManager.LoadScene("Title");
	}

}
