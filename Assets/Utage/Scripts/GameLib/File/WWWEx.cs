// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	//WWWの拡張クラス
	public class WWWEx
	{
		public enum Type
		{
			Default,
			Cache,
		};


		//ロードするURL
		public string Url { get; private set; }

		//アセットバンドルのハッシュ値
		public Hash128 AssetBundleHash { get; private set; }

		//アセットバンドルのバージョン値
		public int AssetBundleVersion { get; private set; }

		//ロードタイプ
		public Type LoadType { get; private set; }

		//ロード失敗したときのリトライ回数
		public int RetryCount { get; set; }
		//ロードの進捗がなかったときのタイムアウト時間
		public float TimeOut { get; set; }

		//ロードの進捗
		public float Progress { get; private set; }

		//ロード中の進捗を取得するために
		public Action<WWWEx> OnUpdate { get; set; }

		//デバッグログを無視するか
		public bool IgnoreDebugLog { get; set; }

		//Byteを記録するか
		public bool StoreBytes { get; set; }

		//記録されたBytes
		public byte[] Bytes { get; set; }

		//通常のWWWロード
		public WWWEx(string url)
		{
			this.LoadType = Type.Default;
			InitSub(url);
		}

		//キャッシュからのロード
		public WWWEx(string url, Hash128 assetBundleHash)
		{
			this.AssetBundleHash = assetBundleHash;
			this.LoadType = Type.Cache;
			InitSub(url);
		}

		//キャッシュからのロード
		public WWWEx(string url, int assetBundleVersion)
		{
			this.AssetBundleVersion = assetBundleVersion;
			this.LoadType = Type.Cache;
			InitSub(url);
		}

		void InitSub(string url)
		{
			this.Url = url;
			this.RetryCount = 5;
			this.TimeOut = 5;
			this.Progress = 0;
		}

		///WWWを使ったロード処理
		IEnumerator LoadAsync(Action<WWW> onCopmlete, Action<WWW> onFailed, Action<WWW> onTimeOut)
		{
			return LoadAsyncSub(onCopmlete, onFailed, onTimeOut, RetryCount);
		}

		///WWWを使ったロード処理
		IEnumerator LoadAsyncSub(Action<WWW> onCopmlete, Action<WWW> onFailed, Action<WWW> onTimeOut, int retryCount)
		{
			if (LoadType == Type.Cache)
			{
				while (!Caching.ready) yield return null;
			}
			bool retry = false;
			//WWWでダウンロード
			using (WWW www = CreateWWW())
			{
				float time = 0;
				bool isTimeOut = false;
				this.Progress = 0;
				//ロード待ち
				while (!www.isDone && !isTimeOut)
				{
					//タイムアウトチェック
					if (0 < TimeOut)
					{
						if (Progress == www.progress)
						{
							time += Time.deltaTime;
							if (time >= TimeOut)
							{
								isTimeOut = true;
							}
						}
						else
						{
							time = 0;
						}
					}
					Progress = www.progress;
					if (OnUpdate != null) OnUpdate(this);
					yield return null;
				}
				if (isTimeOut)
				{
					//タイムアウト
					if (retryCount <= 0)
					{
						if (onTimeOut != null) onTimeOut(www);
					}
					else
					{
						retry = true;
					}
				}
				else if (!string.IsNullOrEmpty(www.error))
				{
					//ロードエラー
					if (retryCount <= 0)
					{
						if (onFailed != null) onFailed(www);
					}
					else
					{
						retry = true;
					}
				}
				else
				{
					Progress = www.progress;
					if (StoreBytes) Bytes = www.bytes;
					if (OnUpdate != null) OnUpdate(this);
					//ロード終了
					if (onCopmlete != null) onCopmlete(www);
				}
			}

			//リトライするなら再帰で呼び出す
			if (retry)
			{
				yield return LoadAsyncSub(onCopmlete, onFailed, onTimeOut, retryCount - 1);
			}
		}


		///WWWを作成
		WWW CreateWWW()
		{
			switch (LoadType)
			{
				case Type.Cache:
					if (AssetBundleHash.isValid)
					{
						return WWW.LoadFromCacheOrDownload(Url, AssetBundleHash);
					}
					else
					{
						return WWW.LoadFromCacheOrDownload(Url, AssetBundleVersion);
					}
				default:
					return new WWW(Url);
			}
		}


		///WWWを使ったロード処理
		public IEnumerator LoadAsync(Action<WWW> onComplete, Action<WWW> onFailed = null)
		{
			return LoadAsync(
				//OnComplete
				(www) =>
				{
					onComplete(www);
				},
				//OnFailed
				(www) =>
				{
					if(!IgnoreDebugLog) Debug.LogError("WWW load error " + www.url + "\n" + www.error);
					if (onFailed != null) onFailed(www);
				},
				//OnTimeOut);
				(www) =>
				{
					if (!IgnoreDebugLog) Debug.LogError("WWW timeout " + www.url);
					if (onFailed != null) onFailed(www);
				}
				);
		}

		///アセットバンドルをロード
		public IEnumerator LoadAssetBundleAsync(Action<WWW, AssetBundle> onComplete, Action<WWW> onFailed)
		{
			return LoadAsync(
				//OnComplete
				(www) =>
				{
					AssetBundle assetBundle = www.assetBundle;
					if (assetBundle != null)
					{
						//成功！
						if (onComplete != null) onComplete(www, assetBundle);
					}
					else
					{
						//失敗
						if (!IgnoreDebugLog) Debug.LogError(www.url + " is not assetBundle");
						if (onFailed != null) onFailed(www);
					}
				},
				//OnFailed
				(www) =>
				{
					//失敗
					if (onFailed != null) onFailed(www);
				}
				);
		}

		///アセットバンドルのメインアセットをロード
		public IEnumerator LoadAssetBundleMainAssetAsync<T>(bool unloadAllLoadedObjects, Action<WWW, T> onComplete, Action<WWW> onFailed) where T : UnityEngine.Object
		{
			return LoadAssetBundleAsync(
				//OnComplete
				(www, assetBundle) =>
				{
					T mainAsset = assetBundle.mainAsset as T;
					if (mainAsset != null)
					{
						//成功！
						if (onComplete != null) onComplete(www, mainAsset);
					}
					else
					{
						//失敗
						if (!IgnoreDebugLog) Debug.LogError(www.url + " is not AssetBundle of " + typeof(T).Name);
						if (onFailed != null) onFailed(www);
					}
					mainAsset = null;
					assetBundle.Unload(unloadAllLoadedObjects);
				},
				//OnFailed
				(www) =>
				{
					//失敗
					if (onFailed != null) onFailed(www);
				}
				);
		}

		///アセットバンドルのメインアセットをロード
		public IEnumerator LoadAssetBundleByNameAsync<T>(string assetName, bool unloadAllLoadedObjects, Action<T> onComplete, Action onFailed) where T : UnityEngine.Object
		{
			AssetBundle assetBundle = null;
			yield return LoadAssetBundleAsync(
				//OnComplete
				(_www, _assetBundle) =>
				{
					assetBundle = _assetBundle;
				},
				//OnFailed
				(_www) =>
				{
					//失敗
					if (onFailed != null) onFailed();
				}
				);

			if (assetBundle == null) yield break;

			AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(assetName);
			while (!request.isDone)
			{
				yield return null;
			}
			T asset = request.asset as T;
			if (asset == null)
			{
				//失敗
				if (!IgnoreDebugLog) Debug.LogError(Url + "  " + assetName + " is not AssetBundle of " + typeof(T).Name);
				if (onFailed != null) onFailed();
			}
			else
			{
				//成功！
				if (onComplete != null) onComplete(asset);
			}
			asset = null;
			request = null;
			assetBundle.Unload(unloadAllLoadedObjects);
		}


		///アセットバンドルのメインアセットをロード
		public IEnumerator LoadAssetBundleAllAsync<T>(bool unloadAllLoadedObjects, Action<T[]> onComplete, Action onFailed) where T : UnityEngine.Object
		{
			AssetBundle assetBundle = null;
			yield return LoadAssetBundleAsync(
				//OnComplete
				(_www, _assetBundle) =>
				{
					assetBundle = _assetBundle;
				},
				//OnFailed
				(_www) =>
				{
					//失敗
					if (onFailed != null) onFailed();
				}
				);

			if (assetBundle == null) yield break;

			AssetBundleRequest request = assetBundle.LoadAllAssetsAsync<T>();
			while (!request.isDone)
			{
				yield return null;
			}
			T[] assets = request.allAssets as T[];
			if (assets == null || assets.Length <= 0)
			{
				//失敗
				if (!IgnoreDebugLog) Debug.LogError(Url + "  " + " is not AssetBundle of " + typeof(T).Name);
				if (onFailed != null) onFailed();
			}
			else
			{
				//成功！
				if (onComplete != null) onComplete(assets);
			}
			assets = null;
			request = null;
			assetBundle.Unload(unloadAllLoadedObjects);
		}
	}
}