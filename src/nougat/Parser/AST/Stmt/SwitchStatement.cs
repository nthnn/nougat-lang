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
using Nougat.Core;
using Nougat.Parser;
using Nougat.Parser.AST;

public class SwitchStatement : Statement {
	public Expression Condition;
	public List<KeyValuePair<List<Expression>, Statement>> Cases;
	public Statement DefaultStmt;

	public SwitchStatement(Token ID, Expression Condition, List<KeyValuePair<List<Expression>, Statement>> Cases, Statement DefaultStmt) {
		base.ID = ID;
		this.Condition = Condition;
		this.Cases = Cases;
		this.DefaultStmt = DefaultStmt;
	}

	public override void Resolve(SymbolTable Symtab) {
		Condition.Resolve(Symtab);

		foreach(KeyValuePair<List<Expression>, Statement> @case in Cases) {
			foreach(Expression item in @case.Key)
				item.Resolve(Symtab);

			@case.Value.Resolve(Symtab);
		}

		if(DefaultStmt != null)
			DefaultStmt.Resolve(Symtab);
	}

	public override void Visit(SymbolTable Symtab) {
		object obj = Condition.Visit(Symtab);
		bool flag = true;

		foreach(KeyValuePair<List<Expression>, Statement> @case in Cases)
			foreach(Expression item in @case.Key)
				if(obj.Equals(item.Visit(Symtab))) {
					@case.Value.Visit(Symtab);
					flag = false;
				}

		if(flag)
			DefaultStmt.Visit(Symtab);
	}
}