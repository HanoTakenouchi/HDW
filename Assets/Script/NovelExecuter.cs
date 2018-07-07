using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// ノベル実行
/// ・ノベルデータの読み込み
/// </summary>
public class NovelExecuter
{
	/// <summary>
	/// コマンド間共有変数
	/// </summary>
	public NovelCommand.SharedVariable sharedVariable { get; private set; }

	/// <summary>
	/// コマンド間共有データ
	/// </summary>
	public NovelCommand.SharedData sharedData { get; private set; }

	/// <summary>
	/// 実行するノベルデータ
	/// </summary>
	private NovelData novelData;

	/// <summary>
	/// コマンドタイプと型のリスト
	/// </summary>
	private Dictionary<int, Type> commandTypeDic = new Dictionary<int, Type>();

	/// <summary>
	/// 実行中のコマンド(イベント通知に用いる)
	/// </summary>
	private NovelCommand.NovelCommandInterface command;


	/// <summary>
	/// コンストラクタ(NovelCommandからTypeを取得)
	/// </summary>
	public NovelExecuter(NovelData data)
	{
		this.novelData = data;

		// NovelCommandのインナークラスをすべて取得
		var nestedType = typeof(NovelCommand).GetNestedTypes(System.Reflection.BindingFlags.Public);

		// コマンド以外のクラスも含まれている為、除外しつつコマンドを集計
		commandTypeDic = nestedType
			.Where(type => 0 < type.GetCustomAttributes(typeof(NovelCommandAttribute), false).Length)
			.Select(type => type)
			.ToDictionary(type => ((NovelCommandAttribute)type.GetCustomAttributes(typeof(NovelCommandAttribute), false).First()).id);
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="data"></param>
	/// <param name="commandTypeDic"></param>
	public NovelExecuter(NovelData data, Dictionary<int, Type> commandTypeDic)
	{
		this.novelData = data;
		this.commandTypeDic = commandTypeDic;
	}

	/// <summary>
	/// イベント処理
	/// </summary>
	public IEnumerator EventCoroutine(NovelCommand.EventData data)
	{
		if (command == null) { yield break; }
		if (sharedData == null) { yield break; }
		if (sharedVariable == null) { yield break; }
		if (data == null) { yield break; }

		yield return command.Event(sharedData, sharedVariable, data);
	}

	/// <summary>
	/// セットアップコルーチン
	/// Include, Import, Groupコマンドの必要情報を集めます
	/// </summary>
	public IEnumerator SetupCoroutine(NovelCommand.SharedData data)
	{
		if (data == null) { yield break; }

		sharedData = data;
		sharedData.meta = new NovelMetaData();
		sharedData.meta.groupDic = new Dictionary<string, NovelMetaData.GroupData>();

		// セットアップ用に専用の変数を用意
		// indexはRunCoroutine時とは別物である為(開始位置に関わらずSetupは全部見る)
		var variable = new NovelCommand.SharedVariable()
		{
			values =  new Dictionary<string, string>(),
			handles = new Dictionary<string, UnityEngine.GameObject>(),
			index = 0,
		};

		// セットアップが必要なコマンド
		var setupIDs = new int[] {
			(int)NovelCommandType.Include,
			(int)NovelCommandType.Import,
			(int)NovelCommandType.Group,
		};

		while (variable.index < novelData.commands.Count)
		{
			// 次に処理するコマンドデータ
			// 各コマンドはここから引数を取得します
			sharedData.command = novelData.commands[variable.index];

			// セットアップが必要なコマンドのみ実行
			if (sharedData.command.id.IsAny(setupIDs))
			{
				// コマンドデータのIDから型を取得
				Type commandType;
				if (commandTypeDic.TryGetValue(sharedData.command.id, out commandType))
				{
					// 型からインスタンスを生成
					// このインスタンスはUndoを簡易的に行う為のものです
					command = Activator.CreateInstance(commandType) as NovelCommand.NovelCommandInterface;

					// 必ず1フレーム消費するので、これだとシビアなタイミングがまずい
					//yield return command.Do(sharedData, sharedVariable);

					// 同フレームで処理できるように自分でコルーチンを回す
					var coroutine = command.Do(sharedData, variable);
					while (coroutine.MoveNext())
					{
						if (coroutine.Current != null)
						{
							// 何かしらが返ってきたら同フレームを保証する必要はなさそう
							var nestCoroutine = coroutine.Current as IEnumerator;
							if (nestCoroutine != null)
							{
								yield return nestCoroutine;
							}
						}

						yield return null;
					}
				}
			}

			variable.index++;
		}

		yield break;
	}

	/// <summary>
	/// 実行コルーチン
	/// </summary>
	public IEnumerator RunCoroutine(NovelCommand.SharedVariable variable, int endIndex)
	{
		if (variable == null) { yield break; }
		if (sharedData == null) { yield break; }

		sharedVariable = variable;

		// handlesが存在する場合は引き継いでいる想定
		// (最初には存在しなかった余計なオブジェクトも取得してしまうので処理しない)
		if (sharedVariable.handles.Count() <= 0)
		{
			// Scene上のGameObjectをハンドルに登録
			foreach (var pair in sharedData.view.Canvases.GetComponentsInChildren<UnityEngine.Transform>(true))
			{
				sharedVariable.handles[pair.name] = pair.gameObject;
			}
		}

		// 履歴登録が不要なID
		var noHistoryID = new int[] {
			(int)NovelCommandType.Import,
			(int)NovelCommandType.GroupRun,
		};

		while (sharedVariable.index < novelData.commands.Count && sharedVariable.index <= endIndex)
		{
			// 次に処理するコマンドデータ
			// 各コマンドはここから引数を取得します
			sharedData.command = novelData.commands[sharedVariable.index];

			// コマンドデータのIDから型を取得
			Type commandType;
			if (commandTypeDic.TryGetValue(sharedData.command.id, out commandType))
			{
				// 型からインスタンスを生成
				// このインスタンスはUndoを簡易的に行う為のものです
				command = Activator.CreateInstance(commandType) as NovelCommand.NovelCommandInterface;

				// 履歴保存
				if (!sharedData.command.id.IsAny(noHistoryID))
				{
					sharedData.system.history.Add(
						sharedData,
						sharedVariable,
						command);
				}

				// 必ず1フレーム消費するので、これだとシビアなタイミングがまずい
				//yield return command.Do(sharedData, sharedVariable);

				// 同フレームで処理できるように自分でコルーチンを回す
				var coroutine = command.Do(sharedData, sharedVariable);
				while (coroutine.MoveNext())
				{
					if (coroutine.Current != null)
					{
						// 何かしらが返ってきたら同フレームを保証する必要はなさそう
						var nestCoroutine = coroutine.Current as IEnumerator;
						if (nestCoroutine != null)
						{
							yield return nestCoroutine;
						}
					}

					yield return null;
				}
			}

			sharedVariable.index++;
		}

		yield break;
	}
}
