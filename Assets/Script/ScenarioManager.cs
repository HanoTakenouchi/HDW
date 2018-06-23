using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

[RequireComponent(typeof(TextController))]
public class ScenarioManager : SingletonMonoBehaviourFast<ScenarioManager>
{

    public string LoadFileName;

    private string[] m_scenarios;
    private int m_currentLine = 0;
    private bool m_isCallPreload = false;

    private TextController m_textController;
    private CommandController m_commandController;

    void RequestNextLine()
    {
        var currentText = m_scenarios[m_currentLine];

        m_textController.SetNextLine(CommandProcess(currentText));
        m_currentLine++;
        m_isCallPreload = false;
    }

    public void UpdateLines(string fileName)
    {
        var scenarioText = Resources.Load<TextAsset>("Scenario/" + fileName);
		Debug.Log(fileName);

        if (scenarioText == null)
        {
            Debug.LogError("シナリオファイルが見つかりませんでした");
            Debug.LogError("ScenarioManagerを無効化します");
            enabled = false;
            return;
        }
        m_scenarios = scenarioText.text.Split(new string[] { "@br" }, System.StringSplitOptions.None);
        m_currentLine = 0;
		Debug.Log(scenarioText.text);
		Debug.Log("m_scenarios[0] = " + m_scenarios[0]);
		Debug.Log("m_scenarios[1] = " + m_scenarios[1]);
		Debug.Log("m_scenarios[2] = " + m_scenarios[2]);
		Debug.Log(m_scenarios.Length);

        Resources.UnloadAsset(scenarioText);
    }

    private string CommandProcess(string line)
    {
        var lineReader = new StringReader(line);
        var lineBuilder = new StringBuilder();
        var text = string.Empty;
        while ((text = lineReader.ReadLine()) != null)
        {
			Debug.Log(text);
            var commentCharacterCount = text.IndexOf("//");
            if (commentCharacterCount != -1)
            {
                text = text.Substring(0, commentCharacterCount);
            }

            if (!string.IsNullOrEmpty(text))
            {
                if (text[0] == '@' && m_commandController.LoadCommand(text))
                    continue;
                lineBuilder.AppendLine(text);
            }
        }

        return lineBuilder.ToString();
    }

    #region UNITY_CALLBACK

    void Start()
    {
        m_textController = GetComponent<TextController>();
        m_commandController = GetComponent<CommandController>();

        UpdateLines(LoadFileName);
        RequestNextLine();
    }

    void Update()
    {
        if (m_textController.IsCompleteDisplayText)
        {
            if (m_currentLine < m_scenarios.Length)
            {
                if (!m_isCallPreload)
                {
					Debug.Log("m_currentLine=" + m_currentLine);
					Debug.Log("m_currentLine[]=" + m_scenarios[m_currentLine]);

                    m_commandController.PreloadCommand(m_scenarios[m_currentLine]);
                    m_isCallPreload = true;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    RequestNextLine();
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_textController.ForceCompleteDisplayText();
            }
        }
    }

    #endregion
}