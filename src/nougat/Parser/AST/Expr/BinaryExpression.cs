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
using Nougat.Parser.AST.Expr;

public class BinaryExpression : Expression {
	public readonly Token Operation;
	public readonly Expression Left;
	public readonly Expression Right;

	public BinaryExpression(Expression Left, Token Operation, Expression Right) {
		this.Left = Left;
		this.Operation = Operation;
		this.Right = Right;
	}

	public override object Visit(SymbolTable Symtab) {
		if(Operation.Image == "?") {
			object obj = Left.Visit(Symtab);

			if(obj == null)
				return Right.Visit(Symtab);
			return obj;
		}

		if(Left is IdentifierExpression && Operation.Image == "=") {
			object obj2 = Right.Visit(Symtab);
			Symtab.Set((Left as IdentifierExpression).ID, obj2);
			return obj2;
		}

		if(Left is AccessExpression && Operation.Image == "=") {
			InterpreterException ex = new InterpreterException(ID, "Use array library for array operations instead.");

			if((Left as AccessExpression).Accessible is IdentifierExpression) {
				AccessExpression accessExpression = Left as AccessExpression;
				object obj3 = accessExpression.Accessible.Visit(Symtab);
				Token iD = (accessExpression.Accessible as IdentifierExpression).ID;

				if(obj3 is object[]) {
					object[] array = (object[])accessExpression.Accessible.Visit(Symtab);
					object result = (array[(int)(double)accessExpression.Index.Visit(Symtab)] = Right.Visit(Symtab));
					Symtab.Set(iD, array);
					return result;
				}

				if(obj3 is List<KeyValuePair<object, object>>) {
					List<KeyValuePair<object, object>> list = obj3 as List<KeyValuePair<object, object>>;
					object result2 = null;
					object obj4 = accessExpression.Index.Visit(Symtab);

					foreach(KeyValuePair<object, object> item in list) {
						if(obj4.Equals(item.Key)) {
							list.Remove(item);
							list.Add(new KeyValuePair<object, object>(obj4, Right.Visit(Symtab)));

							result2 = item.Value;
							break;
						}
					}

					Symtab.Set(iD, list);
					return result2;
				}

				throw new InterpreterException(accessExpression.ID, "Inaccessible expression.");
			}

			if(!((Left as AccessExpression).Accessible is AccessExpression))
				throw ex;

			List<object> list2 = new List<object>();
			_ = Left;

			AccessExpression accessExpression2 = (Left as AccessExpression).Accessible as AccessExpression;
			list2.Add((Left as AccessExpression).Index.Visit(Symtab));
			list2.Add(accessExpression2.Index.Visit(Symtab));

			while (accessExpression2.Accessible is AccessExpression) {
				accessExpression2 = accessExpression2.Accessible as AccessExpression;
				list2.Add(accessExpression2.Index.Visit(Symtab));
			}

			if(!(accessExpression2.Accessible is IdentifierExpression))
				throw ex;

			object obj5 = accessExpression2.Accessible.Visit(Symtab);
			list2.Reverse();

			if(obj5 is object[]) {
				List<int> list3 = new List<int>();
				foreach (object item2 in list2)
					list3.Add(Convert.ToInt32(item2));

				object[] array2 = (object[])obj5;
				object[] array3 = null;

				int num = list3[list3.Count - 1];
				list3.RemoveAt(list3.Count - 1);

				foreach(int item3 in list3) {
					try {
						array3 = ((array3 != null) ? ((object[])array3[item3]) : ((object[])array2[item3]));
					}
					catch(InvalidCastException) {
						throw new InterpreterException(ID, "Cannot access to a non-array object.");
					}
				}

				object result3 = (array3[num] = Right.Visit(Symtab));
				Symtab.Set((accessExpression2.Accessible as IdentifierExpression).ID, array2);

				return result3;
			}

			if(obj5 is List<KeyValuePair<object, object>>) {
				List<KeyValuePair<object, object>> list4 = (List<KeyValuePair<object, object>>)obj5;
				List<KeyValuePair<object, object>> list5 = null;

				object obj6 = list2[list2.Count - 1];
				list2.Remove(obj6);

				foreach(object item4 in list2) {
					try {
						if(list5 == null) {
							foreach(KeyValuePair<object, object> item5 in list4)
								if(item5.Key.Equals(item4))
									list5 = (List<KeyValuePair<object, object>>)item5.Value;

							continue;
						}

						foreach(KeyValuePair<object, object> item6 in list5)
							if(item6.Key.Equals(item4))
								list5 = (List<KeyValuePair<object, object>>)item6.Value;
					}
					catch(InvalidCastException) {
						throw new InterpreterException(ID, "Cannot access to a non-map object.");
					}
				}

				object obj7 = Right.Visit(Symtab);
				foreach(KeyValuePair<object, object> item7 in list5) {
					if(item7.Key.Equals(obj6)) {
						list5.Remove(item7);
						list5.Add(new KeyValuePair<object, object>(item7.Key, obj7));

						break;
					}
				}

				Symtab.Set((accessExpression2.Accessible as IdentifierExpression).ID, list4);
				return obj7;
			}
		}
		else {
			object obj8 = Left.Visit(Symtab);
			object obj9 = Right.Visit(Symtab);

			if(obj8 == null)
				obj8 = "nil";
			if(obj9 == null)
				obj9 = "nil";

			if(obj8 is string && obj9 is double) {
				string text = NougatUtil.ToString(obj8);
				double num2 = (double)obj9;

				if(Operation.Image == "+")
					return text + num2;

				if(Operation.Image == "-")
					return text.Replace(num2.ToString(), "");

				if(Operation.Image == "*") {
					string text2 = text;
					int num3 = RoundUp((double)obj9) - 1;

					for(int i = 0; i < num3; i++)
						text2 += text;
					return text2;
				}

				if(Operation.Image == "!=")
					return obj8 != obj9;

				if(Operation.Image == "==")
					return obj8 == obj9;

				throw new NodeException("Invalid binary operator for string and double.");
			}

			if(obj8 is double && obj9 is string) {
				double num4 = (double)obj8;
				string text3 = NougatUtil.ToString(obj9);

				if(Operation.Image == "+")
					return num4 + text3;

				if(Operation.Image == "-")
					return num4.ToString().Replace(text3, "");

				if(Operation.Image == "*") {
					string text4 = text3;
					int num5 = RoundUp(num4) - 1;

					for (int j = 0; j < num5; j++)
						text4 += text3;
					return text4;
				}

				if(Operation.Image == "!=")
					return obj8 != obj9;

				if(Operation.Image == "==")
					return obj8 == obj9;

				throw new NodeException("Invalid binary operator for double and string.");
			}

			if(!(obj8 is double) || !(obj9 is double)) {
				if(obj8 is string && obj9 is string) {
					string text5 = NougatUtil.ToString(obj8);
					string text6 = NougatUtil.ToString(obj9);

					if (Operation.Image == "+")
						return text5 + text6;

					if (Operation.Image == "-")
						return text5.Replace(text6, "");

					if (Operation.Image == "<")
						return text5.Length < text6.Length;

					if (Operation.Image == "<=")
						return text5.Length <= text6.Length;

					if (Operation.Image == ">")
						return text5.Length > text6.Length;

					if (Operation.Image == ">=")
						return text5.Length >= text6.Length;

					if (Operation.Image == "!=")
						return text5 != text6;

					if (Operation.Image == "==")
						return text5 == text6;

					throw new NodeException("Invalid binary operator for strings.");
				}

				if(Operation.Image == "!=")
					return obj8 != obj9;

				if(Operation.Image == "==")
					return obj8 == obj9;

				if(Operation.Image == "||")
					return (bool)obj8 || (bool)obj9;

				if(Operation.Image == "&&")
					return (bool)obj8 && (bool)obj9;

				if(Operation.Image == "+")
					return NougatUtil.ToString(obj8) + NougatUtil.ToString(obj9);

				throw new NodeException("Invalid binary operator.");
			}

			double num6 = (double)obj8;
			double num7 = (double)obj9;

			if(Operation.Image == "+")
				return num6 + num7;

			if(Operation.Image == "-")
				return num6 - num7;

			if(Operation.Image == "*")
				return num6 * num7;

			if(Operation.Image == "/")
				return num6 / num7;

			if(Operation.Image == "%")
				return num6 % num7;

			if(Operation.Image == "<")
				return num6 < num7;

			if(Operation.Image == ">")
				return num6 > num7;

			if(Operation.Image == "<=")
				return num6 <= num7;

			if(Operation.Image == ">=")
				return num6 >= num7;

			if(Operation.Image == "!=")
				return num6 != num7;

			if(Operation.Image == "==")
				return num6 == num7;

			if(Operation.Image == "<<")
				return (double)(RoundUp(num6) << RoundUp(num7));

			if(Operation.Image == ">>")
				return (double)(RoundUp(num6) >> RoundUp(num7));

			if(Operation.Image == "^")
				return (double)(RoundUp(num6) ^ RoundUp(num7));

			if(Operation.Image == "&")
				return (double)(RoundUp(num6) & RoundUp(num7));

			if(Operation.Image == "|")
				return (double)(RoundUp(num6) | RoundUp(num7));

			if(Operation.Image == "<<")
				return (double)(RoundUp(num6) << RoundUp(num7));

			if(Operation.Image == ">>")
				return (double)(RoundUp(num6) >> RoundUp(num7));
		}

		return null;
	}

	public override void Resolve(SymbolTable Symtab) {
		Left.Resolve(Symtab);
		Right.Resolve(Symtab);
	}

	private static int RoundUp(double Value) {
		return (int)Math.Floor(Value);
	}
}