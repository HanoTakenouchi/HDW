using System.Collections.Generic;


/// <summary>
/// ノベルメタデータ
/// NovelExecuter.Setup時に生成されるデータ
/// ・Includeのコマンド読み込み（未実装）
/// ・Importのコマンド読み込み（未実装）
/// ・Groupのジャンプ情報
/// </summary>
public class NovelMetaData
{
	/// <summary>
	/// グループデータ
	/// </summary>
	public class GroupData
	{
		/// <summary>
		/// ノベルデータ
		/// </summary>
		public NovelData data;

		/// <summary>
		/// 開始インデックス
		/// </summary>
		public int startIndex;

		/// <summary>
		/// 終了インデックス
		/// </summary>
		public int endIndex;
	}

	/// <summary>
	/// グループデータ
	/// </summary>
	public Dictionary<string, GroupData> groupDic = new Dictionary<string, GroupData>();

}
