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

using System.Collections.Generic;
using System.IO;
using Nougat.Compiler;
using Nougat.Core;
using Nougat.Parser;
using Nougat.Parser.AST;

public class IncludeStatement : Statement {
	public readonly List<string> Files;
	private List<KeyValuePair<string, string>> Includes = new List<KeyValuePair<string, string>>();

	public IncludeStatement(Token ID, List<string> Files) {
		base.ID = ID;
		this.Files = Files;
	}

	public override void Visit(SymbolTable Symtab) {
		NougatCompiler.New(Includes).Conduct(Symtab, CallMain: false);
	}

	public override void Resolve(SymbolTable Symtab) {
		foreach(string file in Files) {
			if(!file.EndsWith(".nou"))
				throw new InterpreterException(ID, "Not a Nougat file.");

			string text = File.ReadAllText(file);
			if(!NougatUtil.HasRanScript(NougatUtil.ToMD5(text)))
				Includes.Add(new KeyValuePair<string, string>(file, text));
		}
	}
}