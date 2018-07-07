using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// ぺージ制御
/// </summary>
public class PageController : MonoBehaviour
{
	[SerializeField]
	private TMPro.TextMeshProUGUI text;

	[SerializeField]
	private Button nextButton;

	[SerializeField]
	private Button backButton;


	private void Awake()
	{
		nextButton.onClick.AddListener(()=>
		{
			if (text.textInfo.pageCount - 1 < text.pageToDisplay) { return; }
			text.pageToDisplay++;
		});

		backButton.onClick.AddListener(() =>
		{
			// ページは1から開始.
			if (text.pageToDisplay <= 1) { return; }
			text.pageToDisplay--;
		});
	}
}
