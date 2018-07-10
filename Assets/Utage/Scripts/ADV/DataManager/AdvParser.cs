// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// ADVデータ解析
	/// </summary>
	public class AdvParser
	{
		public static string Localize(AdvColumnName name)
		{
			//多言語化をしてみたけど、複雑になってかえって使いづらそうなのでやめた
			return name.QuickToString();
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらエラーメッセージを出す）
		public static T ParseCell<T>(StringGridRow row, AdvColumnName name)
		{
			return row.ParseCell<T>(Localize(name));
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらデフォルト値を返す）
		public static T ParseCellOptional<T>(StringGridRow row, AdvColumnName name, T defaultVal)
		{
			return row.ParseCellOptional<T>(Localize(name), defaultVal);
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらfalse）
		public static bool TryParseCell<T>(StringGridRow row, AdvColumnName name, out T val)
		{
			return row.TryParseCell<T>(Localize(name), out val);
		}

		//セルが空かどうか
		public static bool IsEmptyCell(StringGridRow row, AdvColumnName name)
		{
			return row.IsEmptyCell(Localize(name));
		}
	}
}
