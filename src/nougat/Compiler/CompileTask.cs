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
using Nougat.Compiler;
using Nougat.Core;

public class CompileTask {
	public static void Send(string[] FileNames) {
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

		foreach(string text in FileNames) {
			if(!File.Exists(text)) {
				NougatUtil.PrintWithColor(ConsoleColor.Red, "File " + text + " doesn't exists.");
				Environment.Exit(0);
			}
			if(!text.EndsWith(".nou")) {
				NougatUtil.PrintWithColor(ConsoleColor.Red, "Not a Nougat file: " + text);
				Environment.Exit(0);
			}

			try {
				list.Add(new KeyValuePair<string, string>(text, File.ReadAllText(text)));
			}
			catch(Exception ex) {
				NougatUtil.PrintWithColor(ConsoleColor.Red, "Error: " + ex.Message);
				Environment.Exit(0);
			}
		}

		NougatCompiler nougatCompiler = NougatCompiler.New(list);
		SymbolTable globalTable = new SymbolTable();
		nougatCompiler.Conduct(globalTable, CallMain: true);
	}
}