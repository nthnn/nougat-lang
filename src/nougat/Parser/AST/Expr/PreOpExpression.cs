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

public class PreOpExpression : Expression {
	public readonly Token Operation;
	public readonly Expression Expr;

	public PreOpExpression(Token Operation, Expression Expr) {
		this.Operation = Operation;
		this.Expr = Expr;
	}

	public override object Visit(SymbolTable Symtab) {
		object obj = Expr.Visit(Symtab);

		if(!(obj is double))
			throw new NodeException("Invalid type for pre-operational expression.");

		if(Operation.Image == "~")
			return (double)(~(int)Math.Floor((double)obj));

		if(Operation.Image == "-")
			return 0.0 - (double)obj;

		if(Operation.Image == "+")
			return (double)obj;

		return null;
	}

	public override void Resolve(SymbolTable Symtab) {
		Expr.Resolve(Symtab);
	}
}