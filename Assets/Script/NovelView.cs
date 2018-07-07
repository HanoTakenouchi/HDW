#pragma warning disable 169 // プライベート変数が宣言されたが、参照されていない
#pragma warning disable 649 // 変数未割り当て

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


/// <summary>
/// ノベルビュー
/// </summary>
public class NovelView : MonoBehaviour
{
	[SerializeField]
	private Transform canvases;
	public Transform Canvases { get { return canvases; } }
	[SerializeField]
	private Canvas stageCanvas;
	public Canvas StageCanvas { get { return stageCanvas; } }
	[SerializeField]
	private CanvasScaler stageScanvasScaler;
	public CanvasScaler StageScanvasScaler { get { return stageScanvasScaler; } }
	[SerializeField]
	private RectTransform stage;
	public RectTransform Stage { get { return stage; } }
	[SerializeField]
	private List<GameObject> stageObjects;
	public List<GameObject> StageObjects { get { return stageObjects; } }

	[SerializeField]
	private Canvas novelCanvas;
	public Canvas NovelCanvas { get { return novelCanvas; } }
	[SerializeField]
	private Image textWindowImage;
	public Image TextWindowImage { get { return textWindowImage; } }
	[SerializeField]
	private TMPro.TextMeshProUGUI text;
	public TMPro.TextMeshProUGUI Text { get { return text; } }
	[SerializeField]
	private Image nextIconImage;
	public Image NextIconImage { get { return nextIconImage; } }
	[SerializeField]
	private Animation nextIconAnimation;
	public Animation NextIconAnimation { get { return nextIconAnimation; } }
	[SerializeField]
	private Image nameTextWindowImage;
	public Image NameTextWindowImage { get { return nameTextWindowImage; } }
	[SerializeField]
	private TMPro.TextMeshProUGUI nameText;
	public TMPro.TextMeshProUGUI NameText { get { return nameText; } }

	[SerializeField]
	private Canvas uiCanvas;
	public Canvas UICanvas { get { return uiCanvas; } }
	[SerializeField]
	private Button settingButton;
	public Button SettingButton { get { return settingButton; } }
	[SerializeField]
	private Image settingButtonImage;
	public Image SettingButtonImage { get { return settingButtonImage; } }
	[SerializeField]
	private GameObject touchScreenObject;
	public GameObject TouchScreenObject { get { return touchScreenObject; } }
}
