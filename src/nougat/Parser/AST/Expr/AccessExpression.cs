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
using Nougat.Parser.AST;

public class AccessExpression : Expression {
	public readonly Expression Accessible;
	public readonly Expression Index;

	public AccessExpression(Expression Accessible, Expression Index) {
		this.Accessible = Accessible;
		this.Index = Index;
	}

	public override object Visit(SymbolTable Symtab) {
		object obj = Accessible.Visit(Symtab), obj2 = Index.Visit(Symtab);

		if(obj is string)
			return obj.ToString()[Convert.ToInt32(obj2)];

		if(obj is object[])
			return ((object[])obj)[Convert.ToInt32(obj2)];

		if(obj is List<KeyValuePair<object, object>>) {
			foreach(KeyValuePair<object, object> item in (List<KeyValuePair<object, object>>)obj)
				if(item.Key.Equals(obj2))
					return item.Value;

			return null;
		}

		throw new InterpreterException(ID, "Expression is inaccessible.");
	}

	public override void Resolve(SymbolTable Symtab) {
		Accessible.Resolve(Symtab);
		Index.Resolve(Symtab);
	}
}