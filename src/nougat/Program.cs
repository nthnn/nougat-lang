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
using System.Diagnostics;
using Nougat.Compiler;

class Program {
	[STAThread]
	public static void Main(string[] Args) {
		if(Args.Length == 0) {
			ShowBanner();
			Console.WriteLine("  Usages:\n\tnougat <scripts>\tRun script files.\n\tnougat -d\t\tOpen documentation site.");
		}
		else if(Args[0].Equals("-d"))
			Process.Start("explorer", "https://nougat.neocities.org/docs/index.html");
		else CompileTask.Send(Args);
	}

	private static void ShowBanner() {
		ConsoleColor foregroundColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Magenta;
		Console.WriteLine();
		Console.WriteLine("  ──────────────────────────────────────────────────────\n");
		Console.WriteLine("  ███╗   ██╗ ██████╗ ██╗   ██╗ ██████╗  █████╗ ████████╗");
		Console.WriteLine("  ████╗  ██║██╔═══██╗██║   ██║██╔════╝ ██╔══██╗╚══██╔══╝");
		Console.WriteLine("  ██╔██╗ ██║██║   ██║██║   ██║██║  ███╗███████║   ██║   ");
		Console.WriteLine("  ██║╚██╗██║██║   ██║██║   ██║██║   ██║██╔══██║   ██║   ");
		Console.WriteLine("  ██║ ╚████║╚██████╔╝╚██████╔╝╚██████╔╝██║  ██║   ██║   ");
		Console.WriteLine("  ╚═╝  ╚═══╝ ╚═════╝  ╚═════╝  ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ");
		Console.WriteLine("\n      Nougat Programming Language v1.0.0 pre-alpha");
		Console.WriteLine("       Copyright (c) 2021 - " + DateTime.Now.Year + " - @nathannestein");
		Console.WriteLine("  ──────────────────────────────────────────────────────\n");
		Console.ForegroundColor = foregroundColor;
	}
}