using UnityEngine;
using TMPro;


/// <summary>
/// TextEventHandlerの使い方サンプル
/// </summary>
[RequireComponent(typeof(TextEventHandler))]
public class TextEventReceiver : MonoBehaviour
{
	private TextEventHandler textEventHandler;

	private void Awake()
	{
		textEventHandler = GetComponent<TextEventHandler>();
	}

	void OnEnable()
	{
		if (textEventHandler != null)
		{
			textEventHandler.onCharacterSelection.AddListener(OnCharacterSelection);
			textEventHandler.onWordSelection.AddListener(OnWordSelection);
			textEventHandler.onLineSelection.AddListener(OnLineSelection);
			textEventHandler.onLinkSelection.AddListener(OnLinkSelection);
		}
	}

	void OnDisable()
	{
		if (textEventHandler != null)
		{
			textEventHandler.onCharacterSelection.RemoveListener(OnCharacterSelection);
			textEventHandler.onWordSelection.RemoveListener(OnWordSelection);
			textEventHandler.onLineSelection.RemoveListener(OnLineSelection);
			textEventHandler.onLinkSelection.RemoveListener(OnLinkSelection);
		}
	}

	void OnCharacterSelection(char c, int index)
	{
		Debug.Log("Character [" + c + "] at Index: " + index + " has been selected.");
	}

	void OnWordSelection(string word, int firstCharacterIndex, int length)
	{
		Debug.Log("Word [" + word + "] with first character index of " + firstCharacterIndex + " and length of " + length + " has been selected.");
	}

	void OnLineSelection(string lineText, int firstCharacterIndex, int length)
	{
		Debug.Log("Line [" + lineText + "] with first character index of " + firstCharacterIndex + " and length of " + length + " has been selected.");
	}

	void OnLinkSelection(string linkID, string linkText, int linkIndex)
	{
		Debug.Log("Link Index: " + linkIndex + " with ID [" + linkID + "] and Text \"" + linkText + "\" has been selected.");
	}

}
