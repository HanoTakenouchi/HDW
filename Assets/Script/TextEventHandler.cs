using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using TMPro;


/// <summary>
/// TextMesh Pro/Examples/Script/TMP_TextEventHandler.csをスマホ用に改造したもの
/// </summary>
public class TextEventHandler: MonoBehaviour
{
	[Serializable]
	public class CharacterSelectionEvent : UnityEvent<char, int> { }

	[Serializable]
	public class WordSelectionEvent : UnityEvent<string, int, int> { }

	[Serializable]
	public class LineSelectionEvent : UnityEvent<string, int, int> { }

	[Serializable]
	public class LinkSelectionEvent : UnityEvent<string, string, int> { }

	/// <summary>
	/// Event delegate triggered when pointer is over a character.
	/// </summary>
	public CharacterSelectionEvent onCharacterSelection
	{
		get { return m_OnCharacterSelection; }
		set { m_OnCharacterSelection = value; }
	}
	[SerializeField]
	private CharacterSelectionEvent m_OnCharacterSelection = new CharacterSelectionEvent();

	/// <summary>
	/// Event delegate triggered when pointer is over a word.
	/// </summary>
	public WordSelectionEvent onWordSelection
	{
		get { return m_OnWordSelection; }
		set { m_OnWordSelection = value; }
	}
	[SerializeField]
	private WordSelectionEvent m_OnWordSelection = new WordSelectionEvent();


	/// <summary>
	/// Event delegate triggered when pointer is over a line.
	/// </summary>
	public LineSelectionEvent onLineSelection
	{
		get { return m_OnLineSelection; }
		set { m_OnLineSelection = value; }
	}
	[SerializeField]
	private LineSelectionEvent m_OnLineSelection = new LineSelectionEvent();


	/// <summary>
	/// Event delegate triggered when pointer is over a link.
	/// </summary>
	public LinkSelectionEvent onLinkSelection
	{
		get { return m_OnLinkSelection; }
		set { m_OnLinkSelection = value; }
	}
	[SerializeField]
	private LinkSelectionEvent m_OnLinkSelection = new LinkSelectionEvent();



	private TMP_Text m_TextComponent;

	private Camera m_Camera;
	private Canvas m_Canvas;

	private int m_selectedLink = -1;
	private int m_lastCharIndex = -1;
	private int m_lastWordIndex = -1;
	private int m_lastLineIndex = -1;



	void Awake()
	{
		// Get a reference to the text component.
		m_TextComponent = GetComponent<TMP_Text>();

		// Get a reference to the camera rendering the text taking into consideration the text component type.
		if (m_TextComponent.GetType() == typeof(TextMeshProUGUI))
		{
			m_Canvas = GetComponentInParent<Canvas>();
			if (m_Canvas != null)
			{
				if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
					m_Camera = null;
				else
					m_Camera = m_Canvas.worldCamera;
			}
		}
		else
		{
			m_Camera = Camera.main;
		}
	}


	void LateUpdate()
	{
		// タッチ座標とマウス座標の両方で機能させる
		var touchPosition = Input.touchCount <= 0 ?
			Input.mousePosition : (Vector3)Input.GetTouch(0).position;

		var touchDown = Input.touchCount <= 0 ? Input.GetMouseButtonDown(0) : true;

		// サンプルでマウス座標だったところをtouchPositionに置換
		if (TMP_TextUtilities.IsIntersectingRectTransform(m_TextComponent.rectTransform, touchPosition, m_Camera))
		{
			#region Example of Character Selection
			int charIndex = TMP_TextUtilities.FindIntersectingCharacter(m_TextComponent, touchPosition, m_Camera, true);
			if (charIndex != -1 && charIndex != m_lastCharIndex)
			{
				m_lastCharIndex = charIndex;

				// Send event to any event listeners.
				SendOnCharacterSelection(m_TextComponent.textInfo.characterInfo[charIndex].character, charIndex);
			}
			#endregion


			#region Example of Word Selection
			// Check if Mouse intersects any words and if so assign a random color to that word.
			int wordIndex = TMP_TextUtilities.FindIntersectingWord(m_TextComponent, touchPosition, m_Camera);
			if (wordIndex != -1 && wordIndex != m_lastWordIndex)
			{
				m_lastWordIndex = wordIndex;

				// Get the information about the selected word.
				TMP_WordInfo wInfo = m_TextComponent.textInfo.wordInfo[wordIndex];

				// Send the event to any listeners.
				SendOnWordSelection(wInfo.GetWord(), wInfo.firstCharacterIndex, wInfo.characterCount);
			}
			#endregion


			#region Example of Line Selection
			// Check if Mouse intersects any words and if so assign a random color to that word.
			int lineIndex = TMP_TextUtilities.FindIntersectingLine(m_TextComponent, touchPosition, m_Camera);
			if (lineIndex != -1 && lineIndex != m_lastLineIndex)
			{
				m_lastLineIndex = lineIndex;

				// Get the information about the selected word.
				TMP_LineInfo lineInfo = m_TextComponent.textInfo.lineInfo[lineIndex];

				// Send the event to any listeners.
				char[] buffer = new char[lineInfo.characterCount];
				for (int i = 0; i < lineInfo.characterCount && i < m_TextComponent.textInfo.characterInfo.Length; i++)
				{
					buffer[i] = m_TextComponent.textInfo.characterInfo[i + lineInfo.firstCharacterIndex].character;
				}

				string lineText = new string(buffer);
				SendOnLineSelection(lineText, lineInfo.firstCharacterIndex, lineInfo.characterCount);
			}
			#endregion


			#region Example of Link Handling
			// 入力があった時のみ、Linkとの当たり判定をとります
			if (touchDown)
			{
				// Check if mouse intersects with any links.
				int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextComponent, touchPosition, m_Camera);

				// Clear previous link selection if one existed.
				if ((linkIndex == -1 && m_selectedLink != -1) || linkIndex != m_selectedLink)
				{
					SendOnLinkSelection(string.Empty, string.Empty, linkIndex);
					m_selectedLink = -1;
				}

				// Handle new Link selection.
				if (linkIndex != -1 && linkIndex != m_selectedLink)
				{
					m_selectedLink = linkIndex;

					// Get information about the link.
					TMP_LinkInfo linkInfo = m_TextComponent.textInfo.linkInfo[linkIndex];

					// Send the event to any listeners. 
					SendOnLinkSelection(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkIndex);
				}
			}
			#endregion
		}
		// 範囲外をタップした時は、選択状態を解除します
		else
		{
			#region Example of Link Handling
			if (touchDown)
			{
				if (m_selectedLink != -1)
				{
					m_selectedLink = -1;
					SendOnLinkSelection(string.Empty, string.Empty, m_selectedLink);
				}
			}
			#endregion
		}
	}


	private void SendOnCharacterSelection(char character, int characterIndex)
	{
		if (onCharacterSelection != null)
			onCharacterSelection.Invoke(character, characterIndex);
	}

	private void SendOnWordSelection(string word, int charIndex, int length)
	{
		if (onWordSelection != null)
			onWordSelection.Invoke(word, charIndex, length);
	}

	private void SendOnLineSelection(string line, int charIndex, int length)
	{
		if (onLineSelection != null)
			onLineSelection.Invoke(line, charIndex, length);
	}

	private void SendOnLinkSelection(string linkID, string linkText, int linkIndex)
	{
		if (onLinkSelection != null)
			onLinkSelection.Invoke(linkID, linkText, linkIndex);
	}
}
