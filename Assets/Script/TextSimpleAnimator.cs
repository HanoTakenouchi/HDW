using UnityEngine;
using System.Collections;


/// <summary>
/// 文字送りアニメーション
/// </summary>
public class TextSimpleAnimator : MonoBehaviour
{
	/// <summary>
	/// アニメーション中かどうか
	/// </summary>
	[System.NonSerialized]
	public bool isAnimating;

	/// <summary>
	/// 1文字あたりの表示速度
	/// </summary>
	public float speedPerCharacter = 0.1f;
	
	/// <summary>
	/// 自動再生
	/// </summary>
	[SerializeField]
	private bool isAuto = false;
	
	/// <summary>
	/// TextMeshPro
	/// </summary>
	private TMPro.TextMeshProUGUI text;
	
	/// <summary>
	/// 稼働中のコルーチン
	/// </summary>
	private IEnumerator coroutine;


	/// <summary>
	/// Unity Event Awake
	/// </summary>
	private void Awake()
	{
		text = GetComponent<TMPro.TextMeshProUGUI>();
	}

	/// <summary>
	/// Unity Event Start
	/// </summary>
	private void Start()
	{
		if (isAuto) { Play(); }
	}

	/// <summary>
	/// Unity Event OnEnable
	/// </summary>
	private void OnEnable()
	{
		if (isAuto) { Play(); }
	}

	/// <summary>
	/// アニメーション再生開始
	/// </summary>
	public void Play()
	{
		isAnimating = true;
		if (coroutine != null) { StopCoroutine(coroutine); }
		StartCoroutine(coroutine = AnimationCoroutine());
	}

	/// <summary>
	/// アニメーション強制終了
	/// </summary>
	public void Finish()
	{
		isAnimating = false;
		if (coroutine != null) { StopCoroutine(coroutine); }
		var textInfo = text.textInfo;
		var visibleCharacters = textInfo.characterCount;
		text.maxVisibleCharacters = visibleCharacters;
	}

	/// <summary>
	/// アニメーションコルーチン
	/// </summary>
	private IEnumerator AnimationCoroutine()
	{
		isAnimating = true;
		text.ForceMeshUpdate();

		var textInfo = text.textInfo;
		var visibleCharacters = textInfo.characterCount;

		float time = 0.0f;
		float maxTime = visibleCharacters * speedPerCharacter;
		while (time < maxTime)
		{
			text.maxVisibleCharacters = Mathf.FloorToInt(time / speedPerCharacter);
			yield return null;
			time += Time.deltaTime;
		}

		text.maxVisibleCharacters = visibleCharacters;
		isAnimating = false;
		yield break;
	}
}
