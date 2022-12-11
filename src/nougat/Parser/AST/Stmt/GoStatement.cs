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
using System.Threading;
using Nougat.Core;
using Nougat.Parser;
using Nougat.Parser.AST;
using Nougat.Parser.AST.Stmt;

public class GoStatement : Statement {
	public readonly Statement Stmt;
	private SymbolTable Symtab;

	public GoStatement(Token ID, Statement Stmt) {
		base.ID = ID;
		this.Stmt = Stmt;
	}

	private void Call() {
		try {
			Stmt.Visit(Symtab);
		}
		catch(Exception ex) {
			if(ex is ReturnValue)
				NougatUtil.PrintWithColor(ConsoleColor.Yellow, (ex as ReturnValue).Value.ToString());
			else if(ex is ResolveException) {
				Console.WriteLine("Encountered runtime resolve error: ");
				NougatUtil.PrintWithColor(ConsoleColor.Red, (ex as ResolveException).Message);
			}
			else if(ex is RuntimeInterpreterException) {
				RuntimeInterpreterException ex2 = ex as RuntimeInterpreterException;
				NougatUtil.PrintWithColor(ConsoleColor.Red, "Uncaught exception: " + ex2.Thrown.ToString() + "\n\tAt " + ex2.ID);
			}
			else NougatUtil.PrintWithColor(ConsoleColor.Red, "Unhandled exception: " + ex.Message + "\n\tFrom line " + ID.Line + ", column " + ID.Column + ".");

			Environment.Exit(0);
		}
	}

	public override void Visit(SymbolTable Symtab) {
		this.Symtab = Symtab;
		new Thread(Call).Start();
	}

	public override void Resolve(SymbolTable Symtab) {
		if(Stmt is DeferStatement || Stmt is GoStatement || Stmt is ReturnStatement || Stmt is ThrowStatement || Stmt is BreakStatement || Stmt is ContinueStatement)
			throw new ResolveException(ID, "Invalid go statement for concurrency.");

		Stmt.Resolve(Symtab);
	}
}
