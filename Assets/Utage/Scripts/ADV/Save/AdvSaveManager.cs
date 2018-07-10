// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Events;
using UtageExtensions;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif


namespace Utage
{

	/// <summary>
	/// セーブデータ管理クラス
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/SaveManager")]
	public class AdvSaveManager : MonoBehaviour
	{
		FileIOManager FileIOManager { get { return this.fileIOManager ?? (this.fileIOManager = FindObjectOfType<FileIOManager>()); } }
		[SerializeField]
		FileIOManager fileIOManager;

		/// <summary>
		/// セーブのタイプ
		/// </summary>
		public enum SaveType
		{
			Default,		//全ページ
			SavePoint,		//セーブポイントのみ
			Disable,        //セーブをしない
		};
		public SaveType Type { get { return type; } }
		[SerializeField]
		SaveType type = SaveType.Default;


		/// <summary>
		/// オートセーブが有効か
		/// </summary>
		public bool IsAutoSave { get { return isAutoSave; } }
		[SerializeField]
		bool isAutoSave = true;
		
		/// <summary>
		/// ディレクトリ名
		/// </summary>
		public string DirectoryName
		{
			get { return directoryName; }
			set { directoryName = value; }
		}
		[SerializeField]
		string directoryName = "Save";

		/// <summary>
		/// セーブファイル名(実際には連番のIDがさらに末尾につく)
		/// </summary>
		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}
		[SerializeField]
		string fileName = "save";


		/// <summary>
		/// セーブデータの設定
		/// </summary>
		[System.Serializable]
		 class SaveSetting
		{
			/// <summary>
			/// セーブ時のスクリーンショット解像度（幅）
			/// </summary>
			public int CaptureWidth { get { return this.captureWidth; } }
			[SerializeField]
			int captureWidth = 256;

			/// <summary>
			/// セーブ時のスクリーンショット解像度（高さ）
			/// </summary>
			public int CaptureHeight { get { return this.captureHeight; } }
			[SerializeField]
			int captureHeight = 256;

			/// <summary>
			/// セーブファイルの数
			/// </summary>
			public int SaveMax { get { return this.saveMax; } }
			[SerializeField]
			int saveMax = 9;
		}

		[SerializeField]
		SaveSetting defaultSetting;		//セーブデータの設定（デフォルト）
		[SerializeField]
		SaveSetting webPlayerSetting;		//セーブデータの設定（WebPlayer用）

#if UNITY_WEBPLAYER || UNITY_WEBGL
		public int CaptureWidth {get {return webPlayerSetting.CaptureWidth;}}
		public int CaptureHeight { get { return webPlayerSetting.CaptureHeight; } }
		int SaveMax { get { return webPlayerSetting.SaveMax; } }
#else
		public int CaptureWidth { get { return defaultSetting.CaptureWidth; } }
		public int CaptureHeight { get { return defaultSetting.CaptureHeight; } }
		int SaveMax { get { return defaultSetting.SaveMax; } }
#endif

		public List<GameObject> CustomSaveDataObjects;

		public List<IBinaryIO> CustomSaveDataIOList
		{
			get
			{
				List<IBinaryIO> list = new List<IBinaryIO>();
				foreach (GameObject go in CustomSaveDataObjects)
				{
					IAdvSaveData io = go.GetComponent<IAdvSaveData>();
					if (io == null)
					{
						Debug.LogError(go.name + "is not contains IAdvCustomSaveDataIO ", go);
						continue;
					}
					else
					{
						list.Add(io);
					}
				}
				return list;
			}
		}

		//バージョンアップ用のセーブデータ
		public List<IBinaryIO> GetSaveIoListCreateIfMissing(AdvEngine engine)
		{
			if (saveIoList == null)
			{
				saveIoList = new List<IBinaryIO>();
				saveIoList.AddRange(this.GetComponentsInChildren<IAdvSaveData>(true));
			}
			return saveIoList;
		}
		List<IBinaryIO> saveIoList;

		/// <summary>
		/// オートセーブ
		/// </summary>
		public AdvSaveData AutoSaveData { get { return autoSaveData; } }
		AdvSaveData autoSaveData;

		/// <summary>
		/// オートセーブ用のデータ
		/// </summary>
		public AdvSaveData CurrentAutoSaveData { get { return currentAutoSaveData; } }
		AdvSaveData currentAutoSaveData;


		/// <summary>
		/// クイックセーブ用のデータ
		/// </summary>
		public AdvSaveData QuickSaveData { get { return quickSaveData; } }
		AdvSaveData quickSaveData;

		/// <summary>
		/// セーブデータのリスト
		/// </summary>
		public List<AdvSaveData> SaveDataList{get{return saveDataList;}}
		List<AdvSaveData> saveDataList;

		/// <summary>
		/// キャプチャー画面
		/// </summary>
		public Texture2D CaptureTexture
		{
			get
			{
				return captureTexture;
			}
			set
			{
				ClearCaptureTexture();
				captureTexture = value;
			}
		}
		Texture2D captureTexture;


