using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

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
			StartCoroutine(CoTalk());
		}

		if (Robo.transform.position.x == -221)
		{
			scenarioLabel = "Bossmae";
            StartCoroutine(CoTalk());
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
