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
using System.Linq;

public class Array {
	public static object contains(object[] ArrayObj, object Object) {
		return ArrayObj.ToList().Contains(Object);
	}

	public static object insert(object[] ArrayObj, object Object) {
		List<object> list = ArrayObj.ToList();
		list.Add(Object);

		return list.ToArray();
	}

	public static object insertArray(object[] ArrayObj, object Index, object[] ArrObj) {
		if(!(Index is double))
			throw new Exception("Index parameter is not of number type.");

		List<object> list = ArrayObj.ToList();
		list.InsertRange(Convert.ToInt32(Index), ArrObj);

		return list.ToArray();
	}

	public static object insertAt(object[] ArrayObj, object Index, object Object) {
		if (!(Index is double))
			throw new Exception("Index parameter is not of number type.");

		List<object> list = ArrayObj.ToList();
		list.Insert(Convert.ToInt32(Index), Object);

		return list.ToArray();
	}

	public static object indexOf(object[] ArrayObj, object Object) {
		return ArrayObj.ToList().IndexOf(Object);
	}

	public static object lastIndexOf(object[] ArrayObj, object Obj) {
		return (double)ArrayObj.ToList().LastIndexOf(Obj);
	}

	public static object remove(object[] ArrayObj, object obj) {
		List<object> list = ArrayObj.ToList();
		list.Remove(obj);

		return list.ToArray();
	}

	public static object removeAt(object[] ArrayObj, object Index) {
		if(!(Index is double))
			throw new Exception("Index parameter is not of number type.");

		List<object> list = ArrayObj.ToList();
		list.RemoveAt(Convert.ToInt32(Index));

		return list.ToArray();
	}

	public static object removeRange(object[] ArrayObj, object Index, object Count) {
		if(!(Index is double))
			throw new Exception("Index parameter is not of number type.");

		if(!(Count is double))
			throw new Exception("Count parameter is not of number type.");

		List<object> list = ArrayObj.ToList();
		list.RemoveRange(Convert.ToInt32(Index), Convert.ToInt32(Count));

		return list.ToArray();
	}

	public static object reverse(object[] ArrayObj) {
		List<object> list = ArrayObj.ToList();
		list.Reverse();

		return list.ToArray();
	}

	public static object reverseRange(object[] ArrayObj, object Index, object Count) {
		if(!(Index is double))
			throw new Exception("Index parameter is not of number type.");

		if(!(Count is double))
			throw new Exception("Count parameter is not of number type.");

		List<object> list = ArrayObj.ToList();
		list.Reverse(Convert.ToInt32(Index), Convert.ToInt32(Count));

		return list.ToArray();
	}

	public static object size(object[] ArrayObj) {
		return Convert.ToDouble(ArrayObj.Length);
	}

	public static object slice(object[] ArrayObj, object Index, object Count) {
		if(!(Index is double))
			throw new Exception("Index parameter is not of number type.");

		if(!(Count is double))
			throw new Exception("Count parameter is not of number type.");

		return ArrayObj.ToList().GetRange(Convert.ToInt32(Index), Convert.ToInt32(Count)).ToArray();
	}
}