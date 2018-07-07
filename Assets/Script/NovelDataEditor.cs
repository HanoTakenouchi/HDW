using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;


/// <summary>
/// NovelDataEditor
/// NovelDataをInspectorで使いやすくするためのエディタ
/// </summary>
[CustomEditor(typeof(NovelData))]
public class NovelDataEditor : Editor
{
	/// <summary>
	/// コマンド番号表示の幅
	/// </summary>
	public const float CommandIndexWidth = 30.0f;
	/// <summary>
	/// コマンド表示の幅
	/// </summary>
	public const float CommandPropertyWidth = 150.0f;
	/// <summary>
	/// コマンド引数表示の幅
	/// </summary>
	public const float CommandParameterPaddingWidth = 40.0f;

	/// <summary>
	/// コマンド名
	/// </summary>
	private string[] commands = null;
	/// <summary>
	/// グループ名
	/// </summary>
	public string[] groups = null;
	/// <summary>
	/// 属性
	/// </summary>
	private NovelCommandEditorAttribute[] attributes = null;
	/// <summary>
	/// ID索引用属性
	/// </summary>
	private Dictionary<int, NovelCommandEditorAttribute> attributeDic = null;
	/// <summary>
	/// コマンド引数描画エディタ
	/// </summary>
	private Dictionary<int, NovelCommandEditor.NovelCommandPropertyDrawerBase> drawers = null;
	/// <summary>
	/// 高さリスト
	/// </summary>
	private List<float> heights = new List<float>();
	/// <summary>
	/// グループコマンド索引
	/// </summary>
	public Dictionary<string, SerializedProperty> groupDic = null;
	/// <summary>
	/// 並び変え可能なリスト
	/// </summary>
	private ReorderableList reorderableList;


	/// <summary>
	/// セットアップ
	/// </summary>
	private void Setup()
	{
		// リビルドすると自然にnullになる(他にいい方法があれば…)
		if (attributes != null) { return; }

		// NovelCommandのインナークラスをすべて取得
		var nestedType = typeof(NovelCommandEditor).GetNestedTypes(System.Reflection.BindingFlags.Public);

		// コマンド以外のクラスも含まれている為、除外しつつNovelCommandEditorAttributeを集計
		attributes = nestedType
			.Where(type => 0 < type.GetCustomAttributesData().Count)
			.SelectMany(type => type.GetCustomAttributes(typeof(NovelCommandEditorAttribute), false))
			.Cast<NovelCommandEditorAttribute>()
			.ToArray();

		// コマンド描画のIDとインスタンスのペアを生成
		drawers = nestedType
			.Where(type => 0 < type.GetCustomAttributesData().Count)
			.Select(type => new {
				instance = System.Activator.CreateInstance(type) as NovelCommandEditor.NovelCommandPropertyDrawerBase,
				id = attributes.First(attr => type.GetCustomAttributes(typeof(NovelCommandEditorAttribute), false).Contains(attr)).id, })
			.ToDictionary(pair => pair.id, pair => pair.instance);

		// コマンド描画に高さリストを渡す
		foreach (var drawer in drawers.Values)
		{
			drawer.heights = heights;
		}

		// インデックスはAttributeに渡す
		for (int i = 0; i < attributes.Length; ++i)
		{
			attributes[i].idx = i;
		}

		// ID索引用属性作成
		attributeDic = attributes.ToDictionary(attr => attr.id);

		// コマンドリスト生成
		SetupCommandsList();
	}

	/// <summary>
	/// コマンドリストのセットアップ
	/// Group等で再調整が都度入る為、Setupとは分離
	/// </summary>
	public void SetupCommandsList()
	{
		Setup();

		// コマンド名配列の作成(attributesとインデックスを一致させる)
		var commandList = attributeDic.Select(pair => pair.Value.name).ToList();

		// グループコマンド配列の作成(attributesのインデックスからはみ出る)
		groupDic = new Dictionary<string, SerializedProperty>();
		var serializedCommands = serializedObject.FindProperty("commands");
		for (int i = 0; i < serializedCommands.arraySize; ++i)
		{
			var serializedCommand = serializedCommands.GetArrayElementAtIndex(i);
			var id = serializedCommand.FindPropertyRelative("id").intValue;
			if (id != (int)NovelCommandType.Group) { continue; }

			var parameters = serializedCommand.FindPropertyRelative("parameters");
			if (parameters.arraySize <= 0) { continue; }
			var groupName = parameters.GetArrayElementAtIndex(0).stringValue;
			if (string.IsNullOrWhiteSpace(groupName)) { continue; }

			groupDic[groupName] = serializedCommand;
		}

		groups = groupDic.Keys.ToArray();
		commandList.AddRange(groupDic.Keys);
		commands = commandList.ToArray();
	}

