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
using Nougat.Core;
using Nougat.Parser;
using Nougat.Parser.AST;
using Nougat.Parser.AST.Stmt;

public class BlockStatement : Statement {
	private SymbolTable block;
	public List<Statement> Statements { get; private set; }

	public BlockStatement(Token ID) {
		base.ID = ID;
		Statements = new List<Statement>();
	}

	public override void Visit(SymbolTable Symtab) {
		List<Statement> list = new List<Statement>();
		List<Statement> list2 = new List<Statement>();
		Statement statement = null;
		int num = 0;

		try {
			foreach(Statement statement2 in Statements)
				if(statement2 is DeferStatement)
					list2.Add(statement2);
				else list.Add(statement2);

			list2.Reverse();
			foreach (Statement item in list) {
				if(statement != null && (statement is ReturnStatement || statement is ThrowStatement || statement is BreakStatement || statement is ContinueStatement))
					throw new InterpreterException(statement.ID, "Unreachable code detected.");

				statement = item;
				item.Visit(block);
			}

			foreach(Statement item2 in list2) {
				if(statement != null && (statement is ReturnStatement || statement is ThrowStatement || statement is BreakStatement || statement is ContinueStatement))
					throw new InterpreterException(statement.ID, "Unreachable code detected.");

				statement = item2;
				item2.Visit(block);
				num++;
			}
		}
		catch(Exception ex) {
			statement = null;

			if((ex is ReturnValue || ex is ThrownStatement.Break || ex is ThrownStatement.Continue || (ex is RuntimeInterpreterException && (ex as RuntimeInterpreterException).IsFromThrowStmt)) && num != list2.Count) {
				for(int i = num; i < list2.Count; i++) {
					if(statement != null && (statement is ReturnStatement || statement is ThrowStatement || statement is BreakStatement || statement is ContinueStatement))
						throw new InterpreterException(statement.ID, "Unreachable code detected.");

					(statement = list2[i]).Visit(block);
					num++;
				}
			}

			throw ex;
		}
	}

	public override void Resolve(SymbolTable Symtab) {
		block = new SymbolTable(Symtab);

		foreach (Statement statement in Statements)
			statement.Resolve(block);
	}
}