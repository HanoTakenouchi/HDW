using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// ノベルデータ
/// </summary>
[CreateAssetMenu(fileName = "NovelData", menuName = "ScriptableObject/NovelData")]
public partial class NovelData : ScriptableObject
{
	/// <summary>
	/// ノベルデータの説明(注釈)
	/// </summary>
	[SerializeField]
	[Tooltip("ノベルデータの説明等(自由欄)")]
	public string comment;

	/// <summary>
	/// ノベルコマンド
	/// </summary>
	[SerializeField]
	public List<Command> commands = new List<Command>();


	/// <summary>
	/// コマンドデータ
	/// </summary>
	[System.Serializable]
	public class Command
	{
		/// <summary>
		/// コマンドID
		/// </summary>
		[SerializeField]
		public int id;

		/// <summary>
		/// コマンド引数
		/// </summary>
		[SerializeField]
		public string[] parameters;
	}
}
