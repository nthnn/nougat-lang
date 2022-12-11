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
using System.IO;
using System.Reflection;
using Nougat.Core;

public class ModuleLoader {
	protected ModuleLoader() { }

	public static void Initialize(SymbolTable Symtab) {
		try {
			Init(Symtab);

			string[] files = Directory.GetFiles(NougatUtil.GetCurrentDirectoryWith("plug-ins"));
			foreach(string text in files) {
				if(text.EndsWith(".dll"))
					Load(text, Symtab);
			}
		}
		catch(Exception ex) {
			NougatUtil.PrintWithColor(ConsoleColor.DarkRed, "Error: " + ex.Message);
			Environment.Exit(0);
		}
	}

	private static void LoadFromType(string DllFileName, string ParentClass, Type type, SymbolTable Symtab) {
		MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

		foreach(MethodInfo methodInfo in methods) {
			string text = ((ParentClass == null) ? "" : (ParentClass + "::")) + type.Name.Replace(".", "::") + "::" + methodInfo.Name;

			if (!Symtab.Has(text))
				Symtab.Symbols.Add(new NougatNativeFunction(DllFileName, text, methodInfo));
		}

		Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Public);
		foreach(Type type2 in nestedTypes)
			LoadFromType(DllFileName, type.Name, type2, Symtab);
	}

	public static void Init(SymbolTable Symtab) {
		Type[] types = Assembly.LoadFrom(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "Nougat-API.dll").GetTypes();

		foreach(Type type in types)
			if(!type.IsNested)
				LoadFromType("std", null, type, Symtab);
	}

	public static void Load(string DllFileName, SymbolTable Symtab) {
		Type[] types = Assembly.LoadFrom(DllFileName).GetTypes();

		foreach (Type type in types)
			if (!type.IsNested)
				LoadFromType(DllFileName, null, type, Symtab);
	}

	public static object Invoke(MethodInfo Method, List<object> Arg) {
		return Method.Invoke(null, new object[1] { Arg });
	}
}