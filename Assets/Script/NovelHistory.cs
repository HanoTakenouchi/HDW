using System.Collections.Generic;


/// <summary>
/// ノベル履歴
/// </summary>
public class NovelHistory
{
	/// <summary>
	/// 最大履歴数
	/// </summary>
	private readonly int MaxHistoryCount;

	/// <summary>
	/// ログデータ
	/// </summary>
	public class LogData
	{
		/// <summary>
		/// 履歴ID
		/// (実行順にインクリメントされた番号 = Undoはこれを辿ります)
		/// </summary>
		public int historyID;

		/// <summary>
		/// 発言者名
		/// </summary>
		public string name;

		/// <summary>
		/// テキスト
		/// </summary>
		public string text;
	}

	/// <summary>
	/// 履歴データ
	/// </summary>
	public class HistoryData
	{
		/// <summary>
		/// 履歴ID
		/// (実行順にインクリメントされた番号 = Undoはこれを辿ります)
		/// </summary>
		public int historyID;

		/// <summary>
		/// ノベルデータコマンドのインデックス
		/// </summary>
		public int index;

		/// <summary>
		/// コマンド
		/// </summary>
		public NovelCommand.NovelCommandInterface command;
	}

	/// <summary>
	/// グループデータ(呼び出し履歴)
	/// </summary>
	public class GroupData
	{
		/// <summary>
		/// 履歴ID
		/// (実行順にインクリメントされた番号 = Undoはこれを辿ります)
		/// </summary>
		public int historyID;

		/// <summary>
		/// 開始かどうか(ListへのPush/Popを保存)
		/// </summary>
		public bool isRun;

		/// <summary>
		/// インデックス(GroupRun時)
		/// </summary>
		public int index;
	}

	/// <summary>
	/// ログ(表示用)
	/// </summary>
	private List<LogData> logs;

	/// <summary>
	/// コマンド履歴
	/// </summary>
	private List<HistoryData> histories;

	/// <summary>
	/// グループ呼び出しスタック
	/// </summary>
	private List<GroupData> groups;


	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="maxLogCount">保存する最大履歴数</param>
	public NovelHistory(int maxLogCount)
	{
		MaxHistoryCount = maxLogCount;
		logs = new List<LogData>(maxLogCount);
		histories = new List<HistoryData>(maxLogCount);
		groups = new List<GroupData>();
	}

	/// <summary>
	/// 現在の履歴IDの取得
	/// </summary>
	/// <returns></returns>
	public int GetCurrentHistoryID()
	{
		return (histories.Count <= 0) ? 0 : histories[0].historyID;
	}

	/// <summary>
	/// ログ取得(実行時間が新しい順に注意)
	/// </summary>
	public LogData[] GetLogs()
	{
		// 先頭は入力中データの為、テキストが空なら除外
		return logs
			.FindAll(data => !string.IsNullOrEmpty(data.text))
			.ToArray();
	}

	/// <summary>
	/// 履歴取得<br/>
	/// 最新～historyIDまでの履歴を取得<br/>
	/// (実行時間が新しい順に注意)
	/// </summary>
	/// <param name="historyID">指定履歴ID分も取得対象)</param>
	/// <returns></returns>
	public HistoryData[] GetHistories(int historyID)
	{
		return histories.FindAll(data => historyID <= data.historyID).ToArray();
	}

	/// <summary>
	/// グループ呼び出し履歴の取得<br/>
	/// 最古～historyIDまでの履歴を取得<br/>
	/// グループのネストの数だけQueueに積まれます<br/>
	/// </summary>
	/// <param name="historyID">指定履歴ID分も取得対象)</param>
	/// <returns></returns>
	public Queue<GroupData> GetGroups(int historyID)
	{
		// Push/Popの組み合わせから、必要なPush情報の個数を取得
		var pushCount = groups.FindAll(d => d.historyID <= historyID);
		// 最新順なので最古順にする
		pushCount.Reverse();

		// Push/Popを組み合わせていき、残ったPushを探す
		var list = new List<GroupData>();
		for (int i = 0; i < pushCount.Count; ++i)
		{
			var history = pushCount[i];
			if (history.isRun)
			{
				// Pushは末尾にAdd
				list.Add(history);
			}
			else
			{
				// Remove状況によってはPushPopから始まる可能性もある
				// その際のPopは無視して問題ない
				if (list.Count <= 0) { continue; }
				// Popなので末尾からRemove
				list.RemoveAt(list.Count - 1);
			}
		}

		return new Queue<GroupData>(list);
	}

