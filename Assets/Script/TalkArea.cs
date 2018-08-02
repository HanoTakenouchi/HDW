using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

public class TalkArea : MonoBehaviour {

	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
    public AdvEngine engine;
    public string scenarioLabel;


    void Start()
    {
        StartCoroutine(CoTalk());
    }

    IEnumerator CoTalk()
    {
        //「宴」のシナリオを呼び出す
        Engine.JumpScenario(scenarioLabel);

        //「宴」のシナリオ終了待ち
        while (!Engine.IsEndScenario)
        {
            yield return 0;
        }
    }

	public void Clear()
    {
        engine.Param.GetParameter("flag1");
        engine.Param.TrySetParameter("flag1", true);
    }

}