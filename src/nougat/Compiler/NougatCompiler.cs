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
using System.Reflection;
using Nougat.Compiler;
using Nougat.Core;
using Nougat.Parser;
using Nougat.Parser.AST;

public class NougatCompiler {
	private List<KeyValuePair<string, string>> Sources;

	protected NougatCompiler(List<KeyValuePair<string, string>> Sources) {
		this.Sources = Sources;
	}

	public static NougatCompiler New(List<KeyValuePair<string, string>> Sources) {
		return new NougatCompiler(Sources);
	}

	public void Conduct(SymbolTable globalTable, bool CallMain) {
		List<Token> list = new List<Token>();

		foreach(KeyValuePair<string, string> source in Sources) {
			NougatTokenizer nougatTokenizer = new NougatTokenizer(source.Key, source.Value);
			nougatTokenizer.Run();

			if(!nougatTokenizer.IsSuccess()) {
				NougatUtil.PrintWithColor(ConsoleColor.Red, "Lexical analysis error:");
				NougatUtil.ShowErrors(nougatTokenizer.Errors);
				return;
			}

			list = NougatUtil.Merge(list, nougatTokenizer.Tokens);
		}

		NougatParser nougatParser = new NougatParser(list);
		try {
			nougatParser.Run();

			if(nougatParser.Errors.Count != 0)
				throw new ParserException();
		}
		catch(ParserException) {
			NougatUtil.PrintWithColor(ConsoleColor.Red, "Syntax error" + ((nougatParser.Errors.Count != 1) ? "s" : "") + ":");
			NougatUtil.ShowErrors(nougatParser.Errors);
			Environment.Exit(0);
		}

		if(nougatParser.IsSuccess())
			Execute(globalTable, nougatParser.Globals, CallMain);
	}

	private void Execute(SymbolTable GlobalTable, List<Statement> Statements, bool CallMain){
		bool flag = false;
		ModuleLoader.Initialize(GlobalTable);

		try {
			foreach(Statement Statement in Statements)
				Statement.Resolve(GlobalTable);

			foreach(Statement Statement2 in Statements)
				Statement2.Visit(GlobalTable);

			if(CallMain) {
				Symbol symbol = null;

				if(GlobalTable.Has("main") && (symbol = GlobalTable.Fetch("main")) is CallableSymbol) {
					object obj = (symbol as CallableSymbol).Call(new List<Expression>(), new SymbolTable(GlobalTable));

					if (obj != null)
						NougatUtil.PrintWithColor(ConsoleColor.Yellow, NougatUtil.ToString(obj));
				}
			}
		}
		catch(ReturnValue returnValue) {
			if(returnValue.Value == null)
				returnValue.Value = "nil";

			NougatUtil.PrintWithColor(ConsoleColor.Yellow, returnValue.Value.ToString());
		}
		catch(ResolveException ex) {
			Console.WriteLine("Encountered runtime resolve error: ");

			NougatUtil.PrintWithColor(ConsoleColor.Red, ex.Message);
			flag = true;
		}
		catch(InterpreterException ex2) {
			NougatUtil.PrintWithColor(ConsoleColor.Red, ex2.Message);
			flag = true;
		}
		catch(RuntimeInterpreterException ex3) {
			NougatUtil.PrintWithColor(ConsoleColor.Red, "Uncaught exception: " + ex3.Thrown.ToString() + "\n\tAt " + ex3.ID);
			flag = true;
		}
		catch(Exception ex4) {
			NougatUtil.PrintWithColor(ConsoleColor.Red, "Unhandled exception: " + ((ex4 is TargetInvocationException) ? ((object)ex4.InnerException) : ((object)ex4.Message)));
			flag = true;
		}

		if(flag)
			Environment.Exit(0);
	}
}