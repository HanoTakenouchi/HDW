using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// ゲームシステム
/// ・UI中ボタン処理
/// </summary>
public class GameSystem : MonoBehaviour
{
	/// <summary>
	/// PC起動時のスクリーンサイズ
	/// </summary>
	[SerializeField]
	private Vector2 screenSize;

	/// <summary>
	/// ノベルシステム
	/// </summary>
	[SerializeField]
	private NovelSystem novelSystem;

	/// <summary>
	/// TIPSオブジェクト(子供にTextがついている)
	/// </summary>
	[SerializeField]
	private GameObject tipsGameObject;

	/// <summary>
	/// 設定ボタン
	/// </summary>
	[SerializeField]
	private Button settingButton;

	/// <summary>
	/// ログボタン
	/// </summary>
	[SerializeField]
	private Button logButton;

	/// <summary>
	/// 画面ルート
	/// </summary>
	[SerializeField]
	private Transform pageRoot;

	/// <summary>
	/// ログ画面プレハブ
	/// </summary>
	[SerializeField]
	private GameObject logPagePrefab;

	/// <summary>
	/// 設定画面プレハブ
	/// </summary>
	[SerializeField]
	private GameObject settingPagePrefab;


	/// <summary>
	/// Unity Event Awake
	/// </summary>
	private void Awake()
	{
		// PCならウィンドウサイズ調整
		if (Application.platform == RuntimePlatform.WindowsPlayer ||
			Application.platform == RuntimePlatform.LinuxPlayer ||
			Application.platform == RuntimePlatform.OSXPlayer)
		{
			Screen.SetResolution((int)screenSize.x, (int)screenSize.y, false);
		}

		settingButton.onClick.AddListener(OnClickSettingButton);
		logButton.onClick.AddListener(OnClickLogButton);
	}

	/// <summary>
	/// リンクされたTextを押下
	/// </summary>
	public void OnClickLinkedText(string linkID, string linkText, int linkIndex)
	{
		// リンク外をクリックした
		if (linkIndex == -1)
		{
			if (tipsGameObject.activeSelf)
			{
				tipsGameObject.SetActive(false);
			}
		}
		// リンクをクリックした
		else
		{
			tipsGameObject.SetActive(true);
			var tipsText = tipsGameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
			tipsText.text = string.Format("<#ffff00>【{0}】</color>\n{1}", linkText, linkID);
		}
	}

	/// <summary>
	/// 設定ボタン押下時
	/// </summary>
	private void OnClickSettingButton()
	{
		var page = Instantiate(settingPagePrefab, pageRoot, false);
		var pageTransform = page.transform;

		var pageBackgroundTransform = pageTransform.Find("Background");
		var pageBackgroundButton = pageBackgroundTransform.GetComponent<Button>();
		var continueObject = FindInChildren(pageTransform, "Continue").gameObject;
		var continueButton = continueObject.GetComponent<Button>();
		var continueImage = continueObject.GetComponentInChildren<Image>(true);
		var reloadObject = FindInChildren(pageTransform, "Reload").gameObject;
		var reloadButton = reloadObject.GetComponent<Button>();
		var reloadImage = reloadObject.GetComponentInChildren<Image>(true);
		var finishObject = FindInChildren(pageTransform, "Finish").gameObject;
		var finishButton = finishObject.GetComponent<Button>();
		var finishImage = finishObject.GetComponentInChildren<Image>(true);

		// 非表示メソッド準備（強引）
		var selectColor = continueImage.color;
		var clearMethod = new System.Action(() =>
		{
			continueImage.color = Color.clear;
			reloadImage.color = Color.clear;
			finishImage.color = Color.clear;
		});

		// αを消して非表示に
		clearMethod();

		pageBackgroundButton.onClick.RemoveAllListeners();
		pageBackgroundButton.onClick.AddListener(() =>
		{
			if (continueImage.color.a <= 0 && reloadImage.color.a <= 0 && reloadImage.color.a <= 0)
			{
				Destroy(page);
			}
			clearMethod();
		});

		continueButton.onClick.RemoveAllListeners();
		continueButton.onClick.AddListener(() =>
		{
			if (0 < continueImage.color.a)
			{
				Destroy(page);
			}
			clearMethod();
			continueImage.color = selectColor;
		});

		reloadButton.onClick.RemoveAllListeners();
		reloadButton.onClick.AddListener(() =>
		{
			if (0 < reloadImage.color.a)
			{
				SceneManager.LoadScene(0);
			}
			clearMethod();
			reloadImage.color = selectColor;
		});

		finishButton.onClick.RemoveAllListeners();
		finishButton.onClick.AddListener(() =>
		{
			if (0 < finishImage.color.a)
			{
				Application.Quit();
				Destroy(page);
			}
			clearMethod();
			finishImage.color = selectColor;
		});
	}

