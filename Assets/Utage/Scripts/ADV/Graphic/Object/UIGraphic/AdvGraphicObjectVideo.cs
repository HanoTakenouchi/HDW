// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif
using UtageExtensions;

namespace Utage
{

	/// <summary>
	/// ビデオ（ムービー）オブジェクトの表示
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/Video")]
	public class AdvGraphicObjectVideo : AdvGraphicObjectUguiBase
	{
#if UNITY_5_6_OR_NEWER
		protected override Material Material { get { return RawImage.material; } set { RawImage.material = value; } }
		RawImage RawImage { get; set; }

		//クロスフェード用のファイル参照
		AssetFileReference crossFadeReference;
		void ReleaseCrossFadeReference()
		{
			if (crossFadeReference != null)
			{
				Destroy(crossFadeReference);
				crossFadeReference = null;
			}
		}
		VideoClip VideoClip { get; set; }
		VideoPlayer VideoPlayer { get; set; }
		protected Timer FadeTimer { get; set; }
		//描画先とするバックバッファ
		RenderTexture RenderTexture { get; set; }
		int Width { get; set; }
		int Height { get; set; }

		//初期化処理
		protected override void AddGraphicComponentOnInit()
		{
			this.RawImage = this.GetComponentCreateIfMissing<RawImage>();
			this.FadeTimer = this.gameObject.AddComponent<Timer>();
			this.FadeTimer.AutoDestroy = false;
			this.VideoPlayer = this.gameObject.AddComponent<VideoPlayer>();
		}
		//破棄
		void OnDisable()
		{
			UnityEngine.Profiling.Profiler.BeginSample("OnDisAble");
			this.VideoPlayer.Stop();
			UnityEngine.Profiling.Profiler.EndSample();
		}

		//破棄
		void OnDestroy()
		{
			UnityEngine.Profiling.Profiler.BeginSample("ReleaseTexture");
			ReleaseTexture();
			UnityEngine.Profiling.Profiler.EndSample();
		}

		void ReleaseTexture()
		{
			if (this.RenderTexture)
			{
				if (this.VideoPlayer.isPlaying)
				{
					this.VideoPlayer.Stop();
				}
				if (RenderTexture.active == this.RenderTexture)
				{
					RenderTexture.active = null;
				}
				this.RenderTexture.Release();
				Destroy(this.RenderTexture);
			}
		}

		//エフェクト用の色が変化したとき
		internal override void OnEffectColorsChange(AdvEffectColor color)
		{
		}

		//********描画時にクロスフェードが失敗するであろうかのチェック********//
		internal override bool CheckFailedCrossFade(AdvGraphicInfo graphic)
		{
			return true;
		}

		//********描画時のリソース変更********//
		internal override void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime)
		{
			this.VideoClip = graphic.File.UnityObject as VideoClip;
			this.VideoPlayer.clip = this.VideoClip;
			this.VideoPlayer.isLooping = true;

			this.VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
			ReleaseTexture();
			this.RenderTexture = new RenderTexture((int)VideoClip.width, (int)VideoClip.height, 16, RenderTextureFormat.ARGB32);
			this.VideoPlayer.targetTexture = this.RenderTexture;
			this.VideoPlayer.Play();

			this.RawImage.texture = this.RenderTexture;
			this.RawImage.SetNativeSize();

			//			this.VideoPlayer.alpha = 0.5f;
			//			this.VideoPlayer.loopPointReached += EndReached;
		}
#else
		protected override Material Material
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		protected override void AddGraphicComponentOnInit()
		{
			throw new NotImplementedException();
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime)
		{
			throw new NotImplementedException();
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo graphic)
		{
			throw new NotImplementedException();
		}		
#endif
	}
}
