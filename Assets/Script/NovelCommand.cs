using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;


/// <summary>
/// コマンドID
/// </summary>
public enum NovelCommandType
{
	_Novel = 0,
	None = _Novel + 0,
	Comment = _Novel + 1,
	Include = _Novel + 2,
	Import = _Novel + 3,
	Group = _Novel + 4,
	GroupEnd = _Novel + 5,
	GroupRun = _Novel + 6,

	_Text = 100000,
	TextWrite = _Text + 1001,
	TextClear = _Text + 1002,
	TextShow = _Text + 1003,
	TextHide = _Text + 1004,
	TextAnimationType = _Text + 1101,
	TextAnimationPlay = _Text + 1102,
	TextAnimationPause = _Text + 1103,
	TextAnimationStop = _Text + 1104,
	NameWrite = _Text + 2001,
	NameClear = _Text + 2002,
	NameShow = _Text + 2003,
	NameHide = _Text + 2004,
	NameAnimationType = _Text + 2101,
	NameAnimationPlay = _Text + 2102,
	NameAnimationPause = _Text + 2103,
	NameAnimationStop = _Text + 2104,

	_Graphics = 200000,
	Image = _Graphics + 1001,
	ImageRemove = _Graphics + 1002,
	ImageChange = _Graphics + 1003,
	ImageColor = _Graphics + 1004,
	ImageOrder = _Graphics + 1005,

	BackgroundImage = _Graphics + 2001,
	BackgroundImageRemove = _Graphics + 2002,
	BackgroundImageChange = _Graphics + 2003,

	Model = _Graphics + 3001,
	ModelRemove = _Graphics + 3002,
	ModelColor = _Graphics + 3003,
	ModelOrder = _Graphics + 3004,

	_Effect = 300000,

	_Sound = 400000,
	SoundSePlay = _Sound + 6001,
	SoundSePause = _Sound + 6002,
	SoundSeStop = _Sound + 6003,
	SoundBgmPlay = _Sound + 6004,
	SoundBgmPause = _Sound + 6005,
	SoundBgmStop = _Sound + 6006,
	SoundVoicePlay = _Sound + 6007,
	SoundVoicePause = _Sound + 6008,
	SoundVoiceStop = _Sound + 6009,

	_Animation = 500000,
	AnimationType = _Animation + 5001,
	AnimationPlay = _Animation + 5002,
	AnimationPause = _Animation + 5003,
	AnimationStop = _Animation + 5004,

	AnimationPosition = _Animation + 6001,
	AnimationRotation = _Animation + 6002,
	AnimationScale = _Animation + 6003,

	_System = 99000000,
	Jump = _System + 1,
	Label = _System + 2,
	Choices = _System + 3,

	WaitTime = _System + 1001,
	WaitEvent = _System + 1002,
	Pause = _System + 1003,

	ValueAssign = _System + 2001,
	ValueAdd = _System + 2002,
	ValueSub = _System + 2003,
	ValueMul = _System + 2004,
	ValueDiv = _System + 2005,

	ObjectInstance = _System + 3001,
	ObjectDestroy = _System + 3002,
	ObjectPosition = _System + 3003,
	ObjectRotation = _System + 3004,
	ObjectScale = _System + 3005,

	FadeInScreen = _System + 4001,
	FadeOutScreen = _System + 4002,
	FadeInStage = _System + 4003,
	FadeOutStage = _System + 4004,
	FadeInBackground = _System + 4005,
	FadeOutBackground = _System + 4006,
}


/// <summary>
/// ノベルコマンド属性(各コマンドに付加情報を与える)
/// </summary>
public class NovelCommandAttribute : Attribute
{
	/// <summary>固有ID</summary>
	public int id;

	public NovelCommandAttribute(NovelCommandType type)
	{
		this.id = (int)type;
	}
}


/// <summary>
/// ノベルコマンド群をまとめたクラス
/// </summary>
public class NovelCommand
{
	/// <summary>
	/// コマンド間共有データ
	/// </summary>
	public class SharedData
	{
		public NovelSystem system;
		public NovelView view;
		public NovelData data;
		public NovelData.Command command;
		public NovelMetaData meta = new NovelMetaData();
	}