	/// <summary>
	/// グループ呼び出し履歴追加
	/// </summary>
	public GroupData PeekGroup()
	{
		return (groups.Count <= 0) ? null : groups[0];
	}

	/// <summary>
	/// グループ呼び出し履歴追加
	/// </summary>
	public void PushGroup(bool isRun, int index)
	{
		groups.Insert(0, new GroupData()
		{
			historyID = 0 < histories.Count ? histories[0].historyID : 0,
			isRun = isRun,
			index = index,
		});
	}

	/// <summary>
	/// グループ呼び出し履歴削除
	/// </summary>
	public GroupData PopGroup()
	{
		if (groups.Count <= 0) { return null; }
		var group = groups[0];
		groups.RemoveAt(0);
		return group;
	}

	/// <summary>
	/// 履歴追加
	/// </summary>
	public void Add(NovelCommand.SharedData data, NovelCommand.SharedVariable variable, NovelCommand.NovelCommandInterface commandInstance)
	{
		// 履歴追加
		histories.Insert(0, new HistoryData()
		{
			historyID = GetCurrentHistoryID() + 1,
			index = variable.index,
			command = commandInstance,
		});

		// 初回データを用意(TextClear時に履歴追加したい為)
		if (logs.Count <= 0)
		{
			logs.Insert(0, new LogData());
			logs[0].historyID = 0;
		}

		// コマンドID毎に処理
		var commandData = data.data.commands[variable.index];
		switch (commandData.id)
		{
		case (int)NovelCommandType.NameWrite:
			{
				logs[0].name += variable.FindValue(commandData.parameters[0]);
			}
			break;

		case (int)NovelCommandType.NameClear:
			{
				logs[0].name = "";
			}
			break;

		case (int)NovelCommandType.TextWrite:
			{
				logs[0].text += variable.FindValue(commandData.parameters[0]);
			}
			break;

		case (int)NovelCommandType.TextClear:
			{
				if (MaxHistoryCount <= logs.Count)
				{
					// 古い順で履歴の削除
					var log = logs[MaxHistoryCount - 1];
					logs.RemoveAt(MaxHistoryCount - 1);
					histories.RemoveAll(d => d.historyID <= log.historyID);
					//groups.RemoveAll(d => d.historyID <= log.historyID);
					
					// groupsはGroup/GrouopEndコマンドの巻き戻しがある為徐々に増えていきます。
					// 古いデータを消す為の処理をコメントアウトしていますが、この処理は正しく動作しません。
					// GroupコマンドはUndoの際に開始位置を把握する為、単純に古いhistoryIDを消すとGroupのPush/Popが壊れます。
					// GroupEndコマンドを消さないとGroupコマンドは消せません。
					// この辺りは都合により未実装なので、執筆者githubより最新版をご確認ください。
				}

				// 履歴を残すべきデータがあるかどうか確認
				if (!string.IsNullOrEmpty(logs[0].name) ||
					!string.IsNullOrEmpty(logs[0].text))
				{
					logs.Insert(0, new LogData()
					{
						// historyIDはTextClear時の履歴リストの番号
						historyID = histories[0].historyID,
						// 名前は前回のものを引き継ぐ
						name = logs[0].name,
						text = "",
					});
				}
			}
			break;
		}
	}

	/// <summary>
	/// 新しい順で履歴削除
	/// </summary>
	/// <param name="historyID">指定インデックス分の削除(引数数値も削除対象)</param>
	public void Remove(int historyID = -1)
	{
		logs.RemoveAll(d => historyID <= d.historyID);
		histories.RemoveAll(d => historyID <= d.historyID);
		groups.RemoveAll(d => historyID <= d.historyID);
	}
}
