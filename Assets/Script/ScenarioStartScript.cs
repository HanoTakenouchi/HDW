using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class ScenarioStartScript : MonoBehaviour {

	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
    public AdvEngine engine;
	private string scenarioLabel;
	public static bool Scenarioflag;

	private void Awake()
	{
		Scenarioflag = true;
		scenarioLabel = "1mae";
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(CoTalk());
	}
	
	// Update is called once per frame
	void Update () {
		
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