		/// <summary>
		/// キャプチャ画像をクリア
		/// </summary>
		public void ClearCaptureTexture()
		{
			if (captureTexture != null)
			{
				UnityEngine.Object.Destroy(captureTexture);
				captureTexture = null;
			}			
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init()
		{
			//セーブデータのディレクトリがなければ作成
			FileIOManager.CreateDirectory(ToDirPath());

			//オートセーブデータ。読み込み用と書き込み用
			autoSaveData = new AdvSaveData( AdvSaveData.SaveDataType.Auto, ToFilePath("Auto")); ;
			currentAutoSaveData = new AdvSaveData(AdvSaveData.SaveDataType.Auto, ToFilePath("Auto")); ;

			quickSaveData = new AdvSaveData(AdvSaveData.SaveDataType.Quick, ToFilePath("Quick")); ;

			saveDataList = new List<AdvSaveData>();
			for (int i = 0; i < SaveMax; i++)
			{
				AdvSaveData data = new AdvSaveData(AdvSaveData.SaveDataType.Default, ToFilePath("" + (i + 1)));
				saveDataList.Add(data);
			}
		}

		string ToFilePath(string id)
		{
			return FilePathUtil.Combine(ToDirPath(),FileName + id);
		}
		string ToDirPath()
		{
			return FilePathUtil.Combine(FileIOManager.SdkPersistentDataPath, DirectoryName + "/");
		}

		/// <summary>
		/// オートセーブ用のデータを読み込み
		/// </summary>
		/// <returns></returns>
		public bool ReadAutoSaveData()
		{
			if (!isAutoSave) return false;
			return ReadSaveData(AutoSaveData);
		}

		/// <summary>
		/// クイックセーブ用のデータを読み込み
		/// </summary>
		/// <returns></returns>
		public bool ReadQuickSaveData()
		{
			return ReadSaveData(QuickSaveData);
		}

		/// <summary>
		/// セーブデータを全て読み込み
		/// </summary>
		public void ReadAllSaveData()
		{
			Profiler.BeginSample("ReadAllSaveData");
			ReadAutoSaveData();
			ReadQuickSaveData();
			foreach (AdvSaveData item in SaveDataList)
			{
				ReadSaveData(item);
			}
			Profiler.EndSample();
		}

		/// <summary>
		/// セーブデータを読み込み
		/// </summary>
		/// <param name="saveData">読み込むセーブデータ</param>
		/// <returns>読み込めたかどうか</returns>
		bool ReadSaveData(AdvSaveData saveData)
		{
			if (FileIOManager.Exists(saveData.Path))
			{
				return FileIOManager.ReadBinaryDecode(saveData.Path, saveData.Read);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// オートセーブデータを更新（書き込みはしない）
		/// </summary>
		internal void UpdateAutoSaveData(AdvEngine engine)
		{
			Profiler.BeginSample("UpdateAutoSaveData");
			CurrentAutoSaveData.UpdateAutoSaveData(engine, null, CustomSaveDataIOList, GetSaveIoListCreateIfMissing(engine));
			Profiler.EndSample();
		}

		/// <summary>
		/// セーブデータを書き込み
		/// その場の状態を書き込まず、各ページ冒頭のオートセーブデータを利用する
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		/// <param name="saveData">書き込むセーブデータ</param>
		public void WriteSaveData(AdvEngine engine, AdvSaveData saveData)
		{
			if (!CurrentAutoSaveData.IsSaved)
			{
				Debug.LogError("SaveData is Disabled");
				return;
			}

			//セーブ
			saveData.SaveGameData(CurrentAutoSaveData, engine, UtageToolKit.CreateResizeTexture(CaptureTexture, CaptureWidth, CaptureHeight));
			FileIOManager.WriteBinaryEncode(saveData.Path, saveData.Write);
		}

		//セーブデータを消去して終了(SendMessageでコールバックされるので名前固定)
		void OnDeleteAllSaveDataAndQuit()
		{
			DeleteAllSaveData();
			isAutoSave = false;
		}

		/// <summary>
		/// セーブデータを全て消去
		/// </summary>
		public void DeleteAllSaveData()
		{
			DeleteSaveData(AutoSaveData);
			DeleteSaveData(QuickSaveData);
			foreach (AdvSaveData item in SaveDataList)
			{
				DeleteSaveData(item);
			}
		}

		/// <summary>
		/// セーブデータを削除
		/// </summary>
		/// <param name="saveData">削除するセーブデータ</param>
		public void DeleteSaveData(AdvSaveData saveData)
		{
			if (FileIOManager.Exists(saveData.Path))
			{
				FileIOManager.Delete(saveData.Path);
			}
			saveData.Clear();
		}
		//ゲーム終了時
		void OnApplicationQuit()
		{
			AutoSave();
		}

		//アプリがポーズしたとき
		void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				AutoSave();
			}
		}

		void AutoSave()
		{
			if (IsAutoSave && AutoSaveData != null)
			{
				if (CurrentAutoSaveData.IsSaved)
				{
					FileIOManager.WriteBinaryEncode(CurrentAutoSaveData.Path, CurrentAutoSaveData.Write);
				}
			}
		}
	}
}