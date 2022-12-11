/*
	Copyright © 2022 Nathanne Isip

	Permission is hereby granted, free of charge,
	to any person obtaining a copy of this software
	and associated documentation files (the “Software”),
	to deal in the Software without restriction,
	including without limitation the rights to use, copy,
	modify, merge, publish, distribute, sublicense,
	and/or sell copies of the Software, and to permit
	persons to whom the Software is furnished to do so,
	subject to the following conditions:

	The above copyright notice and this permission notice
	shall be included in all copies or substantial portions
	of the Software.

	THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF
	ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
	TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
	THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
	DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
	CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
	CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
	IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Nougat.Core;
using Nougat.Parser;

public sealed class NougatUtil {
	public static List<string> ScriptHash = new List<string>();

	public static string ToString(object Obj) {
		if(Obj == null)
			return "nil";

		if(Obj is bool flag)
			return flag.ToString().ToLower();

		if(Obj is object[]) {
			string text = "[ ";
			object[] array = (object[])Obj;

			foreach (object obj in array)
				text = ((!(obj is string)) ? (text + ToString(obj) + ", ") : (text + "\"" + obj.ToString() + "\", "));

			return ((((object[])Obj).Length != 0) ? (text.Substring(0, text.Length - 2) + " ") : text) + "]";
		}

		if(Obj is List<KeyValuePair<object, object>>) {
			string text2 = "{ ";

			foreach (KeyValuePair<object, object> item in (List<KeyValuePair<object, object>>)Obj) {
				text2 = ((!(item.Key is string)) ? ((!(item.Key is NougatMapIdentifier)) ? (text2 + ToString(item.Key) + ": ") : (text2 + (item.Key as NougatMapIdentifier).Name.Image + ": ")) : (text2 + "\"" + item.Key.ToString() + "\": "));
				text2 = ((!(item.Value is string)) ? (text2 + ToString(item.Value) + ", ") : (text2 + "\"" + item.Value.ToString() + "\", "));
			}

			return ((((List<KeyValuePair<object, object>>)Obj).Count > 0) ? (text2.Substring(0, text2.Length - 2) + " ") : text2) + "}";
		}

		return Obj.ToString();
	}

	public static double Digitize(string Image) {
		double num = 0.0;

		if(Image.StartsWith("0b"))
			return Convert.ToInt32(Image.Replace("0b", ""), 2);

		if(Image.StartsWith("0c"))
			return Convert.ToInt32(Image.Replace("0c", ""), 8);

		if(Image.StartsWith("0x"))
			return Convert.ToInt32(Image.Replace("0x", ""), 16);

		return Convert.ToDouble(Image);
	}

	public static string Stringify(string Image) {
		return Image.Replace("[[\\\\]]", "\\").Replace("[[\\n]]", "\n").Replace("[[\\r]]", "\r")
			.Replace("[[\\t]]", "\t")
			.Replace("[[\\f]]", "\f")
			.Replace("[[\\v]]", "\v")
			.Replace("[[\\a]]", "\a")
			.Replace("[[\\s]]", "\b")
			.Replace("[[\\\"]]", "\"");
	}

	public static List<Token> Merge(List<Token> Dest, List<Token> Source) {
		foreach (Token item in Source)
			Dest.Add(item);

		return Dest;
	}

	public static void PrintWithColor(ConsoleColor Foreground, string Text) {
		ConsoleColor foregroundColor = Console.ForegroundColor;
		Console.ForegroundColor = Foreground;
		Console.WriteLine(Text);
		Console.ForegroundColor = foregroundColor;
	}

	public static void ShowErrors(Dictionary<SourceAddress, string> Errors) {
		ConsoleColor foregroundColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;

		foreach (KeyValuePair<SourceAddress, string> Error in Errors)
			Console.WriteLine("[line " + Error.Key.Line + ", column " + Error.Key.Column + "] " + Error.Value);

		Console.ForegroundColor = foregroundColor;
	}

	public static string GetCurrentDirectoryWith(string FileName) {
		return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileName);
	}

	public static string StringifySelectionPath(List<Token> Path, int Pad = 0) {
		string text = Path[0].Image;

		for (int i = 1; i < Path.Count - Pad; i++)
			text = text + "::" + Path[i].Image;
		return text;
	}

	public static bool HasRanScript(string Hash) {
		return ScriptHash.Contains(Hash);
	}

	public static void DefineHash(string Hash) {
		ScriptHash.Add(Hash);
	}

	public static string ToMD5(string Input) {
		using MD5 mD = MD5.Create();

		byte[] array = mD.ComputeHash(Encoding.ASCII.GetBytes(Input));
		StringBuilder stringBuilder = new StringBuilder();

		for (int i = 0; i < array.Length; i++)
			stringBuilder.Append(array[i].ToString("x2"));
		return stringBuilder.ToString();
	}
}