	/// <summary>
	/// コマンド間共有変数
	/// </summary>
	public class SharedVariable
	{
		/// <summary>
		/// 現在実行中のインデックス
		/// </summary>
		public int index;

		/// <summary>
		/// GameObjectハンドル
		/// </summary>
		public Dictionary<string, GameObject> handles;
		
		/// <summary>
		/// 変数
		/// </summary>
		public Dictionary<string, string> values;

		/// <summary>
		/// グループ変数
		/// </summary>
		public Dictionary<string, string> groupValues;

		/// <summary>
		/// Undoで巻き戻した時に適応するGroupのスキップ履歴
		/// </summary>
		public Queue<NovelHistory.GroupData> groupHistory;


		public string GetValue(string name)
		{
			string value;
			if (groupValues != null && groupValues.TryGetValue(name, out value)) { return value; }
			if (values != null && values.TryGetValue(name, out value)) { return value; }
			return null;
		}

		public string FindValue(string name)
		{
			if (string.IsNullOrEmpty(name)) { return name; }
			string value = name, find = name;
			while ((find = GetValue(find)) != null) { value = find; }
			return value;
		}
	}

	/// <summary>
	/// 外部通知イベントデータ
	/// </summary>
	[System.Serializable]
	public class EventData
	{
		public int intParameter = -1;
		public float floatParameter = -1.0f;
		public string stringParameter = null;
		public GameObject objectParameter = null;
	}

	/// <summary>
	/// ノベルコマンドインターフェース
	/// </summary>
	public interface NovelCommandInterface
	{
		IEnumerator Do(SharedData share, SharedVariable variable);
		IEnumerator Undo(SharedData share, SharedVariable variable);
		IEnumerator Event(SharedData share, SharedVariable variable, EventData e);
	}

