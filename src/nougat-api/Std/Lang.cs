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
using Nougat;
using Nougat.Core;

public class Lang
{
	public static object getType(object Obj)
	{
		if (Obj is string)
		{
			return "string";
		}
		if (Obj is double)
		{
			return "number";
		}
		if (Obj is FunctionCallback)
		{
			return "function";
		}
		if (Obj is object[])
		{
			return "array";
		}
		if (Obj is List<KeyValuePair<object, object>>)
		{
			return "map";
		}
		if (Obj is bool)
		{
			return "boolean";
		}
		return "undefined";
	}

	public static object info()
	{
		return new List<KeyValuePair<object, object>>
		{
			new KeyValuePair<object, object>("version", 1.0),
			new KeyValuePair<object, object>("author", "@nathannestein"),
			new KeyValuePair<object, object>("stable", true)
		};
	}

	public static object isArray(object Obj)
	{
		return Obj is object[];
	}

	public static object isFun(object Obj)
	{
		return Obj is FunctionCallback;
	}

	public static object isMap(object Obj)
	{
		return Obj is List<KeyValuePair<object, object>>;
	}

	public static object isNumber(object Obj)
	{
		return Obj is double;
	}

	public static object isString(object Obj)
	{
		return Obj is string;
	}

	public static object tryCatch(object TryCallback, object CatchCallback)
	{
		if (!(TryCallback is FunctionCallback))
		{
			throw new Exception("TryCallback parameter is not of function type.");
		}
		if (!(CatchCallback is FunctionCallback))
		{
			throw new Exception("CatchCallback parameter is not of function type.");
		}
		object obj = null;
		try
		{
			return (TryCallback as FunctionCallback).Call(new List<object>());
		}
		catch (Exception ex)
		{
			return (CatchCallback as FunctionCallback).Call(new List<object> { (ex is RuntimeInterpreterException) ? (ex as RuntimeInterpreterException).Thrown : ex.Message });
		}
	}
}
