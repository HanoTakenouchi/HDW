using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// ノベルシステム
/// ・ノベルデータの読み込み
/// ・ノベル操作(再生/途中再生/ログ)
/// </summary>
[RequireComponent(typeof(NovelView))]
public class NovelSystem : MonoBehaviour
{
	/// <summary>
	/// 自動再生
	/// </summary>
	[SerializeField]
	private bool isAutoRunning = true;

	/// <summary>
	/// 実行するノベルデータ
	/// </summary>
	[SerializeField]
	public NovelData novelData;

	/// <summary>
	/// コマンドタイプと型のリスト
	/// </summary>
	[NonSerialized]
	public Dictionary<int, Type> commandTypeDic = new Dictionary<int, Type>();

	/// <summary>
	/// 履歴
	/// </summary>
	[NonSerialized]
	public NovelHistory history = new NovelHistory(50);

	/// <summary>
	/// 実行
	/// </summary>
	private NovelExecuter executer;


	/// <summary>
	/// Unity Event Awake
	/// </summary>
	private void Awake()
	{
		// NovelCommandのインナークラスをすべて取得
		var nestedType = typeof(NovelCommand).GetNestedTypes(System.Reflection.BindingFlags.Public);

		// コマンド以外のクラスも含まれている為、除外しつつコマンドを集計
		commandTypeDic = nestedType
			.Where(type => 0 < type.GetCustomAttributes(typeof(NovelCommandAttribute), false).Length)
			.Select(type => type)
			.ToDictionary(type => ((NovelCommandAttribute)type.GetCustomAttributes(typeof(NovelCommandAttribute), false).First()).id);

		// ウィンドウは非表示にしておく(フェードがないとチラ見してしまう為)
		var view = GetComponent<NovelView>();
		view.TextWindowImage.gameObject.SetActive(false);
		view.NameTextWindowImage.gameObject.SetActive(false);
	}

	/// <summary>
	/// Unity Event Start
	/// </summary>
	private void Start()
	{
		if (isAutoRunning)
		{
			Execute();
		}
	}

	/// <summary>
	/// 巻き戻し
	/// </summary>
	public void Rollback(int historyID)
	{
		// コマンド実行処理は止める
		StopAllCoroutines();
		// Undo
		StartCoroutine(RollbackCoroutine(historyID));
	}

	/// <summary>
	/// ノベルコマンドUndo処理コルーチン
	/// </summary>
	private IEnumerator RollbackCoroutine(int historyID)
	{
		var histories = history.GetHistories(historyID);

		// 履歴データにあるcommandを順番に呼び出す
		for (int i = 0; i < histories.Length; ++i)
		{
			var history = histories[i];

			// 次にUndo処理するコマンドデータ
			// sharedの情報は実行するコマンドに合わせて更新する必要がある
			executer.sharedData.command = executer.sharedData.data.commands[history.index];
			executer.sharedVariable.index = history.index;

			// 同フレームで処理できるように自分でコルーチンを回す
			var coroutine = history.command.Undo(executer.sharedData, executer.sharedVariable);
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

		// 履歴削除
		// 全部削除してもいいと思うけども、あえてIDのところまでにしてみる
		history.Remove(historyID);

		// グループ呼び出し履歴を適応
		var groupHistory = history.GetGroups(historyID);
		executer.sharedVariable.groupHistory = groupHistory;

		// グループが積まれていれば、そのインデックスからスタート
		var group = groupHistory.TryPeek();
		if (group != null)
		{
			executer.sharedVariable.index = group.index;
		}

		// 再実行
		// データは引継ぎ
		Execute(executer.sharedData, executer.sharedVariable);
		yield break;
	}

	/// <summary>
	/// ノベル処理開始
	/// </summary>
	public void Execute(NovelCommand.SharedData data = null, NovelCommand.SharedVariable variable = null)
	{
		if (variable == null)
		{
			variable = new NovelCommand.SharedVariable()
			{
				index = 0,
				handles = new Dictionary<string, GameObject>(),
				values = new Dictionary<string, string>(),
				groupValues = new Dictionary<string, string>(),
				groupHistory = new Queue<NovelHistory.GroupData>(),
			};
		}

		if (data == null)
		{
			data = new NovelCommand.SharedData()
			{
				data = novelData,
				system = this,
				view = GetComponent<NovelView>(),
			};
		}

		if (executer == null)
		{
			executer = new NovelExecuter(novelData, commandTypeDic);
		}

		StartCoroutine(RunCoroutine(data, variable));
	}

	/// <summary>
	/// ノベルコマンドDo処理コルーチン
	/// </summary>
	private IEnumerator RunCoroutine(NovelCommand.SharedData data, NovelCommand.SharedVariable variable)
	{
		yield return executer.SetupCoroutine(data);
		yield return executer.RunCoroutine(variable, data.data.commands.Count);
		yield break;
	}

	/// <summary>
	/// イベント処理
	/// </summary>
	public void Event(int intParameter)
	{
		Event(new NovelCommand.EventData() { intParameter = intParameter, });
	}

	/// <summary>
	/// イベント処理
	/// </summary>
	public void Event(float floatParameter)
	{
		Event(new NovelCommand.EventData() { floatParameter = floatParameter, });
	}

	/// <summary>
	/// イベント処理
	/// </summary>
	public void Event(string stringParameter)
	{
		Event(new NovelCommand.EventData() { stringParameter = stringParameter, });
	}

	/// <summary>
	/// イベント処理
	/// </summary>
	public void Event(NovelCommand.EventData data)
	{
		if (executer == null) { return; }
		if (data == null) { return; }

		StartCoroutine(executer.EventCoroutine(data));
	}
}
