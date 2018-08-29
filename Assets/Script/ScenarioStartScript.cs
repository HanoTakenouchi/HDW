using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;
using UnityEngine.SceneManagement;

public class ScenarioStartScript : MonoBehaviour {

	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
    public AdvEngine engine;
	private string scenarioLabel;
	public static bool Scenarioflag;
	public GameObject Robo;

	private void Awake()
	{
		Scenarioflag = true;
		scenarioLabel = "Boss";
	}

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(CoTalk());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Robo.transform.position.x == -121)
		{
			scenarioLabel = "1temae";
			Scenarioflag = true;
			StartCoroutine(CoTalk());
		}

		if (Robo.transform.position.x == -220)
		{
			scenarioLabel = "Bossmae";
			Scenarioflag = true;
            StartCoroutine(CoTalk());
		}

		if(Robo.transform.position.x == -240)
		{
			scenarioLabel = "Boss";
            Scenarioflag = true;
			StartCoroutine(CoTalk());
			SceneManager.LoadScene("Title");

		}
	}

	IEnumerator CoTalk()
    {
		if (Scenarioflag == true)
		{
			Engine.JumpScenario(scenarioLabel);
          
			while (!Engine.IsEndScenario)
			{
                yield return 0;
            }
			Scenarioflag = false;
		}
    }
}