	/// <summary>
	/// ログボタン押下時
	/// </summary>
	private void OnClickLogButton()
	{
		var page = Instantiate(logPagePrefab, pageRoot, false);

		var pageBackgroundTransform = page.transform.Find("Background");
		var pageBackgroundButton = pageBackgroundTransform.GetComponent<Button>();
		pageBackgroundButton.onClick.RemoveAllListeners();
		pageBackgroundButton.onClick.AddListener(()=> { Destroy(page); });

		var contentTransform = FindInChildren(page.transform, "Content");
		var logOriginalObject = FindInChildren(contentTransform, "Log").gameObject;

		var histories = novelSystem.history.GetLogs();
		if (histories != null && 0 < histories.Length)
		{
			for(int i= 0; i <histories.Length; ++i)
			{
				var history = histories[i];

				var logCloneObject = (i == histories.Length - 1) ?
					logOriginalObject : UnityEngine.Object.Instantiate(logOriginalObject, contentTransform, false);

				logCloneObject.transform.SetAsFirstSibling();
				logCloneObject.name = string.Format("Log{0}", i);

				var nameText = FindInChildren(logCloneObject.transform, "Name").GetComponent<TMPro.TextMeshProUGUI>();
				nameText.text = history.name;

				var textText = FindInChildren(logCloneObject.transform, "Text").GetComponent<TMPro.TextMeshProUGUI>();
				textText.text = history.text;

				var button = logCloneObject.GetComponent<Button>();
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => OnClickLogContentButton(page, logCloneObject, history) );
			}
		}

		// スクロール位置を一番下にする
		var scrollViewTransform = page.transform.Find("ScrollView");
		var scrollView = scrollViewTransform.GetComponent<ScrollRect>();
		scrollView.verticalNormalizedPosition = 0.0f;
	}

	/// <summary>
	/// ログコンテンツボタン押下時
	/// </summary>
	private void OnClickLogContentButton(GameObject page, GameObject logObject, NovelHistory.LogData history)
	{
		var logBackgroundImage = logObject.GetComponent<Image>();
		var logBackgroundColor = logBackgroundImage.color;
		logBackgroundColor.a = 0.3f;
		logBackgroundImage.color = logBackgroundColor;

		var jumpConfirm = FindInChildren(page.transform, "JumpConfirm");
		jumpConfirm.gameObject.SetActive(true);

		var backgroundButton = FindInChildren(jumpConfirm, "Background").GetComponent<Button>();
		var yesObject = FindInChildren(jumpConfirm, "Yes").gameObject;
		var yesButton = yesObject.GetComponent<Button>();
		var yesImage = yesObject.GetComponentInChildren<Image>(true);
		var noObject = FindInChildren(jumpConfirm, "No").gameObject;
		var noButton = noObject.GetComponent<Button>();
		var noImage = noObject.GetComponentInChildren<Image>(true);

		// αを消して非表示に
		var selectColor = yesImage.color;
		yesImage.color = Color.clear;
		noImage.color = Color.clear;

		backgroundButton.onClick.RemoveAllListeners();
		backgroundButton.onClick.AddListener(() =>
		{
			if (yesImage.color.a <= 0 && noImage.color.a <= 0)
			{
				logBackgroundColor.a = 0.0f;
				logBackgroundImage.color = logBackgroundColor;
				jumpConfirm.gameObject.SetActive(false);
				yesImage.color = selectColor; // 表示参考値として最初に使うので元に戻す
			}
			else
			{
				yesImage.color = Color.clear;
				noImage.color = Color.clear;
			}
		});

		yesButton.onClick.RemoveAllListeners();
		yesButton.onClick.AddListener(() =>
		{
			if (0 < yesImage.color.a)
			{
				Destroy(page);
				novelSystem.Rollback(history.historyID);
			}
			else
			{
				yesImage.color = selectColor;
				noImage.color = Color.clear;
			}
		});

		noButton.onClick.RemoveAllListeners();
		noButton.onClick.AddListener(() =>
		{
			if (0 < noImage.color.a)
			{
				logBackgroundColor.a = 0.0f;
				logBackgroundImage.color = logBackgroundColor;
				jumpConfirm.gameObject.SetActive(false);
				yesImage.color = selectColor; // 表示参考値として最初に使うので元に戻す
			}
			else
			{
				yesImage.color = Color.clear;
				noImage.color = selectColor;
			}
		});
	}

	/// <summary>
	/// 子階層以下から引数名の子を探索
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="childName"></param>
	/// <returns></returns>
	private Transform FindInChildren(Transform transform, string childName)
	{
		if (transform.name == childName)
		{
			return transform;
		}

		var childCount = transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			var child = transform.GetChild(i);
			child = FindInChildren(child, childName);
			if (child != null) { return child; }
		}

		return null;
	}
}
