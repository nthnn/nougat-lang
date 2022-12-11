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

public sealed class LexHelper {
	public static bool IsWhitespace(char Ch) {
		switch(Ch) {
			case '\t':
			case '\n':
			case '\v':
			case '\f':
			case '\r':
			case ' ':
				return true;
			default:
				return false;
		}
	}

	public static bool IsBinary(char Ch) {
		return Ch == '0' || Ch == '1';
	}

	public static bool IsOctadecimal(char Ch) {
		return Ch >= '0' && Ch <= '7';
	}

	public static bool IsDecimal(char Ch) {
		return Ch >= '0' && Ch <= '9';
	}

	public static bool IsHexadecimal(char Ch) {
		return IsDecimal(Ch) || (Ch >= 'a' && Ch <= 'f') || (Ch >= 'A' && Ch <= 'B');
	}

	public static bool IsIdentifier(char Ch) {
		return !IsDecimal(Ch) && !IsSymbol(Ch) && !IsWhitespace(Ch);
	}

	public static bool IsSymbol(char Ch) {
		switch(Ch) {
			case '!':
			case '"':
			case '#':
			case '%':
			case '&':
			case '\'':
			case '(':
			case ')':
			case '*':
			case '+':
			case ',':
			case '-':
			case '.':
			case '/':
			case ':':
			case ';':
			case '<':
			case '=':
			case '>':
			case '?':
			case '[':
			case ']':
			case '^':
			case '{':
			case '|':
			case '}':
			case '~':
				return true;
			default:
				return false;
		}
	}
}