// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UtageExtensions;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

namespace Utage
{

	/// <summary>
	/// ビデオ表示の管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/VideoManager")]
	public class AdvVideoManager : MonoBehaviour
	{
		public AdvEngine Engine { get { return engine ?? (engine = this.GetComponentInParent<AdvEngine>()); } }
		AdvEngine engine;

#if UNITY_5_6_OR_NEWER
		class VideoInfo
		{
			public bool Cancel { get; set; }
			public bool Started { get; set; }
			public bool Canceled { get; set; }
			public VideoPlayer Player { get; set; }
		}

		Dictionary<string, VideoInfo> Videos { get { return videos; } }
		Dictionary<string, VideoInfo> videos = new Dictionary<string, VideoInfo>();

		internal void Play(string label, string cameraName, AssetFile file, bool loop, bool cancel)
		{
			Play(label, cameraName, file.UnityObject as VideoClip, loop, cancel);
		}

		internal void Play(string label, string cameraName, VideoClip clip, bool loop, bool cancel)
		{
			VideoInfo info = new VideoInfo() { Cancel = cancel, };
			Videos.Add(label, info);
			GameObject go = this.transform.AddChildGameObject(label);
			VideoPlayer videoPlayer = go.AddComponent<VideoPlayer>();
			videoPlayer.isLooping = loop;
			videoPlayer.clip = clip;
			videoPlayer.targetCamera = Engine.EffectManager.FindTarget(AdvEffectManager.TargetType.Camera, cameraName).GetComponentInChildren<Camera>();
			videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
			videoPlayer.Play();
			videoPlayer.started += (x => OnStarted(info));
			info.Player = videoPlayer;
		}

		void OnStarted(VideoInfo info)
		{
			info.Started = true;
		}

		internal void Cancel(string label)
		{
			if (!Videos[label].Cancel)
			{
				return;
			}
			Videos[label].Canceled = true;
			Videos[label].Player.Stop();
		}

		internal bool IsEndPlay(string label)
		{
			if (!Videos.ContainsKey(label)) return true;

			//キャンセル済み
			if (Videos[label].Canceled) return true;
			//まだロード終ってないなら
			if (!Videos[label].Started) return false;

			//最初の0フレームで呼ばれることがある模様
			return Videos[label].Player.time > 0 && !Videos[label].Player.isPlaying;
//			return !Videos[label].Player.isPlaying;
		}

		//終了処理
		internal void Complete(string label)
		{
			VideoInfo info = Videos[label];
			info.Player.targetCamera = null;
			//描画オブジェクトを消す
			GameObject.Destroy(info.Player.gameObject);
			Videos.Remove(label);
		}
#else
		internal void Play(string label, string cameraName, AssetFile file, bool loop, bool cancel)
		{

		}

		internal void Cancel(string label)
		{
		}

		internal void Complete(string label)
		{
		}

		internal bool IsEndPlay(string label)
		{
			return true;
		}

		internal void Remove(string label)
		{
		}
#endif
	}
}
