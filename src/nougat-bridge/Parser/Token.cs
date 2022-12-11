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

using Nougat.Parser;

public class Token {
	public readonly int Line;
	public readonly int Column;
	public readonly TokenType Kind;
	public readonly string Image;
	public readonly string FileName;

	protected Token(int Line, int Column, TokenType Kind, string FileName, string Image) {
		this.Line = Line;
		this.Column = Column;
		this.Kind = Kind;
		this.FileName = FileName;
		this.Image = Image;
	}

	public static Token Create(int Line, int Column, TokenType Kind, string FileName, string Image) {
		return new Token(Line, Column, Kind, FileName, Image);
	}

	public SourceAddress Locate() {
		return new SourceAddress(Line, Column);
	}

	public override string ToString() {
		return ((Kind == TokenType.STRING) ? ("\"" + Image + "\"") : Image) + " [line " + Line + ", column " + Column + "] (" + FileName + ")";
	}
}