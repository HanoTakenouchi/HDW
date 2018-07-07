using System;
using System.Collections.Generic;


/// <summary>
/// ノベル用ユーティリティ（便利メソッド置き場）
/// </summary>
public static class NovelUtility
{
	/// <summary>Resources以下にある選択肢プレハブのパス</summary>
	static public string ResourcesChoisePath = "Choices/";
	/// <summary>Resources以下にある2Dイメージのパス</summary>
	static public string ResourcesImagePath = "Images/";
	/// <summary>Resources以下にある2Dイメージのパス</summary>
	static public string ResourcesTextAnimationDataPath = "TextAnimationData/";
	/// <summary>Resources以下にある背景イメージのパス</summary>
	static public string ResourcesBackgroundPath = "Backgrounds/";
	/// <summary>Resources以下にある3Dモデルのパス</summary>
	static public string ResourcesModelPath = "Models/";
	/// <summary>Resources以下にある2Dイメージマテリアルのパス</summary>
	static public string ResourcesImageMaterialPath = "Materials/ImageMaterial";


	/// <summary>
	/// 値型のTryParse型のデリゲート
	/// </summary>
	public delegate bool TryParse<T>(string from, out T to);

	/// <summary>
	/// 文字から必要な型に変換
	/// </summary>
	/// <typeparam name="TType">変換型</typeparam>
	/// <param name="str">文字列</param>
	/// <param name="tryParse">変換方.TryParse()メソッド</param>
	/// <returns>変換後パラメータ（失敗時はdefault）</returns>
	static public TType ParseValueOrDefault<TType>(string str, TryParse<TType> tryParse)
	{
		TType value;
		if (tryParse(str, out value)) { return value; }
		return default(TType);
	}

	/// <summary>
	/// boolに変換
	/// </summary>
	static public bool ParseBool(this string self)
	{
		return ParseValueOrDefault<bool>(self, bool.TryParse);
	}

	/// <summary>
	/// intに変換
	/// </summary>
	static public int ParseInt(this string self)
	{
		return ParseValueOrDefault<int>(self, int.TryParse);
	}

	/// <summary>
	/// uintに変換
	/// </summary>
	static public uint ParseUInt(this string self)
	{
		return ParseValueOrDefault<uint>(self, uint.TryParse);
	}

	/// <summary>
	/// floatに変換
	/// </summary>
	static public float ParseFloat(this string self)
	{
		return ParseValueOrDefault<float>(self, float.TryParse);
	}

	/// <summary>
	/// いずれかに当てはまるかどうか
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	/// <param name="compares"></param>
	/// <returns></returns>
	static public bool IsAny<TType>(this TType self, params TType[] compares) where TType : IComparable
	{
		for (int i = 0; i < compares.Length; ++i)
		{
			if (0 == compares[i].CompareTo(self))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// キューから可能ならDequeueする
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	/// <param name="self"></param>
	/// <returns></returns>
	static public TType TryDequeue<TType>(this Queue<TType> self)
	{
		if (self.Count <= 0) { return default(TType); }
		return self.Dequeue();
	}

	/// <summary>
	/// キューから可能ならPeekする
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	/// <param name="self"></param>
	/// <returns></returns>
	static public TType TryPeek<TType>(this Queue<TType> self)
	{
		if (self.Count <= 0) { return default(TType); }
		return self.Peek();
	}
}
