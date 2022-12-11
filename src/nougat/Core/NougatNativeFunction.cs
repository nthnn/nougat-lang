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
using System.Reflection;
using Nougat;
using Nougat.Core;
using Nougat.Parser;
using Nougat.Parser.AST;
using Nougat.Parser.AST.Expr;

public class NougatNativeFunction : CallableSymbol {
	public class BridgeProxy : FunctionCallback {
		private CallableSymbol Symb;
		private SymbolTable Symtab;

		public BridgeProxy(CallableSymbol Symb, SymbolTable Symtab) {
			this.Symb = Symb;
			this.Symtab = Symtab;
		}

		public object Call(List<object> Args) {
			List<Expression> list = new List<Expression>();

			foreach(object Arg in Args)
				list.Add(new LiteralExpression(Arg));
			return Symb.Call(list, Symtab);
		}
	}

	public readonly MethodInfo Info;
	public NougatNativeFunction(string ModuleFileName, string NativeName, MethodInfo Info) {
		Name = Token.Create(0, 0, TokenType.IDENTIFIER, ModuleFileName, NativeName);
		this.Info = Info;
	}

	public override object Call(List<Expression> Arguments, SymbolTable Symtab) {
		List<object> list = new List<object>();

		foreach (Expression Argument in Arguments) {
			object obj = Argument.Visit(Symtab);
			if (obj is CallableSymbol)
				list.Add(new BridgeProxy(obj as CallableSymbol, Symtab));
			else list.Add(obj);
		}

		return Info.Invoke(null, list.ToArray());
	}

	public override object Visit(SymbolTable Symtab) {
		Resolve(Symtab);
		return this;
	}

	public override void Resolve(SymbolTable Symtab) { }

	public override string ToString() {
		string text = Name.ToString();
		return "<native " + ((!text.EndsWith(" (std)")) ? text : (text.Split(" [")[0] + " (std)")) + ">";
	}
}