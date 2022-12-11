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

public class TryCatchFinallyStatement : Statement {
	public readonly Statement Try;
	public readonly Statement Catch;
	public readonly Statement Finally;
	public readonly Token Catcher;
	private SymbolTable block;

	public TryCatchFinallyStatement(Token ID, Statement Try, Token Catcher, Statement Catch, Statement Finally) {
		base.ID = ID;
		this.Try = Try;
		this.Catcher = Catcher;
		this.Catch = Catch;
		this.Finally = Finally;
	}

	public override void Visit(SymbolTable Symtab) {
		try {
			Try.Visit(Symtab);
		}
		catch(Exception ex) {
			if(ex is RuntimeInterpreterException) {
				if (Catcher != null)
					block.Set(Catcher, (ex as RuntimeInterpreterException).Thrown);

				Catch.Visit(block);
				return;
			}

			throw ex;
		}
		finally {
			if(Finally != null)
				Finally.Visit(Symtab);
		}
	}

	public override void Resolve(SymbolTable Symtab) {
		block = new SymbolTable(Symtab);
		Try.Resolve(Symtab);

		if(block.Has(Catcher.Image))
			throw new ResolveException(Catcher, "Name already be in use: " + Catcher);

		if(Catcher != null)
			block.Symbols.Add(new NougatVariable(Catcher, null));

		Catch.Resolve(block);
		if(Finally != null)
			Finally.Resolve(Symtab);
	}
}