	/// <summary>
	/// Unity Event OnEnable
	/// </summary>
	private void OnEnable()
	{
		var serializedCommands = serializedObject.FindProperty("commands");

		if (heights.Count != serializedCommands.arraySize)
		{
			heights.AddRange(new float[serializedCommands.arraySize]);
		}

		reorderableList = new ReorderableList(
			serializedObject,
			serializedCommands
		);
		reorderableList.drawHeaderCallback = (rect) =>
		{
			EditorGUI.LabelField(rect, "コマンド");
		};
		reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
		{
			var element = serializedCommands.GetArrayElementAtIndex(index);
			OnDrawCommand(index, rect, element, isActive, isFocused);
		};
		reorderableList.onAddCallback += (list) =>
		{
			serializedCommands.arraySize++;
			heights.Add(NovelUtilityEditor.SingleLineHeight);
		};
		reorderableList.onRemoveCallback += (list) =>
		{
			serializedCommands.arraySize--;
			heights.RemoveAt(0);
		};
		reorderableList.elementHeightCallback += (index) =>
		{
			if (heights.Count <= index) { return NovelUtilityEditor.SingleLineHeight; }
			return heights[index];
		};
	}

	/// <summary>
	/// Unity Event OnInspectorGUI
	/// </summary>
	public override void OnInspectorGUI()
	{
		Setup();

		var property = serializedObject.FindProperty("comment");
		property.stringValue = EditorGUILayout.TextArea(property.stringValue);

		reorderableList.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}

	/// <summary>
	/// コマンドの引数設定エディタ描画
	/// </summary>
	/// <param name="index"></param>
	/// <param name="position"></param>
	/// <param name="property"></param>
	/// <param name="label"></param>
	private void OnDrawCommand(int index, Rect position, SerializedProperty property, bool isActive, bool isFocused)
	{
		EditorGUI.BeginProperty(position, GUIContent.none, property);
		//using (new EditorGUI.PropertyScope(position, GUIContent.none, property))
		{
			// 同じIDのAttribute(Editor側)の取得
			NovelCommandEditorAttribute selectedAttribute;
			int selectedAttributeIndex = 0;
			var serializedID = property.FindPropertyRelative("id");
			if (attributeDic.TryGetValue(serializedID.intValue, out selectedAttribute))
			{
				selectedAttributeIndex = selectedAttribute.idx;
				if (selectedAttributeIndex < 0) { selectedAttributeIndex = 0; }
			}

			// 色付け
			if (selectedAttribute != null)
			{
				if (0.0f < selectedAttribute.color.a)
				{
					Handles.DrawSolidRectangleWithOutline(
						position,
						selectedAttribute.color,
						selectedAttribute.color);
				}
			}

			position.height = NovelUtilityEditor.SingleLineHeight;

			// コマンド番号
			var commandPosition = new Rect(position);
			commandPosition.width = CommandIndexWidth;
			EditorGUI.LabelField(commandPosition, index.ToString().PadLeft(4) + ":");

			// コマンド
			commandPosition.x += CommandIndexWidth;
			commandPosition.width = CommandPropertyWidth;
			selectedAttributeIndex = EditorGUI.Popup(commandPosition, selectedAttributeIndex, commands);

			var isSelectedGroup = (attributes.Length <= selectedAttributeIndex);
			serializedID.intValue = (isSelectedGroup) ?
				(int)NovelCommandType.GroupRun : attributes[selectedAttributeIndex].id;

			// 引数
			position.x += CommandParameterPaddingWidth;
			position.width -= CommandParameterPaddingWidth;
			var serializedParameters = property.FindPropertyRelative("parameters");
			if (selectedAttribute != null && serializedParameters != null)
			{
				// 引数個数の調整
				serializedParameters.arraySize = selectedAttribute.parameterCount;

				// グループ選択なら名前を自動入力
				if (isSelectedGroup)
				{
					if (serializedParameters.arraySize <= 0)
					{
						serializedParameters.InsertArrayElementAtIndex(0);
					}

					var groupName = commands[selectedAttributeIndex];
					serializedParameters.GetArrayElementAtIndex(0).stringValue = groupName;
				}

				// 引数描画エディタの取得
				NovelCommandEditor.NovelCommandPropertyDrawerBase drawer;
				if (drawers.TryGetValue(selectedAttribute.id, out drawer))
				{
					drawer.DrawProperties(index, ref position, this, serializedParameters, isActive, isFocused);
				}
			}
			position.x -= CommandParameterPaddingWidth;
			position.width += CommandParameterPaddingWidth;
		}

		EditorGUI.EndProperty();
	}
}
