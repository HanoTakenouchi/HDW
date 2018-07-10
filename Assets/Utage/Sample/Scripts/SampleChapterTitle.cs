// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using Utage;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 章別のDLサンプル
/// DLするかどうかでボタンを変える（実際には併用することはないと思われる）
/// </summary>
[AddComponentMenu("Utage/ADV/Examples/ChapterTitle")]
public class SampleChapterTitle : MonoBehaviour
{

	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>()); } }
	[SerializeField]
	protected AdvEngine engine;

	public UtageUguiTitle title;
	public UtageUguiLoadWait loadWait;
	public UtageUguiMainGame mainGame;
	public List<string> chapterUrlList = new List<string>();
	public List<string> startLabel = new List<string>();

	public bool resetParam = true;
	public bool readSystemSaveData = true;
	public bool readAutoSaveDataParamOnly = false;

	void Start()
	{
	}


	public void OnClickDownLoadChpater(int chapterIndex)
	{
		StartCoroutine(LoadChaptersAsync(chapterIndex));
	}

	IEnumerator LoadChaptersAsync(int chapterIndex)
	{
		//今のパラメーターをバイナリデータとしてとっておく
		//パラメーターをリセットせずに章を追加ロードしたいときに
		byte[] bufferDefaultParam = null;
		byte[] bufferSystemParam = null;
		if (!resetParam)
		{
			bufferDefaultParam = BinaryUtil.BinaryWrite((writer)=> engine.Param.Write(writer,AdvParamData.FileType.Default));
			bufferSystemParam = BinaryUtil.BinaryWrite((writer) => engine.Param.Write(writer, AdvParamData.FileType.System));
		}

		//指定した章よりも前の章はロードする必要がある
		for (int i = 0; i < chapterIndex + 1; ++i )
		{
			string url = chapterUrlList[i];
			//もう設定済みならロードしない
			if (this.Engine.ExitsChapter(url)) continue;

			//ロード自体はこれだけ
			//ただし、URLは
			// http://madnesslabo.net/Utage3Chapter/Windows/chapter2.chapter.asset
			//のように、Windowsなどのプラットフォーム別にフォルダわけなどを終えた絶対URLが必要
			yield return this.Engine.LoadChapterAsync(url);
		}
		//設定データを反映
		this.Engine.GraphicManager.Remake(this.Engine.DataManager.SettingDataManager.LayerSetting);

		//パラメーターをデフォルト値でリセット
		//これは場合によってはリセットしたくない場合もあるので、あえて外にだす
		this.Engine.Param.InitDefaultAll(this.Engine.DataManager.SettingDataManager.DefaultParam);

		//パラメーターの引継ぎ方法は以下のように、いろいろある
		//（ややこしいが、ゲーム起動時なのか、ゲームの最中なのか、そもそもチャプター機能をどう使うかを宴側からは制御できないのでこうなる）

		//その１。メモリ内にとってある場合
		//バイナリデータから読み取る
		if (!resetParam)
		{
			BinaryUtil.BinaryRead(bufferDefaultParam, (reader) => engine.Param.Read(reader, AdvParamData.FileType.Default));
			BinaryUtil.BinaryRead(bufferSystemParam, (reader) => engine.Param.Read(reader, AdvParamData.FileType.System));
		}

		//その２。オートセーブのパラメーターだけをロードする
		//同じやり方で任意のセーブファイルのパラメーターだけをロードするのも可能
		if (readAutoSaveDataParamOnly)
		{
			//オートセーブデータをロード
			this.Engine.SaveManager.ReadAutoSaveData();
			AdvSaveData autoSave = this.Engine.SaveManager.AutoSaveData;
			if (autoSave != null && autoSave.IsSaved)
			{
				autoSave.Buffer.Overrirde(this.Engine.Param.DefaultData);
			}
		}

		//その３。
		//システムセーブデータをロードする
		//ファイルからロードするので、事前に書き込みされてないとダメ
		//チャプターロードを使う場合は、システムセーブデータの読み込みがされないので
		//一度はこれを使う
		if (readSystemSaveData)
		{
			this.Engine.SystemSaveData.Init(this.Engine);
		}

		//リソースファイルのダウンロードを進めておく
		this.Engine.DataManager.DownloadAll();

		//ロード待ちのための画面遷移
		title.Close();
		loadWait.OpenOnChapter();
		loadWait.onClose.RemoveAllListeners();
		loadWait.onClose.AddListener(
			() =>
			{
				mainGame.Open();

				//StartGameはシステム系以外のパラメーターがリセットされてしまうので
				//パラメーターを引き継がない場合のみStartGame			
				if (resetParam && !readAutoSaveDataParamOnly)
				{
					this.Engine.StartGame(startLabel[chapterIndex]);
				}
				else
				{
					this.Engine.JumpScenario(startLabel[chapterIndex]);
				}
			}
			);
	}
}
