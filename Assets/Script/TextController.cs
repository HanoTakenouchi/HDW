using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Messageboxのテキストを制御する
public class TextController : MonoBehaviour
{

    [SerializeField]
    [Range(0.001f, 0.3f)]
    public float intervalForCharacterDisplay = 0.05f;

    private string currentText = string.Empty;
    private float timeUntilDisplay = 0;
    private float timeElapsed = 1;
    private int lastUpdateCharacter = -1;

    [SerializeField]
    private Text _uiText;

    public bool IsCompleteDisplayText
    {
        get { return Time.time > timeElapsed + timeUntilDisplay; }
    }

    // 強制的に全文表示する
    public void ForceCompleteDisplayText()
    {
        timeUntilDisplay = 0;
    }

    // 次に表示する文字列をセットする
    public void SetNextLine(string text)
    {
        currentText = text;
        timeUntilDisplay = currentText.Length * intervalForCharacterDisplay;
        timeElapsed = Time.time;
        lastUpdateCharacter = -1;
    }

    #region UNITY_CALLBACK  

    void Update()
    {
        int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * currentText.Length);
        if (displayCharacterCount != lastUpdateCharacter)
        {
            _uiText.text = currentText.Substring(0, displayCharacterCount);
            lastUpdateCharacter = displayCharacterCount;
        }
    }

    #endregion
}