using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class CommandJumpNextScenario : ICommand
{
    public string Tag
    {
        get { return "jump"; }
    }

    public void Command(Dictionary<string, string> command)
    {
        var scenario = ScenarioManager.Instance;
        var fileName = command["fileName"];
        scenario.UpdateLines(fileName);
    }
}