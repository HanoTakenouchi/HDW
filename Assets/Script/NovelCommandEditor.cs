using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// ノベルコマンド属性(各コマンドに付加情報を与える)
/// </summary>
public class NovelCommandEditorAttribute : Attribute
{
	/// <summary>固有ID</summary>
	public int id;
	/// <summary>インデックス(NovelDataEditorにて設定)</summary>
	public int idx;
	/// <summary>コマンド名</summary>
	public string name;
	/// <summary>引数の数</summary>
	public int parameterCount;
	/// <summary>背景色</summary>
	public Color color;

	public NovelCommandEditorAttribute(NovelCommandType type)
	{
		this.id = (int)type;
		this.name = type.ToString();
		this.parameterCount = 0;
		this.color = Color.clear;
	}

	public NovelCommandEditorAttribute(NovelCommandType type, float r, float g, float b, float a)
	{
		this.id = (int)type;
		this.name = type.ToString();
		this.parameterCount = 0;
		this.color = new Color(r, g, b, a);
	}
}


/// <summary>
/// ノベルコマンド群のエディターをまとめたクラス
/// </summary>
public class NovelCommandEditor
{
	/// <summary>
	/// ノベルコマンドエディタのベース
	/// </summary>
	public abstract class NovelCommandPropertyDrawerBase
	{
		/// <summary>
		/// コマンドの高さリスト
		/// </summary>
		public List<float> heights;

		/// <summary>
		/// プロパティの描画
		/// OnDrawPropertiesの描画にかかった高さを計算している
		/// </summary>
		public void DrawProperties(int index, ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var height = position.y;
			OnDrawProperties(ref position, editor, serializedParameters, isActive, isFocused);
			height = position.y + position.height - height;

			if (heights != null)
			{
				if (heights.Count <= index) { return; }
				heights[index] = height;
			}
		}