	/// <summary>
	/// なし
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.None)]
	public class None : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 注釈行(無視されるコマンド)
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.Comment)]
	public class Comment : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// ノベルコマンドのグループ化(構文なので処理内容は無し)
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.Group)]
	public class Group : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var groupName = share.command.parameters[0];

			// メタデータの取得
			NovelMetaData.GroupData data;
			if (share.meta.groupDic.TryGetValue(groupName, out data))
			{
				// GroupEndまで読み飛ばす
				variable.index = data.endIndex + 1;
			}
			// メタデータがなければ生成する
			else
			{
				share.meta.groupDic[groupName] = data = new NovelMetaData.GroupData()
				{
					data = share.data,
					// Groupの次のコマンドが開始位置
					startIndex = variable.index + 1,
				};

				// GroupEndの場所を探索しつつ読み飛ばす
				for (; variable.index < share.data.commands.Count; ++variable.index)
				{
					var command = share.data.commands[variable.index];
					if (command.id == (int)NovelCommandType.GroupEnd)
					{
						// GroupEndの前のコマンドが終了位置
						data.endIndex = variable.index - 1;
						break;
					}
				}
			}

			yield break;
		}
		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			// Groupまで巻き戻す
			yield break;
		}
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// ノベルコマンドのグループ実行
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.GroupRun)]
	public class GroupRun : NovelCommandInterface
	{
		private NovelExecuter executer;
		private SharedVariable variable;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			// メタデータの取得(ない場合は処理できない)
			NovelMetaData.GroupData data;
			if (!share.meta.groupDic.TryGetValue(share.command.parameters[0], out data)) { yield break; }

			// 呼び出し履歴確認
			var currentGroup = variable.groupHistory.TryDequeue();
			var nextGroup = variable.groupHistory.TryPeek();

			// 呼び出し履歴に残っている = 既にPushされている
			if (currentGroup == null)
			{
				// 呼び出し履歴登録
				share.system.history.PushGroup(true, variable.index);
			}

			// 開始位置の決定
			var startIndex = data.startIndex;
			if (currentGroup != null)
			{
				if (nextGroup != null)
				{
					startIndex += nextGroup.historyID - currentGroup.historyID;
				}
				else
				{
					// 自分自身(GroupRun)が実行されるとともに履歴に追加されているので、その分を引く
					var historyID = share.system.history.GetCurrentHistoryID();
					startIndex += historyID - currentGroup.historyID;
				}
			}

			// 実行準備
			this.executer = new NovelExecuter(share.data, share.system.commandTypeDic);
			this.variable = new SharedVariable()
			{
				// グループ引数はグループ内でしか用いらない変数なので、複製する必要がある
				groupValues = new Dictionary<string, string>(variable.groupValues),
				groupHistory = variable.groupHistory,
				values = variable.values,
				handles = variable.handles,
				index = startIndex,
			};

			// グループ引数の登録
			var parameterCount = share.command.parameters[1].ParseInt();
			for (int i = 0; i < parameterCount; ++i)
			{
				var name = share.command.parameters[i * 2 + 2];
				var value = share.command.parameters[i * 2 + 3];
				this.variable.values[name] = value;
			}

			// グループの実行
			yield return executer.SetupCoroutine(share);
			yield return executer.RunCoroutine(this.variable, data.endIndex);

			// 呼び出し履歴登録
			share.system.history.PushGroup(false, variable.index);
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e)
		{
			yield return executer.EventCoroutine(e);
			yield break;
		}
	}

	/// <summary>
	/// テキストの書き込み
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.TextWrite)]
	public class TextWrite : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			share.view.Text.text += variable.FindValue(share.command.parameters[0]);
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// テキストのクリア
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.TextClear)]
	public class TextClear : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			share.view.Text.text = "";
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// テキストウィンドウの表示
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.TextShow)]
	public class TextShow : NovelCommandInterface
	{
		private bool prevActive;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			prevActive = share.view.TextWindowImage.gameObject.activeSelf;
			share.view.TextWindowImage.gameObject.SetActive(true);
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			share.view.TextWindowImage.gameObject.SetActive(prevActive);
			yield break;
		}
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// テキストウィンドウの非表示
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.TextHide)]
	public class TextHide : NovelCommandInterface
	{
		private bool prevActive;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			prevActive = share.view.TextWindowImage.gameObject.activeSelf;
			share.view.TextWindowImage.gameObject.SetActive(false);
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			share.view.TextWindowImage.gameObject.SetActive(prevActive);
			yield break;
		}
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// テキストのアニメーション設定
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.TextAnimationType)]
	public class TextAnimationType : NovelCommandInterface
	{
		private TextAnimation backup;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var fileName = share.command.parameters[0];

			var request = Resources.LoadAsync<TextAnimationData>(NovelUtility.ResourcesTextAnimationDataPath + fileName);
			yield return request;

			var data = request.asset as TextAnimationData;
			if (null == data) { yield break; }

			var animator = share.view.Text.GetComponent<TextAnimator>();
			if (animator == null) { animator = share.view.Text.gameObject.AddComponent<TextAnimator>(); }

			backup = animator.animation;
			animator.SetAnimation(data.animation);
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			var animator = share.view.Text.GetComponent<TextAnimator>();
			if (animator == null) { yield break; }
			animator.SetAnimation(backup);
			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e)
		{
			yield break;
		}
	}

	/// <summary>
	/// テキストのアニメーション再生
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.TextAnimationPlay)]
	public class TextAnimationPlay : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var animator = share.view.Text.GetComponent<TextAnimator>();
			if (animator == null) { animator = share.view.Text.gameObject.AddComponent<TextAnimator>(); }

			animator.Play();
			yield return new WaitWhile(() => { return animator.isAnimating; });

			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e)
		{
			if (e.intParameter == 0)
			{
				var animator = share.view.Text.GetComponent<TextAnimator>();
				animator.Finish();
			}

			yield break;
		}
	}

	/// <summary>
	/// 名前の書き込み
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.NameWrite)]
	public class NameWrite : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			share.view.NameText.text += variable.FindValue(share.command.parameters[0]);
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 名前のクリア
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.NameClear)]
	public class NameClear : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			share.view.NameText.text = "";
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 名前ウィンドウの表示
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.NameShow)]
	public class NameShow : NovelCommandInterface
	{
		private bool prevActive;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			prevActive = share.view.NameTextWindowImage.gameObject.activeSelf;
			share.view.NameTextWindowImage.gameObject.SetActive(true);
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			share.view.NameTextWindowImage.gameObject.SetActive(prevActive);
			yield break;
		}
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 名前ウィンドウの非表示
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.NameHide)]
	public class NameHide : NovelCommandInterface
	{
		private bool prevActive;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			prevActive = share.view.NameTextWindowImage.gameObject.activeSelf;
			share.view.NameTextWindowImage.gameObject.SetActive(false);
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			share.view.NameTextWindowImage.gameObject.SetActive(prevActive);
			yield break;
		}
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 名前のアニメーション再生
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.NameAnimationPlay)]
	public class NameAnimationPlay : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var animator = share.view.Text.GetComponent<TextAnimator>();
			if (animator == null) { animator = share.view.NameText.gameObject.AddComponent<TextAnimator>(); }

			animator.Play();
			yield return new WaitWhile(() => { return animator.isAnimating; });

			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e)
		{
			if (e.intParameter == 0)
			{
				var animator = share.view.Text.GetComponent<TextAnimator>();
				animator.Finish();
			}

			yield break;
		}
	}

	/// <summary>
	/// 2Dイメージの表示
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.Image)]
	public class Image : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var handleName = share.command.parameters[0];
			var fileName = share.command.parameters[1];
			var xRatio = share.command.parameters[2].ParseFloat();
			var yRatio = share.command.parameters[3].ParseFloat();
			var time = share.command.parameters[4].ParseFloat();

			var request = Resources.LoadAsync<Sprite>(NovelUtility.ResourcesImagePath + fileName);
			yield return request;

			var sprite = request.asset as Sprite;
			if (null == sprite) { yield break; }

			var gameObject = new GameObject(handleName);
			var transform = gameObject.transform;
			transform.SetParent(share.view.Stage, false);
			transform.localPosition = new Vector3(Screen.width * xRatio, Screen.height * yRatio, 0.0f);

			variable.handles[handleName] = gameObject;

			var color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			var image = gameObject.AddComponent<UnityEngine.UI.Image>();
			image.sprite = sprite;
			image.SetNativeSize();
			image.color = color;

			if (0.0f < time)
			{
				float t = 0.0f;
				while (t < time)
				{
					yield return null;
					t += Time.deltaTime;
					color.a = Mathf.Clamp01(t / time);
					image.color = color;
				}
			}

			color.a = 1.0f;
			image.color = color;
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			var handleName = share.command.parameters[0];

			GameObject handleObject;
			if (!variable.handles.TryGetValue(handleName, out handleObject)) { yield break; }
			variable.handles.Remove(handleName);
			if (handleObject == null) { yield break; }

			UnityEngine.Object.Destroy(handleObject);

			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 2Dイメージの非表示
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.ImageRemove)]
	public class ImageRemove : NovelCommandInterface
	{
		/// <summary>Undo用キャッシュ(参照が残り続けることに注意)</summary>
		private GameObject gameObject;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var handleName = share.command.parameters[0];
			var time = share.command.parameters[1].ParseFloat();

			if (!variable.handles.TryGetValue(handleName, out gameObject)) { yield break; }
			variable.handles.Remove(handleName);
			if (gameObject == null) { yield break; }

			var image = gameObject.GetComponent<UnityEngine.UI.Image>();
			var color = image.color;
			var colorAlpha = color.a;
			if (colorAlpha <= 0.0f) { yield break; }

			if (0.0f < time)
			{
				float t = time;
				while (0.0f < t)
				{
					yield return null;
					t -= Time.deltaTime;
					color.a = Mathf.Lerp(0.0f, colorAlpha, Mathf.Clamp01(t / time));
					image.color = color;
				}
			}

			color.a = 0.0f;
			image.color = color;

			// Undoの復元用に情報は残したいのでDestroyしない
			gameObject.SetActive(false);
#if UNITY_EDITOR
			gameObject.hideFlags |= HideFlags.HideInHierarchy;
#endif
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			var handleName = share.command.parameters[0];
			variable.handles[handleName] = gameObject;

			gameObject.SetActive(true);
#if UNITY_EDITOR
			gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
#endif

			var image = gameObject.GetComponent<UnityEngine.UI.Image>();
			var color = image.color;
			color.a = 1.0f;
			image.color = color;

			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 2Dイメージの切り替え
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.ImageChange)]
	public class ImageChange : NovelCommandInterface
	{
		private string prevFileName = null;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var handleName = share.command.parameters[0];
			var fileName = share.command.parameters[1];
			var time = share.command.parameters[2].ParseFloat();

			// 対象となるGameObjectの取得
			GameObject handleObject;
			if (!variable.handles.TryGetValue(handleName, out handleObject)) { yield break; }
			if (handleObject == null) { yield break; }

			// 画像読み込み
			var request = Resources.LoadAsync<Sprite>(NovelUtility.ResourcesImagePath + fileName);
			yield return request;

			var sprite = request.asset as Sprite;
			if (null == sprite) { yield break; }

			// Undo用にファイル名を保持
			var image = handleObject.GetComponent<UnityEngine.UI.Image>();
			prevFileName = image.sprite.name;

			// 画像切り替え
			if (time <= 0.0f)
			{
				image.sprite = sprite;
			}
			// 画像切り替えフェード
			else
			{
				// 元々透明なら切り替えフェードはしない
				if (image.color.a <= 0.0f)
				{
					image.sprite = sprite;
				}
				else
				{
					// 専用マテリアル読み込み
					var material = Resources.Load<Material>(NovelUtility.ResourcesImageMaterialPath);
					image.material = UnityEngine.Object.Instantiate(material);
					material = image.materialForRendering;
					material.SetTexture("_SubTex", sprite.texture);

					float t = 0.0f;
					while (t < time)
					{
						yield return null;
						t += Time.deltaTime;
						material.SetFloat("_CrossAlpha", Mathf.Clamp01(t / time));
					}

					image.sprite = sprite;
					image.material = null;
					UnityEngine.Object.Destroy(material);
				}
			}

			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			if (string.IsNullOrEmpty(prevFileName)) { yield break; }

			var handleName = share.command.parameters[0];

			// 対象となるGameObjectの取得
			GameObject handleObject;
			if (!variable.handles.TryGetValue(handleName, out handleObject)) { yield break; }
			if (handleObject == null) { yield break; }

			// 画像読み込み
			var request = Resources.LoadAsync<Sprite>(NovelUtility.ResourcesImagePath + prevFileName);
			yield return request;

			var sprite = request.asset as Sprite;
			if (null == sprite) { yield break; }

			// Undo用にファイル名を保持
			var image = handleObject.GetComponent<UnityEngine.UI.Image>();
			image.sprite = sprite;

			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 背景イメージの切り替え
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.BackgroundImageChange)]
	public class BackgroundImageChange : NovelCommandInterface
	{
		private string prevFileName = null;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var handleName = share.command.parameters[0];
			var fileName = share.command.parameters[1];
			var time = share.command.parameters[2].ParseFloat();

			// 対象となるGameObjectの取得
			GameObject handleObject;
			if (!variable.handles.TryGetValue(handleName, out handleObject)) { yield break; }
			if (handleObject == null) { yield break; }

			// 画像読み込み
			var request = Resources.LoadAsync<Texture>(NovelUtility.ResourcesBackgroundPath + fileName);
			yield return request;

			var texture = request.asset as Texture;
			if (null == texture) { yield break; }

			// Undo用にファイル名を保持
			var rawImage = handleObject.GetComponent<UnityEngine.UI.RawImage>();
			prevFileName = rawImage.texture.name;

			// 画像切り替え
			if (time <= 0.0f)
			{
				rawImage.texture = texture;
			}
			// 画像切り替えフェード
			else
			{
				// 元々透明なら切り替えフェードはしない
				if (rawImage.color.a <= 0.0f)
				{
					rawImage.texture = texture;
				}
				else
				{
					// 専用マテリアル読み込み
					var material = Resources.Load<Material>(NovelUtility.ResourcesImageMaterialPath);
					rawImage.material = UnityEngine.Object.Instantiate(material);
					material = rawImage.materialForRendering;
					material.SetTexture("_SubTex", texture);

					float t = 0.0f;
					while (t < time)
					{
						yield return null;
						t += Time.deltaTime;
						material.SetFloat("_CrossAlpha", Mathf.Clamp01(t / time));
					}

					rawImage.texture = texture;
					rawImage.material = null;
					UnityEngine.Object.Destroy(material);
				}
			}

			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			if (string.IsNullOrEmpty(prevFileName)) { yield break; }

			var handleName = share.command.parameters[0];

			// 対象となるGameObjectの取得
			GameObject handleObject;
			if (!variable.handles.TryGetValue(handleName, out handleObject)) { yield break; }
			if (handleObject == null) { yield break; }

			// 画像読み込み
			var request = Resources.LoadAsync<Texture>(NovelUtility.ResourcesBackgroundPath + prevFileName);
			yield return request;

			var texture = request.asset as Texture;
			if (null == texture) { yield break; }

			var rawImage = handleObject.GetComponent<UnityEngine.UI.RawImage>();
			rawImage.texture = texture;

			if (rawImage.material != rawImage.defaultMaterial)
			{
				UnityEngine.Object.Destroy(rawImage.material);
				rawImage.material = null;
			}

			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// ジャンプ
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.Jump)]
	public class Jump : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var jumpLabel = share.command.parameters[0];

			for (variable.index++; variable.index < share.data.commands.Count; ++variable.index)
			{
				var command = share.data.commands[variable.index];
				if (command.id == (int)NovelCommandType.Label)
				{
					if (command.parameters[0] == jumpLabel)
					{
						yield break;
					}
				}
			}

			yield break;
		}
		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// ラベル
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.Label)]
	public class Label : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 選択肢
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.Choices)]
	public class Choices : NovelCommandInterface
	{
		private GameObject gameObject;
		private bool isNext = false;
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var fileName = share.command.parameters[0];
			var buttonCount = share.command.parameters[1].ParseInt();

			var request = Resources.LoadAsync<GameObject>(NovelUtility.ResourcesChoisePath + fileName);
			yield return request;

			var prefab = request.asset as GameObject;
			if (null == prefab) { yield break; }

			gameObject = UnityEngine.Object.Instantiate(prefab, share.view.NovelCanvas.transform, false);
			var choiseObject = gameObject;
			var choiseTransform = choiseObject.transform;
			var buttonObject = choiseTransform.Find("Button");

			for (int i = 0; i < buttonCount; ++i)
			{
				var buttonName = share.command.parameters[i * 2 + 2];
				var jumpLabel = share.command.parameters[i * 2 + 3];

				var buttonCloneObject = (i == buttonCount - 1) ?
					buttonObject : UnityEngine.Object.Instantiate(buttonObject, choiseTransform, false);

				buttonCloneObject.SetAsLastSibling();
				buttonCloneObject.name = buttonName;

				var button = buttonCloneObject.GetComponent<Button>();
				var text = buttonCloneObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener( ()=> { share.system.Event(jumpLabel); } );
				text.text = buttonName;
			}

			yield return new WaitForSecondsRealtime(1.5f);

			share.view.NextIconImage.enabled = false;
			share.view.TouchScreenObject.SetActive(false);
			yield return new WaitUntil(() => { return isNext; });
			UnityEngine.Object.Destroy(choiseObject);
			share.view.TouchScreenObject.SetActive(true);
			share.view.NextIconImage.enabled = true;
			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			UnityEngine.Object.Destroy(gameObject);
			share.view.TouchScreenObject.SetActive(true);
			share.view.NextIconImage.enabled = true;
			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e)
		{
			if (e.intParameter == 0) { yield break; }

			Debug.Log(e.stringParameter);

			for (variable.index++; variable.index < share.data.commands.Count; ++variable.index)
			{
				var command = share.data.commands[variable.index];
				if (command.id == (int)NovelCommandType.Label)
				{
					if (command.parameters[0] == e.stringParameter)
					{
						isNext = true;
						yield break;
					}
				}
			}

			isNext = true;
			yield break;
		}
	}

	/// <summary>
	/// 一定時間待ち
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.WaitTime)]
	public class WaitTime : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var time = share.command.parameters[0].ParseFloat();
			if (time <= 0.0f) { yield break; }
			yield return new WaitForSecondsRealtime(time);
			yield break;
		}
		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 特定イベント待ち
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.WaitEvent)]
	public class WaitEvent : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e) { yield break; }
	}

	/// <summary>
	/// 入力待ち
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.Pause)]
	public class Pause : NovelCommandInterface
	{
		private bool isNext = false;
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			share.view.NextIconImage.gameObject.SetActive(true);
			yield return new WaitUntil(() => { return isNext || Input.GetKeyDown(KeyCode.Z); });
			share.view.NextIconImage.gameObject.SetActive(false);
			yield break;
		}
		public IEnumerator Undo(SharedData share, SharedVariable variable) { yield break; }
		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e)
		{
			// TouchScreenに触れた
			if (e.intParameter == 0)
			{
				// テキスト中のリンクに触れた
				var touchPosition = Input.touchCount <= 0 ?
					Input.mousePosition : (Vector3)Input.GetTouch(0).position;

				if (TMP_TextUtilities.IsIntersectingRectTransform(share.view.Text.rectTransform, touchPosition, null))
				{
					int linkIndex = TMP_TextUtilities.FindIntersectingLink(share.view.Text, touchPosition, null);
					if (linkIndex != -1) { yield break; }
				}

				isNext = true;
			}
			yield break;
		}
	}

	/// <summary>
	/// 値の代入
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.ValueAssign)]
	public class ValueAssign : NovelCommandInterface
	{
		private string prevValue = null;

		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var valueName = share.command.parameters[0];
			var parameterString = share.command.parameters[1];

			// Undo用に上書き前の値を保存
			if (!variable.values.TryGetValue(valueName, out prevValue)) { prevValue = null; }

			variable.values[valueName] = parameterString;

			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			if (prevValue != null)
			{
				var valueName = share.command.parameters[0];
				variable.values[valueName] = prevValue;
			}
			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e)
		{
			yield break;
		}
	}

	/// <summary>
	/// 値の加算
	/// </summary>
	[NovelCommandAttribute(NovelCommandType.ValueAdd)]
	public class ValueAdd : NovelCommandInterface
	{
		public IEnumerator Do(SharedData share, SharedVariable variable)
		{
			var valueName = share.command.parameters[0];
			var parameterString = share.command.parameters[1];

			string valueString = null;
			if (!variable.values.TryGetValue(valueName, out valueString)){ yield break; }

			// FloatかIntかで処理を分ける（浮動小数点計算による誤差を防ぐため）
			if (valueString.Contains(".") || parameterString.Contains("."))
			{
				var param = parameterString.ParseFloat();
				var value = valueString.ParseFloat();
				value += param;
				variable.values[valueName] = value.ToString();
			}
			else
			{
				var param = parameterString.ParseInt();
				var value = valueString.ParseInt();
				value += param;
				variable.values[valueName] = value.ToString();
			}

			yield break;
		}

		public IEnumerator Undo(SharedData share, SharedVariable variable)
		{
			var valueName = share.command.parameters[0];
			var parameterString = share.command.parameters[1];

			string valueString = null;
			if (!variable.values.TryGetValue(valueName, out valueString)) { yield break; }

			// FloatかIntかで処理を分ける（浮動小数点計算による誤差を防ぐため）
			if (valueString.Contains(".") || parameterString.Contains("."))
			{
				var param = parameterString.ParseFloat();
				var value = valueString.ParseFloat();
				value -= param;
				variable.values[valueName] = value.ToString();
			}
			else
			{
				var param = parameterString.ParseInt();
				var value = valueString.ParseInt();
				value -= param;
				variable.values[valueName] = value.ToString();
			}

			yield break;
		}

		public IEnumerator Event(SharedData share, SharedVariable variable, EventData e)
		{
			yield break;
		}
	}
}
