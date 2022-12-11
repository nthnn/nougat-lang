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

public class SymbolTable {
	public readonly SymbolTable Parent;
	public readonly List<Symbol> Symbols = new List<Symbol>();

	public SymbolTable(SymbolTable Parent = null) {
		this.Parent = Parent;
	}

	public void Set(Token Name, object Value) {
		if(Parent != null && Parent.Has(Name)) {
			Parent.Set(Name, Value);
			return;
		}

		foreach(Symbol symbol in Symbols) {
			if(symbol.Name.Image == Name.Image) {
				Symbols.Remove(symbol);
				Symbols.Add(new NougatVariable(Name, Value));
				break;
			}
		}
	}

	public bool Has(string Name) {
		foreach(Symbol symbol in Symbols)
			if(symbol.Name.Image == Name)
				return true;

		if (Parent != null)
			return Parent.Has(Name);
		return false;
	}

	public bool Has(Token Name) {
		return Has(Name.Image);
	}

	public Symbol Fetch(string Name) {
		foreach(Symbol symbol in Symbols)
			if (symbol.Name.Image == Name)
				return symbol;

		if(Parent != null) {
			return Parent.Fetch(Name);
		return null;
	}
}