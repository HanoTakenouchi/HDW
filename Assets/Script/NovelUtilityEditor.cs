using UnityEditor;


/// <summary>
/// ノベル用ユーティリティ（便利メソッド置き場）
/// </summary>
public static class NovelUtilityEditor
{
	/// <summary>singleLineHeight</summary>
	static public float SingleLineHeight = EditorGUIUtility.singleLineHeight;

	/// <summary>singleLineHeightが微妙に一致していなかったので微調整</summary>
	static public float TextAreaSingleLineHeight = EditorGUIUtility.singleLineHeight - 2.0f;

}
