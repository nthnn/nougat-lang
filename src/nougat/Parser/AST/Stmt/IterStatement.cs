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
using Nougat.Core;
using Nougat.Parser;
using Nougat.Parser.AST;

public class IterStatement : Statement {
	public readonly Token Placeholder;
	public readonly Expression Array;
	public readonly Statement Body;
	private SymbolTable block;

	public IterStatement(Token ID, Token Placeholder, Expression Array, Statement Body) {
		base.ID = ID;
		this.Placeholder = Placeholder;
		this.Array = Array;
		this.Body = Body;
	}

	public override void Visit(SymbolTable Symtab) {
		object obj = Array.Visit(Symtab);

		if(!(obj is object[]))
			throw new InterpreterException(ID, "Expression is not iterable. Type received: " + obj.GetType());

		object[] array = (object[])obj;
		foreach(object value in array) {
			try {
				block.Set(Placeholder, value);
				Body.Visit(block);
			}
			catch(Exception ex) {
				if(ex is ThrownStatement.Break)
					break;

				if(ex is ThrownStatement.Continue)
					continue;

				throw ex;
			}
		}
	}

	public override void Resolve(SymbolTable Symtab) {
		block = new SymbolTable(Symtab);
		if(block.Has(Placeholder.Image))
			throw new ResolveException(Placeholder, "Name already been in use: " + Placeholder);

		block.Symbols.Add(new NougatVariable(Placeholder, null));
		Array.Resolve(block);
		Body.Resolve(block);
	}
}