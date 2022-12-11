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
using System.Threading;

public class IO {
	public static object appendToFile(object Path, object Data) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		File.AppendAllText(Path.ToString(), Data.ToString());
		return null;
	}

	public static object backgroundColor(object Color) {
		if(!(Color is double))
			throw new Exception("Color parameter is not a valid number.");

		Console.BackgroundColor = (ConsoleColor)Convert.ToInt32(Color);
		return null;
	}

	public static object cls() {
		Console.Clear();
		return null;
	}

	public static object createDir(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		Directory.CreateDirectory(Path.ToString());
		return dirExists(Path);
	}

	public static object deleteDir(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		Directory.Delete(Path.ToString());
		return null;
	}

	public static object deleteFile(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		File.Delete(Path.ToString());
		return null;
	}

	public static object dirExists(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		return Directory.Exists(Path.ToString());
	}

	public static object exit(object ExitCode) {
		if(!(ExitCode is double))
			throw new Exception("ExitCode parameter is not of number type.");

		Environment.Exit(Convert.ToInt32(ExitCode));
		return null;
	}

	public static object fileExists(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		return File.Exists(Path.ToString());
	}

	public static object foregroundColor(object Color){
		if(!(Color is double))
			throw new Exception("Color parameter is not a valid number.");

		Console.ForegroundColor = (ConsoleColor)Convert.ToInt32(Color);
		return null;
	}

	public static object isFile(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		return File.Exists(Path.ToString()) && !Directory.Exists(Path.ToString());
	}

	public static object isFolder(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		return !File.Exists(Path.ToString()) && Directory.Exists(Path.ToString());
	}

	public static object[] listDirFiles(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		List<object> list = new List<object>();
		string[] files = Directory.GetFiles(Path.ToString());

		foreach(string item in files)
			list.Add(item);

		files = Directory.GetDirectories(Path.ToString());
		foreach(string item2 in files)
			list.Add(item2);

		return list.ToArray();
	}

	public static object print(object Data) {
		Console.Write(Data);
		return Data;
	}

	public static object printLine(object Data) {
		Console.WriteLine(Data);
		return Data;
	}

	public static object readFromFile(object Path) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		return File.ReadAllText(Path.ToString());
	}

	public static object readKey() {
		return Console.ReadKey().KeyChar.ToString();
	}

	public static object readLine() {
		return Console.ReadLine();
	}

	public static object sleep(object Timeout) {
		if(!(Timeout is string))
			throw new Exception("Timeout parameter is not of number type.");

		Thread.Sleep(Convert.ToInt32(Timeout));
		return Timeout;
	}

	public static object writeToFile(object Path, object Data) {
		if(!(Path is string))
			throw new Exception("Path parameter is not of string type.");

		File.WriteAllText(Path.ToString(), Data.ToString());
		return null;
	}
}