		/// <summary>
		/// プロパティーの描画
		/// </summary>
		protected abstract void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused);
	}

	/// <summary>
	/// なし
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.None)]
	public class None : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// 注釈行(無視されるコマンド)
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.Comment, 0.3f, 0.7f, 0.3f, 0.5f, parameterCount = 1)]
	public class Comment : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedLabelName = serializedParameters.GetArrayElementAtIndex(0);
			var lineCount = serializedLabelName.stringValue.Count(c => c == '\n') + 1;

			if (1 < lineCount)
			{
				position.y += position.height;

				if (isActive)
				{
					position.height = NovelUtilityEditor.TextAreaSingleLineHeight * lineCount;
					serializedLabelName.stringValue = EditorGUI.TextArea(position, serializedLabelName.stringValue);
				}
				else
				{
					position.height = NovelUtilityEditor.TextAreaSingleLineHeight * lineCount;
					EditorGUI.LabelField(position, serializedLabelName.stringValue);
				}

				position.y += position.height;
				position.y -= NovelUtilityEditor.SingleLineHeight;
				position.height = NovelUtilityEditor.SingleLineHeight;
			}
			else
			{
				position.x += NovelDataEditor.CommandPropertyWidth;
				position.width -= NovelDataEditor.CommandPropertyWidth;

				if (isActive)
				{
					serializedLabelName.stringValue = EditorGUI.TextArea(position, serializedLabelName.stringValue);
				}
				else
				{
					EditorGUI.LabelField(position, serializedLabelName.stringValue);
				}

				position.x -= NovelDataEditor.CommandPropertyWidth;
				position.width += NovelDataEditor.CommandPropertyWidth;
			}
		}
	}

	/// <summary>
	/// ノベルコマンドのグループ化
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.Group, 0.4f, 0.3f, 0.3f, 0.5f, parameterCount = 20)]
	public class Group : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedGroupName = serializedParameters.GetArrayElementAtIndex(0);
			var serializedParameterCount = serializedParameters.GetArrayElementAtIndex(1);

			position.x += NovelDataEditor.CommandPropertyWidth;
			position.width -= NovelDataEditor.CommandPropertyWidth;
			serializedGroupName.stringValue = EditorGUI.TextField(position, "コマンド名", serializedGroupName.stringValue);
			position.x -= NovelDataEditor.CommandPropertyWidth;
			position.width += NovelDataEditor.CommandPropertyWidth;

			position.y += position.height;
			var parameterCount = EditorGUI.IntSlider(position, "引数", serializedParameterCount.stringValue.ParseInt(), 0, 6);
			serializedParameterCount.stringValue = parameterCount.ToString();

			if (parameterCount <= 0) { return; }

			const float TypeWidth = 0.25f;
			const float NameWidth = 0.75f;
			var width = position.width;
			var x = position.x;

			position.y += position.height;
			position.width = width * TypeWidth;
			EditorGUI.LabelField(position, "[型]");
			position.x += position.width;
			position.width = width * NameWidth;
			EditorGUI.LabelField(position, "[引数名]");

			position.x = x;
			position.width = width;

			for (int i = 0; i < parameterCount; ++i)
			{
				var serializedParameterType = serializedParameters.GetArrayElementAtIndex(i * 2 + 2);
				var serializedParameterName = serializedParameters.GetArrayElementAtIndex(i * 2 + 3);

				position.width = width * TypeWidth;	
				position.y += position.height;

				var typeIndex = EditorGUI.Popup(position, serializedParameterType.stringValue.ParseInt(), new string[] {
					"bool(真偽)",
					"int(整数)",
					"float(実数)",
					"string(文字)",
					"text(文章)",
				});
				serializedParameterType.stringValue = typeIndex.ToString();

				position.x += position.width;
				position.width = width * NameWidth;
				// 先頭に@をつける(引数かどうかの区別をつけやすくする為)
				if (0 < serializedParameterName.stringValue.Length && serializedParameterName.stringValue[0] != '@')
				{
					serializedParameterName.stringValue = serializedParameterName.stringValue.Insert(0, "@");
				}
				serializedParameterName.stringValue = EditorGUI.TextField(position, serializedParameterName.stringValue);

				position.x = x;
				position.width = width;
			}
		}
	}

	/// <summary>
	/// ノベルコマンドのグループ化終了
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.GroupEnd, 0.4f, 0.3f, 0.3f, 0.5f)]
	public class GroupEnd : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// ノベルコマンドのグループ実行
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.GroupRun, parameterCount = 13)]
	public class GroupRun : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedGroupName = serializedParameters.GetArrayElementAtIndex(0);

			SerializedProperty serializedGroupProperty;
			if (editor.groupDic.TryGetValue(serializedGroupName.stringValue, out serializedGroupProperty))
			{
				// グループ名
				position.x += NovelDataEditor.CommandPropertyWidth;
				position.width -= NovelDataEditor.CommandPropertyWidth;

				var index = System.Array.FindIndex(editor.groups, name => name == serializedGroupName.stringValue);
				index = EditorGUI.Popup(position, index, editor.groups);
				serializedGroupName.stringValue = editor.groups[index];

				position.x -= NovelDataEditor.CommandPropertyWidth;
				position.width += NovelDataEditor.CommandPropertyWidth;

				// グループ引数の表示
				var serializedGroupParameters = serializedGroupProperty.FindPropertyRelative("parameters");
				if (serializedGroupParameters.arraySize <= 1)
				{
					//Elementの削除/追加によってキャッシュしているSerializePropertyが壊れることがある
					editor.SetupCommandsList();
					return;
				}

				var serializedGroupParameterCount = serializedGroupParameters.GetArrayElementAtIndex(1);
				var groupParameterCount = serializedGroupParameterCount.stringValue.ParseInt();

				var serializedParameterCount = serializedParameters.GetArrayElementAtIndex(1);
				serializedParameterCount.stringValue = groupParameterCount.ToString();

				if (groupParameterCount <= 0) { return; }

				if (serializedGroupParameters.arraySize < groupParameterCount + 1)
				{
					//Elementの削除/追加によってキャッシュしているSerializePropertyが壊れることがある
					editor.SetupCommandsList();
					return;
				}


				for (int i = 0; i < groupParameterCount; ++i)
				{
					var serializedGroupParameterType = serializedGroupParameters.GetArrayElementAtIndex(i * 2 + 2);
					var serializedGroupParameterName = serializedGroupParameters.GetArrayElementAtIndex(i * 2 + 3);
					var serializedParameterName = serializedParameters.GetArrayElementAtIndex(i * 2 + 2);
					var serializedParameterValue = serializedParameters.GetArrayElementAtIndex(i * 2 + 3);
					serializedParameterName.stringValue = serializedGroupParameterName.stringValue;

					position.y += position.height;

					switch (serializedGroupParameterType.stringValue.ParseInt())
					{
					case 0: // bool
						serializedParameterValue.stringValue = EditorGUI.Toggle(
							position,
							serializedGroupParameterName.stringValue,
							serializedParameterValue.stringValue.ParseBool()).ToString();
						break;
					case 1: // int
						serializedParameterValue.stringValue = EditorGUI.IntField(
							position,
							serializedGroupParameterName.stringValue,
							serializedParameterValue.stringValue.ParseInt()).ToString();
						break;
					case 2: // float
						serializedParameterValue.stringValue = EditorGUI.FloatField(
							position,
							serializedGroupParameterName.stringValue,
							serializedParameterValue.stringValue.ParseFloat()).ToString();
						break;
					case 3: // string
						serializedParameterValue.stringValue = EditorGUI.TextField(
							position,
							serializedGroupParameterName.stringValue,
							serializedParameterValue.stringValue);
						break;
					case 4: // text
						EditorGUI.LabelField(position, serializedGroupParameterName.stringValue);
						position.y += position.height;
						position.height = NovelUtilityEditor.SingleLineHeight * 3;
						serializedParameterValue.stringValue = EditorGUI.TextArea(
							position,
							serializedParameterValue.stringValue);
						position.y += NovelUtilityEditor.SingleLineHeight * 2;
						position.height = NovelUtilityEditor.SingleLineHeight;
						break;
					}
				}
			}
			else
			{
				// 見つからないグループ名が入力されている場合
				// 大元のグループ名を変更してしまった、インクルードを消してしまった等
				// 複数の原因が絡むので、下記表示を行う
				// ・考えられる原因のエラー表示
				// ・本来のグループ名を入力してもらう

				position.x += NovelDataEditor.CommandPropertyWidth;
				position.width -= NovelDataEditor.CommandPropertyWidth;
				serializedGroupName.stringValue = EditorGUI.TextField(position, serializedGroupName.stringValue);
				position.x -= NovelDataEditor.CommandPropertyWidth;
				position.width += NovelDataEditor.CommandPropertyWidth;

				position.y += position.height;
				position.height = NovelUtilityEditor.SingleLineHeight * 3;
				EditorGUI.HelpBox(position,
					"設定されているグループ名が見つかりませんでした\n" +
					"・Groupのグループ名を変更した、あるいは削除した\n"+
					"・Includeのファイルを変更した、あるいは削除した", MessageType.Error);
			}
		}
	}

	/// <summary>
	/// テキストの書き込み
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.TextWrite, parameterCount = 1)]
	public class TextWrite : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			position.y += position.height;
			position.height += NovelUtilityEditor.SingleLineHeight;

			var serializedText = serializedParameters.GetArrayElementAtIndex(0);
			serializedText.stringValue = EditorGUI.TextArea(position, serializedText.stringValue);
		}
	}

	/// <summary>
	/// テキストのクリア
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.TextClear)]
	public class TextClear : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// テキストウィンドウの表示
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.TextShow)]
	public class TextShow : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// テキストウィンドウの非表示
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.TextHide)]
	public class TextHide : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// テキストのアニメーション設定
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.TextAnimationType, parameterCount = 1)]
	public class TextAnimationType : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedFilePath = serializedParameters.GetArrayElementAtIndex(0);

			position.y += position.height;
			serializedFilePath.stringValue = EditorGUI.TextField(position, "ファイル名", serializedFilePath.stringValue);
		}
	}

	/// <summary>
	/// テキストのアニメーション再生
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.TextAnimationPlay)]
	public class TextAnimationPlay : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// 名前の書き込み
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.NameWrite, parameterCount = 1)]
	public class NameWrite : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			position.y += position.height;
			var serializedName = serializedParameters.GetArrayElementAtIndex(0);
			serializedName.stringValue = EditorGUI.TextField(position, serializedName.stringValue);
		}
	}

	/// <summary>
	/// 名前のクリア
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.NameClear)]
	public class NameClear : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// 名前ウィンドウの表示
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.NameShow)]
	public class NameShow : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// 名前ウィンドウの非表示
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.NameHide)]
	public class NameHide : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// 名前ウィンドウのアニメーション再生
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.NameAnimationPlay)]
	public class NameAnimationPlay : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// 2Dイメージの表示
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.Image, parameterCount = 6)]
	public class Image : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedHandleName = serializedParameters.GetArrayElementAtIndex(0);
			var serializedFilePath = serializedParameters.GetArrayElementAtIndex(1);
			var serializedXRatio = serializedParameters.GetArrayElementAtIndex(2);
			var serializedYRatio = serializedParameters.GetArrayElementAtIndex(3);
			var serializedTime = serializedParameters.GetArrayElementAtIndex(4);
			var serializedIsStandBy = serializedParameters.GetArrayElementAtIndex(5);

			position.x += NovelDataEditor.CommandPropertyWidth;
			position.width -= NovelDataEditor.CommandPropertyWidth;

			var isStandBy = EditorGUI.ToggleLeft(position, "調整のみ(表示しない)", serializedIsStandBy.stringValue.ParseBool());
			serializedIsStandBy.stringValue = isStandBy.ToString();
			position.x -= NovelDataEditor.CommandPropertyWidth;
			position.width += NovelDataEditor.CommandPropertyWidth;
			position.y += position.height;

			serializedHandleName.stringValue = EditorGUI.TextField(position, "ハンドル名", serializedHandleName.stringValue);
			position.y += position.height;
			serializedFilePath.stringValue = EditorGUI.TextField(position, "ファイル名", serializedFilePath.stringValue);
			position.y += position.height;
			serializedXRatio.stringValue = EditorGUI.Slider(position, "X座標", serializedXRatio.stringValue.ParseFloat(), -1.0f, 1.0f).ToString();
			position.y += position.height;
			serializedYRatio.stringValue = EditorGUI.Slider(position, "Y座標", serializedYRatio.stringValue.ParseFloat(), -1.0f, 1.0f).ToString();
			position.y += position.height;

			if (isStandBy)
			{
				serializedTime.stringValue = "0";
				return;
			}

			serializedTime.stringValue = EditorGUI.FloatField(position, "表示時間(フェードイン)", serializedTime.stringValue.ParseFloat()).ToString();
		}
	}

	/// <summary>
	/// 2Dイメージの非表示
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.ImageRemove, parameterCount = 2)]
	public class ImageRemove : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedHandleName = serializedParameters.GetArrayElementAtIndex(0);
			var serializedTime = serializedParameters.GetArrayElementAtIndex(1);

			position.y += position.height;
			serializedHandleName.stringValue = EditorGUI.TextField(position, "ハンドル名", serializedHandleName.stringValue);
			position.y += position.height;
			serializedTime.stringValue = EditorGUI.FloatField(position, "表示時間(フェードアウト)", serializedTime.stringValue.ParseFloat()).ToString();
		}
	}

	/// <summary>
	/// 2Dイメージの切り替え
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.ImageChange, parameterCount = 4)]
	public class ImageChange : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedHandleName = serializedParameters.GetArrayElementAtIndex(0);
			var serializedFilePath = serializedParameters.GetArrayElementAtIndex(1);
			var serializedTime = serializedParameters.GetArrayElementAtIndex(2);
			var serializedIsSmooth = serializedParameters.GetArrayElementAtIndex(3);

			position.x += NovelDataEditor.CommandPropertyWidth;
			position.width -= NovelDataEditor.CommandPropertyWidth;

			var isRapid = EditorGUI.ToggleLeft(position, "即時切替(フェードなし)", serializedIsSmooth.stringValue.ParseBool());
			serializedIsSmooth.stringValue = isRapid.ToString();
			position.x -= NovelDataEditor.CommandPropertyWidth;
			position.width += NovelDataEditor.CommandPropertyWidth;
			position.y += position.height;

			serializedHandleName.stringValue = EditorGUI.TextField(position, "ハンドル名", serializedHandleName.stringValue);
			position.y += position.height;
			serializedFilePath.stringValue = EditorGUI.TextField(position, "ファイル名", serializedFilePath.stringValue);
			position.y += position.height;

			if (isRapid)
			{
				serializedTime.stringValue = "0";
				return;
			}

			serializedTime.stringValue = EditorGUI.FloatField(position, "切替時間(フェード)", serializedTime.stringValue.ParseFloat()).ToString();
		}
	}

	/// <summary>
	/// 背景イメージの切り替え
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.BackgroundImageChange, parameterCount = 4)]
	public class BackgroundImageChange : ImageChange
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedHandleName = serializedParameters.GetArrayElementAtIndex(0);
			var serializedFilePath = serializedParameters.GetArrayElementAtIndex(1);
			var serializedTime = serializedParameters.GetArrayElementAtIndex(2);
			var serializedIsSmooth = serializedParameters.GetArrayElementAtIndex(3);


			position.x += NovelDataEditor.CommandPropertyWidth;
			position.width -= NovelDataEditor.CommandPropertyWidth;

			var isRapid = EditorGUI.ToggleLeft(position, "即時切替(フェードなし)", serializedIsSmooth.stringValue.ParseBool());
			serializedIsSmooth.stringValue = isRapid.ToString();
			position.x -= NovelDataEditor.CommandPropertyWidth;
			position.width += NovelDataEditor.CommandPropertyWidth;
			position.y += position.height;

			//if (string.IsNullOrWhiteSpace(serializedHandleName.stringValue))
			//{
			//	serializedHandleName.stringValue = "Background";
			//}

			serializedHandleName.stringValue = EditorGUI.TextField(position, "ハンドル名", serializedHandleName.stringValue);
			position.y += position.height;
			serializedFilePath.stringValue = EditorGUI.TextField(position, "ファイル名", serializedFilePath.stringValue);
			position.y += position.height;

			if (isRapid)
			{
				serializedTime.stringValue = "0";
				return;
			}

			serializedTime.stringValue = EditorGUI.FloatField(position, "切替時間(フェード)", serializedTime.stringValue.ParseFloat()).ToString();
		}
	}

	/// <summary>
	/// ジャンプ
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.Jump, 0.3f, 0.3f, 0.4f, 0.5f, parameterCount = 1)]
	public class Jump : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedLabelName = serializedParameters.GetArrayElementAtIndex(0);

			position.x += NovelDataEditor.CommandPropertyWidth;
			position.width -= NovelDataEditor.CommandPropertyWidth;
			serializedLabelName.stringValue = EditorGUI.TextField(position, "ラベル名", serializedLabelName.stringValue);
			position.x -= NovelDataEditor.CommandPropertyWidth;
			position.width += NovelDataEditor.CommandPropertyWidth;
		}
	}

	/// <summary>
	/// ラベル
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.Label, 0.3f, 0.3f, 0.4f, 0.5f, parameterCount = 1)]
	public class Label : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedLabelName = serializedParameters.GetArrayElementAtIndex(0);

			position.x += NovelDataEditor.CommandPropertyWidth;
			position.width -= NovelDataEditor.CommandPropertyWidth;
			serializedLabelName.stringValue = EditorGUI.TextField(position, "ラベル名", serializedLabelName.stringValue);
			position.x -= NovelDataEditor.CommandPropertyWidth;
			position.width += NovelDataEditor.CommandPropertyWidth;
		}
	}

	/// <summary>
	/// 選択肢
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.Choices, 0.3f, 0.3f, 0.4f, 0.5f, parameterCount = 14)]
	public class Choices : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedFileName = serializedParameters.GetArrayElementAtIndex(0);
			var serializedButtonCount = serializedParameters.GetArrayElementAtIndex(1);

			position.y += position.height;
			serializedFileName.stringValue = EditorGUI.TextField(position, "ファイル名", serializedFileName.stringValue);
			position.y += position.height;

			var buttonCount = EditorGUI.IntSlider(position, "ボタン数", serializedButtonCount.stringValue.ParseInt(), 1, 6);
			serializedButtonCount.stringValue = buttonCount.ToString();
			position.y += position.height;

			for (int i = 0; i < buttonCount; ++i)
			{
				var serializedButtonName = serializedParameters.GetArrayElementAtIndex(i * 2 + 2);
				var serializedJumpLabel = serializedParameters.GetArrayElementAtIndex(i * 2 + 3);

				serializedButtonName.stringValue = EditorGUI.TextField(position, "選択肢" + (i + 1), serializedButtonName.stringValue);
				position.y += position.height;
				position.x += NovelDataEditor.CommandParameterPaddingWidth;
				position.width -= NovelDataEditor.CommandParameterPaddingWidth;
				serializedJumpLabel.stringValue = EditorGUI.TextField(position, "ジャンプ先", serializedJumpLabel.stringValue);
				position.y += position.height;
				position.x -= NovelDataEditor.CommandParameterPaddingWidth;
				position.width += NovelDataEditor.CommandParameterPaddingWidth;
			}
		}
	}

	/// <summary>
	/// 一定時間待ち
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.WaitTime, parameterCount = 1)]
	public class WaitTime : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedTime = serializedParameters.GetArrayElementAtIndex(0);

			position.x += NovelDataEditor.CommandPropertyWidth;
			position.width -= NovelDataEditor.CommandPropertyWidth;
			serializedTime.stringValue = EditorGUI.FloatField(position, "待機時間(秒)", serializedTime.stringValue.ParseFloat()).ToString();
			position.x -= NovelDataEditor.CommandPropertyWidth;
			position.width += NovelDataEditor.CommandPropertyWidth;
		}
	}

	/// <summary>
	/// 特定イベント待ち
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.WaitEvent)]
	public class WaitEvent : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// 入力待ち
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.Pause)]
	public class Pause : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
		}
	}

	/// <summary>
	/// 値の代入
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.ValueAssign, parameterCount = 2)]
	public class ValueAssign : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedValueName = serializedParameters.GetArrayElementAtIndex(0);
			var serializedParameter = serializedParameters.GetArrayElementAtIndex(1);

			var valueName = serializedValueName.stringValue;
			var parameter = serializedParameter.stringValue;

			position.y += position.height;
			EditorGUI.LabelField(position, string.Format("{0} ← {1}", valueName, parameter));

			position.y += position.height;
			position.width *= 0.5f;
			serializedValueName.stringValue = EditorGUI.TextField(position, serializedValueName.stringValue);

			// 先頭に$をつける(引数かどうかの区別をつけやすくする為)
			if (0 < serializedValueName.stringValue.Length && serializedValueName.stringValue[0] != '$')
			{
				serializedValueName.stringValue = serializedValueName.stringValue.Insert(0, "$");
			}

			position.x += position.width;
			serializedParameter.stringValue = EditorGUI.TextField(position, serializedParameter.stringValue);

			position.x -= position.width;
			position.width *= 2.0f;
		}
	}

	/// <summary>
	/// 値の加算
	/// </summary>
	[NovelCommandEditorAttribute(NovelCommandType.ValueAdd, parameterCount = 2)]
	public class ValueAdd : NovelCommandPropertyDrawerBase
	{
		protected override void OnDrawProperties(ref Rect position, NovelDataEditor editor, SerializedProperty serializedParameters, bool isActive, bool isFocused)
		{
			var serializedValueName = serializedParameters.GetArrayElementAtIndex(0);
			var serializedParameter = serializedParameters.GetArrayElementAtIndex(1);

			var valueName = serializedValueName.stringValue;
			var parameter = serializedParameter.stringValue;

			position.y += position.height;
			EditorGUI.LabelField(position, string.Format("{0} ← {0} + {1}", valueName, parameter));

			position.y += position.height;
			position.width *= 0.5f;
			serializedValueName.stringValue = EditorGUI.TextField(position, serializedValueName.stringValue);

			// 先頭に$をつける(引数かどうかの区別をつけやすくする為)
			if (0 < serializedValueName.stringValue.Length && serializedValueName.stringValue[0] != '$')
			{
				serializedValueName.stringValue = serializedValueName.stringValue.Insert(0, "$");
			}

			position.x += position.width;
			serializedParameter.stringValue = EditorGUI.FloatField(position, serializedParameter.stringValue.ParseFloat()).ToString();

			position.x -= position.width;
			position.width *= 2.0f;
		}
	}
}
