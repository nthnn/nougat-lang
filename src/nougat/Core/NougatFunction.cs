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

public class NougatFunction : CallableSymbol {
	public readonly Statement Body;
	public readonly List<KeyValuePair<Token, Expression>> Parameters;
	public readonly SymbolTable Local;

	private bool HasBeenResolved;

	public NougatFunction(Token Name, List<KeyValuePair<Token, Expression>> Parameters, Statement Body, SymbolTable Local) {
		base.Name = Name;

		this.Body = Body;
		this.Parameters = Parameters;
		this.Local = Local;
	}

	public override object Call(List<Expression> Arguments, SymbolTable Symtab) {
		Symtab = new SymbolTable(Symtab);

		try {
			int num = 0;

			foreach(KeyValuePair<Token, Expression> parameter in Parameters) {
				if(num < Arguments.Count && Arguments[num] != null) {
					Local.Set(parameter.Key, Arguments[num++].Visit(Local));
					continue;
				}

				if(parameter.Value != null) {
					Local.Set(parameter.Key, parameter.Value.Visit(Local));
					num++;

					continue;
				}

				break;
			}

			if(Parameters.Count != num)
				throw new InterpreterException(Name, "Expects " + Parameters.Count + " parameter" + ((Parameters.Count != 1) ? "s" : "") + ", but got " + Arguments.Count + ".");

			Body.Visit(Local);
		}
		catch(ReturnValue returnValue) {
			return returnValue.Value;
		}

		return null;
	}

	public override object Visit(SymbolTable Symtab) {
		Resolve(Symtab);
		return this;
	}

	public override void Resolve(SymbolTable Symtab) {
		if(!HasBeenResolved) {
			foreach(KeyValuePair<Token, Expression> parameter in Parameters) {
				if(Local.Has(parameter.Key))
					throw new ResolveException(parameter.Key, "Symbol name in use.");

				Local.Symbols.Add(new NougatVariable(parameter.Key, parameter.Value));
			}

			HasBeenResolved = true;
		}

		Body.Resolve(Local);
		Symtab.Symbols.Add(this);
	}

	public override string ToString() {
		return "<fun " + Name?.ToString() + ">";
	}
}